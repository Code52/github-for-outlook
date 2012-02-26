using System;

namespace GithubForOutlook.Tests
{
    public class AddinBootstrapper2 : IDisposable
    {
        public object Resolve(Type type)
        {
            return Activator.CreateInstance(type);
        }

        public T Resolve<T>()
        {
            return Activator.CreateInstance<T>();
        }

        public void Dispose()
        { }
    }
}
