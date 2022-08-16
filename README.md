# UnityValidationModule
## Version : 1.0.2

## Installation
1) Unity Package Manager
2) Add package from git URL:
3) https://github.com/fidt17/UnityValidationModule.git?path=/Assets/fidt17/UnityValidationModule

## Validation Attributes

### [NotNullValidation] : [FieldValidation]
  Fails if <b>MyObject</b> is null or missing.
  Validate in prefab: should validator check this field in prefabs?
  Recursive validation: should validator try to v
  ```css
  [NotNullValidation(validateInPrefab: true, recursiveValidation: true)] public Object MyObject;
  ```
  
### [NotNullCollection] : [NotNullValidation]
  Fails if <b>MyCollection</b> if any of it's elements are null.
  Must be used on fields of type that implements <b>ICollection</b> interface
  ```css
  [NotNullCollection(validateInPrefab: true, recursiveValidation: true)] public IList<Object> MyCollection;
  ```
  
### [NotEmptyCollection] : [NotNullValidation]
  Fails if <b>MyCollection</b> is null or empty.
  Must be used on fields of type that implements <b>ICollection</b> interface
  ```css
  [NotEmptyCollection] public IList<Object> MyCollection;
  ```
  
### UnityEventValidation : NotNullValidation
  
  ```css
  [UnityEventValidation] public UnityEvent UEvent;
  ```

### [ValidationMethod]
  Validation methods can be used for complex validation logic.
  ```css
  [ValidationMethod(message: "This message will be displayed if validation fails.")]
  private bool MyValidationMethod()
  {
      bool validationResult = false;
      // ...
      return validationResult;
  }
  ```
