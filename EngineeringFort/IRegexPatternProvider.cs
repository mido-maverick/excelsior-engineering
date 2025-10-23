namespace EngineeringFort;

internal interface IRegexPatternProvider<TSelf> where TSelf : IRegexPatternProvider<TSelf>
{
    static abstract string RegexPattern { get; }
}
