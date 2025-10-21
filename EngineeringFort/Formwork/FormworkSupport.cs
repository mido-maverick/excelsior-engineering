namespace EngineeringFort.Formwork;

public record class FormworkSupport : FormworkComponent
{
    public virtual ICrossSection CrossSection { get; } = new RectangularCrossSection();
}
