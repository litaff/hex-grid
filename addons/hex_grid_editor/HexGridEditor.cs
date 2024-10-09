#if TOOLS
namespace hex_grid.addons.hex_grid_editor;

using Godot;
using scripts.hex_grid;
using scripts.utils;
using HexGridMap = scripts.hex_grid.HexGridMap;

[Tool]
public partial class HexGridEditor : EditorPlugin
{
	private PackedScene dockScene = GD.Load<PackedScene>("res://addons/hex_grid_editor/hex_grid_editor.tscn");
	private Material lineMaterial = GD.Load<Material>("res://addons/hex_grid_editor/line_material.tres");
	private Material chunkMaterial = GD.Load<Material>("res://addons/hex_grid_editor/chunk_material.tres");
	private Material debugHexMaterial = GD.Load<Material>("res://addons/hex_grid_editor/debug_hex_material.tres");

	private Control rootView;
	private View view;
	
	private HexGridMap hexGridMap;
	private InputHandler inputHandler;
	private WireHexGridMesh gridMesh;
	private WireHexGridMesh chunkMesh;
	private PrimitiveHexGridMesh primitiveHexMesh;
	private bool isSelectionActive;
	private Rid selectedMeshInstanceRid;
	private int selectedMeshIndex;
	
	private bool enabled;
	
	private float CellSize => hexGridMap.CellSize;

	/// <summary>
	/// Called when cursor is in the 3D viewport.
	/// </summary>
	public override int _Forward3DGuiInput(Camera3D viewportCamera, InputEvent @event)
	{
		inputHandler.UpdateCursorPosition(viewportCamera, @event as InputEventMouseMotion);
		gridMesh.UpdateMesh(inputHandler.HexPosition.ToWorldPosition(CellSize));
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
		hexGridMap.OnPropertyChange += Reset;
		hexGridMap.Initialize();
		inputHandler = new InputHandler(hexGridMap.Position, CellSize);
		inputHandler.OnDeselectRequested += OnDeselectRequestedHandler;
		gridMesh = new WireHexGridMesh(hexGridMap.GetWorld3D(), CellSize, lineMaterial, hexGridMap.EditorGridSize, hexGridMap.EditorGridAlphaFalloff);
		UpdateChunk();
		primitiveHexMesh = new PrimitiveHexGridMesh(hexGridMap.GetWorld3D(), CellSize, debugHexMaterial);
		UpdatePrimitive();
		AddControlToDock(DockSlot.RightBl, rootView);
		view = rootView.GetNode<View>(".");
		view.MapResetButton.Confirmed += OnMapResetRequestedHandler;
		view.UpdateList(hexGridMap.MeshLibrary);
		view.ItemList.ItemSelected += OnItemSelectedHandler;
	}

	private void Disable()
	{
		if (!enabled) return;
		enabled = false;
		
		hexGridMap.OnPropertyChange -= Reset;
		RemoveControlFromDocks(rootView);
		view.MapResetButton.Confirmed -= OnMapResetRequestedHandler;
		view.ItemList.ItemSelected -= OnItemSelectedHandler;
		view.ItemList.Clear();
		gridMesh?.Dispose();
		gridMesh = null;
		chunkMesh?.Dispose();
		chunkMesh = null;
		primitiveHexMesh?.Dispose();
		primitiveHexMesh = null;
		OnDeselectRequestedHandler();
	}

	private void OnMapResetRequestedHandler()
	{
		hexGridMap.ResetMap();
		UpdatePrimitive();
	}

	private void Reset()
	{
		Disable();
		Enable(hexGridMap);
	}
	
	private void OnDeselectRequestedHandler()
	{
		view.ItemList.DeselectAll();
		inputHandler.OnHexCenterUpdated -= OnHexCenterUpdatedHandler;
		inputHandler.OnAddHexRequested -= OnAddHexRequestedHandler;
		inputHandler.OnRemoveHexRequest -= OnRemoveHexRequestHandler;
		selectedMeshInstanceRid.FreeRid();
		isSelectionActive = false;
	}

	private void OnItemSelectedHandler(long index)
	{
		OnDeselectRequestedHandler();
		
		isSelectionActive = true;
		selectedMeshIndex = (int)index;
		var mesh = hexGridMap.MeshLibrary.GetItemMesh((int)index);
		selectedMeshInstanceRid = RenderingServer.InstanceCreate();
		RenderingServer.InstanceSetBase(selectedMeshInstanceRid, mesh.GetRid());
		RenderingServer.InstanceSetScenario(selectedMeshInstanceRid, hexGridMap.GetWorld3D().Scenario);
		OnHexCenterUpdatedHandler(inputHandler.HexCenter);
		inputHandler.OnHexCenterUpdated += OnHexCenterUpdatedHandler;
		inputHandler.OnAddHexRequested += OnAddHexRequestedHandler;
		inputHandler.OnRemoveHexRequest += OnRemoveHexRequestHandler;
	}

	private void OnAddHexRequestedHandler()
	{
		if (!isSelectionActive) return;
		hexGridMap.AddHex(inputHandler.HexPosition, selectedMeshIndex);
		UpdatePrimitive();
	}

	private void OnRemoveHexRequestHandler()
	{
		if (!isSelectionActive) return;
		hexGridMap.RemoveHex(inputHandler.HexPosition);
		UpdatePrimitive();
	}

	private void UpdatePrimitive()
	{
		if (!hexGridMap.DisplayDebugHexes) return;
		primitiveHexMesh.UpdateMesh(hexGridMap.Map);
	}

	private void OnHexCenterUpdatedHandler(Vector3 hexCenter)
	{
		RenderingServer.InstanceSetTransform(selectedMeshInstanceRid, new Transform3D(Basis.Identity, hexCenter));
		
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
		chunkMesh ??= new WireHexGridMesh(hexGridMap.GetWorld3D(), CellSize, chunkMaterial, hexGridMap.ChunkSize, false);
		var hexesChunkPosition = inputHandler.HexPosition.ToChunkPosition(hexGridMap.ChunkSize);
		chunkMesh.UpdateMesh(hexesChunkPosition.FromChunkPosition(hexGridMap.ChunkSize).ToWorldPosition(CellSize));
	}
}
#endif
