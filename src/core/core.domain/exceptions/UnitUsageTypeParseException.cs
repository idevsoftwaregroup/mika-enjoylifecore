namespace core.domain.exceptions
{
    public class UnitUsageTypeParseException : Exception
    {
        public UnitUsageTypeParseException(string unitUsageTypes) : base($"unit usage values should only be from {unitUsageTypes}")
        {
        }
    }
}
