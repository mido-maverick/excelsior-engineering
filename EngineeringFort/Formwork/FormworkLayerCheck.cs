namespace EngineeringFort.Formwork;

public record class FormworkLayerCheck : Check
{
    public virtual Pressure MaxPressure { get; set; }
}
