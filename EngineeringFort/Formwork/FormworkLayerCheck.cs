namespace EngineeringFort.Formwork;

public record class FormworkLayerCheck : Check
{
    public virtual Pressure Pressure { get; set; }

    public FormworkComponent? FormworkComponent { get; set; }
}
