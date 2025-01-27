namespace HexGridMap.Tests.Vector;

using global::HexGridMap.Vector;

[TestFixture]
public class CubeHexVectorTests
{
    [TestCase(0, 0)]
    [TestCase(1, 1)]
    [TestCase(-1, -1)]
    [TestCase(1, -1)]
    [TestCase(-1, 1)]
    public void Constructor_CalculatesThirdComponent(int q, int r)
    {
        var s = -q - r;
        var vector = new CubeHexVector(q, r);
        
        Assert.Multiple(() =>
        {
            Assert.That(vector.Q, Is.EqualTo(q));
            Assert.That(vector.R, Is.EqualTo(r));
            Assert.That(vector.S, Is.EqualTo(s));
        });
    }

    [Test]
    public void Constructor_ThrowsArgumentException_ForIncorrectComponents()
    {
        Assert.Throws<ArgumentException>(() => { _ = new CubeHexVector(1, 1, 1); });
    }

    [Test]
    public void Zero_Is_0_0_0()
    {
        Assert.Multiple(() =>
        {
            Assert.That(CubeHexVector.Zero.Q, Is.EqualTo(0));
            Assert.That(CubeHexVector.Zero.R, Is.EqualTo(0));
            Assert.That(CubeHexVector.Zero.S, Is.EqualTo(0));
        });
    }
    
    [Test]
    public void North_Is_0_Minus1_1()
    {
        Assert.Multiple(() =>
        {
            Assert.That(CubeHexVector.North.Q, Is.EqualTo(0));
            Assert.That(CubeHexVector.North.R, Is.EqualTo(-1));
            Assert.That(CubeHexVector.North.S, Is.EqualTo(1));
        });
    }
    
    [Test]
    public void EastNorth_Is_1_Minus1_0()
    {
        Assert.Multiple(() =>
        {
            Assert.That(CubeHexVector.EastNorth.Q, Is.EqualTo(1));
            Assert.That(CubeHexVector.EastNorth.R, Is.EqualTo(-1));
            Assert.That(CubeHexVector.EastNorth.S, Is.EqualTo(0));
        });
    }
    
    [Test]
    public void EastSouth_Is_1_0_Minus1()
    {
        Assert.Multiple(() =>
        {
            Assert.That(CubeHexVector.EastSouth.Q, Is.EqualTo(1));
            Assert.That(CubeHexVector.EastSouth.R, Is.EqualTo(0));
            Assert.That(CubeHexVector.EastSouth.S, Is.EqualTo(-1));
        });
    }
    
    [Test]
    public void South_Is_0_1_Minus1()
    {
        Assert.Multiple(() =>
        {
            Assert.That(CubeHexVector.South.Q, Is.EqualTo(0));
            Assert.That(CubeHexVector.South.R, Is.EqualTo(1));
            Assert.That(CubeHexVector.South.S, Is.EqualTo(-1));
        });
    }
    
    [Test]
    public void WestSouth_Is_Minus1_1_0()
    {
        Assert.Multiple(() =>
        {
            Assert.That(CubeHexVector.WestSouth.Q, Is.EqualTo(-1));
            Assert.That(CubeHexVector.WestSouth.R, Is.EqualTo(1));
            Assert.That(CubeHexVector.WestSouth.S, Is.EqualTo(0));
        });
    }
    
    [Test]
    public void WestNorth_Is_Minus1_0_1()
    {
        Assert.Multiple(() =>
        {
            Assert.That(CubeHexVector.WestNorth.Q, Is.EqualTo(-1));
            Assert.That(CubeHexVector.WestNorth.R, Is.EqualTo(0));
            Assert.That(CubeHexVector.WestNorth.S, Is.EqualTo(1));
        });
    }
    
    [Test]
    public void DiagonalEast_Is_2_Minus1_Minus1()
    {
        Assert.Multiple(() =>
        {
            Assert.That(CubeHexVector.DiagonalEast.Q, Is.EqualTo(2));
            Assert.That(CubeHexVector.DiagonalEast.R, Is.EqualTo(-1));
            Assert.That(CubeHexVector.DiagonalEast.S, Is.EqualTo(-1));
        });
    }
    
    [Test]
    public void DiagonalSouthEast_Is_1_1_Minus2()
    {
        Assert.Multiple(() =>
        {
            Assert.That(CubeHexVector.DiagonalSouthEast.Q, Is.EqualTo(1));
            Assert.That(CubeHexVector.DiagonalSouthEast.R, Is.EqualTo(1));
            Assert.That(CubeHexVector.DiagonalSouthEast.S, Is.EqualTo(-2));
        });
    }
    
    [Test]
    public void DiagonalSouthWest_Is_Minus1_2_Minus1()
    {
        Assert.Multiple(() =>
        {
            Assert.That(CubeHexVector.DiagonalSouthWest.Q, Is.EqualTo(-1));
            Assert.That(CubeHexVector.DiagonalSouthWest.R, Is.EqualTo(2));
            Assert.That(CubeHexVector.DiagonalSouthWest.S, Is.EqualTo(-1));
        });
    }
    
    [Test]
    public void DiagonalWest_Is_Minus2_1_1()
    {
        Assert.Multiple(() =>
        {
            Assert.That(CubeHexVector.DiagonalWest.Q, Is.EqualTo(-2));
            Assert.That(CubeHexVector.DiagonalWest.R, Is.EqualTo(1));
            Assert.That(CubeHexVector.DiagonalWest.S, Is.EqualTo(1));
        });
    }
    
    [Test]
    public void DiagonalNorthWest_Is_Minus1_Minus1_2()
    {
        Assert.Multiple(() =>
        {
            Assert.That(CubeHexVector.DiagonalNorthWest.Q, Is.EqualTo(-1));
            Assert.That(CubeHexVector.DiagonalNorthWest.R, Is.EqualTo(-1));
            Assert.That(CubeHexVector.DiagonalNorthWest.S, Is.EqualTo(2));
        });
    }
    
    [Test]
    public void DiagonalNorthEast_Is_1_Minus2_1()
    {
        Assert.Multiple(() =>
        {
            Assert.That(CubeHexVector.DiagonalNorthEast.Q, Is.EqualTo(1));
            Assert.That(CubeHexVector.DiagonalNorthEast.R, Is.EqualTo(-2));
            Assert.That(CubeHexVector.DiagonalNorthEast.S, Is.EqualTo(1));
        });
    }

    [TestCase(0, 0, 0)]
    [TestCase(0, -1, 1)]
    [TestCase(1, -1, 1)]
    [TestCase(1, 0, 1)]
    [TestCase(0, 1, 1)]
    [TestCase(-1, 1, 1)]
    [TestCase(-1, 0, 1)]
    [TestCase(2, -1, 2)]
    [TestCase(1, 1, 2)]
    [TestCase(-1, 2, 2)]
    [TestCase(-2, 1, 2)]
    [TestCase(-1, -1, 2)]
    [TestCase(1, -2, 2)]
    public void Distance_ReturnsHexDistance_BetweenThisAndTarget(int q, int r, int expectedDistance)
    {
        var start = CubeHexVector.Zero;
        var target = new CubeHexVector(q, r);

        var calculatedDistance = start.Distance(target);
        
        Assert.That(calculatedDistance, Is.EqualTo(expectedDistance));
    }
}