using System;

namespace fidt17.UnityValidationModule.Editor.ContextParsers
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class ValidationResultParserAttribute : Attribute
    {
        public readonly Type ValueContextType;
        
        public ValidationResultParserAttribute(Type valueContextType)
        {
            ValueContextType = valueContextType;
        }
    }
}