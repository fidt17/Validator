using fidt17.UnityValidationModule.Editor.ValidationResultDrawers;
using fidt17.UnityValidationModule.Runtime.ValidationResults;

namespace fidt17.UnityValidationModule.Editor.ContextParsers
{
    internal abstract class BaseResultParser
    {
        public BaseResultParser(ValidationResult vl)
        {
        }

        public abstract IValidationResultDrawer GetDrawer();
        public abstract bool IsContextValid(System.Object obj);
    }
}