namespace HexGrid.Map.Tests.Renderer.Room;

using Map.Hex;
using Map.Renderer.Room;
using Map.Vector;

[TestFixture]
public class RoomMapTests
{
    private RoomMap map;
    
    [SetUp]
    public void SetUp()
    {
        map = new RoomMap();
    }

    [Test]
    public void AddTo_AddsHexesToRendererAtPosition()
    {
        var rendererPosition = HexVector.Zero;
        var hexPosition = HexVector.Zero;
        var hex = new Hex(hexPosition, new Properties(), new MeshData());
        
        map.AddTo([hex], rendererPosition);
        
        Assert.That(map.FromHexPosition(hexPosition), Is.Not.Null);
        Assert.That(map.FromHexPosition(hexPosition).Hexes, Contains.Item(hex));
    }

    [Test]
    public void AddTo_AddsRenderer_WhenNoAtPosition()
    { 
        var rendererPosition = HexVector.Zero;
        var hexPosition = HexVector.Zero;
        var hex = new Hex(hexPosition, new Properties(), new MeshData());
        Assert.That(map.Renderers, Is.Empty);
        
        map.AddTo([hex], rendererPosition);
        
        Assert.That(map.Renderers, Is.Not.Empty);
    }

    [Test]
    public void RemoveHex_DoesNotThrow_WhenNoHex()
    {
        Assert.That(map.Renderers, Is.Empty);
        Assert.DoesNotThrow(() => map.RemoveHex(HexVector.Zero));
    }

    [Test]
    public void RemoveHex_RemovesRoom_WhenRoomEmpty()
    {
        var rendererPosition = HexVector.Zero;
        var hexPosition = HexVector.Zero;
        var hex = new Hex(hexPosition, new Properties(), new MeshData());
        Assert.That(map.Renderers, Is.Empty);
        map.AddTo([hex], rendererPosition);
        Assert.That(map.Renderers, Is.Not.Empty);
        
        map.RemoveHex(hexPosition);
        
        Assert.That(map.Renderers, Is.Empty);
    }

    [Test]
    public void FromHexPosition_ReturnsRenderer_WithHexOfPosition()
    {
        var rendererPosition = HexVector.Zero;
        var hexPosition = HexVector.Forward;
        var hex = new Hex(hexPosition, new Properties(), new MeshData());
        Assert.That(map.Renderers, Is.Empty);
        map.AddTo([hex], rendererPosition);
        Assert.That(map.Renderers, Is.Not.Empty);

        var renderer = map.FromHexPosition(hexPosition);
        
        Assert.That(renderer, Is.Not.Null);
        Assert.That(renderer.Hexes, Contains.Item(hex));
    }
}