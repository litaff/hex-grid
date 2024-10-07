#if TOOLS
namespace hex_grid.addons.hex_grid_editor;

using Godot;
using scripts;
using scripts.hex_grid;
using scripts.utils;
using HexGridMap = scripts.hex_grid.HexGridMap;

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
	private HexGridWireMesh gridWireMesh;
	private HexGridWireMesh chunkWireMesh;
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
		gridWireMesh.UpdateMesh(inputHandler.HexPosition, hexGridMap.EditorGridSize, hexGridMap.EditorGridAlphaFalloff);
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
		gridWireMesh?.Dispose();
		gridWireMesh = null;
		chunkWireMesh?.Dispose();
		chunkWireMesh = null;
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
		inputHandler = new InputHandler(hexGridMap.Position, hexGridMap.CellSize);
		inputHandler.OnDeselectRequested += OnDeselectRequestedHandler;
		gridWireMesh = new HexGridWireMesh(hexGridMap.GetWorld3D(), hexGridMap.CellSize, lineMaterial);
		chunkWireMesh = new HexGridWireMesh(hexGridMap.GetWorld3D(), hexGridMap.CellSize, chunkMaterial);
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
		gridWireMesh?.Dispose();
		gridWireMesh = null;
		chunkWireMesh?.Dispose();
		chunkWireMesh = null;
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
			chunkWireMesh?.Dispose();
			chunkWireMesh = null;
			return;
		}
		chunkWireMesh ??= new HexGridWireMesh(hexGridMap.GetWorld3D(), hexGridMap.CellSize, chunkMaterial);
		var hexesChunkPosition = inputHandler.HexPosition.ToChunkPosition(hexGridMap.ChunkSize);
		chunkWireMesh.UpdateMesh(hexesChunkPosition.FromChunkPosition(hexGridMap.ChunkSize), hexGridMap.ChunkSize,
			false);
	}
}
#endif
