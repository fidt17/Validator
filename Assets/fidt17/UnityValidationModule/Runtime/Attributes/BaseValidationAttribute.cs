using System;
using UnityEditor;

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
            return _validateInPrefab || !unityObject.IsAssetPrefab();
        } 
    }
}