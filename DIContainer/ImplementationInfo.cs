using System;

namespace DIContainer
{
    public class ImplementationInfo
    {
        public readonly Type implClassType;
        public readonly bool isSingleton;
        //private object implementation;
        //public object Implementation
        //{
        //    get
        //    { 
        //        return implementation;
        //    }
        //    set
        //    {
        //        implementation = value;
        //    }
        //}

        public ImplementationInfo(bool _isSingleton, Type impl)
        {
            implClassType = impl;
            isSingleton = _isSingleton;
        }

    }
}
