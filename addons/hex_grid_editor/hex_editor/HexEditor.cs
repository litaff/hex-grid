namespace hex_grid.addons.hex_grid_editor.hex_editor;

using System;
using Godot;
using provider;
using scripts.hex_grid;
using scripts.hex_grid.hex;
using scripts.utils;

public class HexEditor(
	HexGridMap gridMap, 
	HexEditorView view, 
	IHexEditorInputProvider inputHandler)
	: ILayerDataProvider
{
	private Rid selectedMeshInstanceRid;
	private int selectedMeshIndex;
	private int rotationAngle;
	private HexType selectedHexType;
	private BaseHexPropertiesView selectedPropertiesView;

	public int CurrentLayer { get; private set; }
	public bool IsSelectionActive { get; private set; }
	public Vector3 CurrentLayerOffset => CurrentLayer * Vector3.Up * gridMap.LayerHeight;

	public void Enable()
	{
		inputHandler.OnDeselectRequested += OnDeselectRequestedHandler;
		inputHandler.OnPipetteRequested += OnPipetteRequestedHandler;
		inputHandler.OnDisplayAllLayersRequested += OnDisplayAllLayersRequestedHandler;
		view.OnHexTypeSelected += OnHexTypeSelectedHandler;
		view.MapResetButton.Confirmed += OnClearMapRequestedHandler;
		view.LayerResetButton.Confirmed += OnClearLayerRequestedHandler;
		view.OnMeshSelected += OnMeshSelectedHandler;
		view.OnLayerChanged += OnLayerChangedHandler;
		view.Initialize(CurrentLayer);
	}
	
	public void Disable()
	{
		inputHandler.OnDeselectRequested -= OnDeselectRequestedHandler;
		inputHandler.OnPipetteRequested -= OnPipetteRequestedHandler;
		inputHandler.OnDisplayAllLayersRequested -= OnDisplayAllLayersRequestedHandler;
		view.MapResetButton.Confirmed -= OnClearMapRequestedHandler;
		view.LayerResetButton.Confirmed -= OnClearLayerRequestedHandler;
		view.OnMeshSelected -= OnMeshSelectedHandler;
		view.OnHexTypeSelected -= OnHexTypeSelectedHandler;
		view.OnLayerChanged -= OnLayerChangedHandler;
		view.UpdateList(null);
		view.Dispose();
		
		OnDeselectRequestedHandler();
	}

	public void Deselect()
	{
		OnDeselectRequestedHandler();
	}
	
	private void OnLayerChangedHandler(int layer)
	{
		CurrentLayer = layer;
		if (inputHandler.IsDisplayAllLayersHeld) return;
		gridMap.HideLayers(layerIndex => layerIndex > CurrentLayer);
	}

	private void OnDisplayAllLayersRequestedHandler()
	{
		gridMap.HideLayers([]);
	}

	private void OnClearMapRequestedHandler()
	{
		gridMap.ClearMap();
	}
	
	private void OnClearLayerRequestedHandler()
	{
		gridMap.ClearLayer(CurrentLayer);
	}
	
	private void OnHexTypeSelectedHandler(HexType hexType, BaseHexPropertiesView propertiesView)
	{
		OnDeselectRequestedHandler();
		selectedHexType = hexType;
		selectedPropertiesView = propertiesView;
		view.UpdateList(gridMap.MeshLibraries[hexType]);
	}
	
	private void OnMeshSelectedHandler(int index)
	{
		OnDeselectRequestedHandler();
		
		if (!inputHandler.IsDisplayAllLayersHeld)
		{
			gridMap.HideLayers(layerIndex => layerIndex > CurrentLayer);
		}
		
		IsSelectionActive = true;
		selectedMeshIndex = index;
		var mesh = gridMap.MeshLibraries[selectedHexType].GetItemMesh(index);
		selectedMeshInstanceRid = RenderingServer.InstanceCreate();
		RenderingServer.InstanceSetBase(selectedMeshInstanceRid, mesh.GetRid());
		RenderingServer.InstanceSetScenario(selectedMeshInstanceRid, gridMap.GetWorld3D().Scenario);
		UpdateSelectedHexMesh();
		inputHandler.OnHexCenterUpdated += OnHexCenterUpdatedHandler;
		inputHandler.OnAddHexRequested += OnAddHexRequestedHandler;
		inputHandler.OnRemoveHexRequest += OnRemoveHexRequestHandler;
		inputHandler.OnRotateRequested += OnRotateRequestedHandler;
	}

	private void OnDeselectRequestedHandler()
	{
		gridMap.HideLayers([]);
		view.MeshList.DeselectAll();
		inputHandler.OnHexCenterUpdated -= OnHexCenterUpdatedHandler;
		inputHandler.OnAddHexRequested -= OnAddHexRequestedHandler;
		inputHandler.OnRemoveHexRequest -= OnRemoveHexRequestHandler;
		inputHandler.OnRotateRequested -= OnRotateRequestedHandler;
		rotationAngle = 0;
		selectedMeshInstanceRid.FreeRid();
		IsSelectionActive = false;
	}
	
	private void OnHexCenterUpdatedHandler(Vector3 hexCenter)
	{
		UpdateSelectedHexMesh();
		
		if (inputHandler.IsAddHexHeld)
		{
			OnAddHexRequestedHandler();
		}
		if (inputHandler.IsRemoveHexHeld)
		{
			OnRemoveHexRequestHandler();
		}
		if (!inputHandler.IsDisplayAllLayersHeld)
		{
			gridMap.HideLayers(layerIndex => layerIndex > CurrentLayer);
		}
	}
	
	private void OnAddHexRequestedHandler()
	{
		if (!IsSelectionActive) return; // TODO: This might be useless, as the event is only triggered when selection is active
		var position = inputHandler.HexPosition;
		var meshData = new HexMeshData(selectedMeshIndex, -rotationAngle);
		var hex = selectedHexType switch
		{
			HexType.Elevated => new ElevatedHex(position.Q, position.R, meshData),
			HexType.Accessible => new AccessibleHex(position.Q, position.R, meshData),
			HexType.Base => new CubeHex(position.Q, position.R, meshData),
			_ => throw new ArgumentOutOfRangeException(nameof(selectedHexType), selectedHexType, null)
		};
		var spawnedHex = gridMap.AddHex(hex, CurrentLayer);
		selectedPropertiesView.Apply(spawnedHex);
	}

	private void OnRemoveHexRequestHandler()
	{
		if (!IsSelectionActive) return; // TODO: This might be useless, as the event is only triggered when selection is active
		gridMap.RemoveHex(inputHandler.HexPosition, CurrentLayer);
	}
	
	private void OnRotateRequestedHandler()
	{
		rotationAngle = (rotationAngle + 60) % 360;
		UpdateSelectedHexMesh();
	}

	private void UpdateSelectedHexMesh()
	{
		RenderingServer.InstanceSetTransform(selectedMeshInstanceRid, 
			new Transform3D(Basis.Identity.Rotated(Vector3.Up, Mathf.DegToRad(-rotationAngle)), 
			inputHandler.HexCenter + CurrentLayerOffset));
	}
	
	private void OnPipetteRequestedHandler()
	{
		var hex = gridMap.GetHex(inputHandler.HexPosition, CurrentLayer);
		if (hex == null) return;
		
		view.SetCurrentHexType(hex.Type);
		selectedPropertiesView.SetFrom(hex);
		view.SelectMesh(hex.MeshData.MeshIndex);
		rotationAngle = -hex.MeshData.Rotation;
		UpdateSelectedHexMesh();
	}
}