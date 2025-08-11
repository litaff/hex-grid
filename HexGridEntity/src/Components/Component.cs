namespace HexGrid.Entity.Components;

public abstract class Component
{
    public Entity Entity { get; }
    public bool Enabled { get; protected set; }

    public Component(Entity entity)
    {
        Entity = entity;
    }

    public virtual void Enable()
    {
        Enabled = true;
    }

    public virtual void Disable()
    {
        Enabled = false;
    }
}