namespace HexGrid.Entity;

public interface IEntityHolder
{
    public IFactoryKey FactoryKey { get; }
    public EntityBlueprint Get();
    public void Set(Entity entity);
}