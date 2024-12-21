using System.ComponentModel;

namespace core.domain.entity.enums;

[Flags]
public enum DirectionType
{
    [Description("نامشخص")]
    NONE = 0,
    [Description("شمال")]
    NORTH = 1,
    [Description("شرق")]
    EAST = 2,
    [Description("جنوب")]
    SOUTH = 4,
    [Description("غرب")]
    WEST = 8
}
