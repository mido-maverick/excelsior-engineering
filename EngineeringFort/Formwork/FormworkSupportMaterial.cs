namespace EngineeringFort.Formwork;

public interface IFormworkSupportMaterial : IMaterial
{
    Pressure AllowableBendingStress();

    Pressure ElasticModulus();
}
