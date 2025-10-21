namespace EngineeringFort.Formwork;

public abstract record class FormworkComponent
{
    public virtual string Name { get; set; } = string.Empty;
}
