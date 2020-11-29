using System.Collections.Generic;
using UnitTesting.Interfaces;

namespace UnitTesting.ImplementationClasses
{
    class ClassForIClient : IClient
    {
        public IData data;
        public ClassForIClient(IData _data)
        {
            data = _data;
        }
    }

    class SecondClassForIClent : IClient
    {
        public IEnumerable<IService> serv = null;

        public SecondClassForIClent(IEnumerable<IService> _serv)
        {
            serv = _serv;
        }
    }
}
