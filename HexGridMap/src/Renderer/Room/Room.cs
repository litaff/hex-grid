namespace HexGrid.Map.Renderer.Room;

using Godot;
using Hex;
using Vector;

public class Room : IRenderer
{
    private readonly MeshLibrary? meshLibrary;
    private readonly Node3D? parent;
    private readonly GpuParticlesCollisionSdf3D? collisionSdf;
    private readonly List<MeshInstance3D> meshInstances = [];
    private readonly Dictionary<int, Hex> hexes = [];

    private int instanceIndex;
    
    public HexVector Position { get; }
    public IReadOnlyList<Hex> Hexes => hexes.Values.ToList();

    public Room(HexVector position, MeshLibrary? meshLibrary = null, Node3D? parent = null)
    {
        Position = position;
        this.meshLibrary = meshLibrary;
        if (parent is null) return;
        this.parent = new Node3D();
        this.parent.Position = position.ToWorldPosition();
        this.parent.Name = position.ToString();
        parent.AddChild(this.parent);
        this.parent.Owner = parent.GetTree().EditedSceneRoot;
        collisionSdf = new GpuParticlesCollisionSdf3D();
        collisionSdf.Name = "GpuParticlesCollisionSdf3D";
        this.parent.AddChild(collisionSdf);
        collisionSdf.Owner = this.parent.GetTree().EditedSceneRoot;
    }
    
    public void Add(Hex hex)
    {
        hexes.TryAdd(hex.Position.GetHashCode(), hex);
    }

    public void Remove(HexVector position)
    {
        hexes.Remove(position.GetHashCode());
    }

    public void Display()
    {
        foreach (var instance in meshInstances.Where(GodotObject.IsInstanceValid))
        {
            instance.Visible = true;
        }
    }

    public void Hide()
    {
        foreach (var instance in meshInstances.Where(GodotObject.IsInstanceValid))
        {
            instance.Visible = false;
        }
    }

    public void Update()
    {
        FreeMeshInstances();
        
        if (parent is null || meshLibrary is null) return;

        var sortedHexes = SortHexes().Where(pair => meshLibrary.GetItemMesh(pair.Key) is not null).ToDictionary();
        
        if (sortedHexes.Count == 0) return;

        var meshInstance = NewInstance();
        var combinedMesh = new ArrayMesh();
        var surfaceIndex = 0;
        instanceIndex = 0;

        foreach (var hexGroup in sortedHexes)
        {
            var mesh = meshLibrary.GetItemMesh(hexGroup.Key);
            
            if (combinedMesh._GetSurfaceCount() + mesh.GetSurfaceCount() >= 256)
            {
                meshInstance!.Mesh = combinedMesh;
                combinedMesh = new ArrayMesh();
                meshInstance = NewInstance();
                surfaceIndex = 0;
            }

            var rotations = hexGroup.Value.Select(hex => hex.MeshData.Radians).ToList();
            var positions = hexGroup.Value.Select(hex => hex.Position.ToWorldPosition()).ToList();
            var transforms = positions.Zip(rotations, (position, rotation) => 
                new Transform3D(Basis.Identity.Rotated(Vector3.Up, rotation), position)).ToList();

            surfaceIndex = DuplicateMesh(mesh, transforms, combinedMesh, surfaceIndex);
        }

        meshInstance!.Mesh = combinedMesh;
        UpdateSdf();
    }

    public int DuplicateMesh(Mesh mesh, List<Transform3D> transforms, ArrayMesh combinedMesh, int currentSurfaceIndex)
    {
        for (int i = 0; i < mesh.GetSurfaceCount(); i++)
        {
            var meshArrays = mesh.SurfaceGetArrays(i);

            meshArrays = DuplicateSurface(meshArrays, transforms);
                
            combinedMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, meshArrays);
            combinedMesh.SurfaceSetMaterial(currentSurfaceIndex, mesh.SurfaceGetMaterial(i));
            currentSurfaceIndex++;
        }

