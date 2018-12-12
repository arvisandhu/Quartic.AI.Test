namespace Quartic.AI.Test.SignalEngine
{
    public class JsonSignal
    {
        public string SignalID { get; set; }
        public string ValueType { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            return $"{SignalID} - {ValueType} - {Value}";
        }
    }
}