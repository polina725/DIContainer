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
            Register(typeof(TDependency), typeof(TImplementation), isSingleton);
        }

        public void Register(Type interfaceType,Type classType,bool isSingleton = false)
        {
            if (!interfaceType.IsInterface || classType.IsAbstract || !interfaceType.IsAssignableFrom(classType) && !interfaceType.IsGenericTypeDefinition)
                return;
            if (!registedDependencies.ContainsKey(interfaceType))
            {
                List<ImplementationInfo> impl = new List<ImplementationInfo>();
                impl.Add(new ImplementationInfo(isSingleton, classType));
                registedDependencies.Add(interfaceType, impl);
            }
            else
            {
                registedDependencies[interfaceType].Add(new ImplementationInfo(isSingleton, classType));
            }
        }
    }
}