        return currentSurfaceIndex;
    }

    public Godot.Collections.Array DuplicateSurface(Godot.Collections.Array meshArrays, List<Transform3D> transforms)
    {
        var vertices = (Vector3[])meshArrays[(int)Mesh.ArrayType.Vertex];
        var indices = (int[])meshArrays[(int)Mesh.ArrayType.Index];
        var surfaceVertices = new List<Vector3>();
        var surfaceIndices = new List<int>();
        for (var j = 0; j < transforms.Count; j++)
        {
            var transform = transforms[j];
            surfaceVertices.AddRange(vertices.Select(vertex => transform * vertex));
            surfaceIndices.AddRange(indices.Select(index => index + j * vertices.Length));
        }
        meshArrays[(int)Mesh.ArrayType.Vertex] = surfaceVertices.ToArray();
        meshArrays[(int)Mesh.ArrayType.Index] = surfaceIndices.ToArray();

        RepeatArray<Vector3>(meshArrays, (int)Mesh.ArrayType.Normal, transforms.Count);
        RepeatArray<float>(meshArrays, (int)Mesh.ArrayType.Tangent, transforms.Count);
        RepeatArray<Color>(meshArrays, (int)Mesh.ArrayType.Color, transforms.Count);
        RepeatArray<Vector2>(meshArrays, (int)Mesh.ArrayType.TexUV, transforms.Count);
        RepeatArray<Vector2>(meshArrays, (int)Mesh.ArrayType.TexUV2, transforms.Count);
        RepeatArray<byte>(meshArrays, (int)Mesh.ArrayType.Custom0, transforms.Count);
        RepeatArray<byte>(meshArrays, (int)Mesh.ArrayType.Custom1, transforms.Count);
        RepeatArray<byte>(meshArrays, (int)Mesh.ArrayType.Custom2, transforms.Count);
        RepeatArray<byte>(meshArrays, (int)Mesh.ArrayType.Custom3, transforms.Count);
        RepeatArray<int>(meshArrays, (int)Mesh.ArrayType.Bones, transforms.Count);
        RepeatArray<float>(meshArrays, (int)Mesh.ArrayType.Weights, transforms.Count);

        return meshArrays;
    }

    public void UpdateSdf()
    {
        if (collisionSdf is null) return;
        var aabbs = meshInstances.Select(instance => instance.GetAabb()).ToList();
        if (aabbs.Count == 0) return;
        var merged = aabbs.FirstOrDefault();
        for (var i = 1; i < aabbs.Count; i++)
        {
            merged.Merge(aabbs[i]);
        }
        
        collisionSdf.Size = merged.Size;
        collisionSdf.Position = merged.GetCenter();
    }

    public bool Overlaps(IRenderer renderer)
    {
        return renderer.Hexes.Any(hex => hexes.ContainsKey(hex.Position.GetHashCode()));
    }

    public void Dispose()
    {
        if (parent is null) return;
        
        FreeMeshInstances();

        if (parent is null) return;
        if (!GodotObject.IsInstanceValid(parent)) return;
        parent.Name = " ";
        parent.QueueFree();
    }

    private void RepeatArray<T>(Godot.Collections.Array meshArrays, int index, int times)
    {
        var values = meshArrays[index];
        var cast = values.As<T[]>();
        if (cast.Length == 0) return;
        meshArrays[index] = Variant.From(Enumerable.Repeat(cast, times).SelectMany(list => list).ToArray());
    }

    private void FreeMeshInstances()
    {
        foreach (var instance in meshInstances.Where(GodotObject.IsInstanceValid))
        {
            instance.Name = " "; // Free the name, if this isn't done the name of the next node will be auto generated.
            instance.QueueFree();
        }
        meshInstances.Clear();
    }

    private MeshInstance3D? NewInstance()
    {
        if (parent is null) return null;
        var meshInstance = new MeshInstance3D();
        meshInstance.Name = $"MeshInstance3D_{instanceIndex}";
        instanceIndex++;
        parent.AddChild(meshInstance);
        meshInstance.Owner = parent.GetTree().EditedSceneRoot;
        meshInstances.Add(meshInstance);
        return meshInstance;
    }

    private Dictionary<int, List<Hex>> SortHexes()
    {
        Dictionary<int, List<Hex>> sortedHexes = new();
        
        foreach (var hex in Hexes)
        {
            var key = hex.MeshData.MeshIndex;
            if (sortedHexes.TryGetValue(key, out var value))
            {
                value.Add(hex);
            }
            else
            {
                sortedHexes.Add(key, [hex]);
            }
        }

        return sortedHexes;
    }
}