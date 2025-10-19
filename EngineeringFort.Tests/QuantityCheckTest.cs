using UnitsNet;
using UnitsNet.Units;

namespace EngineeringFort.Tests;

public class QuantityCheckTest
{
    private record class TestCheck : Check
    {
        public QuantityCheck<Pressure> ShearStressCheck { get; init; } = new()
        {
            Value = new Pressure(12, PressureUnit.KilogramForcePerSquareCentimeter),
            Limit = new Pressure(12.3, PressureUnit.KilogramForcePerSquareCentimeter)
        };

        public QuantityCheck<Length> DeflectionCheck { get; init; } = new()
        {
            Value = new Length(1.23, LengthUnit.Centimeter),
            Limit = new Length(1.2, LengthUnit.Centimeter)
        };

        public override ICheck[] SubChecks => [ShearStressCheck, DeflectionCheck];
    }

    [Fact]
    public void QuantityCheck_ShouldBeCorrect()
    {
        var testCheck = new TestCheck();

        Assert.Equal(12, testCheck.ShearStressCheck.Value.Value);
        Assert.Equal(12.3, testCheck.ShearStressCheck.Limit.Value);
        Assert.True(testCheck.ShearStressCheck.IsValid);
        Assert.Equal("<<OK>>", testCheck.ShearStressCheck.CheckStatus);

        Assert.Equal(1.23, testCheck.DeflectionCheck.Value.Value);
        Assert.Equal(1.2, testCheck.DeflectionCheck.Limit.Value);
        Assert.False(testCheck.DeflectionCheck.IsValid);
        Assert.Equal("--NG--", testCheck.DeflectionCheck.CheckStatus);

        Assert.False(testCheck.IsValid);
        Assert.Equal("--NG--", testCheck.CheckStatus);
    }
}
