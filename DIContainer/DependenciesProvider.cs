using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DIContainer
{
    public class DependenciesProvider
    {
        private DependenciesConfiguration configuration;
        private ConcurrentDictionary<Type, object> singletonImplementations = new ConcurrentDictionary<Type, object>();
        private Stack<Type> recursionStackResolver = new Stack<Type>();

        public DependenciesProvider(DependenciesConfiguration config)
        {
            configuration = config;
        }

        public TDependency Resolve<TDependency>()
        {
            return (TDependency)Resolve(typeof(TDependency));
        }

        private object Resolve(Type t)
        {
            Type dependencyType = t;
            List<ImplementationInfo> infos = GetImplementationsInfos(dependencyType);
            if (infos == null && !t.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)))
                return null;
            if (recursionStackResolver.Contains(t))
                return null;
            recursionStackResolver.Push(t);
            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)))
            {
                dependencyType = t.GetGenericArguments()[0];
                infos = GetImplementationsInfos(dependencyType);
                List<object> implementations = new List<object>();
                foreach (ImplementationInfo info in infos)
                {
                    implementations.Add(GetImplementation(info, t));
                }
                return ConvertToIEnumerable(implementations, dependencyType);
            }
            object obj = GetImplementation(infos[0], t);
            recursionStackResolver.Pop();
            return obj;
        }

        private object ConvertToIEnumerable(List<object> implementations,Type t)
        {
            Type newT = typeof(List<>).MakeGenericType(t);
            var enumerableType = typeof(Enumerable);
            var castMethod = enumerableType.GetMethod(nameof(Enumerable.Cast)).MakeGenericMethod(t);
            var toListMethod = enumerableType.GetMethod(nameof(Enumerable.ToList)).MakeGenericMethod(t);

            IEnumerable<object> itemsToCast = implementations;

            var castedItems = castMethod.Invoke(null, new[] { itemsToCast });

            return toListMethod.Invoke(null, new[] { castedItems });
        }

        private object GetImplementation(ImplementationInfo implInfo,Type resolvingDep)
        {
            Type innerTypeForOpenGeneric = null;
            if (implInfo.implClassType.IsGenericType && implInfo.implClassType.IsGenericTypeDefinition && implInfo.implClassType.GetGenericArguments()[0].FullName == null) 
                innerTypeForOpenGeneric = resolvingDep.GetGenericArguments().FirstOrDefault();

            if (implInfo.isSingleton)
            {
                if (!singletonImplementations.ContainsKey(implInfo.implClassType))
                {
                    object singleton = CreateInstanseForDependency(implInfo.implClassType, innerTypeForOpenGeneric);
                    singletonImplementations.TryAdd(implInfo.implClassType, singleton);
                }
                return singletonImplementations[implInfo.implClassType];
                //if (implInfo.Implementation == null)
                //    implInfo.Implementation = CreateInstanseForDependency(implInfo.implClassType, innerTypeForOpenGeneric);
                //return implInfo.Implementation;
            }
            else
            {
                return CreateInstanseForDependency(implInfo.implClassType, innerTypeForOpenGeneric);
            }
        }

        private object CreateInstanseForDependency(Type implClassType, Type innerTypeForOpenGeneric)
        {
            ConstructorInfo[] constructors = implClassType.GetConstructors().OrderByDescending(x => x.GetParameters().Length).ToArray();
            object implInstance = null;
            foreach (ConstructorInfo constructor in constructors)
            {
                ParameterInfo[] parameters = constructor.GetParameters();
                List<object> paramsValues = new List<object>();
                foreach (ParameterInfo parameter in parameters)
                {               
                    if (IsDependecy(parameter.ParameterType))
                    {
                        object obj = Resolve(parameter.ParameterType);
                        paramsValues.Add(obj);
                    }
                    else
                    {
                        object obj = null;
                        try
                        {
                            obj = Activator.CreateInstance(parameter.ParameterType, null);
                        }
                        catch { }
                        paramsValues.Add(obj);// null????
                    }
                }
                try
                {
                    if (innerTypeForOpenGeneric!=null)
                        implClassType = implClassType.MakeGenericType(innerTypeForOpenGeneric);
                    implInstance = Activator.CreateInstance(implClassType, paramsValues.ToArray());
                    break;
                }
                catch { }
            }
            return implInstance;
        }

        private bool IsDependecy(Type t)
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)))
                return IsDependecy(t.GetGenericArguments()[0]);
            return configuration.registedDependencies.ContainsKey(t);
        }

        private List<ImplementationInfo> GetImplementationsInfos(Type depType)
        {
            if (configuration.registedDependencies.ContainsKey(depType))
                return configuration.registedDependencies[depType];
            if (depType.IsGenericType)
            {
                Type genericDef = depType.GetGenericTypeDefinition();
                if (configuration.registedDependencies.ContainsKey(genericDef))
                    return configuration.registedDependencies[genericDef];
            }

            return null;
        }

        private List<ImplementationInfo> CheckIfOpenGeneric(Type intref)
        {

            return null;
        }
    }
}
