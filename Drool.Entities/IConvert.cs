
namespace Drool.Entities
{
    public interface IConvert
    {
        ConversionResult Convert(decimal inputValue, string inputFrom, string inputTo);
    }
}
