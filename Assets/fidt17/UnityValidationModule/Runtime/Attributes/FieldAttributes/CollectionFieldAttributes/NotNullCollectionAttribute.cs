using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using fidt17.UnityValidationModule.Runtime.ValidationResults;
using Object = System.Object;

namespace fidt17.UnityValidationModule.Runtime.Attributes.FieldAttributes.CollectionFieldAttributes
{
    public class NotNullCollectionAttribute : NotNullValidationAttribute
    {
        public NotNullCollectionAttribute() : this(true, true)
        {
            
        }
        
        public NotNullCollectionAttribute(bool validateInPrefab = true, bool recursiveValidation = true) : base(validateInPrefab, recursiveValidation)
        {
            
        }
        
        public override ValidationResult ValidateField(FieldInfo field, Object target)
        {
            if (field.FieldType.GetInterfaces().All(x => x != typeof(ICollection)))
            {
                return new FailResult($"Incorrect [NotNullCollection] attribute usage on {field.Name} @ {target.GetType()}. Field must derive from ICollection", target);
            }
            
            var baseResult = base.ValidateField(field, target);
            if (baseResult.Result == false) return baseResult;
            
            foreach (var o in (ICollection) field.GetValue(target))
            {
                if (UnityExtensions.IsUnityNull(o))
                {
                    return new FailResult($"Element of collection of type {field.FieldType} on {target.GetType()} is missing.", target);
                }
            }

            return new PassResult(targetContext: target);
        }
    }
}