#if TOOLS
namespace addons.hex_grid_map_editor;

using editor_grid_indicator;
using fov_display;
using Godot;
using hex_editor;

[Tool]
public partial class HexGridMapEditorPlugin : EditorPlugin
{
	private PackedScene editorScene = GD.Load<PackedScene>("res://addons/hex_grid_map_editor/hex_grid_editor.tscn");

	private Control rootView = null!;
	private HexGridEditorView? view;

	private IHexGridMapEditionProvider mapEditionProvider = null!;
	private HexGridEditorInputHandler? hexGridEditorInputHandler;

	private HexEditor hexEditor = null!;
	private EditorGridIndicator gridIndicator = null!;
	private FovDisplay fovDisplay = null!;
	
	private bool pluginEnabled;

	#region EditorPlugin overrides

	public override int _Forward3DGuiInput(Camera3D viewportCamera, InputEvent @event)
	{
		if (@event is InputEventMouseMotion)
		{
			UpdateOverlays();
		}

		if (hexGridEditorInputHandler is null) return base._Forward3DGuiInput(viewportCamera, @event);
		
		hexGridEditorInputHandler.UpdateCursorPosition(mapEditionProvider.Position + hexEditor.CurrentLayerOffset, viewportCamera, @event as InputEventMouseMotion);

		return (int)hexGridEditorInputHandler.FinalizeInput(viewportCamera, @event, hexEditor.IsSelectionActive);
	}
	
	public override void _Forward3DDrawOverViewport(Control viewportControl)
	{
		base._Forward3DDrawOverViewport(viewportControl);
		if (hexGridEditorInputHandler is null) return;
		var hexPosition = hexGridEditorInputHandler.HexPosition;
		var text = $"Q: {hexPosition.Q} R: {hexPosition.R} S: {hexPosition.S}";
		viewportControl.DrawString(
			ThemeDB.FallbackFont, 
			new Vector2(0, viewportControl.Size.Y - 8), 
			text, 
			HorizontalAlignment.Right,
			viewportControl.Size.X - 8,
			14,
			new Color(.5f, .5f, .5f));
	}
	
	public override void _EnterTree()
	{
		rootView = editorScene.Instantiate<Control>();
	}

	public override void _ExitTree()
	{
		Disable();
		rootView.QueueFree();
	}

	public override bool _Handles(GodotObject @object)
	{
		if (@object is not IHexGridMapEditionProvider mapProvider)
		{
			Disable();
			return true;
		}
		Enable(mapProvider);
		return true;
	}

	#endregion

	private void Enable(IHexGridMapEditionProvider mapEditionProvider)
	{
		if (pluginEnabled) return;

		this.mapEditionProvider = mapEditionProvider;
		mapEditionProvider.Initialize();

		hexGridEditorInputHandler = new HexGridEditorInputHandler();

		var world = mapEditionProvider.World3D;
		
		view = rootView.GetNode<HexGridEditorView>(".");
		if (view == null)
		{
			GD.Print($"[HexGridEditor] Couldn't get {typeof(HexGridEditorView)} from root view.");
			return;
		}
		hexEditor = new HexEditor(mapEditionProvider, view.HexEditor, hexGridEditorInputHandler);
		fovDisplay = new FovDisplay(world, view.FovDisplay, mapEditionProvider, hexEditor, hexGridEditorInputHandler);
		gridIndicator = new EditorGridIndicator(world, view.EditorGridIndicator, hexEditor, hexGridEditorInputHandler);

		hexEditor.Enable();
		fovDisplay.Enable();
		gridIndicator.Enable();
		
		mapEditionProvider.OnPropertyChange += Reset;
		view.TabContainer.TabChanged += OnTabChangedHandler;
		hexGridEditorInputHandler.OnPipetteRequested += OnPipetteRequestedHandler;
		
		AddControlToDock(DockSlot.RightBl, rootView);
		
		pluginEnabled = true;
	}

	private void Disable()
	{
		if (!pluginEnabled) return;
		
		pluginEnabled = false;

		RemoveControlFromDocks(rootView);

		mapEditionProvider.OnPropertyChange -= Reset;
		view!.TabContainer.TabChanged -= OnTabChangedHandler;
		hexGridEditorInputHandler.OnPipetteRequested -= OnPipetteRequestedHandler;

		hexEditor.Disable();
		fovDisplay.Disable();
		gridIndicator.Disable();
	}

	private void Reset()
	{
		Disable();
		Enable(mapEditionProvider);
	}

	private void OnTabChangedHandler(long index)
	{
		if (index != 0)
		{
			hexEditor.Deselect();
		}
	}

	private void OnPipetteRequestedHandler()
	{
		// Check if a hex was under pointer to switch to hex editor tab.
		var hex = mapEditionProvider.GetHex(hexGridEditorInputHandler.HexPosition, hexEditor.CurrentLayer);
		if (hex == null) return;

		view!.TabContainer.CurrentTab = 0;
	}
}
#endif
