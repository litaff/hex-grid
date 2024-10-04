#if TOOLS
namespace hex_grid.addons.hex_grid_editor;

using Godot;
using scripts;

[Tool]
public partial class HexGridEditor : EditorPlugin
{
	private PackedScene dockScene = GD.Load<PackedScene>("res://addons/hex_grid_editor/hex_grid_editor.tscn");

	private Control rootView;
	private View view;
	
	private HexGridMap hexGridMap;
	private InputHandler inputHandler;
	private GridMesh gridMesh;
	private bool isSelectionActive;
	
	private bool enabled;

	/// <summary>
	/// Called when cursor is in the 3D viewport.
	/// </summary>
	public override int _Forward3DGuiInput(Camera3D viewportCamera, InputEvent @event)
	{
		inputHandler.UpdateCursorPosition(viewportCamera, @event as InputEventMouseMotion);
		gridMesh.UpdateGrid();
		
		return (int)inputHandler.FinalizeInput(viewportCamera, @event, isSelectionActive);
	}

	public override bool _ForwardCanvasGuiInput(InputEvent @event)
	{
		GD.Print("ForwardCanvasGuiInput");
		return base._ForwardCanvasGuiInput(@event);
	}

	/// <summary>
	/// Called when any input is detected.
	/// </summary>
	/// <param name="event"></param>
	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
	}

	public override void _ShortcutInput(InputEvent @event)
	{
		GD.Print("ShortcutInput");
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		GD.Print("UnhandledInput");
	}

	public override void _UnhandledKeyInput(InputEvent @event)
	{
		GD.Print("UnhandledKeyInput");
	}

	public override void _Forward3DDrawOverViewport(Control viewportControl)
	{
		GD.Print("Forward3DDrawOverViewport");
	}

	public override void _ForwardCanvasDrawOverViewport(Control viewportControl)
	{
		GD.Print("ForwardCanvasDrawOverViewport");
	}

	public override void _EnterTree()
	{
		rootView = dockScene.Instantiate<Control>();
	}

	public override void _ExitTree()
	{
		rootView.QueueFree();
		gridMesh?.Dispose();
		gridMesh = null;
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

	private void Enable(HexGridMap map)
	{
		if (enabled) return;
		enabled = true;
		
		hexGridMap = map;
		inputHandler = new InputHandler(hexGridMap);
		inputHandler.OnDeselectRequested += OnDeselectRequestedHandler;
		gridMesh = new GridMesh(hexGridMap, inputHandler);
		AddControlToDock(DockSlot.RightBl, rootView);
		view = rootView.GetNode<View>(".");
		view.UpdateList(hexGridMap.MeshLibrary);
		view.ItemList.ItemSelected += OnItemSelectedHandler;
	}

	private void Disable()
	{
		if (!enabled) return;
		enabled = false;
		
		RemoveControlFromDocks(rootView);
		view.ItemList.ItemSelected -= OnItemSelectedHandler;
		view.ItemList.Clear();
		gridMesh?.Dispose();
		gridMesh = null;
		OnDeselectRequestedHandler();
	}

	private void OnDeselectRequestedHandler()
	{
		view.ItemList.DeselectAll();
		inputHandler.OnHexCenterUpdated -= OnHexCenterUpdatedHandler;
		inputHandler.OnLeftMouseButtonPressed -= OnLeftMouseButtonPressedHandler;
		inputHandler.OnRightMouseButtonPressed -= OnRightMouseButtonPressedHandler;
		instanceRid.FreeRid();
		isSelectionActive = false;
	}

	private Rid instanceRid;
	private int selectedMeshIndex;
	
	private void OnItemSelectedHandler(long index)
	{
		isSelectionActive = true;
		selectedMeshIndex = (int)index;
		var mesh = hexGridMap.MeshLibrary.GetItemMesh((int)index);
		instanceRid = RenderingServer.InstanceCreate();
		RenderingServer.InstanceSetBase(instanceRid, mesh.GetRid());
		RenderingServer.InstanceSetScenario(instanceRid, hexGridMap.GetWorld3D().Scenario);
		inputHandler.OnHexCenterUpdated += OnHexCenterUpdatedHandler;
		inputHandler.OnLeftMouseButtonPressed += OnLeftMouseButtonPressedHandler;
		inputHandler.OnRightMouseButtonPressed += OnRightMouseButtonPressedHandler;
	}

	private void OnLeftMouseButtonPressedHandler()
	{
		if (!isSelectionActive) return;
		// TODO: Place the tile here.
	}

	private void OnRightMouseButtonPressedHandler()
	{
		if (!isSelectionActive) return;
		// TODO: Remove the tile here.
	}

	private void OnHexCenterUpdatedHandler(Vector3 hexCenter)
	{
		RenderingServer.InstanceSetTransform(instanceRid, new Transform3D(Basis.Identity, hexCenter));
	}
}
#endif
