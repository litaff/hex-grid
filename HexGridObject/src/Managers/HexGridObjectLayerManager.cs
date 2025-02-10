namespace HexGridObject.Managers;

using System.Collections.Generic;
using HexGridMap;
using HexGridMap.Hex;
using HexGridMap.Vector;
using Providers;

public class HexGridObjectLayerManager : IHexStateProvider, IHexGridObjectLayerManager
{
    private readonly Dictionary<int, HexGridObjectStack> stacks;
    private readonly IHexProvider hexProvider;
    private readonly IHexGridObjectManager manager;

    public IReadOnlyDictionary<int, HexGridObjectStack> Stacks => stacks;
    public int Layer { get; }

    public HexGridObjectLayerManager(int layer, IHexProvider hexProvider, IHexGridObjectManager manager)
    {
        Layer = layer;
        this.hexProvider = hexProvider;
        this.manager = manager;
        stacks = new Dictionary<int, HexGridObjectStack>();
    }
    
    public void AddGridObject(HexGridObject hexGridObject)
    {
        AddToStack(hexGridObject);
    }
    
    public void RemoveGridObject(HexGridObject hexGridObject)
    {
        RemoveFromStack(hexGridObject, hexGridObject.GridPosition);
    }

    public void UpdateGridObjectPosition(HexGridObject hexGridObject, CubeHexVector previousPosition)
    {
        RemoveFromStack(hexGridObject, previousPosition);
        AddToStack(hexGridObject);
    }

    public void ChangeGridObjectLayer(HexGridObject hexGridObject, int relativeLayerIndex)
    {
        RemoveFromStack(hexGridObject, hexGridObject.GridPosition);
        // This will remove the object at its current position. This isn't a problem,
        // because we add this object to a different layer. 
        manager.RemoveGridObject(hexGridObject, Layer); 
        manager.AddGridObject(hexGridObject, Layer + relativeLayerIndex);
    }
    
    public float GetHexHeight(CubeHexVector position, List<HexGridObject>? exclude = null)
    {
        var height = Layer * HexGridProperties.LayerHeight;
        
        height += hexProvider.GetHex(position)?.Height ?? 0;

        if (!stacks.TryGetValue(position.GetHashCode(), out var stack)) return height;
        
        height += exclude != null ? stack.GetStackHeight(exclude) : stack.GetStackHeight();
        
        return height;
    }
    
    public bool Contains<T>(CubeHexVector position)
    {
        return stacks.TryGetValue(position.GetHashCode(), out var stack) && stack.Contains<T>();
    }
    
    public bool Exists(CubeHexVector position)
    {
        return hexProvider.GetHex(position) is not null;
    }
    
    private void AddToStack(HexGridObject hexGridObject)
    {
        var objectPosition = hexGridObject.GridPosition;
        if (stacks.ContainsKey(objectPosition.GetHashCode()))
        {
            stacks[objectPosition.GetHashCode()].Add(hexGridObject);
            return;
        }
        stacks.Add(objectPosition.GetHashCode(), new HexGridObjectStack(hexGridObject));
    }
    
    private void RemoveFromStack(HexGridObject hexGridObject, CubeHexVector position)
    {        
        if (!stacks.TryGetValue(position.GetHashCode(), out var stack)) return;
        stack.Remove(hexGridObject);
        if (stack.IsEmpty)
        {
            stacks.Remove(position.GetHashCode());
        }
    }
}