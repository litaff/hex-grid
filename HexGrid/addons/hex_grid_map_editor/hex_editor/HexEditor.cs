namespace addons.hex_grid_map_editor.hex_editor;

using Godot;
using GodotUtils.RidExtensions;
using HexGrid.Map.Hex;

public class HexEditor(
	IHexGridMapEditionProvider mapEditionProvider, 
	HexEditorView view, 
	IHexEditorInputProvider inputHandler)
{
	private Rid selectedMeshInstanceRid;
	private int selectedMeshIndex;
	private int rotationAngle;

	public bool IsSelectionActive { get; private set; }

	public void Enable()
	{
		inputHandler.OnDeselectRequested += OnDeselectRequestedHandler;
		inputHandler.OnPipetteRequested += OnPipetteRequestedHandler;
		inputHandler.OnHeightChanged += OnHeightChangedHandler;
		inputHandler.OnDisplayAllLayersRequested += OnDisplayAllLayersRequestedHandler;
		view.MapResetButton.OnConfirmed += OnClearMapRequestedHandler;
		view.OnMeshSelected += OnMeshSelectedHandler;
		view.UpdateList(mapEditionProvider.MeshLibrary);
		view.Initialize();
	}
	
	public void Disable()
	{
		inputHandler.OnDeselectRequested -= OnDeselectRequestedHandler;
		inputHandler.OnPipetteRequested -= OnPipetteRequestedHandler;
		inputHandler.OnHeightChanged -= OnHeightChangedHandler;
		inputHandler.OnDisplayAllLayersRequested -= OnDisplayAllLayersRequestedHandler;
		view.MapResetButton.OnConfirmed -= OnClearMapRequestedHandler;
		view.OnMeshSelected -= OnMeshSelectedHandler;
		view.UpdateList(null);
		view.Dispose();
		
		OnDeselectRequestedHandler();
	}

	public void Deselect()
	{
		OnDeselectRequestedHandler();
	}
	
	private void OnHeightChangedHandler()
	{
		if (inputHandler.IsDisplayAllLayersHeld) return;
		mapEditionProvider.Occlude(position => position.H > inputHandler.HexPosition.H);
	}

	private void OnDisplayAllLayersRequestedHandler()
	{
		mapEditionProvider.Occlude(null);
	}

	private void OnClearMapRequestedHandler()
	{
		mapEditionProvider.ClearMaps();
	}
	
	private void OnMeshSelectedHandler(int index)
	{
		OnDeselectRequestedHandler();
		
		if (!inputHandler.IsDisplayAllLayersHeld)
		{
			mapEditionProvider.Occlude(position => position.H > inputHandler.HexPosition.H);
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
		mapEditionProvider.Occlude(null);
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
	}
	
	private void OnAddHexRequestedHandler()
	{
		if (!IsSelectionActive) return; // TODO: This might be useless, as the event is only triggered when selection is active
		var position = inputHandler.HexPosition;
		var meshData = new MeshData(selectedMeshIndex, -rotationAngle);
		var properties = view.HexProperties.GetProperties();
		var hex = new Hex(position.Q, position.R, inputHandler.HexPosition.H, properties, meshData);
		mapEditionProvider.AddHex(hex);
	}

	private void OnRemoveHexRequestHandler()
	{
		if (!IsSelectionActive) return; // TODO: This might be useless, as the event is only triggered when selection is active
		mapEditionProvider.RemoveHex(inputHandler.HexPosition);
	}
	
	private void OnRotateRequestedHandler()
	{
		rotationAngle = (rotationAngle + 60) % 360;
		UpdateSelectedHexMesh();
	}

	private void UpdateSelectedHexMesh()
	{
		RenderingServer.InstanceSetTransform(selectedMeshInstanceRid, 
			new Transform3D(Basis.Identity.Rotated(Vector3.Up, Mathf.DegToRad(-rotationAngle)), inputHandler.HexCenter));
	}
	
	private void OnPipetteRequestedHandler()
	{
		var hex = mapEditionProvider.GetHex(inputHandler.HexPosition);
		if (hex == null) return;

		view.HexProperties.SetFromProperties(hex.Properties);
		view.SelectMesh(hex.MeshData.MeshIndex);
		rotationAngle = -hex.MeshData.Rotation;
		UpdateSelectedHexMesh();
	}
}