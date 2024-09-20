using System;
using System.Reflection;

namespace fidt17.UnityValidationModule.Runtime.Attributes
{
    public abstract class BaseValidationAttribute : Attribute
    {
        private readonly bool _validateInPrefab;

        protected BaseValidationAttribute(bool validateInPrefab = true)
        {
            _validateInPrefab = validateInPrefab;
        }

        public bool ShouldValidate(Object target)
        {
            if (!(target is UnityEngine.Object unityObject)) return true;
            #if UNITY_EDITOR
            return _validateInPrefab || !unityObject.IsAssetPrefab();
            #else
            return true;
            #endif
        }

        protected string GetTypeMessage(MemberInfo memberInfo, object target)
        {
            return $"\nType: {target.GetType()}.{memberInfo.Name}";
        }
    }
}