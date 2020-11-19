using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections;

namespace DIContainer
{
    public class DependenciesProvider
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
            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)))//t.GetInterface(nameof(IBaseGenerator)) != null
            {
                dependencyType = t.GetGenericArguments()[0];
                infos = GetImplementationsInfos(dependencyType);
                List<object> implementations = new List<object>();
                foreach(ImplementationInfo info in infos)
                {
                    implementations.Add(GetImplementation(info));
                }
                return Convert(implementations, dependencyType);
            }
            return GetImplementation(infos[0]);
        }

        private object Convert(List<object> implementations,Type t)
        {
            Type newT = typeof(List<>).MakeGenericType(t);
            var enumerableType = typeof(System.Linq.Enumerable);
            var castMethod = enumerableType.GetMethod(nameof(System.Linq.Enumerable.Cast)).MakeGenericMethod(t);
            var toListMethod = enumerableType.GetMethod(nameof(System.Linq.Enumerable.ToList)).MakeGenericMethod(t);

            IEnumerable<object> itemsToCast = implementations;

            var castedItems = castMethod.Invoke(null, new[] { itemsToCast });

            return toListMethod.Invoke(null, new[] { castedItems });
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
