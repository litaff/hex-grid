namespace hex_grid.addons.hex_grid_editor.mesh;

using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using hex_grid.scripts.hex_grid.vector;
using scripts.utils;

public class PrimitiveHexGridMesh
{
    private readonly Material defaultMaterial;
    private Rid meshRid;
    private Rid instanceRid;

    public PrimitiveHexGridMesh(HexGridMeshData meshData, Vector3 offset)
    {
        defaultMaterial = meshData.Material;
        instanceRid = RenderingServer.InstanceCreate();
        meshRid = RenderingServer.MeshCreate();
        RenderingServer.InstanceSetBase(instanceRid, meshRid);
        RenderingServer.InstanceSetScenario(instanceRid, meshData.World.Scenario);
        RenderingServer.InstanceSetTransform(instanceRid, new Transform3D(Basis.Identity, offset));
    }
    
    public void UpdateMesh(CubeHexVector[] positions)
    {
        var meshData = new Array();
        meshData.Resize((int)RenderingServer.ArrayType.Max);
        var meshVertices = new List<Vector3[]>();
        foreach (var position in positions)
        {
            var vertices = position.GetHexVertices();
            meshVertices.Add([vertices[0], vertices[1], vertices[2]]);
            meshVertices.Add([vertices[2], vertices[3], vertices[4]]);
            meshVertices.Add([vertices[4], vertices[5], vertices[0]]);
            meshVertices.Add([vertices[0], vertices[2], vertices[4]]);
        }
        meshData[(int)RenderingServer.ArrayType.Vertex] = meshVertices.SelectMany(x => x).ToArray();
        
        RenderingServer.MeshClear(meshRid);
        
        if (meshVertices.Count == 0) return;
        
        RenderingServer.MeshAddSurfaceFromArrays(meshRid, RenderingServer.PrimitiveType.Triangles, meshData);
        RenderingServer.MeshSurfaceSetMaterial(meshRid, 0, defaultMaterial.GetRid());
    }
    
    public void Dispose()
    {
        instanceRid.FreeRid();
        meshRid.FreeRid();
    }
}