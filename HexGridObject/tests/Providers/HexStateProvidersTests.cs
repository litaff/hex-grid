namespace HexGridObject.Tests.Providers;

using global::HexGridObject.Providers;
using Moq;

[TestFixture]
public class HexStateProvidersTests
{
    [Test]
    public void ParameterlessConstructor_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var hexStateProviders = new HexStateProviders();
        });
    }
    
    [Test]
    public void Constructor_ThrowsArgumentException_IfNoKey0Provider()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var hexStateProviders = new HexStateProviders(new Dictionary<int, IHexStateProvider>());
        });
    }
    
    [Test]
    public void Constructor_AssignsProviders()
    {
        var mockProvider = new Mock<IHexStateProvider>();
        
        var hexStateProviders = new HexStateProviders(new Dictionary<int, IHexStateProvider>
        {
            {0, mockProvider.Object}
        });
        
        Assert.That(hexStateProviders.Providers[0], Is.EqualTo(mockProvider.Object));
    }
}