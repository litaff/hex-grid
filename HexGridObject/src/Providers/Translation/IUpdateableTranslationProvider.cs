namespace HexGridObject.Providers.Translation;

public interface IUpdateableTranslationProvider : ITranslationProvider
{
    public bool TranslationComplete { get; }
    public void Update(double delta);
}