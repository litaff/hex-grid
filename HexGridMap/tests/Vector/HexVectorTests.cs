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
    public void Forward_Is_0_Minus1_1()
    {
        Assert.Multiple(() =>
        {
            Assert.That(HexVector.Forward.Q, Is.EqualTo(0));
            Assert.That(HexVector.Forward.R, Is.EqualTo(-1));
            Assert.That(HexVector.Forward.S, Is.EqualTo(1));
        });
    }
    
    [Test]
    public void ForRight_Is_1_Minus1_0()
    {
        Assert.Multiple(() =>
        {
            Assert.That(HexVector.ForRight.Q, Is.EqualTo(1));
            Assert.That(HexVector.ForRight.R, Is.EqualTo(-1));
            Assert.That(HexVector.ForRight.S, Is.EqualTo(0));
        });
    }
    
    [Test]
    public void BackRight_Is_1_0_Minus1()
    {
        Assert.Multiple(() =>
        {
            Assert.That(HexVector.BackRight.Q, Is.EqualTo(1));
            Assert.That(HexVector.BackRight.R, Is.EqualTo(0));
            Assert.That(HexVector.BackRight.S, Is.EqualTo(-1));
        });
    }
    
    [Test]
    public void Backward_Is_0_1_Minus1()
    {
        Assert.Multiple(() =>
        {
            Assert.That(HexVector.Backward.Q, Is.EqualTo(0));
            Assert.That(HexVector.Backward.R, Is.EqualTo(1));
            Assert.That(HexVector.Backward.S, Is.EqualTo(-1));
        });
    }
    
    [Test]
    public void BackLeft_Is_Minus1_1_0()
    {
        Assert.Multiple(() =>
        {
            Assert.That(HexVector.BackLeft.Q, Is.EqualTo(-1));
            Assert.That(HexVector.BackLeft.R, Is.EqualTo(1));
            Assert.That(HexVector.BackLeft.S, Is.EqualTo(0));
        });
    }
    
    [Test]
    public void ForLeft_Is_Minus1_0_1()
    {
        Assert.Multiple(() =>
        {
            Assert.That(HexVector.ForLeft.Q, Is.EqualTo(-1));
            Assert.That(HexVector.ForLeft.R, Is.EqualTo(0));
            Assert.That(HexVector.ForLeft.S, Is.EqualTo(1));
        });
    }
    
    [Test]
    public void DiagonalRight_Is_2_Minus1_Minus1()
    {
        Assert.Multiple(() =>
        {
            Assert.That(HexVector.DiagonalRight.Q, Is.EqualTo(2));
            Assert.That(HexVector.DiagonalRight.R, Is.EqualTo(-1));
            Assert.That(HexVector.DiagonalRight.S, Is.EqualTo(-1));
        });
    }
    
    [Test]
    public void DiagonalBackRight_Is_1_1_Minus2()
    {
        Assert.Multiple(() =>
        {
            Assert.That(HexVector.DiagonalBackRight.Q, Is.EqualTo(1));
            Assert.That(HexVector.DiagonalBackRight.R, Is.EqualTo(1));
            Assert.That(HexVector.DiagonalBackRight.S, Is.EqualTo(-2));
        });
    }
    
    [Test]
    public void DiagonalBackLeft_Is_Minus1_2_Minus1()
    {
        Assert.Multiple(() =>
        {
            Assert.That(HexVector.DiagonalBackLeft.Q, Is.EqualTo(-1));
            Assert.That(HexVector.DiagonalBackLeft.R, Is.EqualTo(2));
            Assert.That(HexVector.DiagonalBackLeft.S, Is.EqualTo(-1));
        });
    }
    
    [Test]
    public void DiagonalLeft_Is_Minus2_1_1()
    {
        Assert.Multiple(() =>
        {
            Assert.That(HexVector.DiagonalLeft.Q, Is.EqualTo(-2));
            Assert.That(HexVector.DiagonalLeft.R, Is.EqualTo(1));
            Assert.That(HexVector.DiagonalLeft.S, Is.EqualTo(1));
        });
    }
    
    [Test]
    public void DiagonalForLeft_Is_Minus1_Minus1_2()
    {
        Assert.Multiple(() =>
        {
            Assert.That(HexVector.DiagonalForLeft.Q, Is.EqualTo(-1));
            Assert.That(HexVector.DiagonalForLeft.R, Is.EqualTo(-1));
            Assert.That(HexVector.DiagonalForLeft.S, Is.EqualTo(2));
        });
    }
    
    [Test]
    public void DiagonalForRight_Is_1_Minus2_1()
    {
        Assert.Multiple(() =>
        {
            Assert.That(HexVector.DiagonalForRight.Q, Is.EqualTo(1));
            Assert.That(HexVector.DiagonalForRight.R, Is.EqualTo(-2));
            Assert.That(HexVector.DiagonalForRight.S, Is.EqualTo(1));
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