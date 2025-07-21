namespace HexGrid.Entity.Tests.Providers;

using HexGrid.Entity.Handlers.Position;
using HexGrid.Entity.Handlers.Rotation;
using HexGrid.Entity.Manager;
using HexGrid.Entity.Providers;
using HexGrid.Entity.Providers.Block;
using HexGrid.Entity.Providers.Position;
using HexGrid.Entity.Providers.Rotation;
using Map.Hex;
using Map.Vector;
using Moq;

[TestFixture]
public class HexStateProviderTests
{
    private Mock<IHexProvider> mockHexProvider;
    private Mock<IEntityStorage> mockEntityStorage;
    private HexStateProvider hexStateProvider;
    
    [SetUp]
    public void SetUp()
    {
        mockHexProvider = new Mock<IHexProvider>();
        mockEntityStorage = new Mock<IEntityStorage>();
        hexStateProvider = new HexStateProvider(mockHexProvider.Object, mockEntityStorage.Object);
    }

    [Test]
    public void GetHexHeight_Returns0_WhenNoHexAtPosition()
    {
        var position = HexVector.Zero;
        mockHexProvider.Setup(mock => mock.Get(position)).Returns((Hex?)null);
        mockEntityStorage.Setup(mock => mock.Stacks).Returns(new Dictionary<int, EntityStack>());

        var height = hexStateProvider.GetHexHeight(position);
        
        Assert.That(height, Is.EqualTo(0));
    }
    
    [Test]
    public void GetHexHeight_ReturnsHexHeight_WhenNoEntities()
    {
        var position = HexVector.Zero;
        const int hexHeight = 1;
        mockHexProvider.Setup(mock => mock.Get(position)).Returns(
            new Hex(position, new Properties(hexHeight), new MeshData()));
        mockEntityStorage.Setup(mock => mock.Stacks).Returns(new Dictionary<int, EntityStack>());

        var height = hexStateProvider.GetHexHeight(position);
        
        Assert.That(height, Is.EqualTo(hexHeight));
    }

    [Test]
    public void Contains_ReturnsFalse_WhenNoStack()
    {
        var position = HexVector.Zero;
        mockEntityStorage.Setup(mock => mock.Stacks).Returns(new Dictionary<int, EntityStack>());

        var contains = hexStateProvider.Contains<Entity>(position);
        
        Assert.That(contains, Is.EqualTo(false));
    }
    
    [Test]
    public void Contains_ReturnsFalse_WhenNoEntityOfType()
    {
        var position = HexVector.Zero;
        mockEntityStorage.Setup(mock => mock.Stacks).Returns(new Dictionary<int, EntityStack>()
        {
            {position.GetHashCode(), new EntityStack(new Entity(null, null, null, null, null, new HeightData()))}
        });

        var contains = hexStateProvider.Contains<MockEntity>(position);
        
        Assert.That(contains, Is.EqualTo(false));
    }
    
    [Test]
    public void Contains_ReturnsTrue_WhenEntityOfTypePresent()
    {
        var position = HexVector.Zero;
        mockEntityStorage.Setup(mock => mock.Stacks).Returns(new Dictionary<int, EntityStack>()
        {
            {position.GetHashCode(), new EntityStack(new Entity(null, null, null, null, null, new HeightData()))}
        });

        var contains = hexStateProvider.Contains<Entity>(position);
        
        Assert.That(contains, Is.EqualTo(true));
    }
    
    private class MockEntity : Entity
    {
        public MockEntity(IPositionProvider positionProvider, IRotationProvider rotationProvider, IPositionHandler positionHandler, IRotationHandler rotationHandler, IBlockProvider blockProvider, HeightData heightData) : base(positionProvider, rotationProvider, positionHandler, rotationHandler, blockProvider, heightData)
        {
        }
    }
}