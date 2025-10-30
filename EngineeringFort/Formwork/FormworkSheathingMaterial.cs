namespace EngineeringFort.Formwork;

public interface IFormworkSheathingMaterial : IMaterial
{
    Pressure AllowableBendingStress(Length thickness);

    Pressure ElasticModulus(Length thickness);
}
