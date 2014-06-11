using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Web.Http.Dependencies;
using System.Web.Mvc;
using IDependencyResolver = System.Web.Http.Dependencies.IDependencyResolver;
using MEF.MVC4;
using Drool.Entities;

namespace MEFMvc
{
    public static class MefConfig
    {
        public static void RegisterMef(string solutionPath)
        {
            var container = ConfigureContainer(solutionPath);

            ControllerBuilder.Current.SetControllerFactory(new MefControllerFactory(container));
            

            var dependencyResolver = System.Web.Http.GlobalConfiguration.Configuration.DependencyResolver;
            System.Web.Http.GlobalConfiguration.Configuration.DependencyResolver = new MefDependencyResolver(container);
        }

        private static CompositionContainer ConfigureContainer(string solutionPath)
        {
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var businessRulesCatalog = new AssemblyCatalog(typeof(IConvertMetaData).Assembly);
            var catalog = new AggregateCatalog(assemblyCatalog, businessRulesCatalog);

            //Adds all the parts found in the specified folder
            catalog.Catalogs.Add(new DirectoryCatalog(solutionPath + @"\Exchanges"));

            var container = new CompositionContainer(catalog);
            return container;
        }
    }
}