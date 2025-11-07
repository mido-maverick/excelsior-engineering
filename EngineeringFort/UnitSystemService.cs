using Microsoft.Extensions.Options;

namespace EngineeringFort;

public class UnitSystemService
{
    public List<string> ActiveUnits { get; } = [];

    public Dictionary<MemberInfo, string[]> QuantityMemberFormats { get; }


    public UnitSystemService(IOptions<QuantityOptions> options)
    {
        ActiveUnits = [.. options.Value.DefaultUnits];
        QuantityMemberFormats = [];
        foreach (var formats in options.Value.Formats)
        {
            var memberInfo = GetMemberInfo(formats.Key);
            if (memberInfo is null) continue;

            QuantityMemberFormats[memberInfo] = formats.Value;
        }
    }

    private static MemberInfo? GetMemberInfo(string str)
    {
        var parts = str.Split('.');
        switch (parts.Length)
        {
            case 1:
                return null;
            case 2:
                // TODO: Optimize
                var type = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes())
                    .FirstOrDefault(type => type.Name == parts[0]);
                return type?.GetMember(parts[1]).FirstOrDefault();
            case > 2:
                return null;
            default:
                return null;
        }
    }
}
