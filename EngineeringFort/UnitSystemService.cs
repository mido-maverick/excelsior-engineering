namespace EngineeringFort;

public class UnitSystemService
{
    public List<string> ActiveUnits { get; } = [];

    public Dictionary<MemberInfo, string[]> TypeMemberActiveUnits { get; } = [];
}
