namespace HexGridObject.Handlers.Translation;

public interface IUpdateableTranslationHandler : ITranslationHandler
{
    public bool TranslationComplete { get; }
    public void Update(double delta);
}