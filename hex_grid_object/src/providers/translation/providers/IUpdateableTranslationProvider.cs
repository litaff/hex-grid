namespace hex_grid_object.providers.translation.providers;

public interface IUpdateableTranslationProvider : ITranslationProvider
{
    public bool TranslationComplete { get; }
    public void Update(double delta);
}