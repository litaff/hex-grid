namespace hex_grid.grid_objects;

using System.Linq;
using Godot;
using HexGrid.Entity;
using HexGrid.Entity.Components.Blocker;
using HexGrid.Entity.Components.Transform;
using HexGrid.Entity.Components.Transform.Position;
using HexGrid.Entity.Components.Transform.Position.Interfaces;
using HexGrid.Entity.Components.Transform.Rotation;
using HexGrid.Map.Vector;

public partial class Blocker : Node3D, IMovable, IEntityHolder, IRotatable
{
    [Export]
    private Vector2I awakePosition;
    [Export]
    private float height;
    [Export]
    private Godot.Collections.Array<Direction> blocks = [];

    private Entity? entity;

    public HexVector InitialPosition => new(awakePosition.X, awakePosition.Y);
    public HexVector InitialRotation => HexVector.Directions[Direction.Forward];
    public IFactoryKey FactoryKey => new BlockerFactoryKey();

    void IMovable.Translate(Vector3 offset)
    {
        Position += offset;
    }

    public void LookTowards(Vector3 direction)
    {
        
    }

    public EntityBlueprint Get()
    {
        var blueprint = new EntityBlueprint();
        blueprint.Add(new TransformDescriptor(0, InitialPosition, InitialRotation));
        blueprint.Add(new BlockerDescriptor(blocks.Select(dir => HexVector.Directions[dir]).ToList()));
        blueprint.Add(new PositionHandlerDescriptor(this, 0));
        blueprint.Add(new RotationHandlerDescriptor(this));
        return blueprint;
    }

    public void Set(Entity entity)
    {
        this.entity = entity;
        entity.Enable();
    }
}