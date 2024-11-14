namespace hex_grid.addons.hex_grid_editor.fov_display;

using Godot;
using mesh;
using provider;
using scripts.hex_grid.fov;

public class FovDisplay(
    World3D world,
    FovDisplayView displayView,
    IFovProvider fovProvider,
    ILayerDataProvider layerDataProvider,
    IEditorHexPositionProvider inputProvider)
{	
    private readonly Material fovMaterial = GD.Load<Material>("res://addons/hex_grid_editor/fov_material.tres");

    private PrimitiveHexGridMesh fovMesh;
    
    private int fovRadius;
    private bool fovEnabled;

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
		
        fovMesh ??= new PrimitiveHexGridMesh(new HexGridMeshData(world, fovMaterial), Vector3.Up * 0.01f + layerDataProvider.CurrentLayerOffset);
        fovMesh.UpdateMesh(fovProvider.GetVisiblePositions(inputProvider.HexPosition, radius, layerDataProvider.CurrentLayer));
    }

    private void OnHexCenterUpdatedHandler(Vector3 position)
    {
        UpdateFovMesh(fovRadius);
    }
}