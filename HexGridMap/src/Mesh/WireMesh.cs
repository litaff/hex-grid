namespace HexGrid.Map.Mesh;

using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotUtils.RidExtensions;
using Vector;
using Array = Godot.Collections.Array;

public class WireMesh
{
	private Rid meshRid;
	private Rid instanceRid;
	
	public WireMesh(MeshData meshData, int radius, bool useAlphaFalloff)
	{
		instanceRid = RenderingServer.InstanceCreate();
		meshRid = RenderingServer.MeshCreate();
		RenderingServer.InstanceSetBase(instanceRid, meshRid);
		RenderingServer.InstanceSetScenario(instanceRid, meshData.World.Scenario);
		RenderingServer.InstanceSetTransform(instanceRid, new Transform3D(Basis.Identity, Vector3.Zero));
		RenderingServer.MeshAddSurfaceFromArrays(meshRid, RenderingServer.PrimitiveType.Lines,
			GetGridMeshData(radius, useAlphaFalloff));
		RenderingServer.MeshSurfaceSetMaterial(meshRid, 0, meshData.Material.GetRid());
	}

	public void UpdateMesh(Vector3 position)
	{
		if (!instanceRid.IsValid) return;
		RenderingServer.InstanceSetTransform(instanceRid, new Transform3D(Basis.Identity, position));
	}
	
	public static Material GetStandardMaterial(Color color)
	{
		BaseMaterial3D material = new StandardMaterial3D();
		material.AlbedoColor = color;
		material.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
		material.ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;
		material.VertexColorUseAsAlbedo = true;
		material.VertexColorIsSrgb = true;
		return material;
	}
    
	public void Dispose()
	{
		instanceRid.RenderingServerFreeRid();
		meshRid.RenderingServerFreeRid();
	}

	private Array GetGridMeshData(int radius, bool useAlphaFalloff)
	{
		var meshData = new Array();
		meshData.Resize((int)RenderingServer.ArrayType.Max);
			
		var hexes = HexVector.Zero.GetSpiral(radius);
		var meshVertices = new List<Vector3[]>();
		var meshColors = new List<Color[]>();

		foreach (var hex in hexes)
		{
			var vertices = hex.GetHexVertices();
			meshVertices.Add([vertices[0], vertices[1]]);
			meshColors.Add([
				GetVertexColor(vertices[0], radius, useAlphaFalloff),
				GetVertexColor(vertices[1], radius, useAlphaFalloff)
			]);
			
			meshVertices.Add([vertices[1], vertices[2]]);
			meshColors.Add([
				GetVertexColor(vertices[1], radius, useAlphaFalloff),
				GetVertexColor(vertices[2], radius, useAlphaFalloff)
			]);
			
			meshVertices.Add([vertices[2], vertices[3]]);
			meshColors.Add([
				GetVertexColor(vertices[2], radius, useAlphaFalloff),
				GetVertexColor(vertices[3], radius, useAlphaFalloff)
			]);
			
			meshVertices.Add([vertices[3], vertices[4]]);
			meshColors.Add([
				GetVertexColor(vertices[3], radius, useAlphaFalloff),
				GetVertexColor(vertices[4], radius, useAlphaFalloff)
			]);
			
			meshVertices.Add([vertices[4], vertices[5]]);
			meshColors.Add([
				GetVertexColor(vertices[4], radius, useAlphaFalloff),
				GetVertexColor(vertices[5], radius, useAlphaFalloff)
			]);
			
			meshVertices.Add([vertices[5], vertices[0]]);
			meshColors.Add([
				GetVertexColor(vertices[5], radius, useAlphaFalloff),
				GetVertexColor(vertices[0], radius, useAlphaFalloff)
			]);
		}
			
		meshData[(int)RenderingServer.ArrayType.Vertex] = meshVertices.SelectMany(x => x).ToArray();
		meshData[(int)RenderingServer.ArrayType.Color] = meshColors.SelectMany(x => x).ToArray();

		return meshData;
	}

	private Color GetVertexColor(Vector3 vertex, int radius, bool useAlphaFalloff)
	{
		return new Color(1f, 1f, 1f, 
			useAlphaFalloff ?
				Mathf.Pow(Mathf.Max(0, 1f - vertex.DistanceTo(HexVector.Zero.ToWorldPosition()) / radius), 2) :
				1f);
	}
}