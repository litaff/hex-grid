namespace HexGrid.Entity.Tests;

using HexGrid.Entity.Handlers.Position;
using HexGrid.Entity.Handlers.Rotation;
using HexGrid.Entity.Providers.Block;
using HexGrid.Entity.Providers.Position;
using HexGrid.Entity.Providers.Rotation;
using Moq;

public class MockEntity
{
    public Entity Entity { get; }
    public Mock<IPositionProvider> MockPositionProvider { get; }
    public Mock<IRotationProvider> MockRotationProvider { get; }
    public Mock<IPositionHandler> MockPositionHandler { get; }
    public Mock<IRotationHandler> MockRotationHandler { get; }
    public Mock<IBlockProvider> MockBlockProvider { get; }

    public MockEntity(HeightData heightData = default)
    {
        MockPositionProvider ??= new Mock<IPositionProvider>();
        MockRotationProvider ??= new Mock<IRotationProvider>();
        MockPositionHandler ??= new Mock<IPositionHandler>();
        MockRotationHandler ??= new Mock<IRotationHandler>();
        MockBlockProvider ??= new Mock<IBlockProvider>();
        Entity = new Entity(
            MockPositionProvider.Object, 
            MockRotationProvider.Object, 
            MockPositionHandler.Object, 
            MockRotationHandler.Object, 
            MockBlockProvider.Object, 
            heightData);
    }
}