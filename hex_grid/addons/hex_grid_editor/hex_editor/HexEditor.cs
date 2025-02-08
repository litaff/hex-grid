namespace addons.hex_grid_editor.hex_editor;

using System;
using Godot;
using GodotUtils.RidExtensions;
using HexGridMap;
using HexGridMap.Hex;
using provider;

public class HexEditor(
	IHexGridMapEditionProvider mapEditionProvider, 
	HexEditorView view, 
	IHexEditorInputProvider inputHandler)
	: ILayerDataProvider
{
	private Rid selectedMeshInstanceRid;
	private int selectedMeshIndex;
	private int rotationAngle;

	public int CurrentLayer { get; private set; }
	public bool IsSelectionActive { get; private set; }
	public Vector3 CurrentLayerOffset => CurrentLayer * Vector3.Up * HexGridProperties.LayerHeight;

	public void Enable()
	{
		inputHandler.OnDeselectRequested += OnDeselectRequestedHandler;
		inputHandler.OnPipetteRequested += OnPipetteRequestedHandler;
		inputHandler.OnDisplayAllLayersRequested += OnDisplayAllLayersRequestedHandler;
		view.MapResetButton.OnConfirmed += OnClearMapRequestedHandler;
		view.LayerResetButton.OnConfirmed += OnClearLayerRequestedHandler;
		view.OnMeshSelected += OnMeshSelectedHandler;
		view.OnLayerChanged += OnLayerChangedHandler;
		view.UpdateList(mapEditionProvider.MeshLibrary);
		view.Initialize(CurrentLayer);
	}
	
	public void Disable()
	{
		inputHandler.OnDeselectRequested -= OnDeselectRequestedHandler;
		inputHandler.OnPipetteRequested -= OnPipetteRequestedHandler;
		inputHandler.OnDisplayAllLayersRequested -= OnDisplayAllLayersRequestedHandler;
		view.MapResetButton.OnConfirmed -= OnClearMapRequestedHandler;
		view.LayerResetButton.OnConfirmed -= OnClearLayerRequestedHandler;
		view.OnMeshSelected -= OnMeshSelectedHandler;
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
		mapEditionProvider.HideLayers(layerIndex => layerIndex > CurrentLayer);
	}

	private void OnDisplayAllLayersRequestedHandler()
	{
		mapEditionProvider.HideLayers([]);
	}

	private void OnClearMapRequestedHandler()
	{
		mapEditionProvider.ClearMaps();
	}
	
	private void OnClearLayerRequestedHandler()
	{
		mapEditionProvider.ClearMap(CurrentLayer);
	}
	
	private void OnMeshSelectedHandler(int index)
	{
		OnDeselectRequestedHandler();
		
		if (!inputHandler.IsDisplayAllLayersHeld)
		{
			mapEditionProvider.HideLayers(layerIndex => layerIndex > CurrentLayer);
		}
		
		IsSelectionActive = true;
		selectedMeshIndex = index;
		var mesh = mapEditionProvider.MeshLibrary.GetItemMesh(index);
		selectedMeshInstanceRid = RenderingServer.InstanceCreate();
		RenderingServer.InstanceSetBase(selectedMeshInstanceRid, mesh.GetRid());
		RenderingServer.InstanceSetScenario(selectedMeshInstanceRid, mapEditionProvider.World3D.Scenario);
		UpdateSelectedHexMesh();
		inputHandler.OnHexCenterUpdated += OnHexCenterUpdatedHandler;
		inputHandler.OnAddHexRequested += OnAddHexRequestedHandler;
		inputHandler.OnRemoveHexRequest += OnRemoveHexRequestHandler;
		inputHandler.OnRotateRequested += OnRotateRequestedHandler;
	}

	private void OnDeselectRequestedHandler()
	{
		mapEditionProvider.HideLayers([]);
		view.MeshList.DeselectAll();
		inputHandler.OnHexCenterUpdated -= OnHexCenterUpdatedHandler;
		inputHandler.OnAddHexRequested -= OnAddHexRequestedHandler;
		inputHandler.OnRemoveHexRequest -= OnRemoveHexRequestHandler;
		inputHandler.OnRotateRequested -= OnRotateRequestedHandler;
		rotationAngle = 0;
		selectedMeshInstanceRid.RenderingServerFreeRid();
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
			mapEditionProvider.HideLayers(layerIndex => layerIndex > CurrentLayer);
		}
	}
	
	private void OnAddHexRequestedHandler()
	{
		if (!IsSelectionActive) return; // TODO: This might be useless, as the event is only triggered when selection is active
		var position = inputHandler.HexPosition;
		var meshData = new HexMeshData(selectedMeshIndex, -rotationAngle);
		var properties = view.HexProperties.GetProperties();
		var hex = new CubeHex(position.Q, position.R, properties, meshData);
		mapEditionProvider.AddHex(hex, CurrentLayer);
	}

	private void OnRemoveHexRequestHandler()
	{
		if (!IsSelectionActive) return; // TODO: This might be useless, as the event is only triggered when selection is active
		mapEditionProvider.RemoveHex(inputHandler.HexPosition, CurrentLayer);
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
		var hex = mapEditionProvider.GetHex(inputHandler.HexPosition, CurrentLayer);
		if (hex == null) return;

		view.HexProperties.SetFromProperties(hex.Properties);
		view.SelectMesh(hex.MeshData.MeshIndex);
		rotationAngle = -hex.MeshData.Rotation;
		UpdateSelectedHexMesh();
	}
}