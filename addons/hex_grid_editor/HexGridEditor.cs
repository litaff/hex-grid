#if TOOLS
namespace hex_grid.addons.hex_grid_editor;

using System;
using Godot;
using scripts.hex_grid;
using scripts.hex_grid.hex;
using scripts.utils;
using views;
using HexGridMap = scripts.hex_grid.HexGridMap;

[Tool]
public partial class HexGridEditor : EditorPlugin
{
	private PackedScene editorScene = GD.Load<PackedScene>("res://addons/hex_grid_editor/hex_grid_editor.tscn");
	private Material lineMaterial = GD.Load<Material>("res://addons/hex_grid_editor/line_material.tres");
	private Material chunkMaterial = GD.Load<Material>("res://addons/hex_grid_editor/chunk_material.tres");
	private Material fovMaterial = GD.Load<Material>("res://addons/hex_grid_editor/fov_material.tres");

	private Control rootView;
	private HexGridEditorView view;
	
	private HexGridMap hexGridMap;
	private InputHandler inputHandler;
	
	private WireHexGridMesh indicatorMesh;
	private int indicatorRadius;
	
	private WireHexGridMesh chunkMesh;
	private bool chunkEnabled;
	
	private PrimitiveHexGridMesh fovMesh;
	private int fovRadius;
	private bool fovEnabled;
	
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
		UpdateIndicatorMesh(indicatorRadius);
		UpdateChunkMesh();
		UpdateFovMesh(fovRadius);

