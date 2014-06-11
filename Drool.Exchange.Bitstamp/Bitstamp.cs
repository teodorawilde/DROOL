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
    [ExportMetadata("Name", " Bitstamp")]
    [ExportMetadata("AvailableCurrencies", new string[] { "USD", "BTC" })]
    class Bitstamp : IConvert
    {
        private List<string> SupportedCurrencies = new List<string> { "USD", "BTC"};

        const string REMOTE_URI = "https://www.bitstamp.net/api/ticker/";
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
            result.TranzactionVolume = 550;

            result.IsValid = true;
            result.ErrorMessage = "";

            decimal resultTo = 0;
            resultTo = GetCurrencyValue(REMOTE_URI);

            if (string.Compare(inputFrom.ToUpper(), inputTo.ToUpper()) == 0)
            {
                result.ConversionValueResult = inputValue;
            }
            else if (string.Compare(inputFrom.ToUpper(), "USD") == 0)
            {
                result.ConversionValueResult = inputValue / resultTo;
            }
            else
            {
                result.ConversionValueResult = resultTo * inputValue;
            }



            return result;
        }

        private decimal GetCurrencyValue(string remoteUri)
        {
            decimal result = 0;
            decimal lastMarketValue = 0;

            WebClient myWebClient = new WebClient();
            string jsonInformation = myWebClient.DownloadString(remoteUri);
            JObject currencies = JObject.Parse(jsonInformation);

            dynamic currencyData = currencies;
            string valueToParse = currencyData.last;
            bool parsingResult = decimal.TryParse(valueToParse, out lastMarketValue);

            if (parsingResult == true)
            {
                result = lastMarketValue;
            }

            return result;
        }

    }
}
