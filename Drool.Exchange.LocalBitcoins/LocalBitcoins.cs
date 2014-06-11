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

namespace BusinessRules
{
    [Export(typeof(IConvert))]
    [ExportMetadata("Name", " Local Bitcoins")]
    [ExportMetadata("AvailableCurrencies", new string[] {"USD", "EUR", "GBP", "RUB",
    "BRL", "MYR", "MXN", "CAD", "ZAR", "COP", "ARS", "THB", "SGD", "SEK", "NZD", "CZK",
    "CLP", "HKD", "INR", "AUD", "PHP", "AED", "NOK", "CNY", "CHF", "RON", "HUF", "PLN",
    "BTC"})]
    class LocalBitcoins : IConvert
    {
        private List<string> SupportedCurrencies = new List<string> {"USD", "EUR", "GBP", "RUB","BRL", "MYR", "MXN", "CAD",
            "ZAR", "COP", "ARS", "THB", "SGD", "SEK", "NZD", "CZK","CLP", "HKD", "INR", "AUD", "PHP", "AED", "NOK", "CNY",
            "CHF", "RON", "HUF", "PLN","BTC"};

        const string REMOTE_URI = "https://localbitcoins.com/bitcoinaverage/ticker-all-currencies/";
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
            result.TranzactionVolume = 670;

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
                    string valueToParse = currencyData.rates.last;
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
