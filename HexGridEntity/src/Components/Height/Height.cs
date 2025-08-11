namespace HexGrid.Entity.Components.Height;

public class Height : Component
{
    public int Value { get; }
    
    public Height(Entity entity, int height) : base(entity)
    {
        Value = height;
    }
}