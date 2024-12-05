namespace addons.hex_grid_editor.fov_display;

using Godot;
using HexGridMap.Fov;
using HexGridMap.Mesh;
using provider;

public class FovDisplay
{
    private readonly World3D world;
    private readonly FovDisplayView displayView;
    private readonly IFovProvider fovProvider;
    private readonly ILayerDataProvider layerDataProvider;
    private readonly IEditorHexPositionProvider inputProvider;
    private readonly Material material;
    
    private PrimitiveHexGridMesh? fovMesh;
    private int fovRadius;
    private bool fovEnabled;

    public FovDisplay(World3D world, FovDisplayView displayView, IFovProvider fovProvider, 
        ILayerDataProvider layerDataProvider, IEditorHexPositionProvider inputProvider)
    {
        this.world = world;
        this.displayView = displayView;
        this.fovProvider = fovProvider;
        this.layerDataProvider = layerDataProvider;
        this.inputProvider = inputProvider;
        material = PrimitiveHexGridMesh.GetStandardMaterial(new Color(0f, 1f, 0f, 0.5f));
    }

    public void Enable()
    {
        inputProvider.OnHexCenterUpdated += OnHexCenterUpdatedHandler;
        displayView.OnFovDisplayRequested += OnFovDisplayRequestedHandler;
        displayView.Initialize(fovEnabled, fovRadius);
    }

    public void Disable()
    {
        inputProvider.OnHexCenterUpdated -= OnHexCenterUpdatedHandler;
        displayView.OnFovDisplayRequested -= OnFovDisplayRequestedHandler;
        displayView.Dispose();
        UpdateFovMesh(0);
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
		
        fovMesh ??= new PrimitiveHexGridMesh(new HexGridMeshData(world, material), Vector3.Up * 0.01f + layerDataProvider.CurrentLayerOffset);
        fovMesh.UpdateMesh(fovProvider.GetVisiblePositions(inputProvider.HexPosition, radius, layerDataProvider.CurrentLayer));
    }

    private void OnHexCenterUpdatedHandler(Vector3 position)
    {
        UpdateFovMesh(fovRadius);
    }
}