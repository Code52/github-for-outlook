using System;
using Analects.SettingsService;
using Autofac;
using GithubForOutlook.Logic.Models;
using GithubForOutlook.Logic.Repositories;
using GithubForOutlook.Logic.Repositories.Interfaces;
using GithubForOutlook.Logic.Ribbons.Email;
using GithubForOutlook.Logic.Ribbons.MainExplorer;
using Microsoft.Office.Interop.Outlook;
using NGitHub;
using NGitHub.Authentication;
using RestSharp;
using VSTOContrib.Core.RibbonFactory.Interfaces;

namespace GithubForOutlook.Logic
{
    public class AddinBootstrapper : IDisposable
    {
        private readonly IContainer container;

        public AddinBootstrapper(NameSpace nameSpace)
        {
            var containerBuilder = new ContainerBuilder();

            RegisterComponents(containerBuilder, nameSpace);

            container = containerBuilder.Build();
        }

        private static void RegisterComponents(ContainerBuilder containerBuilder, NameSpace nameSpace)
        {
            var assembly = typeof(GithubExplorerRibbon).Assembly;

            containerBuilder.RegisterAssemblyTypes(assembly)
                            .Where(t => t.Name.EndsWith("ViewModel"))
                            .AsSelf();

            containerBuilder.Register(c => nameSpace).SingleInstance();

            var settingsService = new SettingsService();
            ApplicationSettings settings;
            // settingsService.Clear(); // uncomment this for testing
            if (!settingsService.ContainsKey("Settings"))
            {
                // NOTE - we can get away without doing basic auth, but i'll leave this here for the moment
                settings = new ApplicationSettings { UserName = "", Password = "" };
                settingsService.Set("client", "9e96382c3109d9f35371");
                settingsService.Set("secret", "60d6c49b946ba4ddc52a34aa0dc1cf43e6077ba6");
                settingsService.Set("redirect", "http://code52.org");
                settingsService.Set("Settings", settings);
                settingsService.Save();
            }
            else
            {
                settings = settingsService.Get<ApplicationSettings>("Settings");
            }

            containerBuilder.Register(c => settings)
                .SingleInstance();

            containerBuilder.RegisterInstance(settingsService)
                            .AsImplementedInterfaces()
                            .SingleInstance();

            // TODO: deprecate basic auth once we are happy with oauth flow
            IAuthenticator authenticator;
            //if (!string.IsNullOrWhiteSpace(settings.AccessToken))
            //{
            //    authenticator = new OAuth2UriQueryParameterAuthenticator(settings.AccessToken);
            //}
            //else
            //{
                authenticator = new HttpBasicAuthenticator(settings.UserName, settings.Password);
            //}

            containerBuilder.RegisterInstance(authenticator)
                            .SingleInstance();

            containerBuilder.RegisterType<GitHubOAuthAuthorizer>()
                            .AsImplementedInterfaces();

            containerBuilder.RegisterType<GitHubClient>()
                            .AsImplementedInterfaces()
                            .PropertiesAutowired()
                            .SingleInstance();

            containerBuilder.RegisterType<OutlookDispatchingRepository>()
                .As<IOutlookRepository>();

            containerBuilder.RegisterType<GithubRepository>()
                .As<IGithubRepository>();

            containerBuilder.RegisterType<GithubMailItem>()
                .As<IRibbonViewModel>()
                .AsSelf();

            containerBuilder.RegisterType<GithubExplorerRibbon>()
                .As<IRibbonViewModel>()
                .AsSelf();
        }

        public object Resolve(Type type)
        {
            return container.Resolve(type);
        }

        public T Resolve<T>()
        {
            return container.Resolve<T>();
        }

        public void Dispose()
        {
            container.Dispose();
        }
    }
}