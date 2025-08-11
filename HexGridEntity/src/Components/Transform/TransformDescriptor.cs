namespace HexGrid.Entity.Components.Transform;

using Map.Vector;

public record TransformDescriptor(int StepRange, HexVector Position, HexVector Forward) : IComponentDescriptor;