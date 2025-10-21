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
}
