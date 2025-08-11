namespace hex_grid.grid_objects;

using HexGrid.Entity;
using HexGrid.Entity.Components.Blocker;
using HexGrid.Entity.Components.Height;
using HexGrid.Entity.Components.Transform;
using HexGrid.Entity.Components.Transform.Position;
using HexGrid.Entity.Components.Transform.Rotation;

public class PlayerFactory : IEntityFactory
{
    private readonly EntityContext context;
    
    public PlayerFactory(EntityContext context)
    {
        this.context = context;
    }
    
    public Entity New(EntityBlueprint blueprint)
    {
        var entity = new Entity();
        
        var transformDescriptor = blueprint.Get<TransformDescriptor>();
        var transform = new Transform(entity, context.HexManager, context.ManagerRegistry.Get<BlockerManager>(),
            transformDescriptor.StepRange, transformDescriptor.Position, transformDescriptor.Forward);
        var positionHandlerDescriptor = blueprint.Get<PositionHandlerDescriptor>();
        var positionHandler = new LinearPositionHandler(positionHandlerDescriptor.movable,
            context.ManagerRegistry.Get<HeightManager>(), positionHandlerDescriptor.Speed);
        transform.Position.SetHandler(positionHandler);
        var rotationHandlerDescriptor = blueprint.Get<RotationHandlerDescriptor>();
        var rotationHandler = new RotationHandler(rotationHandlerDescriptor.Rotatable);
        transform.Rotation.SetHandler(rotationHandler);
        entity.AddComponent(transform);

        var height = new Height(entity, blueprint.Get<HeightDescriptor>().Height);
        entity.AddComponent(height);
        
        return entity;
    }
}