		return (int)inputHandler.FinalizeInput(viewportCamera, @event, isSelectionActive);
	}

	public override void _EnterTree()
	{
		rootView = editorScene.Instantiate<Control>();
	}

	public override void _ExitTree()
	{
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

	private void Enable(HexGridMap map)
	{
		if (enabled) return;
		enabled = true;
		
		hexGridMap = map;
		hexGridMap.OnPropertyChange += Reset;
		hexGridMap.Initialize();
		
		inputHandler = new InputHandler(hexGridMap.Position, CellSize);
		inputHandler.OnDeselectRequested += OnDeselectRequestedHandler;
		inputHandler.OnPipetteRequested += OnPipetteRequestedHandler;
		
		view = rootView.GetNode<HexGridEditorView>(".");

		view.HexEditor.OnHexTypeSelected += OnHexTypeSelectedHandler;
		view.HexEditor.MapResetButton.Confirmed += OnClearMapRequestedHandler;
		view.HexEditor.OnMeshSelected += OnMeshSelectedHandler;
		view.HexEditor.Initialize();

		view.DebugFov.OnFovDisplayRequested += OnFovDisplayRequestedHandler;
		view.DebugFov.Initialize(fovEnabled, fovRadius);

		view.IndicatorGrid.OnIndicatorSizeChanged += OnIndicatorSizeChangedHandler;
		view.IndicatorGrid.Initialize(indicatorRadius);
		
		view.ChunkDisplay.OnDisplayChunkRequested += OnChunkDisplayRequestedHandler;
		view.ChunkDisplay.Initialize(chunkEnabled);
		
		view.TabContainer.TabChanged += OnTabChangedHandler;
		
		AddControlToDock(DockSlot.RightBl, rootView);
	}

	private void Disable()
	{
		if (!enabled) return;
		enabled = false;

		RemoveControlFromDocks(rootView);
		
		hexGridMap.OnPropertyChange -= Reset;
		
		inputHandler.OnDeselectRequested -= OnDeselectRequestedHandler;
		inputHandler.OnPipetteRequested -= OnPipetteRequestedHandler;
		
		UpdateIndicatorMesh(0);
		
		chunkMesh?.Dispose();
		chunkMesh = null;
		
		UpdateFovMesh(0);

		view.TabContainer.TabChanged -= OnTabChangedHandler;
		
		view.ChunkDisplay.OnDisplayChunkRequested -= OnChunkDisplayRequestedHandler;
		view.ChunkDisplay.Dispose();
		
		view.IndicatorGrid.OnIndicatorSizeChanged -= OnIndicatorSizeChangedHandler;
		view.IndicatorGrid.Dispose();
		
		view.DebugFov.OnFovDisplayRequested -= OnFovDisplayRequestedHandler;
		view.DebugFov.Dispose();
		
		view.HexEditor.MapResetButton.Confirmed -= OnClearMapRequestedHandler;
		view.HexEditor.OnMeshSelected -= OnMeshSelectedHandler;
		view.HexEditor.OnHexTypeSelected -= OnHexTypeSelectedHandler;
		view.HexEditor.UpdateList(null);
		view.HexEditor.Dispose();
		
		OnDeselectRequestedHandler();
	}

	private void OnTabChangedHandler(long index)
	{
		if (index != 0)
		{
			OnDeselectRequestedHandler();
		}
	}
	
	private HexType selectedHexType;
	private BaseHexPropertiesView selectedPropertiesView;
	
	private void OnHexTypeSelectedHandler(HexType hexType, BaseHexPropertiesView propertiesView)
	{
		OnDeselectRequestedHandler();
		selectedHexType = hexType;
		selectedPropertiesView = propertiesView;
		view.HexEditor.UpdateList(hexGridMap.MeshLibraries[hexType]);
	}

	private void OnClearMapRequestedHandler()
	{
		hexGridMap.ResetMap();
	}

	private void Reset()
	{
		Disable();
		Enable(hexGridMap);
	}
	
	private void OnDeselectRequestedHandler()
	{
		view.HexEditor.MeshList.DeselectAll();
		inputHandler.OnHexCenterUpdated -= OnHexCenterUpdatedHandler;
		inputHandler.OnAddHexRequested -= OnAddHexRequestedHandler;
		inputHandler.OnRemoveHexRequest -= OnRemoveHexRequestHandler;
		selectedMeshInstanceRid.FreeRid();
		isSelectionActive = false;
	}

	private void OnMeshSelectedHandler(int index)
	{
		OnDeselectRequestedHandler();
		
		isSelectionActive = true;
		selectedMeshIndex = index;
		var mesh = hexGridMap.MeshLibraries[selectedHexType].GetItemMesh(index);
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
		hexGridMap.Storage.Save();
	}

	private void OnRemoveHexRequestHandler()
	{
		if (!isSelectionActive) return;
		hexGridMap.RemoveHex(inputHandler.HexPosition);
		hexGridMap.Storage.Save();
	}
	
	private void OnIndicatorSizeChangedHandler(int radius)
	{
		indicatorRadius = radius > 0 ? radius : indicatorRadius;
		
		indicatorMesh?.Dispose();
		indicatorMesh = new WireHexGridMesh(hexGridMap.GetWorld3D(), CellSize, lineMaterial, radius, true);
		
		UpdateIndicatorMesh(radius);
	}

	private void UpdateIndicatorMesh(int radius)
	{
		indicatorMesh?.UpdateMesh(inputHandler.HexPosition.ToWorldPosition(CellSize));
		
		if (radius > 0) return;
		
		indicatorMesh?.Dispose();
		indicatorMesh = null;
	}
	
	private void OnFovDisplayRequestedHandler(int radius)
	{
		fovRadius = radius > 0 ? radius : fovRadius;
		fovEnabled = radius > 0;
		
		UpdateFovMesh(radius);
	}
	
	private void UpdateFovMesh(int radius)
	{
		if (!fovEnabled || radius <= 0)
		{
			fovMesh?.Dispose();
			fovMesh = null;
			return;
		}
		
		fovMesh ??= new PrimitiveHexGridMesh(hexGridMap.GetWorld3D(), CellSize, fovMaterial, Vector3.Up * 0.01f);
		fovMesh.UpdateMesh(hexGridMap.GetVisiblePositions(inputHandler.HexPosition, radius));
	}

	private void OnChunkDisplayRequestedHandler(bool enabled)
	{
		chunkEnabled = enabled;
		
		if (enabled)
		{
			chunkMesh ??= new WireHexGridMesh(hexGridMap.GetWorld3D(), CellSize, chunkMaterial, hexGridMap.ChunkSize, false);
			UpdateChunkMesh();
		}
		else
		{
			chunkMesh?.Dispose();
			chunkMesh = null;
		}
	}
	
	private void UpdateChunkMesh()
	{
		var hexesChunkPosition = inputHandler.HexPosition.ToChunkPosition(hexGridMap.ChunkSize);
		chunkMesh?.UpdateMesh(hexesChunkPosition.FromChunkPosition(hexGridMap.ChunkSize).ToWorldPosition(CellSize));
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
	
	private void OnPipetteRequestedHandler()
	{
		var hex = hexGridMap.Storage.Get(inputHandler.HexPosition);
		if (hex == null) return;
		view.TabContainer.CurrentTab = 0;
		view.HexEditor.SetCurrentHexType(hex.Type);
		selectedPropertiesView.SetFrom(hex);
		view.HexEditor.SelectMesh(hex.LibraryIndex);
	}
}
#endif
