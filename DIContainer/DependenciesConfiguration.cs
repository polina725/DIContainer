using System;
using System.Collections.Generic;

namespace DIContainer
{
    public class DependenciesConfiguration
    {
        public readonly Dictionary<Type, List<ImplementationInfo>> registedDependencies;

        public DependenciesConfiguration()
        {
            registedDependencies = new Dictionary<Type, List<ImplementationInfo>>();
        }

        public void Register<TDependency, TImplementation>(bool isSingleton = false)
        {
            Type classType = typeof(TImplementation);
            Type interfaceType = typeof(TDependency);
            if (!registedDependencies.ContainsKey(interfaceType))
            {
                List<ImplementationInfo> impl = new List<ImplementationInfo>();
                impl.Add(new ImplementationInfo(isSingleton, classType));
                registedDependencies.Add(interfaceType, impl);
            }
            else
            {
                registedDependencies[interfaceType].Add(new ImplementationInfo(isSingleton,classType));
            }
        }

    }
}
