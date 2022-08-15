using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using fidt17.UnityValidationModule.Runtime.ValidationResults;

namespace fidt17.UnityValidationModule.Runtime.Attributes.FieldAttributes.CollectionFieldAttributes
{
    public class NotEmptyCollectionAttribute : NotNullValidationAttribute
    {
        private readonly bool _allowNullElements;
        
        public NotEmptyCollectionAttribute() : this(true, true, true)
        {
            
        }
        
        public NotEmptyCollectionAttribute(bool allowNullElements = true, bool validateInPrefab = true, bool recursiveValidation = true) : base(validateInPrefab, recursiveValidation)
        {
            _allowNullElements = allowNullElements;
        }
        
        public override ValidationResult ValidateField(FieldInfo field, object target)
        {
            if (field.FieldType.GetInterfaces().All(x => x != typeof(ICollection)))
            {
                throw new Exception($"Incorrect [NotNullCollection] attribute usage on {field.Name} @ {target.GetType()}. Field must derive from ICollection");
            }

            var baseResult = base.ValidateField(field, target);
            if (baseResult.Result == false) return baseResult;

            var collection = field.GetValue(target) as ICollection;
            
            if (collection.Count == 0)
            {
                return new FailResult($"Collection mustn't be empty.");
            }

            if (_allowNullElements == false)
            {
                foreach (var o in collection)
                {
                    if (UnityExtensions.IsUnityNull(o))
                    {
                        return new FailResult($"Element of collection of type {field.FieldType} on {target.GetType()} is missing.", target);
                    }
                }   
            }

            return new PassResult(targetContext: target);
        }
    }
}