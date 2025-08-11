namespace HexGrid.Entity.Components.Transform.Position;

using Interfaces;

public record PositionHandlerDescriptor(IMovable movable, float Speed) : IComponentDescriptor;