namespace grid_object.managers;

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Keeps the order of entry. 
/// </summary>
public readonly struct GridObjectStack
{
    private readonly List<GridObject> objects;

    public bool IsEmpty => objects.Count == 0;
    
    public GridObjectStack(GridObject initialObject)
    {
        objects = [initialObject];
    }

    public void Add(GridObject gridObject)
    {
        if (objects.Contains(gridObject)) return;
        objects.Add(gridObject);
    }

    public void Remove(GridObject gridObject)
    {
        objects.Remove(gridObject);
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