using UnitTesting.Interfaces;

namespace UnitTesting.ImplementationClasses
{
    class ClassForIData : IData
    {
        public IClient cl;
        public ClassForIData(IClient _cl) 
        {
            cl = _cl;
        }
    }
}
