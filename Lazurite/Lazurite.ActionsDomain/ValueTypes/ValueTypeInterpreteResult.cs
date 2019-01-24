namespace Lazurite.ActionsDomain.ValueTypes
{
    public class ValueTypeInterpreteResult
    {
        public ValueTypeInterpreteResult(bool success, string param)
        {
            Success = success;
            Value = param;
        }

        public bool Success { get; private set; }
        public string Value { get; private set; }
    }
}
