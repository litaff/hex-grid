namespace hex_grid.addons.hex_grid_editor;

using System.Collections.Generic;
using System.Linq;
using Godot;
using scripts.hex_grid;
using scripts.hex_grid.vector;
using scripts.utils;
using Array = Godot.Collections.Array;

public class WireHexGridMesh
{
	private Rid gridMeshRid;
	private Rid gridInstanceRid;
	
	public WireHexGridMesh(World3D scenario, float cellSize, Material material, int radius, bool useAlphaFalloff)
	{
		gridInstanceRid = RenderingServer.InstanceCreate();
		gridMeshRid = RenderingServer.MeshCreate();
		RenderingServer.InstanceSetBase(gridInstanceRid, gridMeshRid);
		RenderingServer.InstanceSetScenario(gridInstanceRid, scenario.Scenario);
		RenderingServer.InstanceSetTransform(gridInstanceRid, new Transform3D(Basis.Identity, Vector3.Zero));
		RenderingServer.MeshAddSurfaceFromArrays(gridMeshRid, RenderingServer.PrimitiveType.Lines,
			GetGridMeshData(cellSize, radius, useAlphaFalloff));
		RenderingServer.MeshSurfaceSetMaterial(gridMeshRid, 0, material.GetRid());
	}

	public void UpdateMesh(Vector3 position)
	{
		RenderingServer.InstanceSetTransform(gridInstanceRid, new Transform3D(Basis.Identity, position));
	}
    
	public void Dispose()
	{
		gridInstanceRid.FreeRid();
		gridMeshRid.FreeRid();
	}

	private Array GetGridMeshData(float cellSize, int radius, bool useAlphaFalloff)
	{
		var meshData = new Array();
		meshData.Resize((int)RenderingServer.ArrayType.Max);
			
		var hexes = CubeHexVector.Zero.GetSpiral(radius);
		var meshVertices = new List<Vector3[]>();
		var meshColors = new List<Color[]>();

		foreach (var hex in hexes)
		{
			var vertices = hex.GetHexVertices(cellSize);
			meshVertices.Add(new[] { vertices[0], vertices[1] });
			meshColors.Add(new[]
			{
				GetVertexColor(vertices[0], cellSize, radius, useAlphaFalloff),
				GetVertexColor(vertices[1], cellSize, radius, useAlphaFalloff)
			});
			
			meshVertices.Add(new[] { vertices[1], vertices[2] });
			meshColors.Add(new[]
			{
				GetVertexColor(vertices[1], cellSize, radius, useAlphaFalloff),
				GetVertexColor(vertices[2], cellSize, radius, useAlphaFalloff)
			});
			
			meshVertices.Add(new[] { vertices[2], vertices[3] });
			meshColors.Add(new[]
			{
				GetVertexColor(vertices[2], cellSize, radius, useAlphaFalloff),
				GetVertexColor(vertices[3], cellSize, radius, useAlphaFalloff)
			});
			
			meshVertices.Add(new[] { vertices[3], vertices[4] });
			meshColors.Add(new[]
			{
				GetVertexColor(vertices[3], cellSize, radius, useAlphaFalloff),
				GetVertexColor(vertices[4], cellSize, radius, useAlphaFalloff)
			});
			
			meshVertices.Add(new[] { vertices[4], vertices[5] });
			meshColors.Add(new[]
			{
				GetVertexColor(vertices[4], cellSize, radius, useAlphaFalloff),
				GetVertexColor(vertices[5], cellSize, radius, useAlphaFalloff)
			});
			
			meshVertices.Add(new[] { vertices[5], vertices[0] });
			meshColors.Add(new[]
			{
				GetVertexColor(vertices[5], cellSize, radius, useAlphaFalloff),
				GetVertexColor(vertices[0], cellSize, radius, useAlphaFalloff)
			});
		}
			
		meshData[(int)RenderingServer.ArrayType.Vertex] = meshVertices.SelectMany(x => x).ToArray();
		meshData[(int)RenderingServer.ArrayType.Color] = meshColors.SelectMany(x => x).ToArray();

		return meshData;
	}

	private Color GetVertexColor(Vector3 vertex, float cellSize, int radius, bool useAlphaFalloff)
	{
		return new Color(1f, 1f, 1f, 
			useAlphaFalloff ?
				Mathf.Pow(Mathf.Max(0, 1f - vertex.DistanceTo(CubeHexVector.Zero.ToWorldPosition(cellSize)) / radius), 2) :
				1f);
	}
}