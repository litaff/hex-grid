namespace hex_grid.addons.hex_grid_editor.editor_grid_indicator;

using Godot;
using mesh;
using provider;

public class EditorGridIndicator(
    World3D world, 
    EditorGridIndicatorView view, 
    ILayerDataProvider layerDataProvider, 
    IEditorHexPositionProvider inputProvider)
{
    private readonly Material lineMaterial = GD.Load<Material>("res://addons/hex_grid_editor/line_material.tres");

    private WireHexGridMesh indicatorMesh;
    
    private int indicatorRadius;

    public void Enable()
    {
        inputProvider.OnHexCenterUpdated += OnHexCenterUpdatedHandler;
        view.OnIndicatorSizeChanged += OnIndicatorSizeChangedHandler;
        view.Initialize(indicatorRadius);
    }
	
    public void Disable()
    {
        inputProvider.OnHexCenterUpdated -= OnHexCenterUpdatedHandler;
        view.OnIndicatorSizeChanged -= OnIndicatorSizeChangedHandler;
        view.Dispose();
        indicatorMesh?.Dispose();
        
        UpdateIndicatorMesh(inputProvider.HexCenter, 0);
    }

    private void OnHexCenterUpdatedHandler(Vector3 position)
    {
        UpdateIndicatorMesh(position, indicatorRadius);
    }

    private void OnIndicatorSizeChangedHandler(int radius)
    {
        indicatorRadius = radius > 0 ? radius : indicatorRadius;
		
        indicatorMesh?.Dispose();
        indicatorMesh = new WireHexGridMesh(new HexGridMeshData(world, lineMaterial), radius, true);
		
        UpdateIndicatorMesh(inputProvider.HexCenter, radius);
    }

    private void UpdateIndicatorMesh(Vector3 position, int radius)
    {
        indicatorMesh?.UpdateMesh(position + layerDataProvider.CurrentLayerOffset);
		
        if (radius > 0) return;
		
        indicatorMesh?.Dispose();
        indicatorMesh = null;
    }
}