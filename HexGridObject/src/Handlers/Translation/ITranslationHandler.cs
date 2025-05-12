namespace HexGridObject.Handlers.Translation;

using HexGridMap.Vector;
using Providers;

/// <summary>
/// This provider should assume that the object being moved is already on the passed position.
/// Meaning that when getting the height of the hex, the object is included.
/// </summary>
public interface ITranslationHandler
{
    public void TranslateTo(CubeHexVector position);
    public void Enable(IHexStateProvider provider);
    public void Disable();
}