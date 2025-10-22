namespace EngineeringFort.Formwork;

public record class FormworkLayerCheck : Check
{
    public virtual Pressure Pressure { get; set; }
}

public record class FormworkLayerCheck<T> : FormworkLayerCheck where T : FormworkComponent
{
    public virtual T? FormworkComponent { get; set; }
}
