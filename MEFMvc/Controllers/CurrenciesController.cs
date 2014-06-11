using Drool.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web.Http;

namespace MEFMvc.Controllers
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CurrenciesController : ApiController
    {
        // Import exchanges from dlls' folder
        [ImportMany]
        public IEnumerable<Lazy<IConvert, IConvertMetaData>> Converters { get; private set; }
        
        // GET api/Currencies/Blockchain/1/EUR/BTC
        public decimal Get(string exchangeName, decimal value, string from, string to)
        {
            decimal result = 0;
            int sumTranzactionVolume = 0;
            List<string> selectedExchanges = new List<string>(exchangeName.Split(','));

            List<ConversionResult> conversionResults = new List<ConversionResult>();

            foreach (string selectedExchange in selectedExchanges)
            {
                // Get exchange by name
                Lazy<IConvert, IConvertMetaData> exchange = (from v in Converters
                                                             where v.Metadata.Name == selectedExchange
                                                             select v).FirstOrDefault();

                if (exchange != null)
                {
                    ConversionResult conversionResult = exchange.Value.Convert(value, from, to);
                    if(conversionResult.IsValid)
                    {
                        conversionResults.Add(conversionResult);
                    }
                }
            }
            
            // Compute the result
            foreach (ConversionResult conversionResult  in conversionResults)
            {
                result += conversionResult.ConversionValueResult * conversionResult.TranzactionVolume;
                sumTranzactionVolume += conversionResult.TranzactionVolume;
            }

            if (sumTranzactionVolume != 0)
            {
                result = result / sumTranzactionVolume;
            }
            
            return result;
        }

        //return available currencies
        // GET api/<controller>/5
        public string[] Get(string exchangeName)
        {

            Lazy<IConvert, IConvertMetaData> exchange = (from v in Converters
                                                         where v.Metadata.Name == exchangeName
                                                         select v).FirstOrDefault();
            if (exchange != null)
            {
                return exchange.Metadata.AvailableCurrencies;
            }
            else
            {
                return new string[] { };
            }
        }

    }
}