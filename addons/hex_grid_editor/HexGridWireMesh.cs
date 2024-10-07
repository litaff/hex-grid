namespace hex_grid.addons.hex_grid_editor;

using System.Collections.Generic;
using System.Linq;
using Godot;
using scripts.hex_grid;
using scripts.hex_grid.vector;
using scripts.utils;
using Array = Godot.Collections.Array;

public class HexGridWireMesh
{
	private readonly World3D scenario;
	private readonly float cellSize;
	private readonly Material material;
	private Rid gridMeshRid;
	private Rid gridInstanceRid;
	
	public HexGridWireMesh(World3D scenario, float cellSize, Material material)
	{
		this.scenario = scenario;
		this.cellSize = cellSize;
		this.material = material;
		gridInstanceRid = RenderingServer.InstanceCreate();
		gridMeshRid = RenderingServer.MeshCreate();
	}
	
    public void UpdateMesh(CubeHexVector hexPosition, int radius, bool useAlphaFalloff)
	{
		var meshData = GetGridMeshData(hexPosition, radius, useAlphaFalloff);
			
		RenderingServer.MeshClear(gridMeshRid);
		RenderingServer.MeshAddSurfaceFromArrays(gridMeshRid, RenderingServer.PrimitiveType.Lines, meshData);
		RenderingServer.MeshSurfaceSetMaterial(gridMeshRid, 0, material.GetRid());
		RenderingServer.InstanceSetBase(gridInstanceRid, gridMeshRid);
		RenderingServer.InstanceSetScenario(gridInstanceRid, scenario.Scenario);
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
			
		var allHexes = hexPosition.GetSpiral(radius);
		var allHexVertices = allHexes.Select(center => center.GetHexVertices(cellSize)).ToList();
		var meshVertices = new List<Vector3[]>();
		var meshColors = new List<Color[]>();

		var maxDistance = allHexVertices.Max(vertices => vertices.Max(vertex => vertex.DistanceTo(hexPosition.ToWorldPosition(cellSize))));
			
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
					Mathf.Pow(Mathf.Max(0, 1f - vertices[firstIndex].DistanceTo(hexPosition.ToWorldPosition(cellSize)) / maxDistance), 2) :
					1f);
				colors[secondIndex] = new Color(1f, 1f, 1f, 
					useAlphaFalloff ?
					Mathf.Pow(Mathf.Max(0, 1f - vertices[secondIndex].DistanceTo(hexPosition.ToWorldPosition(cellSize)) / maxDistance), 2) :
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