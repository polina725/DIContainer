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
}
