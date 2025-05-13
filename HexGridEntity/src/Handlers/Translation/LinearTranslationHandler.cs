namespace HexGrid.Entity.Handlers.Translation;

using Godot;
using Map;
using Map.Vector;
using Providers;

public class LinearTranslationHandler : IUpdateableTranslationHandler
{
    private readonly ITranslatable translatable;
    private readonly float translationSpeed;
    private readonly HeightData heightData;
    
    private IHexStateProvider? hexStateProvider;

    private Vector3 targetPosition;
    private Vector3 translationDirection;

    public bool TranslationComplete => translatable.Position.IsEqualApprox(targetPosition);

    public LinearTranslationHandler(float translationSpeed, ITranslatable translatable, HeightData heightData)
    {
        this.translatable = translatable;
        this.translationSpeed = translationSpeed * Properties.CellSize;
        this.heightData = heightData;
        
        targetPosition = translatable.Position;
    }

    public void TranslateTo(HexVector position)
    {
        if (hexStateProvider == null) return;
        
        if (!TranslationComplete) return;
        targetPosition = position.ToWorldPosition() + GetHexHeightVector(position);
        translationDirection = (targetPosition - translatable.Position).Normalized();
    }

    public void Enable(IHexStateProvider provider)
    {
        hexStateProvider = provider;
    }

    public void Disable()
    {
        hexStateProvider = null;
    }
    
    public void Update(double delta)
    {
        if (TranslationComplete) return;

        var step = translationDirection * translationSpeed * (float)delta;
        
        if (EndMovement(step)) return;
        
        translatable.Translate(step);
    }

    private bool EndMovement(Vector3 step)
    {
        var offsetLeft = targetPosition - translatable.Position;
        if (step.LengthSquared() < offsetLeft.LengthSquared()) return false;
        translatable.Translate(offsetLeft);
        return true;
    }

    private Vector3 GetHexHeightVector(HexVector position)
    {
        if (hexStateProvider == null) return Vector3.Zero;
        return (hexStateProvider.GetHexHeight(position) - heightData.Height) * Vector3.Up;
    }
}