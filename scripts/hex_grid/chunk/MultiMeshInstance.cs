namespace hex_grid.scripts.hex_grid.chunk;

using System.Collections.Generic;
using Godot;
using utils;

public class MultiMeshInstance
{
    private Rid instanceRid;
    private Rid multiMeshRid;
    
    public MultiMeshInstance(Mesh mesh, Vector3 position, List<Transform3D> instances, World3D scenario)
    {
        instanceRid = RenderingServer.InstanceCreate();
        multiMeshRid = RenderingServer.MultimeshCreate();
        
        RenderingServer.MultimeshSetMesh(multiMeshRid, mesh.GetRid());
        RenderingServer.MultimeshAllocateData(multiMeshRid, instances.Count, RenderingServer.MultimeshTransformFormat.Transform3D);
        RenderingServer.MultimeshSetVisibleInstances(multiMeshRid, instances.Count);
        
        for (var i = 0; i < instances.Count; i++)
        {
            RenderingServer.MultimeshInstanceSetTransform(multiMeshRid, i, instances[i]);
        }

        RenderingServer.InstanceSetBase(instanceRid, multiMeshRid);
        RenderingServer.InstanceSetScenario(instanceRid, scenario.Scenario);
        RenderingServer.InstanceSetTransform(instanceRid, new Transform3D(Basis.Identity, position));
    }

    public void Dispose()
    {
        multiMeshRid.FreeRid();
        instanceRid.FreeRid();
    }
    
    ~MultiMeshInstance()
    {
        Dispose();
    }
}