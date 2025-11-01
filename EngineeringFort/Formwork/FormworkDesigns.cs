namespace EngineeringFort.Formwork;

public interface ISideFormworkDesign
{
    Pressure MaxSidePressure { get; set; }
}

public interface IBottomFormworkDesign
{
}

[Display(Name = nameof(SideFormworkDesign), ResourceType = typeof(Resources))]
public record class SideFormworkDesign : FormworkDesign, ISideFormworkDesign
{
    public virtual Pressure MaxSidePressure { get; set; }

    [Display(Name = nameof(FormworkLayerCheck), ResourceType = typeof(Resources))]
    public FormworkLayerCheck?[] FormworkLayerChecks { get; } = new FormworkLayerCheck?[5];

    public override IEnumerable<ICheck> SubChecks => FormworkLayerChecks.OfType<ICheck>();
}
