using System;
using System.Collections.Generic;
using System.Reflection;
using fidt17.UnityValidationModule.Runtime.ValidationResults;

namespace fidt17.UnityValidationModule.Runtime.Attributes.FieldAttributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class UniqueValidationAttribute : FieldValidationAttribute
    {
        private readonly string _groupName;
        private readonly string _notAssignedIdValue;

        public UniqueValidationAttribute(string groupName, string notAssignedIdValue = "NaN")
        {
            _groupName = groupName;
            _notAssignedIdValue = notAssignedIdValue;
        }
        
        public override ValidationResult ValidateField(FieldInfo field, object target)
        {
            const string dataKey = "IdValidationTypeToIdMap";
            if (!ValidationSession.Data.ContainsKey(dataKey))
            {
                ValidationSession.Data.Add(dataKey, new Dictionary<string, HashSet<object>>());
            }
            
            var typeToIdMap = (Dictionary<string, HashSet<object>>) ValidationSession.Data[dataKey];
            
            if (!typeToIdMap.ContainsKey(_groupName))
            {
                typeToIdMap.Add(_groupName, new HashSet<object>());
            }

            var value = field.GetValue(target);

            if (value is string str1 && str1 == _notAssignedIdValue)
            {
                return new PassResult(); // todo: might want to store these results in "warning" group
            }

            if (value == null || value is string str && string.IsNullOrWhiteSpace(str))
            {
                return new FailResult($"ID is null or empty in group {_groupName}. \nYou may use {_notAssignedIdValue} as value if this is expected behaviour. {GetTypeMessage(field, target)}", target);
            }

            if (typeToIdMap[_groupName].Contains(value))
            {
                return new FailResult($"ID {value} is already taken in group {_groupName}.{GetTypeMessage(field, target)}", target);
            }

            typeToIdMap[_groupName].Add(value);
            
            return base.ValidateField(field, target);
        }
    }
}