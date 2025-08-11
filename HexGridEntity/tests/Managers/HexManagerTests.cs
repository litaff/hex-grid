namespace HexGrid.Entity.Tests.Managers;

using HexGrid.Entity.Managers;
using Map.Hex;
using Map.Vector;
using Moq;

[TestFixture]
public class HexManagerTests
{
    private HexManager hexManager;
    private Mock<IHexProvider> mockHexProvider;

    [SetUp]
    public void SetUp()
    {
        mockHexProvider = new Mock<IHexProvider>();
        hexManager = new HexManager(mockHexProvider.Object);
    }

    [Test]
    public void GetHeight_Returns0WhenNoHexFound()
    {
        var position = HexVector.Zero;
        mockHexProvider.Setup(provider => provider.Get(position)).Returns((Hex?)null);

        var height = hexManager.GetHeight(position);
        
        Assert.That(height, Is.EqualTo(0));
    }
    
    [Test]
    public void GetHeight_ReturnsHexHeightWhenHexFound()
    {
        var position = new HexVector(0, 0, 10);
        const int expectedHeight = 1;
        mockHexProvider.Setup(provider => provider.Get(position))
            .Returns(new Hex(position, new Properties(height: expectedHeight), new MeshData()));

        var height = hexManager.GetHeight(position);
        
        Assert.That(height, Is.EqualTo(expectedHeight + position.H));
    }

    [Test]
    public void Exists_ReturnsTrueWhenHexExists()
    {
        var position = HexVector.Zero;
        mockHexProvider.Setup(provider => provider.Get(position))
            .Returns(new Hex(position, new Properties(), new MeshData()));

        var exists = hexManager.Exists(position);
        
        Assert.That(exists, Is.True);
    }
    
    [Test]
    public void Exists_ReturnsFalseWhenNoHexFound()
    {
        var position = HexVector.Zero;
        mockHexProvider.Setup(provider => provider.Get(position))
            .Returns((Hex?)null);

        var exists = hexManager.Exists(position);
        
        Assert.That(exists, Is.False);
    }
    
    [Test]
    public void Exists_ReturnsTrueWhenHexExistsWithingRange()
    {
        var position = HexVector.Zero;
        mockHexProvider.Setup(provider => provider.Get(position))
            .Returns(new Hex(position, new Properties(), new MeshData()));

        var exists = hexManager.Exists(position + new HexVector(0, 0, 1), 1);
        
        Assert.That(exists, Is.True);
    }
    
    [Test]
    public void Exists_ReturnsFalseWhenHexExistsOutsideRange()
    {
        var position = HexVector.Zero;
        mockHexProvider.Setup(provider => provider.Get(position))
            .Returns(new Hex(position, new Properties(), new MeshData()));

        var exists = hexManager.Exists(position + new HexVector(0, 0, 1), 0);
        
        Assert.That(exists, Is.False);
    }

    [Test]
    public void GetClosest_ReturnsHexClosestToPositionWhenOnExists()
    {
        var position = HexVector.Zero;
        mockHexProvider.Setup(provider => provider.Get(position))
            .Returns(new Hex(position, new Properties(), new MeshData()));
        mockHexProvider.Setup(provider => provider.Get(position + new HexVector(0, 0, 1)))
            .Returns(new Hex(position + new HexVector(0, 0, 1), new Properties(), new MeshData()));

        var closest = hexManager.GetClosest(position, 1);
        
        Assert.That(closest, Is.EqualTo(position));
    }
    
    [Test]
    public void GetClosest_ReturnsInputPositionWhenNoHexFound()
    {
        var position = HexVector.Zero;
        mockHexProvider.Setup(provider => provider.Get(position))
            .Returns((Hex?)null);

        var closest = hexManager.GetClosest(position, 0);
        
        Assert.That(closest, Is.EqualTo(position));
    }

    [Test]
    public void TryGetClosest_ReturnsTrueWhenClosestFound()
    {
        var position = HexVector.Zero;
        mockHexProvider.Setup(provider => provider.Get(position))
            .Returns(new Hex(position, new Properties(), new MeshData()));
        mockHexProvider.Setup(provider => provider.Get(position + new HexVector(0, 0, 1)))
            .Returns(new Hex(position + new HexVector(0, 0, 1), new Properties(), new MeshData()));

        var found = hexManager.TryGetClosest(position, 1, out _);
        
        Assert.That(found, Is.True);
    }

    [Test]
    public void TryGetClosest_ReturnsFalseWhenClosestNotFound()
    {
        var position = HexVector.Zero;
        mockHexProvider.Setup(provider => provider.Get(position))
            .Returns((Hex?)null);

        var found = hexManager.TryGetClosest(position, 1, out _);
        
        Assert.That(found, Is.False);
    }
}