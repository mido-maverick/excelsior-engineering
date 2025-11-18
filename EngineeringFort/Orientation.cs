namespace EngineeringFort;

public enum Orientation
{
    [Display(Name = nameof(DisplayStrings.HorizontalOrientation), ResourceType = typeof(DisplayStrings))]
    Horizontal,

    [Display(Name = nameof(DisplayStrings.VerticalOrientation), ResourceType = typeof(DisplayStrings))]
    Vertical
}
