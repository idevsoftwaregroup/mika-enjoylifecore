using core.domain.entity.enums;

[AttributeUsage(AttributeTargets.Property)]
public class ListMustHaveValueAttribute : Attribute
{

    public string ErrorMessage { get; set; }

    public bool IsValid(object value)
    {

        if (value is List<DirectionType> directions)
        {
            return directions.Count > 0;
        }
        return false;
    }
}
