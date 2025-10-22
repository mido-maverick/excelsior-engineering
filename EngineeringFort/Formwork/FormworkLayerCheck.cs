using static EngineeringFort.SteelConstructionManual.BeamFormulas;

namespace EngineeringFort.Formwork;

public abstract record class FormworkLayerCheck : Check
{
    public virtual Pressure Pressure { get; set; }
}

public abstract record class FormworkLayerCheck<T> : FormworkLayerCheck where T : FormworkComponent, new()
{
    public virtual T FormworkComponent { get; } = new();
}

public record class FormworkSheathingLayerCheck : FormworkLayerCheck<FormworkSheathing>
{
    public virtual Length SupportSpacing { get; set; }

    public virtual Length UnitStripWidth { get; set; }

    public virtual ForcePerLength UniformlyDistributedLoad => ForcePerLength.FromKilogramsForcePerCentimeter(
        Pressure.KilogramsForcePerSquareCentimeter * UnitStripWidth.Centimeters);

    public virtual Torque MaximumBendingMoment => SimpleBeam.UniformlyDistributedLoad.Mmax(UniformlyDistributedLoad, SupportSpacing);
}

public record class FormworkSupportLayerCheck : FormworkLayerCheck<FormworkSupport>
{
    public virtual Length TributaryWidth { get; set; }

    public virtual ForcePerLength UniformlyDistributedLoad => ForcePerLength.FromKilogramsForcePerCentimeter(
        Pressure.KilogramsForcePerSquareCentimeter * TributaryWidth.Centimeters);

    public virtual Torque MaximumBendingMoment => ContinuousBeam.ThreeEqualSpans.AllSpansLoaded.Mmax(UniformlyDistributedLoad, TributaryWidth);
}

public record class FormworkTieRodLayerCheck : FormworkLayerCheck<FormworkTieRod>
{
}
