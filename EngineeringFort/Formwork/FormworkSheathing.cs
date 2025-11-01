namespace EngineeringFort.Formwork;

[Display(Name = nameof(FormworkSheathing), ResourceType = typeof(DisplayStrings))]
public record class FormworkSheathing : FormworkComponent
{
    public virtual Length Thickness { get; set; }

    public virtual IFormworkSheathingMaterial? Material { get; set; }

    public virtual Pressure? AllowableBendingStress => Material?.AllowableBendingStress(Thickness);

    public virtual Pressure? ElasticModulus => Material?.ElasticModulus(Thickness);
}
