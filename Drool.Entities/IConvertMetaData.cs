
using System.Collections.ObjectModel;

namespace Drool.Entities
{
    public interface IConvertMetaData
    {
        string Name { get; }
        string[] AvailableCurrencies { get; }
    }
}
