namespace EngineeringFort;

public record class Floor : IRegexPatternProvider<Floor>
{
    public static string RegexPattern { get; } = @"[BR]?\d{1,3}FL?|[RP]?RFL?";
}
