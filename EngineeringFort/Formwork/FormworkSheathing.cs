namespace EngineeringFort.Formwork;

public record class FormworkSheathing : FormworkComponent
{
    public virtual Pressure ElasticModulus { get; set; }

    public virtual Length Thickness { get; set; }

    public virtual IFormworkSheathingMaterial? Material { get; set; }

    public virtual Pressure? AllowableBendingStress => Material?.AllowableBendingStress(Thickness);
}
