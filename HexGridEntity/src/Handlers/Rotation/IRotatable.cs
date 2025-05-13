namespace HexGrid.Entity.Handlers.Rotation;

using Godot;

public interface IRotatable
{
    public void LookTowards(Vector3 direction);
}