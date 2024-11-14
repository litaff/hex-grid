namespace hex_grid.addons.hex_grid_editor.chunk_display;

using Godot;
using mesh;
using provider;
using scripts.hex_grid;
using scripts.hex_grid.vector;

public class ChunkDisplay(
    World3D world,
    ChunkDisplayView view,
    ILayerDataProvider layerDataProvider,
    IEditorHexPositionProvider inputProvider)
{	
    private readonly Material chunkMaterial = GD.Load<Material>("res://addons/hex_grid_editor/chunk_material.tres");

    private WireHexGridMesh chunkMesh;
    
    private bool chunkEnabled;

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
            chunkMesh ??= new WireHexGridMesh(new HexGridMeshData(world, chunkMaterial), HexGridData.Instance.ChunkSize, false);
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
        var hexesChunkPosition = inputProvider?.HexPosition.ToChunkPosition();
        if (hexesChunkPosition == null) return;
        chunkMesh?.UpdateMesh(hexesChunkPosition.Value.FromChunkPosition().ToWorldPosition() +
                              layerDataProvider.CurrentLayerOffset);
    }
}