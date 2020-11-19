using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections;

namespace DIContainer
{
    class DependenciesProvider
    {
        private DependenciesConfiguration configuration;

        public DependenciesProvider(DependenciesConfiguration config)
        {
            configuration = config;
        }

        public TDependency Resolve<TDependency>()
        {
            return (TDependency)Resolve(typeof(TDependency));
        }

        /// <summary>
        /// Recursion resolver
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private object Resolve(Type t)
        {
            Type dependencyType = t;
            List<ImplementationInfo> infos = GetImplementationsInfos(dependencyType);
            if (infos == null && !t.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)))
                return null;
            if (t.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)))//t.GetInterface(nameof(IBaseGenerator)) != null
            {
                dependencyType = t.GetGenericArguments()[0];
                IList implementations = (IList)Activator.CreateInstance(typeof(IList<>).MakeGenericType(dependencyType));
                foreach(ImplementationInfo info in infos)
                {
                    implementations.Add(GetImplementation(info));
                }
                return implementations;
            }
            return GetImplementation(infos[0]);
        }

        private object GetImplementation(ImplementationInfo implInfo)
        {
            if (implInfo.isSingleton)
            {
                if (implInfo.Implementation == null)
                    implInfo.Implementation = CreateInstanseForDependency(implInfo.implClassType);
                return implInfo.Implementation;
            }
            else
            {
                return CreateInstanseForDependency(implInfo.implClassType);
            }
        }

        private object CreateInstanseForDependency(Type implClassType)
        {
            ConstructorInfo constructor = implClassType.GetConstructors()[0];
            ParameterInfo[] parameters = constructor.GetParameters();
            List<object> paramsValues = new List<object>();
            foreach(ParameterInfo parameter in parameters)
            {
                if (IsDependecy(implClassType))
                {
                    object obj = Resolve(implClassType);
                    paramsValues.Add(obj);
                }
                else
                {
                    paramsValues.Add(Activator.CreateInstance(implClassType));
                }
            }
            object implInstance = constructor.Invoke(paramsValues.ToArray());
            return implInstance;
        }

        private bool IsDependecy(Type t)
        {
            return configuration.registedDependencies.ContainsKey(t);
        }

        private List<ImplementationInfo> GetImplementationsInfos(Type depType)
        {
            if (configuration.registedDependencies.ContainsKey(depType))
                return configuration.registedDependencies[depType];
            return null;
        }
    }
}
