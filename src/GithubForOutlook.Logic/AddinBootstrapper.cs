using System;
using Autofac;
using GithubForOutlook.Logic.Ribbons;
using NGitHub;
using NGitHub.Authentication;
using VSTOContrib.Core.RibbonFactory.Interfaces;

namespace GithubForOutlook.Logic
{
    public class AddinBootstrapper : IDisposable
    {
        private readonly IContainer container;

        public AddinBootstrapper()
        {
            var containerBuilder = new ContainerBuilder();

            RegisterComponents(containerBuilder);

            container = containerBuilder.Build();
        }

        private static void RegisterComponents(ContainerBuilder containerBuilder)
        {
            var assembly = typeof (GithubTask).Assembly;

            containerBuilder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Name.EndsWith("ViewModel"))
                .AsSelf();

        	containerBuilder.RegisterType<GithubTask>()
				.As<IRibbonViewModel>()
				.AsSelf();

            containerBuilder.RegisterType<GitHubOAuthAuthorizer>()
                            .AsImplementedInterfaces();
            containerBuilder.RegisterType<GitHubClient>()
                            .AsImplementedInterfaces();
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