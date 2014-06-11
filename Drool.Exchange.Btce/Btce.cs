using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using System.Text.RegularExpressions;
using System.Net;
using Newtonsoft.Json.Linq;
using Drool.Entities;

namespace BusinessRules
{
    [Export(typeof(IConvert))]
    [ExportMetadata("Name", " BTC-e")]
    [ExportMetadata("AvailableCurrencies", new string[] { "USD", "EUR", "GBP", "RUR", "CNH", "BTC", "LTC" })]
    public class Btce : IConvert
    {
        private List<string> SupportedCurrencies = new List<string> { "USD", "EUR", "GBP", "RUR", "CNH", "BTC", "LTC" };
        const string DEFAULT_URI = "https://btc-e.com/api/2/";
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
            result.TranzactionVolume = 410;

            result.IsValid = true;
            result.ErrorMessage = "";

            if (string.Compare(inputFrom, inputTo) == 0)
            {
                result.ConversionValueResult = inputValue;
            }
            else if (string.Compare(inputFrom.ToUpper(), "LTC") == 0 || string.Compare(inputFrom.ToUpper(), "BTC") == 0)
            {
                string REMOTE_URI = DEFAULT_URI + inputFrom.ToLower() + "_" + inputTo.ToLower() + "/ticker";
                result.ConversionValueResult = GetCurrencyValue(REMOTE_URI) * inputValue;
            }
            else if (string.Compare(inputTo.ToUpper(), "BTC") == 0 || string.Compare(inputTo.ToUpper(), "LTC") == 0)
            {
                string REMOTE_URI = DEFAULT_URI + inputTo.ToLower() + "_" + inputFrom.ToLower() + "/ticker";
                decimal resultValue = GetCurrencyValue(REMOTE_URI);
                result.ConversionValueResult = inputValue / resultValue;
            }
            else
            {
                decimal resultTo = 0;
                decimal resultFrom = 0;
                string remoteUriTo = DEFAULT_URI + "btc" + "_" + inputTo.ToLower() + "/ticker";
                string remoteUriFrom = DEFAULT_URI + "btc" + "_" + inputFrom.ToLower() + "/ticker";
                resultTo = GetCurrencyValue(remoteUriTo);
                resultFrom = GetCurrencyValue(remoteUriFrom);

                result.ConversionValueResult = (resultFrom / resultTo) * inputValue;
            }



            return result;
        }

        private decimal GetCurrencyValue(string remoteUri)
        {
            WebClient myWebClient = new WebClient();
            string jsonInformation = myWebClient.DownloadString(remoteUri);
            JObject currencies = JObject.Parse(jsonInformation);

            decimal result = 0;
            decimal lastMarketValue = 0;

            dynamic currencyData = currencies;
            string valueToParse = currencyData.ticker.last;
            bool parsingResult = decimal.TryParse(valueToParse, out lastMarketValue);

            if (parsingResult == true)
            {
                result = lastMarketValue;
            }

            return result;
        }
    }
}
