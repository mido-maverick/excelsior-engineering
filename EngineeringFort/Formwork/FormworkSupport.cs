namespace EngineeringFort.Formwork;

[Display(Name = nameof(FormworkSupport), ResourceType = typeof(Resources))]
public record class FormworkSupport : FormworkComponent
{
    [Display(Name = nameof(CrossSection), ResourceType = typeof(Resources))]
    public virtual ICrossSection CrossSection { get; } = new RectangularCrossSection();

    [Display(Name = nameof(Material), ResourceType = typeof(Resources))]
    public virtual IFormworkSupportMaterial? Material { get; set; }

    public virtual Pressure? AllowableBendingStress => Material?.AllowableBendingStress();

    public virtual Pressure? ElasticModulus => Material?.ElasticModulus();
}
