#if TOOLS
namespace hex_grid.addons.hex_grid_editor;

using System.Linq;
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
	private Material fovMaterial = GD.Load<Material>("res://addons/hex_grid_editor/fov_material.tres");

	private Control rootView;
	private views.View view;
	
	private HexGridMap hexGridMap;
	private InputHandler inputHandler;
	private WireHexGridMesh gridMesh;
	private WireHexGridMesh chunkMesh;
	private PrimitiveHexGridMesh debugMesh;
	private PrimitiveHexGridMesh fovMesh;
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
		UpdateFovMesh();

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
		debugMesh = new PrimitiveHexGridMesh(hexGridMap.GetWorld3D(), CellSize, debugHexMaterial, Vector3.Zero);
		fovMesh = new PrimitiveHexGridMesh(hexGridMap.GetWorld3D(), CellSize, fovMaterial, Vector3.Up * 0.01f);
		UpdateDebugMesh();
		AddControlToDock(DockSlot.RightBl, rootView);
		view = rootView.GetNode<views.View>(".");
		view.OnHexTypeSelected += OnHexTypeSelectedHandler;
		view.Initialize();
		view.MapResetButton.Confirmed += OnMapResetRequestedHandler;
		view.MeshList.ItemSelected += OnMeshSelectedHandler;
	}

	private void Disable()
	{
		if (!enabled) return;
		enabled = false;
		
		hexGridMap.OnPropertyChange -= Reset;
		RemoveControlFromDocks(rootView);
		view.MapResetButton.Confirmed -= OnMapResetRequestedHandler;
		view.MeshList.ItemSelected -= OnMeshSelectedHandler;
		view.OnHexTypeSelected -= OnHexTypeSelectedHandler;
		view.UpdateList(null);
		view.Dispose();
		gridMesh?.Dispose();
		gridMesh = null;
		chunkMesh?.Dispose();
		chunkMesh = null;
		debugMesh?.Dispose();
		debugMesh = null;
		fovMesh?.Dispose();
		fovMesh = null;
		OnDeselectRequestedHandler();
	}

	private HexType selectedHexType;
	private views.BaseHexPropertiesView selectedPropertiesView;
	
	private void OnHexTypeSelectedHandler(HexType hexType, views.BaseHexPropertiesView propertiesView)
	{
		OnDeselectRequestedHandler();
		selectedHexType = hexType;
		selectedPropertiesView = propertiesView;
		view.UpdateList(hexGridMap.MeshLibraries[hexType]);
	}

	private void OnMapResetRequestedHandler()
	{
		hexGridMap.ResetMap();
		UpdateDebugMesh();
	}

	private void Reset()
	{
		Disable();
		Enable(hexGridMap);
	}
	
	private void OnDeselectRequestedHandler()
	{
		view.MeshList.DeselectAll();
		inputHandler.OnHexCenterUpdated -= OnHexCenterUpdatedHandler;
		inputHandler.OnAddHexRequested -= OnAddHexRequestedHandler;
		inputHandler.OnRemoveHexRequest -= OnRemoveHexRequestHandler;
		selectedMeshInstanceRid.FreeRid();
		isSelectionActive = false;
	}

	private void OnMeshSelectedHandler(long index)
	{
		OnDeselectRequestedHandler();
		
		isSelectionActive = true;
		selectedMeshIndex = (int)index;
		var mesh = hexGridMap.MeshLibraries[selectedHexType].GetItemMesh((int)index);
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
		var spawnedHex = hexGridMap.AddHex(inputHandler.HexPosition, selectedMeshIndex, selectedHexType);
		selectedPropertiesView.Apply(spawnedHex);
		UpdateDebugMesh();
	}

	private void OnRemoveHexRequestHandler()
	{
		if (!isSelectionActive) return;
		hexGridMap.RemoveHex(inputHandler.HexPosition);
		UpdateDebugMesh();
	}

	private void UpdateDebugMesh()
	{
		if (!hexGridMap.DisplayDebugHexes) return;
		debugMesh.UpdateMesh(hexGridMap.Map.Select(hex => hex.Position).ToArray());
	}
	
	private void UpdateFovMesh()
	{
		if (!hexGridMap.DisplayFieldOfView) return;
		
		fovMesh.UpdateMesh(hexGridMap.GetVisiblePositions(inputHandler.HexPosition, hexGridMap.FieldOfViewRadius));
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
