namespace HexGrid.Map.Tests.Vector;

using Map.Vector;

[TestFixture]
public class HexFracVectorTests
{
    [TestCase(0, 0)]
    [TestCase(1, 1)]
    [TestCase(-1, -1)]
    [TestCase(1, -1)]
    [TestCase(-1, 1)]
    [TestCase(0.5f, 0.1f)]
    [TestCase(-0.5f, -0.1f)]
    [TestCase(-0.5f, 0.1f)]
    [TestCase(0.5f, -0.1f)]
    public void Constructor_CalculatesThirdComponent(float q, float r)
    {
        var s = -q - r;
        var vector = new HexFracVector(q, r);
        
        Assert.Multiple(() =>
        {
            Assert.That(vector.Q, Is.EqualTo(q));
            Assert.That(vector.R, Is.EqualTo(r));
            Assert.That(vector.S, Is.EqualTo(s));
        });
    }
}