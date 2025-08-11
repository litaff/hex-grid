namespace hex_grid.grid_objects;

using HexGrid.Entity;
using HexGrid.Entity.Components.Blocker;
using HexGrid.Entity.Components.Height;
using HexGrid.Entity.Components.Transform;
using HexGrid.Entity.Components.Transform.Position;
using HexGrid.Entity.Components.Transform.Rotation;

public class BlockerFactory : IEntityFactory
{
    private readonly EntityContext context;

    public BlockerFactory(EntityContext context)
    {
        this.context = context;
    }

    public Entity New(EntityBlueprint blueprint)
    {
        var entity = new Entity();

        var transformDescriptor = blueprint.Get<TransformDescriptor>();
        var transform = new Transform(entity, context.HexManager, context.ManagerRegistry.Get<BlockerManager>(),
            transformDescriptor.StepRange, transformDescriptor.Position, transformDescriptor.Forward);
        entity.AddComponent(transform);
        var positionHandlerDescriptor = blueprint.Get<PositionHandlerDescriptor>();
        var positionHandler = new InstantPositionHandler(positionHandlerDescriptor.movable, 
            context.ManagerRegistry.Get<HeightManager>(), context.Converter);
        transform.Position.SetHandler(positionHandler);
        var rotationHandlerDescriptor = blueprint.Get<RotationHandlerDescriptor>();
        var rotationHandler = new RotationHandler(rotationHandlerDescriptor.Rotatable, context.Converter);
        transform.Rotation.SetHandler(rotationHandler);
        
        var blocker = new HexGrid.Entity.Components.Blocker.Blocker(entity, blueprint.Get<BlockerDescriptor>().Blocked);
        entity.AddComponent(blocker);
        
        return entity;
    }
}