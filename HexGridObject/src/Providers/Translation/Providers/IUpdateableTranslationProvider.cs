namespace HexGridObject.Providers.Translation.Providers;

public interface IUpdateableTranslationProvider : ITranslationProvider
{
    public bool TranslationComplete { get; }
    public void Update(double delta);
}