#if TOOLS
namespace hex_grid.addons.hex_grid_editor;

using Godot;
using scripts;

[Tool]
public partial class HexGridEditor : EditorPlugin
{
	private PackedScene dockScene = GD.Load<PackedScene>("res://addons/hex_grid_editor/hex_grid_editor.tscn");
	private Material lineMaterial = GD.Load<Material>("res://addons/hex_grid_editor/line_material.tres");
	private Material chunkMaterial = GD.Load<Material>("res://addons/hex_grid_editor/chunk_material.tres");

	private Control rootView;
	private View view;
	
	private HexGridMap hexGridMap;
	private HexMapStorage mapStorage;
	private InputHandler inputHandler;
	private GridMesh gridMesh;
	private GridMesh chunkMesh;
	private bool isSelectionActive;
	private Rid instanceRid;
	private int selectedMeshIndex;
	
	private bool enabled;

	/// <summary>
	/// Called when cursor is in the 3D viewport.
	/// </summary>
	public override int _Forward3DGuiInput(Camera3D viewportCamera, InputEvent @event)
	{
		inputHandler.UpdateCursorPosition(viewportCamera, @event as InputEventMouseMotion);
		gridMesh.UpdateGrid(inputHandler.HexPosition, hexGridMap.EditorGridSize, hexGridMap.EditorGridAlphaFalloff);
		UpdateChunk();

		return (int)inputHandler.FinalizeInput(viewportCamera, @event, isSelectionActive);
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
		chunkMesh?.Dispose();
		chunkMesh = null;
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
		mapStorage = hexGridMap.InitializeStorage();
		inputHandler = new InputHandler(hexGridMap);
		inputHandler.OnDeselectRequested += OnDeselectRequestedHandler;
		gridMesh = new GridMesh(hexGridMap, lineMaterial);
		chunkMesh = new GridMesh(hexGridMap, chunkMaterial);
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
		chunkMesh?.Dispose();
		chunkMesh = null;
		OnDeselectRequestedHandler();
	}

	private void OnDeselectRequestedHandler()
	{
		view.ItemList.DeselectAll();
		inputHandler.OnHexCenterUpdated -= OnHexCenterUpdatedHandler;
		inputHandler.OnAddHexRequested -= OnAddHexRequestedHandler;
		inputHandler.OnRemoveHexRequest -= OnRemoveHexRequestHandler;
		instanceRid.FreeRid();
		isSelectionActive = false;
	}

	private void OnItemSelectedHandler(long index)
	{
		OnDeselectRequestedHandler();
		
		isSelectionActive = true;
		selectedMeshIndex = (int)index;
		var mesh = hexGridMap.MeshLibrary.GetItemMesh((int)index);
		instanceRid = RenderingServer.InstanceCreate();
		RenderingServer.InstanceSetBase(instanceRid, mesh.GetRid());
		RenderingServer.InstanceSetScenario(instanceRid, hexGridMap.GetWorld3D().Scenario);
		OnHexCenterUpdatedHandler(inputHandler.HexCenter);
		inputHandler.OnHexCenterUpdated += OnHexCenterUpdatedHandler;
		inputHandler.OnAddHexRequested += OnAddHexRequestedHandler;
		inputHandler.OnRemoveHexRequest += OnRemoveHexRequestHandler;
	}

	private void OnAddHexRequestedHandler()
	{
		if (!isSelectionActive) return;
		mapStorage.Add(inputHandler.HexPosition);
		hexGridMap.UpdateMesh();
	}

	private void OnRemoveHexRequestHandler()
	{
		if (!isSelectionActive) return;
		mapStorage.Remove(inputHandler.HexPosition);		
		hexGridMap.UpdateMesh();
	}

	private void OnHexCenterUpdatedHandler(Vector3 hexCenter)
	{
		RenderingServer.InstanceSetTransform(instanceRid, new Transform3D(Basis.Identity, hexCenter));
		
		if (inputHandler.IsAddHexHeld)
		{
			OnAddHexRequestedHandler();
		}
		if (inputHandler.IsRemoveHexHeld)
		{
			OnRemoveHexRequestHandler();
		}
	}

	private void UpdateChunk()
	{
		if (!hexGridMap.DisplayChunks)
		{
			chunkMesh?.Dispose();
			chunkMesh = null;
			return;
		}
		chunkMesh ??= new GridMesh(hexGridMap, chunkMaterial);
		var hexesChunkPosition = hexGridMap.ToChunkCoordinates(inputHandler.HexPosition);
		chunkMesh.UpdateGrid(hexGridMap.FromChunkCoordinates(hexesChunkPosition), hexGridMap.ChunkSize, false);
	}
}
#endif
