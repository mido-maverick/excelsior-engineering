using System.Numerics;
using UnitsNet;

namespace EngineeringFort;

public record class QuantityCheck<TQuantity> : Check
    where TQuantity : IQuantity, IComparisonOperators<TQuantity, TQuantity, bool>, new()
{
    public TQuantity Value { get; set; } = new();
    public TQuantity Limit { get; set; } = new();
    public override bool IsValid => Value < Limit;
}
