# UnityValidationModule
## Version : 1.0.3

## Installation
1) Unity Package Manager
2) Add package from git URL:
3) https://github.com/fidt17/UnityValidationModule.git?path=/Assets/fidt17/UnityValidationModule

## How to use

You can run validator either by pressing [Alt+V] 
<br> Or by navigating to "Tools/Validator/Validator Window"

![](Screenshots/Validator%20Window%20Screenshot%20Main.png?raw=true))

## Validation Attributes

### NotNullValidation
  Fails if <b>MyObject</b> is null or missing
  ```csharp
  [NotNullValidation(validateInPrefab: true, recursiveValidation: true)]
  public Object MyObject;
  ```
  
### NotNullCollection
  Fails if <b>MyCollection</b> or any of it's elements are null
  > Field must be of type that implements <b>ICollection</b> interface
  ```csharp
  [NotNullCollection(validateInPrefab: true, recursiveValidation: true)]
  public IList<Object> MyCollection;
  ```
  
### NotEmptyCollection
  Fails if <b>MyCollection</b> is null or empty
  > Field must be of type that implements <b>ICollection</b> interface
  ```csharp
  [NotEmptyCollection(allowNullElements: true, validateInPrefab: true, recursiveValidation: true)]
  public ICollection<Object> MyCollection;
  ```
  
### UnityEventValidation
  Fails if any of call targets or methods are invalid
  > Field must be of type <b>UnityEvent</b>
  ```csharp
  [UnityEventValidation(validateInPrefab: true, recursiveValidation: true)]
  public UnityEvent UEvent;
  ```

### ValidationMethod
  Validation methods can be used for complex validation logic
  > Must be used on a method
  > <br> Method must return a boolean
  ```csharp
  [ValidationMethod(message: "This message will be displayed if validation fails.")]
  private bool MyValidationMethod()
  {
      bool validationResult = false;
      // ...
      return validationResult;
  }
  ```
