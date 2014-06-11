using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.ComponentModel.Composition;
using MEFMvc.ViewModels;
using Drool.Entities;
using System.ComponentModel.Composition.Hosting;

namespace MEFMvc.Controllers
{
    public class HomeController : Controller
    {
        [ImportMany]
        public IEnumerable<Lazy<IConvert, IConvertMetaData>> Converters { get; private set; }

        [HttpGet]
        public ActionResult Converter()
        {
            ConvertionFormModel convertionFormModel = new ConvertionFormModel();

            return View(convertionFormModel);
        }

        public ActionResult Comparer()
        {
            return View();
        }

        public ActionResult Visualizer()
        {
            return View();
        }

        public ActionResult Drool()
        {
            return View();
        }

        public ActionResult UserGuideEn()
        {
            return View();
        }

        public ActionResult UserGuideRo()
        {
            return View();
        }

        public ActionResult DocumentationEn()
        {
            return View();
        }

        public ActionResult DocumentationRo()
        {
            return View();
        }
    }
}