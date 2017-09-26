using System;
using System.Configuration;
using System.Security.Principal;
using System.Threading;
using System.Web;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using MongoDB.Driver;
using NetBazaar.App_Start;
using NetBazaar.BusinessLogic;
using NetBazaar.BusinessLogic.Interfaces;
using NetBazaar.Dalc.Dtos;
using NetBazaar.Dalc.Interfaces;
using NetBazaar.Dalc.MongoDb;
using NetBazaar.Dalc.Sql;
using Ninject;
using Ninject.Activation;
using Ninject.Web.Common;
using WebActivatorEx;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(NinjectWebCommon), "Start")]
[assembly: ApplicationShutdownMethod(typeof(NinjectWebCommon), "Stop")]

namespace NetBazaar.App_Start
{
    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        private static readonly ThreadLocal<NetBazaarDatabase> _localNetBazaarDatabaseLocal =
            new ThreadLocal<NetBazaarDatabase>(() => new NetBazaarDatabase());

        /// <summary>
        ///     Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        ///     Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        ///     Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        ///     Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<IAspNetUserStore>().To<AspNetUserStoreSql>();
            kernel.Bind<ICategoriesStore>().To<CategoriesStoreSql>();
            kernel.Bind<IGeneralSettingsStore>().To<GeneralSettingsStoreSql>();
            kernel.Bind<IGeneralSettingsCache>().ToConstant(new GeneralSettingsCache());
            kernel.Bind<IImageReferencesStore>().To<ImageReferencesStoreSql>();
            kernel.Bind<IPostingsStore>().To<PostingsStoreMongoDb>();
            kernel.Bind<IImageHostingService>().To<AzureImageHostingService>();
            kernel.Bind<NetBazaarDatabase>().ToMethod(GetDataContextFromRequestContext);
            kernel.Bind<IMongoDatabase>().ToMethod(GetMongoDbContextFromRequestContext);
            kernel.Bind<IIdentity>().ToMethod(GetCurrentUser);
        }

        private static IMongoDatabase GetMongoDbContextFromRequestContext(IContext arg)
        {
            if (!HttpContext.Current.Items.Contains("NetBazaarMongoDb"))
            {
                //take database name from connection string
                var connectionString = ConfigurationManager.ConnectionStrings["NetBazaarMongoDb"].ToString();
                var databaseName = MongoUrl.Create(connectionString).DatabaseName;
                var mongoClient = new MongoClient(connectionString);

                HttpContext.Current.Items.Add("NetBazaarMongoDb", mongoClient.GetDatabase(databaseName));
            }

            return (IMongoDatabase) HttpContext.Current.Items["NetBazaarMongoDb"];
        }

        private static NetBazaarDatabase GetDataContextFromRequestContext(IContext context)
        {
            if (!HttpContext.Current.Items.Contains("NetBazaarDatabase"))
            {
                HttpContext.Current.Items.Add("NetBazaarDatabase", new NetBazaarDatabase());
            }

            return (NetBazaarDatabase) HttpContext.Current.Items["NetBazaarDatabase"];
        }

        private static IIdentity GetCurrentUser(IContext context)
        {
            return HttpContext.Current.User.Identity;
        }
    }
}