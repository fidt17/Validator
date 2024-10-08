# Unity Validator
Version : 1.1.1

## Installation
1) Unity Package Manager
2) Add package from git URL:
3) https://github.com/fidt17/Validator.git?path=/Assets/fidt17/UnityValidationModule

## How to use

Run validator either by pressing [Alt+V] or by navigating to "Tools/Validator/Validator Window"

![](Screenshots/Validator%20Window%20Screenshot%20Main.png?raw=true))

Specify Validation mode by selecting one of the options from dropdown menu
- "Active Scene": Validate all objects on loaded scene
- "In Build Scenes": Validate all objects across all build scenes. You will be asked to save current scene if required
- "Project Assets": Validate all Prefabs and ScriptableObjects
- "Everything": Validate "In Build Scene" + "Project Assets"

### Settings

- "Run Validator on build":  If enabled will start Validator in "Everything" mode before each new build
- "Stop build on Validator failure":  If enabled an exception will be thrown once Validator finds an error. Which should cancel active build process

## Validation Attributes

- #### NotNullValidation
  > Fails if <b>MyObject</b> is null or missing
  ```csharp
  [NotNullValidation(validateInPrefab: true, recursiveValidation: true)]
  public Object MyObject;
  ```

  > Fails if <b>MyCollection</b> or any of it's elements are null
  ```csharp
  [NotNullValidation(validateInPrefab: true, recursiveValidation: true)]
  public IList<Object> MyCollection;
  ```
  
- #### NotEmptyCollection
  > Fails if <b>MyCollection</b> is null or empty
  > Field must be of type that implements <b>ICollection</b> interface
  ```csharp
  [NotEmptyCollection(allowNullElements: false, validateInPrefab: true, recursiveValidation: true)]
  public ICollection<Object> MyCollection;
  ```
  
- #### UnityEventValidation
  > Fails if any of call targets or methods are invalid
  > Field must be of type <b>UnityEvent</b>
  ```csharp
  [UnityEventValidation(validateInPrefab: true, recursiveValidation: true)]
  public UnityEvent UEvent;
  ```

- #### ValidationMethod
  > Method must return a boolean
  ```csharp
  [ValidationMethod(message: "This message will be displayed if validation fails.")]
  private bool MyValidationMethod()
  {
      bool validationResult = false;
      // ...
      return validationResult;
  }
  ```

## Custom Validation Attributes
  You can create your own validation attributes with complex logic
  ```csharp
  public class LocalizationValidationAttribute : FieldValidationAttribute
  {
      public override ValidationResult ValidateField(FieldInfo field, object target)
      {
          var key = (string)field.GetValue(target);
          var languages = LocalizationUtility.GetAllLanguages();
          foreach (var language in languages)
          {
              if (string.IsNullOrEmpty(key)) continue;
              var localizationMap = LocalizationUtility.GetLanguageMap(language);
              if (localizationMap.IsValidLocalizationKey(key)) continue;

              return new FailResult($"Missing key [{key}] in [{language}] map.", target);
          }

          return new PassResult();
      }
  }
  ```
  Usage
  ```csharp
  [LocalizationValidation]
  public string MyLocalizationKey;
  ```
