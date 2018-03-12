using System;
using System.Collections.Generic;

namespace Gravity.IoC
{
    public interface IDependencyContainer<TContainer> where TContainer: class
    {
        TContainer BuildContainer();

        T Resolve<T>();

        IEnumerable<T> ResolveAll<T>();

        object Resolve(Type serviceType);

        T ResolveNamed<T>(string serviceName);

        object ResolveNamed(string serviceName, Type serviceType);
    }
}
