namespace HexGrid.Entity;

public class EntityFactory : IEntityFactory
{
    private readonly EntityContext context;
    
    public EntityFactory(EntityContext context)
    {
        this.context = context;
    }

    public Entity New(EntityBlueprint blueprint)
    {
        return new Entity();
    }
}