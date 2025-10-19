using UnitsNet;

namespace EngineeringFort;

public interface I斷面
{
    Area 斷面積A { get; }
    Volume 斷面模數S { get; }
    AreaMomentOfInertia 慣性矩I { get; }
}

public record class 實心矩形斷面 : I斷面
{
    public static Area CalculateA(Length w, Length h) => w * h;
    public static Volume CalculateS(Length w, Length h) => w * h * h / 6;
    public static AreaMomentOfInertia CalculateI(Length w, Length h) => throw new NotImplementedException();

    public Length 寬度W { get; set; }
    public Length 高度H { get; set; }
    public Area 斷面積A => CalculateA(寬度W, 高度H);
    public Volume 斷面模數S => CalculateS(寬度W, 高度H);
    public AreaMomentOfInertia 慣性矩I => CalculateI(寬度W, 高度H);
}
