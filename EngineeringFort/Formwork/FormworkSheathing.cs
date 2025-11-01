namespace EngineeringFort.Formwork;

[Display(Name = nameof(FormworkSheathing), ResourceType = typeof(Resources))]
public record class FormworkSheathing : FormworkComponent
{
    public virtual Length Thickness { get; set; }

    [Display(Name = nameof(Material), ResourceType = typeof(Resources))]
    public virtual IFormworkSheathingMaterial? Material { get; set; }

    public virtual Pressure? AllowableBendingStress => Material?.AllowableBendingStress(Thickness);

    public virtual Pressure? ElasticModulus => Material?.ElasticModulus(Thickness);
}
