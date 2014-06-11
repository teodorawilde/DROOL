using Drool.Entities;
using MEF.MVC4;
using MEFMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MEFMvc.Controllers
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RoutePrefix("api/Exchanges")]
    public class ExchangesController : ApiController
    {
        [ImportMany]
        public IEnumerable<Lazy<IConvert, IConvertMetaData>> Converters { get; private set; }

        [HttpGet]
        [Route("GetExchanges")]
        public IEnumerable<Exchange> Get()
        {

            List<Exchange> exchanges = new List<Exchange>();

            if(Converters!=null)
            {
                foreach (var item in Converters)
                {
                    Exchange exchange = new Exchange();
                    exchange.ExchangeName = item.Metadata.Name;
                    exchanges.Add(exchange);
                }
            }

            return exchanges;
        }
    }
}
