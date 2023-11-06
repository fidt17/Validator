using System.Collections;
using System.Reflection;
using fidt17.UnityValidationModule.Runtime.ValidationResults;

namespace fidt17.UnityValidationModule.Runtime.Attributes.FieldAttributes.CollectionFieldAttributes
{
    public class NotEmptyCollectionAttribute : NotNullValidationAttribute
    {
        private readonly bool _allowNullElements;
        
        public NotEmptyCollectionAttribute() : this(false) { }
        
        public NotEmptyCollectionAttribute(bool allowNullElements = false, bool validateInPrefab = true, bool recursiveValidation = true) : base(validateInPrefab, recursiveValidation)
        {
            _allowNullElements = allowNullElements;
        }
        
        public override ValidationResult ValidateField(FieldInfo field, object target)
        {
            if (!(field.GetValue(target) is ICollection collection))
            {
                return new FailResult($"Incorrect [NotNullCollection] attribute usage on {field.Name} at {target.GetType()}. Field must derive from ICollection", target);
            }

            var baseResult = base.ValidateField(field, target);
            if (baseResult.Result == false) return baseResult;
            
            if (collection.Count == 0)
            {
                return new FailResult($"Collection {field.Name} on {target.GetType().Name} must not be empty.", target);
            }

            if (_allowNullElements) return new PassResult(targetContext: target);
            
            foreach (var o in collection)
            {
                if (UnityExtensions.IsUnityNull(o))
                {
                    return new FailResult($"Element of collection of type {field.FieldType} on {target.GetType().Name} is missing.", target);
                }
            }

            return new PassResult(targetContext: target);
        }
    }
}