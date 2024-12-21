namespace core.domain.exceptions
{
    public class DirectionTypeParseException : Exception
    {
        public DirectionTypeParseException(string directionTypes) : base($"direction values should only be from {directionTypes}")
        {
        }
    }
}
