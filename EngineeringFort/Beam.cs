namespace EngineeringFort;

public record class Beam : IRegexPatternProvider<Beam>
{
    public static string RegexPattern { get; } = @"C?[GgBb]\d{1,3}[a-z]?";
}
