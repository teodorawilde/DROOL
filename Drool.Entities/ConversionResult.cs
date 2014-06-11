
namespace Drool.Entities
{
    public class ConversionResult
    {
        public bool IsValid { get; set; }
        public decimal ConversionValueResult { get; set; }
        public string ErrorMessage { get; set; }
        public int TranzactionVolume { get; set; }
    }
}
