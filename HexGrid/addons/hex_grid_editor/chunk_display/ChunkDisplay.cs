namespace addons.hex_grid_editor.chunk_display;

using Godot;
using HexGrid.Map;
using HexGrid.Map.Mesh;
using HexGrid.Map.Vector;
using provider;

public class ChunkDisplay
{
    private readonly World3D world;
    private readonly ChunkDisplayView view;
    private readonly ILayerDataProvider layerDataProvider;
    private readonly IEditorHexPositionProvider inputProvider;
    private readonly Material material;

    private WireMesh? chunkMesh;
    
    private bool chunkEnabled;

    public ChunkDisplay(World3D world, ChunkDisplayView view, ILayerDataProvider layerDataProvider,
        IEditorHexPositionProvider inputProvider)
    {
        this.world = world;
        this.view = view;
        this.layerDataProvider = layerDataProvider;
        this.inputProvider = inputProvider;
        material = WireMesh.GetStandardMaterial(Colors.Green);
    }

    public void Enable()
    {
        view.OnDisplayChunkRequested += OnChunkDisplayRequestedHandler;
        view.Initialize(chunkEnabled);
        inputProvider.OnHexCenterUpdated += OnHexCenterUpdatedHandler;
    }
    
    public void Disable()
    {
        inputProvider.OnHexCenterUpdated -= OnHexCenterUpdatedHandler;
        view.OnDisplayChunkRequested -= OnChunkDisplayRequestedHandler;
        view.Dispose();
        chunkMesh?.Dispose();
        chunkMesh = null;
    }

    private void OnChunkDisplayRequestedHandler(bool enabled)
    {
        chunkEnabled = enabled;
		
        if (enabled)
        {
            chunkMesh ??= new WireMesh(new MeshData(world, material), Properties.ChunkSize, false);
            UpdateChunkMesh();
        }
        else
        {
            chunkMesh?.Dispose();
            chunkMesh = null;
        }
    }

    private void OnHexCenterUpdatedHandler(Vector3 position)
    {
        UpdateChunkMesh();
    }

    private void UpdateChunkMesh()
    {
        var hexesChunkPosition = inputProvider.HexPosition.ToChunkPosition();
        //if (hexesChunkPosition == null) return;
        chunkMesh?.UpdateMesh(hexesChunkPosition.FromChunkPosition().ToWorldPosition() +
                              layerDataProvider.CurrentLayerOffset);
    }
}