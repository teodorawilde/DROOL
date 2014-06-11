using Drool.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;

namespace Drool.Exchange
{
    [Export(typeof(IConvert))]
    [ExportMetadata("Name", " Bitfinex")]
    [ExportMetadata("AvailableCurrencies", new string[] { "USD", "BTC", "LTC" })]
    class Bitfinex : IConvert
    {
        private List<string> SupportedCurrencies = new List<string>{ "USD", "BTC", "LTC"};

        const string DEFAULT_URI = "https://api.bitfinex.com/v1/ticker/";
        const string ECXHANGE_PATTERN = @"^[A-Z]{3}$";

        public ConversionResult Convert(decimal inputValue, string inputFrom, string inputTo)
        {
            ConversionResult result = new ConversionResult();
            

            if(SupportedCurrencies.Contains(inputFrom) == false || 
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

        private ConversionResult ConvertCurrency(decimal inputValue, string inputFrom, string inputTo)
        {
            ConversionResult result = new ConversionResult();
            result.TranzactionVolume = 500;

            result.IsValid = true;
            result.ErrorMessage = "";

            decimal resultTo = 0;
            string REMOTE_URI = DEFAULT_URI;

            // The case in which the convertion is made to the same currency
            if (string.Compare(inputFrom.ToUpper(), inputTo.ToUpper()) == 0)
            {
                result.ConversionValueResult = inputValue;
            }
            // The case in which the conversion is not made from BTC
            else if (string.Compare(inputFrom.ToUpper(), "USD") == 0)
            {
                REMOTE_URI += inputTo.ToLower() + "usd";
                resultTo = GetCurrencyValue(REMOTE_URI);
                result.ConversionValueResult = (1 * inputValue) / resultTo;
            }
            // The case in which the conversion is to the USD currency
            else if (string.Compare(inputTo.ToUpper(), "USD", true) == 0)
            {
                REMOTE_URI += inputFrom.ToLower() + inputTo.ToLower();
                resultTo = GetCurrencyValue(REMOTE_URI);
                result.ConversionValueResult = resultTo * inputValue;
            }
            else
            {
                REMOTE_URI += inputTo.ToLower() + "usd";
                string REMOTE_URI_FROM = DEFAULT_URI + inputFrom.ToLower() + "usd";

                decimal resultFrom = GetCurrencyValue(REMOTE_URI_FROM);
                resultTo = GetCurrencyValue(REMOTE_URI);

                result.ConversionValueResult = (resultFrom / resultTo) * inputValue;
            }

            return result;
        }

        private decimal GetCurrencyValue(string remoteUri)
        {
            decimal result = 0;
            decimal lastMarketValue = 0;

            try
            {
                // Getting the Json Information from API into currencies using the JObject "DOM"
                WebClient myWebClient = new WebClient();
                string jsonInformation = myWebClient.DownloadString(remoteUri);
                JObject currencies = JObject.Parse(jsonInformation);

                // Saving the last price of the currency
                dynamic currencyData = currencies;
                string valueToParse = currencyData.last_price;
                bool parsingResult = decimal.TryParse(valueToParse, out lastMarketValue);

                if (parsingResult == true)
                {
                    result = lastMarketValue;
                }
            }
            catch(Exception ex)
            {
                Debug.Write(ex);
            }
            return result;
        }
    }
}
