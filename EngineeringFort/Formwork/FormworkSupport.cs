namespace EngineeringFort.Formwork;

public record class FormworkSupport : FormworkComponent
{
    public virtual Pressure ElasticModulus { get; set; }

    public virtual ICrossSection CrossSection { get; } = new RectangularCrossSection();

    public virtual IFormworkSupportMaterial? Material { get; set; }

    public virtual Pressure? AllowableBendingStress => Material?.AllowableBendingStress();
}
