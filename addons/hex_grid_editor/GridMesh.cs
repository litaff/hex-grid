namespace hex_grid.addons.hex_grid_editor;

using System.Collections.Generic;
using System.Linq;
using Godot;
using scripts;
using scripts.utils;
using Array = Godot.Collections.Array;

public class GridMesh
{
	private readonly Material material;
	private readonly HexGridMap hexGridMap;
	private Rid gridMeshRid;
	private Rid gridInstanceRid;
	
	public GridMesh(HexGridMap hexGridMap, Material material)
	{
		this.hexGridMap = hexGridMap;
		this.material = material;
		gridInstanceRid = RenderingServer.InstanceCreate();
		gridMeshRid = RenderingServer.MeshCreate();
	}
	
    public void UpdateGrid(CubeHexVector hexPosition, int radius, bool useAlphaFalloff)
	{
		var meshData = GetGridMeshData(hexPosition, radius, useAlphaFalloff);
			
		RenderingServer.MeshClear(gridMeshRid);
		RenderingServer.MeshAddSurfaceFromArrays(gridMeshRid, RenderingServer.PrimitiveType.Lines, meshData);
		RenderingServer.MeshSurfaceSetMaterial(gridMeshRid, 0, material.GetRid());
		RenderingServer.InstanceSetBase(gridInstanceRid, gridMeshRid);
		RenderingServer.InstanceSetScenario(gridInstanceRid, hexGridMap.GetWorld3D().Scenario);
		RenderingServer.InstanceSetTransform(gridInstanceRid, new Transform3D(Basis.Identity, Vector3.Zero));
	}
    
	public void Dispose()
	{
		RenderingServer.MeshClear(gridMeshRid);
		gridInstanceRid.FreeRid();
		gridMeshRid.FreeRid();
	}
	
	private Array GetGridMeshData(CubeHexVector hexPosition, int radius, bool useAlphaFalloff)
	{
		var meshData = new Array();
		meshData.Resize((int)RenderingServer.ArrayType.Max);
			
		var allHexes = hexGridMap.GetSpiral(hexPosition, radius);
		var allHexVertices = allHexes.Select(center => hexGridMap.GetHexVertices(hexGridMap.GetWorldPosition(center))).ToList();
		var meshVertices = new List<Vector3[]>();
		var meshColors = new List<Color[]>();

		var maxDistance = allHexVertices.Max(vertices => vertices.Max(vertex => vertex.DistanceTo(hexGridMap.GetWorldPosition(hexPosition))));
			
		foreach (var singleHexVertices in allHexVertices)
		{
			var vertices = new Vector3[singleHexVertices.Length * 2];
			var colors = new Color[vertices.Length];
			
			for (var i = 0; i < singleHexVertices.Length; i++)
			{
				var firstIndex = i * 2; // Index of the first vertex in this line segment.
				var secondIndex = firstIndex + 1; // Index of the second vertex in this line segment.
					
				vertices[firstIndex] = singleHexVertices[i]; // First vertex of the line segment.
				vertices[secondIndex] = singleHexVertices[(i + 1) % singleHexVertices.Length]; // Second vertex of the line segment.

				// Colors for both vertices of the line segment.
				colors[firstIndex] = new Color(1f, 1f, 1f, 
					useAlphaFalloff ?
					Mathf.Pow(Mathf.Max(0, 1f - vertices[firstIndex].DistanceTo(hexGridMap.GetWorldPosition(hexPosition)) / maxDistance), 2) :
					1f);
				colors[secondIndex] = new Color(1f, 1f, 1f, 
					useAlphaFalloff ?
					Mathf.Pow(Mathf.Max(0, 1f - vertices[secondIndex].DistanceTo(hexGridMap.GetWorldPosition(hexPosition)) / maxDistance), 2) :
					1f);
			}
				
			meshVertices.Add(vertices);
			meshColors.Add(colors);
		}
			
		meshData[(int)RenderingServer.ArrayType.Vertex] = meshVertices.SelectMany(x => x).ToArray();
		meshData[(int)RenderingServer.ArrayType.Color] = meshColors.SelectMany(x => x).ToArray();

		return meshData;
	}
}