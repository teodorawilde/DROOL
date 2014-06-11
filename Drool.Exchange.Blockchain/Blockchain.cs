using Drool.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Drool.Exchange
{
    [Export(typeof(IConvert))]
    [ExportMetadata("Name", " Blockchain")]
    [ExportMetadata("AvailableCurrencies", new string[] { "USD", "EUR", "CNY","JPY","SGD",
    "HKD", "CAD", "NZD", "AUD", "CLP", "GBP", "DKK", "SEK", "ISK", "CHF", "BRL", "RUB",
    "PLN", "THB", "KRW", "TWD", "BTC"})]
    public class Blockchain : IConvert
    {
        private List<string> SupportedCurrencies = new List<string>  {"USD", "EUR", "CNY","JPY","SGD", "HKD", "CAD", "NZD", "AUD", 
            "CLP", "GBP", "DKK", "SEK", "ISK", "CHF", "BRL", "RUB","PLN", "THB", "KRW", "TWD", "BTC"};

        const string REMOTE_URI = "https://blockchain.info/ro/ticker";
        const string ECXHANGE_PATTERN = @"^[A-Z]{3}$";

        public ConversionResult Convert(decimal inputValue, string inputFrom, string inputTo)
        {
            ConversionResult result = new ConversionResult();
            

            if (SupportedCurrencies.Contains(inputFrom) == false ||
               SupportedCurrencies.Contains(inputTo) == false)
            {
                result.IsValid = false;
                result.ErrorMessage = string.Format("Currency is not supported.");
            }
            else if (inputValue <= 0 || !Regex.IsMatch(inputFrom, ECXHANGE_PATTERN) || !Regex.IsMatch(inputTo, ECXHANGE_PATTERN))
            {
                result.IsValid = false;
                result.ErrorMessage = string.Format("The input provided is not valid.");
            }
            else
            {
                result = ConvertCurrency(inputValue, inputFrom, inputTo);
            }

            return result;
        }

        public ConversionResult ConvertCurrency(decimal inputValue, string inputFrom, string inputTo)
        {
            ConversionResult result = new ConversionResult();
            result.TranzactionVolume = 600;

            result.IsValid = true;
            result.ErrorMessage = "";

            decimal resultTo = 0;

            if (string.Compare(inputFrom, inputTo) == 0)
            {
                result.ConversionValueResult = inputValue;
            }
            else if (string.Compare(inputTo.ToUpper(), "BTC") == 0)
            {
                resultTo = GetCurrencyValue(REMOTE_URI, inputFrom);
                result.ConversionValueResult = inputValue / resultTo;
            }
            else
            {
                resultTo = GetCurrencyValue(REMOTE_URI, inputTo);
                result.ConversionValueResult = resultTo * inputValue;

                if (string.Compare("BTC", inputFrom, true) != 0)
                {
                    decimal resultFrom = 0;
                    resultFrom = GetCurrencyValue(REMOTE_URI, inputFrom) * inputValue;
                    result.ConversionValueResult = (resultTo * inputValue) / resultFrom;
                }
            }



            return result;
        }

        private decimal GetCurrencyValue(string REMOTE_URI, string currency)
        {
            decimal result = 0;
            WebClient myWebClient = new WebClient();
            string jsonInformation = myWebClient.DownloadString(REMOTE_URI);
            JObject currencies = JObject.Parse(jsonInformation);

            foreach (var item in currencies)
            {
                if (string.Compare(item.Key, currency, true) == 0)
                {
                    dynamic currencyData = ((dynamic)item.Value);
                    decimal lastMarketValue = 0;
                    string valueToParse = currencyData.last;
                    bool parsingResult = decimal.TryParse(valueToParse, out lastMarketValue);

                    if (parsingResult == true)
                    {
                        result = lastMarketValue;
                    }

                    break;
                }
            }

            return result;
        }
    }
}
