namespace hex_grid.grid_objects;

using System.Linq;
using Godot;
using HexGrid.Entity;
using HexGrid.Entity.Handlers.Position;
using HexGrid.Entity.Handlers.Rotation;
using HexGrid.Entity.Providers.Block;
using HexGrid.Entity.Providers.Position;
using HexGrid.Entity.Providers.Rotation;
using HexGrid.Map.Vector;

public partial class Blocker : Node3D, ITranslatable, IEntityProvider, IRotatable
{
    [Export]
    private Vector2I awakePosition;
    [Export]
    private float height;
    [Export]
    private Godot.Collections.Array<Direction> blocks = [];

    public Entity Entity => GetEntity();

    private Entity? entity;

    void ITranslatable.Translate(Vector3 offset)
    {
        Position += offset;
    }

    public void LookTowards(Vector3 direction)
    {
        
    }

    private Entity GetEntity()
    {
        if (entity != null) return entity;
        
        var heightData = new HeightData(height, 0);
        var positionProvider = new PositionProvider(new HexVector(awakePosition.X, awakePosition.Y), heightData);
        var rotationProvider = new RotationProvider();
        var translationHandler = new InstantPositionHandler(this, heightData);
        var rotationHandler = new InstantRotationHandler(this);
        var blockProvider = new BlockProvider(blocks.Select(dir => HexVector.Directions[dir]).ToList());
        entity = new Entity(positionProvider, rotationProvider, translationHandler, rotationHandler, blockProvider, heightData);
        
        return entity;
    }
}