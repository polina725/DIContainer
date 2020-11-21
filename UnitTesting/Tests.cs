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
        private DependenciesProvider provider;

        [TestInitialize]
        public void Init()
        {
            DependenciesConfiguration config = new DependenciesConfiguration();
            config.Register<IService, FirstForIService>();
            config.Register<IService, SecondClForIService>();
            config.Register<IClient, ClassForIClient>();
            config.Register<ISmth, ClassForISmth>(true);
            config.Register<IData, ClassForIData>(true);
            provider = new DependenciesProvider(config);
        }

        [TestMethod]
        public void SimpleDependency()
        {
            object obj = provider.Resolve<List<int>>();
            Assert.IsNull(obj);
            ClassForISmth cl = (ClassForISmth)provider.Resolve<ISmth>();
            Assert.IsNotNull(cl);
        }

        [TestMethod]
        public void DependencyTTLCheck()
        {
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
            IEnumerable<IService> impls = provider.Resolve<IEnumerable<IService>>();
            Assert.IsNotNull(impls);
            Assert.AreEqual(2, (impls as List<IService>).Count);
        }

        [TestMethod]
        public void InnerDependencyCheck()
        {
            FirstForIService cl = (FirstForIService)provider.Resolve<IService>();
            Assert.IsNotNull(cl.smth);
        }

        [TestMethod]
        public void SimpleRecursionCheck()
        {
            ClassForIClient cl = (ClassForIClient)provider.Resolve<IClient>();
            Assert.IsNull((cl.data as ClassForIData).cl);
        }
    }

}
