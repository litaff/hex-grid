namespace hex_grid.scripts.hex_grid.mesh;

using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using utils;

public class PrimitiveHexGridMesh
{
    private readonly HexGridMap gridMap;
    private readonly Material defaultMaterial;
    private Rid meshRid;
    private Rid instanceRid;

    public PrimitiveHexGridMesh(HexGridMap gridMap, Material defaultMaterial)
    {
        this.gridMap = gridMap;
        this.defaultMaterial = defaultMaterial;
    }
    
    public void UpdateMesh(CubeHex[] hexes)
    {
        if (!meshRid.IsValid)
        {
            meshRid = RenderingServer.MeshCreate();
        }
        if (!instanceRid.IsValid)
        {
            instanceRid = RenderingServer.InstanceCreate();
        }
        if (instanceRid.IsValid && meshRid.IsValid)
        {
            RenderingServer.InstanceSetBase(instanceRid, meshRid);
            RenderingServer.InstanceSetScenario(instanceRid, gridMap.GetWorld3D().Scenario);
            RenderingServer.InstanceSetTransform(instanceRid, new Transform3D(Basis.Identity, Vector3.Zero));
        }
        var meshData = new Array();
        meshData.Resize((int)RenderingServer.ArrayType.Max);
        var meshVertices = new List<Vector3[]>();
        foreach (var hex in hexes)
        {
            var position = gridMap.GetWorldPosition(hex.Position);
            var vertices = gridMap.GetHexVertices(position);
            meshVertices.Add(new[] { vertices[0], vertices[1], vertices[2] });
            meshVertices.Add(new[] { vertices[2], vertices[3], vertices[4] });
            meshVertices.Add(new[] { vertices[4], vertices[5], vertices[0] });
            meshVertices.Add(new[] { vertices[0], vertices[2], vertices[4] });
        }
        meshData[(int)RenderingServer.ArrayType.Vertex] = meshVertices.SelectMany(x => x).ToArray();
        
        RenderingServer.MeshClear(meshRid);
        
        if (meshVertices.Count == 0)
        {
            return;
        }
        
        RenderingServer.MeshAddSurfaceFromArrays(meshRid, RenderingServer.PrimitiveType.Triangles, meshData);
        RenderingServer.MeshSurfaceSetMaterial(meshRid, 0, defaultMaterial.GetRid());
    }
    
    public void Dispose()
    {
        RenderingServer.MeshClear(meshRid);
        instanceRid.FreeRid();
        meshRid.FreeRid();
    }
}