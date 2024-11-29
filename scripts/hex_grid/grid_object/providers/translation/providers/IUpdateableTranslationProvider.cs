namespace hex_grid.scripts.hex_grid.grid_object.providers.translation.providers;

public interface IUpdateableTranslationProvider : ITranslationProvider
{
    public bool TranslationComplete { get; }
    public void Update(double delta);
}