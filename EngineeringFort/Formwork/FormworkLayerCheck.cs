namespace EngineeringFort.Formwork;

public abstract record class FormworkLayerCheck : Check
{
    public virtual Pressure Pressure { get; set; }
}

public record class FormworkLayerCheck<T> : FormworkLayerCheck where T : FormworkComponent
{
    public virtual T? FormworkComponent { get; set; }
}

public record class FormworkSheathingLayerCheck : FormworkLayerCheck<FormworkSheathing>
{
}

public record class FormworkSupportLayerCheck : FormworkLayerCheck<FormworkSupport>
{
}

public record class FormworkTieRodLayerCheck : FormworkLayerCheck<FormworkTieRod>
{
}
