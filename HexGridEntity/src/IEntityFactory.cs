namespace HexGrid.Entity;

public interface IEntityFactory
{
    public Entity New(EntityBlueprint blueprint);
}