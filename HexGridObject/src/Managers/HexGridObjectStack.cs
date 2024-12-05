namespace HexGridObject.Managers;

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Keeps the order of entry. 
/// </summary>
public readonly struct HexGridObjectStack
{
    private readonly List<HexGridObject> objects;

    public bool IsEmpty => objects.Count == 0;
    
    public HexGridObjectStack(HexGridObject initialObject)
    {
        objects = [initialObject];
    }

    public void Add(HexGridObject hexGridObject)
    {
        if (objects.Contains(hexGridObject)) return;
        objects.Add(hexGridObject);
    }

    public void Remove(HexGridObject hexGridObject)
    {
        objects.Remove(hexGridObject);
    }

    public float GetStackHeight()
    {
        return objects.Sum(gridObject => gridObject.HeightData.Height);
    }

    public bool Contains<T>()
    {
        return objects.Any(gridObject => gridObject is T);
    }
}