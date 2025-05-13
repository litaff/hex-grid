namespace HexGrid.Map.Tests.Vector;

using Map.Vector;

[TestFixture]
public class HexVectorTests
{
    [TestCase(0, 0)]
    [TestCase(1, 1)]
    [TestCase(-1, -1)]
    [TestCase(1, -1)]
    [TestCase(-1, 1)]
    public void Constructor_CalculatesThirdComponent(int q, int r)
    {
        var s = -q - r;
        var vector = new HexVector(q, r);
        
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
        Assert.Throws<ArgumentException>(() => { _ = new HexVector(1, 1, 1); });
    }

    [Test]
    public void Zero_Is_0_0_0()
    {
        Assert.Multiple(() =>
        {
            Assert.That(HexVector.Zero.Q, Is.EqualTo(0));
            Assert.That(HexVector.Zero.R, Is.EqualTo(0));
            Assert.That(HexVector.Zero.S, Is.EqualTo(0));
        });
    }
    
    [Test]
    public void North_Is_0_Minus1_1()
    {
        Assert.Multiple(() =>
        {
            Assert.That(HexVector.North.Q, Is.EqualTo(0));
            Assert.That(HexVector.North.R, Is.EqualTo(-1));
            Assert.That(HexVector.North.S, Is.EqualTo(1));
        });
    }
    
    [Test]
    public void EastNorth_Is_1_Minus1_0()
    {
        Assert.Multiple(() =>
        {
            Assert.That(HexVector.EastNorth.Q, Is.EqualTo(1));
            Assert.That(HexVector.EastNorth.R, Is.EqualTo(-1));
            Assert.That(HexVector.EastNorth.S, Is.EqualTo(0));
        });
    }
    
    [Test]
    public void EastSouth_Is_1_0_Minus1()
    {
        Assert.Multiple(() =>
        {
            Assert.That(HexVector.EastSouth.Q, Is.EqualTo(1));
            Assert.That(HexVector.EastSouth.R, Is.EqualTo(0));
            Assert.That(HexVector.EastSouth.S, Is.EqualTo(-1));
        });
    }
    
    [Test]
    public void South_Is_0_1_Minus1()
    {
        Assert.Multiple(() =>
        {
            Assert.That(HexVector.South.Q, Is.EqualTo(0));
            Assert.That(HexVector.South.R, Is.EqualTo(1));
            Assert.That(HexVector.South.S, Is.EqualTo(-1));
        });
    }
    
    [Test]
    public void WestSouth_Is_Minus1_1_0()
    {
        Assert.Multiple(() =>
        {
            Assert.That(HexVector.WestSouth.Q, Is.EqualTo(-1));
            Assert.That(HexVector.WestSouth.R, Is.EqualTo(1));
            Assert.That(HexVector.WestSouth.S, Is.EqualTo(0));
        });
    }
    
    [Test]
    public void WestNorth_Is_Minus1_0_1()
    {
        Assert.Multiple(() =>
        {
            Assert.That(HexVector.WestNorth.Q, Is.EqualTo(-1));
            Assert.That(HexVector.WestNorth.R, Is.EqualTo(0));
            Assert.That(HexVector.WestNorth.S, Is.EqualTo(1));
        });
    }
    
    [Test]
    public void DiagonalEast_Is_2_Minus1_Minus1()
    {
        Assert.Multiple(() =>
        {
            Assert.That(HexVector.DiagonalEast.Q, Is.EqualTo(2));
            Assert.That(HexVector.DiagonalEast.R, Is.EqualTo(-1));
            Assert.That(HexVector.DiagonalEast.S, Is.EqualTo(-1));
        });
    }
    
    [Test]
    public void DiagonalSouthEast_Is_1_1_Minus2()
    {
        Assert.Multiple(() =>
        {
            Assert.That(HexVector.DiagonalSouthEast.Q, Is.EqualTo(1));
            Assert.That(HexVector.DiagonalSouthEast.R, Is.EqualTo(1));
            Assert.That(HexVector.DiagonalSouthEast.S, Is.EqualTo(-2));
        });
    }
    
    [Test]
    public void DiagonalSouthWest_Is_Minus1_2_Minus1()
    {
        Assert.Multiple(() =>
        {
            Assert.That(HexVector.DiagonalSouthWest.Q, Is.EqualTo(-1));
            Assert.That(HexVector.DiagonalSouthWest.R, Is.EqualTo(2));
            Assert.That(HexVector.DiagonalSouthWest.S, Is.EqualTo(-1));
        });
    }
    
    [Test]
    public void DiagonalWest_Is_Minus2_1_1()
    {
        Assert.Multiple(() =>
        {
            Assert.That(HexVector.DiagonalWest.Q, Is.EqualTo(-2));
            Assert.That(HexVector.DiagonalWest.R, Is.EqualTo(1));
            Assert.That(HexVector.DiagonalWest.S, Is.EqualTo(1));
        });
    }
    
    [Test]
    public void DiagonalNorthWest_Is_Minus1_Minus1_2()
    {
        Assert.Multiple(() =>
        {
            Assert.That(HexVector.DiagonalNorthWest.Q, Is.EqualTo(-1));
            Assert.That(HexVector.DiagonalNorthWest.R, Is.EqualTo(-1));
            Assert.That(HexVector.DiagonalNorthWest.S, Is.EqualTo(2));
        });
    }
    
    [Test]
    public void DiagonalNorthEast_Is_1_Minus2_1()
    {
        Assert.Multiple(() =>
        {
            Assert.That(HexVector.DiagonalNorthEast.Q, Is.EqualTo(1));
            Assert.That(HexVector.DiagonalNorthEast.R, Is.EqualTo(-2));
            Assert.That(HexVector.DiagonalNorthEast.S, Is.EqualTo(1));
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
        var start = HexVector.Zero;
        var target = new HexVector(q, r);

        var calculatedDistance = start.Distance(target);
        
        Assert.That(calculatedDistance, Is.EqualTo(expectedDistance));
    }

    [TestCase(0, 0)]
    [TestCase(100, 100)]
    [TestCase(1, 0)]
    [TestCase(0, 1)]
    [TestCase(-100, -100)]
    [TestCase(-1, 0)]
    [TestCase(0, -1)]
    [TestCase(1, -1)]
    [TestCase(-1, -1)]
    [TestCase(100, -100)]
    [TestCase(-100, 100)]
    [TestCase(0, 100)]
    [TestCase(100, 0)]
    [TestCase(0, -100)]
    [TestCase(-100, 0)]
    public void Normalized_ReturnsVector_OfAbsoluteSumOf2Or0(int q, int r)
    {
        var vector = new HexVector(q, r);

        var normalized = vector.Normalized();
        
        var absoluteSum = Math.Abs(normalized.Q) + Math.Abs(normalized.R) + Math.Abs(normalized.S); 
        Assert.That(absoluteSum, Is.EqualTo(2).Or.Zero);
    }
    
    [TestCase(2, -1, 1, 0)]
    [TestCase(-2, 1, -1, 0)]
    [TestCase(-1, 2, -1, 1)]
    [TestCase(1, -2, 1, -1)]
    [TestCase(-1, -1, 0, -1)]
    [TestCase(1, 1, 0, 1)]
    public void Normalized_ReturnsClockWiseVector_WhenDiagonal(int q, int r, int expQ, int expR)
    {
        var vector = new HexVector(q, r);
        var expected = new HexVector(expQ, expR);

        var normalized = vector.Normalized();
        
        Assert.That(normalized, Is.EqualTo(expected));
    }
}