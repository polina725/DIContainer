using Microsoft.VisualStudio.TestTools.UnitTesting;
using DIContainer;
using UnitTesting.Interfaces;
using UnitTesting.ImplementationClasses;
using System.Collections.Generic;

namespace UnitTesting
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void SimpleDependency()
        {
            DependenciesConfiguration config = new DependenciesConfiguration();
            config.Register<ISmth, ClassForISmth>(true);
            DependenciesProvider provider = new DependenciesProvider(config);

            object obj = provider.Resolve<List<int>>();
            Assert.IsNull(obj);
            ClassForISmth cl = (ClassForISmth)provider.Resolve<ISmth>();
            Assert.IsNotNull(cl);
        }

        [TestMethod]
        public void DependencyTTLCheck()
        {
            DependenciesConfiguration config = new DependenciesConfiguration();
            config.Register<ISmth, ClassForISmth>(true);
            config.Register<IService, FirstForIService>();
            DependenciesProvider provider = new DependenciesProvider(config);

            ClassForISmth cl = (ClassForISmth)provider.Resolve<ISmth>();
            ClassForISmth cl2 = (ClassForISmth)provider.Resolve<ISmth>();
            Assert.AreEqual(cl, cl2);
            IService s1 = provider.Resolve<IService>();
            IService s2 = provider.Resolve<IService>();
            Assert.AreNotEqual(s1, s2);
        }

        [TestMethod]
        public void ManyImplementationsResolve()
        {
            DependenciesConfiguration config = new DependenciesConfiguration();
            config.Register<IService, FirstForIService>();
            config.Register<IService, SecondClForIService>();
            DependenciesProvider provider = new DependenciesProvider(config);

            IEnumerable<IService> impls = provider.Resolve<IEnumerable<IService>>();
            Assert.IsNotNull(impls);
            Assert.AreEqual(2, (impls as List<IService>).Count);
        }

        [TestMethod]
        public void InnerDependencyCheck()
        {
            DependenciesConfiguration config = new DependenciesConfiguration();
            config.Register<ISmth, ClassForISmth>();
            config.Register<IService, FirstForIService>();
            config.Register<IService, SecondClForIService>();
            config.Register<IClient, SecondClassForIClent>();
            DependenciesProvider provider = new DependenciesProvider(config);

            FirstForIService cl = (FirstForIService)provider.Resolve<IService>();
            Assert.IsNotNull(cl.smth);

            SecondClassForIClent cl1 = (SecondClassForIClent)provider.Resolve<IClient>();
            Assert.IsNotNull(cl1.serv);
        }

        [TestMethod]
        public void SimpleRecursionCheck()
        {
            DependenciesConfiguration config = new DependenciesConfiguration();
            config.Register<IClient, ClassForIClient>();
            config.Register<IData, ClassForIData>();
            DependenciesProvider provider = new DependenciesProvider(config);

            ClassForIClient cl = (ClassForIClient)provider.Resolve<IClient>();
            Assert.IsNull((cl.data as ClassForIData).cl);
        }

        [TestMethod]
        public void SimpleOpenGeneric()
        {
            DependenciesConfiguration config = new DependenciesConfiguration();
            config.Register<IAnother<ISmth>, First<ISmth>>();
            config.Register(typeof(IFoo<>), typeof(Second<>));
            DependenciesProvider provider = new DependenciesProvider(config);

            IAnother<ISmth> cl = provider.Resolve<IAnother<ISmth>>();
            Assert.IsNotNull(cl);
            IFoo<IService> cl1 = provider.Resolve<IFoo<IService>>();
            Assert.IsNotNull(cl1);
        }
    }


    interface IAnother<T>
        where T : ISmth
    {

    }

    class First<T> : IAnother<T>
        where T : ISmth
    {

    }

    interface IFoo<T>
        where T : IService
    {

    }

    class Second<T> : IFoo<T>
        where T : IService
    {

    }
}
