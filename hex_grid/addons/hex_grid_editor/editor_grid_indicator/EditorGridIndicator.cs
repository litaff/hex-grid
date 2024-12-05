namespace addons.hex_grid_editor.editor_grid_indicator;

using Godot;
using HexGridMap.Mesh;
using provider;

public class EditorGridIndicator
{
    private readonly World3D world;
    private readonly EditorGridIndicatorView view;
    private readonly ILayerDataProvider layerDataProvider;
    private readonly IEditorHexPositionProvider inputProvider;
    private readonly Material material;

    private WireHexGridMesh? indicatorMesh;
    
    private int indicatorRadius;

    public EditorGridIndicator(World3D world, EditorGridIndicatorView view, ILayerDataProvider layerDataProvider,
        IEditorHexPositionProvider inputProvider)
    {
        this.world = world;
        this.view = view;
        this.layerDataProvider = layerDataProvider;
        this.inputProvider = inputProvider;
        material = WireHexGridMesh.GetStandardMaterial(new Color(0.8f, 0.5f, 0.1f));
    }
    
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
        indicatorMesh = new WireHexGridMesh(new HexGridMeshData(world, material), radius, true);
		
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