#if TOOLS
namespace hex_grid.addons.hex_grid_editor;

using chunk_display;
using editor_grid_indicator;
using fov_display;
using Godot;
using hex_editor;
using mesh;
using HexGridMap = scripts.hex_grid.HexGridMap;

[Tool]
public partial class HexGridEditor : EditorPlugin
{
	private PackedScene editorScene = GD.Load<PackedScene>("res://addons/hex_grid_editor/hex_grid_editor.tscn");

	private Control rootView;
	private HexGridEditorView view;
	
	private HexGridMap hexGridMap;
	private HexGridEditorInputHandler hexGridEditorInputHandler;

	private HexEditor hexEditor;
	private EditorGridIndicator gridIndicator;
	private ChunkDisplay chunkDisplay;
	private FovDisplay fovDisplay;
	private GridMapMeshData gridMapMeshData;
	
	private bool pluginEnabled;

	private float CellSize => hexGridMap.CellSize;

	#region EditorPlugin overrides

	public override int _Forward3DGuiInput(Camera3D viewportCamera, InputEvent @event)
	{
		hexGridEditorInputHandler.UpdateCursorPosition(hexGridMap.Position + hexEditor.CurrentLayerOffset, viewportCamera, @event as InputEventMouseMotion);

		return (int)hexGridEditorInputHandler.FinalizeInput(viewportCamera, @event, hexEditor.IsSelectionActive);
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
		if (@object is not HexGridMap map)
		{
			Disable();
			return false;
		}
		Enable(map);
		return true;
	}

	#endregion

	private void Enable(HexGridMap map)
	{
		if (pluginEnabled) return;
		pluginEnabled = true;
		
		view = rootView.GetNode<HexGridEditorView>(".");
		
		hexGridMap = map;
		hexGridMap.Initialize();
		
		hexGridEditorInputHandler ??= new HexGridEditorInputHandler(CellSize);
		
		gridMapMeshData.CellSize = CellSize;
		gridMapMeshData.World = hexGridMap.GetWorld3D();
		
		hexEditor ??= new HexEditor(hexGridMap, view.HexEditor, hexGridEditorInputHandler);
		fovDisplay ??= new FovDisplay(gridMapMeshData, view.FovDisplay, hexGridMap, hexEditor, hexGridEditorInputHandler);
		gridIndicator ??= new EditorGridIndicator(gridMapMeshData, view.EditorGridIndicator, hexEditor, hexGridEditorInputHandler);
		chunkDisplay ??= new ChunkDisplay(gridMapMeshData, hexGridMap.ChunkSize, view.ChunkDisplay, hexEditor, hexGridEditorInputHandler);
		chunkDisplay.ChunkSize = hexGridMap.ChunkSize;

		hexEditor.Enable();
		fovDisplay.Enable();
		gridIndicator.Enable();
		chunkDisplay.Enable();
		
		hexGridMap.OnPropertyChange += Reset;
		view.TabContainer.TabChanged += OnTabChangedHandler;
		hexGridEditorInputHandler.OnPipetteRequested += OnPipetteRequestedHandler;
		
		AddControlToDock(DockSlot.RightBl, rootView);
	}

	private void Disable()
	{
		if (!pluginEnabled) return;
		pluginEnabled = false;

		RemoveControlFromDocks(rootView);

		hexGridMap.OnPropertyChange -= Reset;
		view.TabContainer.TabChanged -= OnTabChangedHandler;
		hexGridEditorInputHandler.OnPipetteRequested -= OnPipetteRequestedHandler;

		hexEditor.Disable();
		fovDisplay.Disable();
		gridIndicator.Disable();
		chunkDisplay.Disable();
	}

	private void Reset()
	{
		Disable();
		Enable(hexGridMap);
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
		var hex = hexGridMap.GetHex(hexGridEditorInputHandler.HexPosition, hexEditor.CurrentLayer);
		if (hex == null) return;
		
		view.TabContainer.CurrentTab = 0;
	}
}
#endif
