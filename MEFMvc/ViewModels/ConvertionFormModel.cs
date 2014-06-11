using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MEFMvc.ViewModels
{
    public class ConvertionFormModel
    {
        public decimal InputValue { get; set; }

        public decimal ConversionResult { get; set; }
        public List<SelectListItem> Exchanges { get; set; }
        public List<SelectListItem> AvailableCurrencies { get; set; }
        public string Exchange { get; set; }
        public string InputFrom { get; set; }
        public string InputTo { get; set; }
        public string StatusLabel { get; set; }
    }
}