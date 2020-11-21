using UnitTesting.Interfaces;

namespace UnitTesting.ImplementationClasses
{
    class FirstForIService : IService
    {
        public ISmth smth;

        public FirstForIService() { }
        public FirstForIService(ISmth _smth)
        {
            smth = _smth;
        }
    }
}
