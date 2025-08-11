namespace HexGrid.Entity.Components.Blocker;

using Map.Vector;

public record BlockerDescriptor(List<HexVector> Blocked) : IComponentDescriptor;