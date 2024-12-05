namespace addons.hex_grid_editor.hex_editor;

using System;
using Godot;
using GodotUtils.RidExtensions;
using hex_grid_map;
using hex_grid_map.hex;
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
	private HexType selectedHexType;
	private BaseHexPropertiesView? selectedPropertiesView;

	public int CurrentLayer { get; private set; }
	public bool IsSelectionActive { get; private set; }
	public Vector3 CurrentLayerOffset => CurrentLayer * Vector3.Up * HexGridData.Instance.LayerHeight;

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
	
	private void OnHexTypeSelectedHandler(HexType hexType, BaseHexPropertiesView propertiesView)
	{
		OnDeselectRequestedHandler();
		selectedHexType = hexType;
		selectedPropertiesView = propertiesView;
		view.UpdateList(mapEditionProvider.MeshLibraries[hexType]);
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
		var mesh = mapEditionProvider.MeshLibraries[selectedHexType].GetItemMesh(index);
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
		var hex = selectedHexType switch
		{
			HexType.Elevated => new ElevatedHex(position.Q, position.R, meshData),
			HexType.Accessible => new AccessibleHex(position.Q, position.R, meshData),
			HexType.Base => new CubeHex(position.Q, position.R, meshData),
			_ => throw new ArgumentOutOfRangeException(nameof(selectedHexType), selectedHexType, null)
		};
		if (selectedPropertiesView != null)
		{
			selectedPropertiesView.Apply(hex);
		}
		else
		{
			GD.PushError("[HexEditor] Didn't apply properties, property view was null.");
		}
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
		
		view.SetCurrentHexType(hex.Type);
		if (selectedPropertiesView != null)
		{
			selectedPropertiesView.SetFrom(hex);
		}
		else
		{
			GD.PushError("[HexEditor] Didn't set properties from hex, property view was null.");
		}
		view.SelectMesh(hex.MeshData.MeshIndex);
		rotationAngle = -hex.MeshData.Rotation;
		UpdateSelectedHexMesh();
	}
}