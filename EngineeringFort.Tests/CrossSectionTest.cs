using UnitsNet;

namespace EngineeringFort.Tests;

public class CrossSectionTest
{
    [Fact]
    public void RectangularCrossSection_Calculations_ShouldBeCorrect()
    {
        // Arrange
        var width = Length.FromMeters(0.12);
        var height = Length.FromMeters(0.24);

        // Act
        var crossSectionalArea = RectangularCrossSection.CalculateCrossSectionalArea(width, height);
        var sectionModulus = RectangularCrossSection.CalculateSectionModulus(width, height);
        var momentOfInertia = RectangularCrossSection.CalculateMomentOfInertia(width, height);

        // Assert
        Assert.Equal(2.88E-2, crossSectionalArea.SquareMeters, 6);
        Assert.Equal(1.152E-3, sectionModulus.CubicMeters, 6);
        Assert.Equal(1.3824E8, momentOfInertia.MillimetersToTheFourth, 6);
    }
}
