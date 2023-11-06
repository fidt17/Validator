using fidt17.UnityValidationModule.Editor.Helpers;
using UnityEditor;
using UnityEngine;

namespace fidt17.UnityValidationModule.Editor
{
    internal class ValidatorSettings : ScriptableObject
    {
        public bool RunValidatorOnBuild
        {
            get => _runValidatorOnBuild;
            set => _runValidatorOnBuild = value;
        }

        public bool StopBuildIfValidatorFails
        {
            get => _stopBuildIfValidatorFails;
            set => _stopBuildIfValidatorFails = value;
        }

        public ValidationScopeEnum SelectedScope
        {
            get => _selectedScope;
            set => _selectedScope = value;
        }

        [SerializeField] private bool _runValidatorOnBuild;
        [SerializeField] private bool _stopBuildIfValidatorFails = true;
        [SerializeField] private ValidationScopeEnum _selectedScope = ValidationScopeEnum.ActiveScene;
		
        public static ValidatorSettings GetSettings()
        {
            var path = $"Assets/Resources/Validator/{ValidatorEditorConstants.ValidatorSettingsName}.asset";
            var settings = AssetDatabase.LoadAssetAtPath<ValidatorSettings>(path);
            if (settings == null)
            {
                settings = CreateInstance<ValidatorSettings>();

                Debug.Log("Create validator settings asset");

                if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                {
                    AssetDatabase.CreateFolder("Assets", "Resources");
                    AssetDatabase.SaveAssets();
                }

                if (!AssetDatabase.IsValidFolder("Assets/Resources/Validator"))
                {
                    AssetDatabase.CreateFolder("Assets/Resources", "Validator");
                    AssetDatabase.SaveAssets();
                }
				
                AssetDatabase.CreateAsset(settings, path);
                AssetDatabase.SaveAssets();
            }

            return settings;
        }
    }
}