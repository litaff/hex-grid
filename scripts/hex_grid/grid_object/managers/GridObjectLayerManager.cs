namespace hex_grid.scripts.hex_grid.grid_object.managers;

using System.Collections.Generic;
using hex;
using interfaces;
using providers;
using vector;

public class GridObjectLayerManager : IHexStateProvider, IGridObjectLayerManager
{
    private readonly Dictionary<int, GridObjectStack> stacks;
    private readonly IHexProvider hexProvider;
    private readonly IGridObjectManager manager;
    
    public int Layer { get; }

    public GridObjectLayerManager(int layer, IHexProvider hexProvider, IGridObjectManager manager)
    {
        Layer = layer;
        this.hexProvider = hexProvider;
        this.manager = manager;
        stacks = new Dictionary<int, GridObjectStack>();
    }
    
    public void AddGridObject(GridObject gridObject)
    {
        AddToStack(gridObject);
    }
    
    public void RemoveGridObject(GridObject gridObject)
    {
        RemoveFromStack(gridObject, gridObject.GridPosition);
    }

    public void UpdateGridObjectPosition(GridObject gridObject, CubeHexVector previousPosition)
    {
        RemoveFromStack(gridObject, previousPosition);
        AddToStack(gridObject);
    }

    public void ChangeGridObjectLayer(GridObject gridObject, int relativeLayerIndex)
    {
        RemoveFromStack(gridObject, gridObject.GridPosition);
        // This will remove the object at its current position. This isn't a problem,
        // because we add this object to a different layer. 
        manager.RemoveGridObject(gridObject, Layer); 
        manager.AddGridObject(gridObject, Layer + relativeLayerIndex);
    }

    public float GetHexHeight(CubeHexVector position)
    {
        var height = Layer * HexGridData.Instance.LayerHeight;
        var cubeHex = hexProvider.GetHex(position);
        if (cubeHex is ElevatedHex elevatedHex)
        {
            height += elevatedHex.Height;
        }
        if (stacks.TryGetValue(position.GetHashCode(), out var stack))
        {
            height += stack.GetStackHeight();
        }
        return height;
    }
    
    public bool Contains<T>(CubeHexVector position)
    {
        return stacks.TryGetValue(position.GetHashCode(), out var stack) && stack.Contains<T>();
    }
    
    public bool HexIs<T>(CubeHexVector position)
    {
        return hexProvider.GetHex(position) is T;
    }
    
    private void AddToStack(GridObject gridObject)
    {
        var objectPosition = gridObject.GridPosition;
        if (stacks.ContainsKey(objectPosition.GetHashCode()))
        {
            stacks[objectPosition.GetHashCode()].Add(gridObject);
            return;
        }
        stacks.Add(objectPosition.GetHashCode(), new GridObjectStack(gridObject));
    }
    
    private void RemoveFromStack(GridObject gridObject, CubeHexVector position)
    {        
        if (!stacks.TryGetValue(position.GetHashCode(), out var stack)) return;
        stack.Remove(gridObject);
        if (stack.IsEmpty)
        {
            stacks.Remove(position.GetHashCode());
        }
    }
}