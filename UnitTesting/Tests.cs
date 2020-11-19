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
       // private DependenciesConfiguration config;

        [TestInitialize]
        public void Init()
        {
            DependenciesConfiguration config = new DependenciesConfiguration();
            config.Register<IService, FirstForIService>();
            config.Register<IService, SecondClForIService>();
            config.Register<IClient, ClassForIClient>();
            config.Register<IData, ClassForIData>(true);
            config.Register<IData, BigDataClass>(true);
            provider = new DependenciesProvider(config);
        }

        [TestMethod]
        public void SimpleDendency()
        {
            object obj = provider.Resolve<List<int>>();
            Assert.IsNull(obj);
            ClassForIData cl = (ClassForIData)provider.Resolve<IData>();
            Assert.IsNotNull(cl);
            ClassForIData cl2 = (ClassForIData)provider.Resolve<IData>();
            Assert.AreEqual(cl, cl2);
        }

        [TestMethod]
        public void DependencyTTLCheck()
        {
            ClassForIData cl = (ClassForIData)provider.Resolve<IData>();
            ClassForIData cl2 = (ClassForIData)provider.Resolve<IData>();
            Assert.AreEqual(cl, cl2);
            IService s1 = provider.Resolve<IService>();
            IService s2 = provider.Resolve<IService>();
            Assert.AreNotEqual(s1, s2);
        }

        [TestMethod]
        public void ManyImplementationsResolve()
        {
           // Enumerable t;
            IEnumerable<IService> impls = provider.Resolve<IEnumerable<IService>>();
            Assert.IsNotNull(impls);
            Assert.AreEqual(2, (impls as List<IService>).Count);
        }
    }
}
