namespace EngineeringFort.Formwork;

public interface ISideFormworkDesign
{
    Pressure MaxSidePressure { get; set; }
}

public interface IBottomFormworkDesign
{
}

public record class SideFormworkDesign : FormworkDesign, ISideFormworkDesign
{
    public virtual Pressure MaxSidePressure { get; set; }

    public FormworkLayerCheck?[] FormworkLayerChecks { get; } = new FormworkLayerCheck?[5];

    public override IEnumerable<ICheck> SubChecks => FormworkLayerChecks.OfType<ICheck>();
}
