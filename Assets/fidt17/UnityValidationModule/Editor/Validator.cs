using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using fidt17.UnityValidationModule.Editor.Helpers;
using fidt17.UnityValidationModule.Runtime.Attributes.FieldAttributes;
using fidt17.UnityValidationModule.Runtime.Attributes.MethodAttributes;
using fidt17.UnityValidationModule.Runtime.ValidationResults;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

[assembly: InternalsVisibleTo("UnityValidationModule.Tests")]
[assembly: InternalsVisibleTo("UnityValidationModule.Tests.Editor")]

namespace fidt17.UnityValidationModule.Editor
{
    internal static class Validator
    {
        private static Dictionary<Type, List<FieldAttributePair>> _cachedValidationFields;
        private static Dictionary<Type, List<MethodAttributePair>> _cachedValidationMethods;

        public static void Reset()
        {
            _cachedValidationFields = new Dictionary<Type, List<FieldAttributePair>>();
            _cachedValidationMethods = new Dictionary<Type, List<MethodAttributePair>>();
        }
        
        private struct FieldAttributePair
        {
            public FieldInfo Field;
            public FieldValidationAttribute Attribute;
        }
        
        private struct MethodAttributePair
        {
            public MethodInfo Method;
            public BaseMethodValidationAttribute Attribute;
        }
        
        public static IEnumerable<ValidationResult> Validate(System.Object validationTarget, HashSet<System.Object> alreadyValidated = null, Object lastContext = null)
        {
            if (validationTarget == null) yield break;
            if (alreadyValidated == null)
            {
                alreadyValidated = new HashSet<System.Object>();
            }
            alreadyValidated.Add(validationTarget);

            if (_cachedValidationFields == null)
            {
                _cachedValidationFields = new Dictionary<Type, List<FieldAttributePair>>();
            }

            if (_cachedValidationMethods == null)
            {
                _cachedValidationMethods = new Dictionary<Type, List<MethodAttributePair>>();
            }

            var validationTargetType = validationTarget.GetType();
            if (!_cachedValidationFields.ContainsKey(validationTargetType))
            {
                var l = new List<FieldAttributePair>();
                foreach (var field in validationTargetType.GetValidationFields())
                {
                    foreach (var attribute in field.GetAttributesOfType<FieldValidationAttribute>())
                    {
                        l.Add(new FieldAttributePair()
                        {
                            Field = field,
                            Attribute = attribute
                        });
                    }
                }
                _cachedValidationFields.Add(validationTargetType, l);
            }

            if (!_cachedValidationMethods.ContainsKey(validationTargetType))
            {
                var l = new List<MethodAttributePair>();
                foreach (var validationMethod in validationTarget.GetType().GetValidationMethods())
                {
                    foreach (var validationAttribute in validationMethod.GetAttributesOfType<BaseMethodValidationAttribute>())
                    {
                        l.Add(new MethodAttributePair()
                        {
                            Attribute = validationAttribute,
                            Method = validationMethod
                        });

                        ValidationResult r;
                        try
                        {
                            r = validationAttribute.ValidateMethod(validationMethod, validationTarget);
                        }
                        catch (Exception e)
                        {
                            r = new FailResult($"Validation Failed with exception:\n {e}", targetContext: validationTarget);
                        }
                        yield return r;
                    }
                }
                _cachedValidationMethods.Add(validationTargetType, l);
            }

            //Field Validation
            var validationPairs = _cachedValidationFields[validationTargetType];
            for (var i = 0; i < validationPairs.Count; i++)
            {
                var attribute = validationPairs[i].Attribute;
                var field = validationPairs[i].Field;

                if (attribute.ShouldValidate(validationTarget) == false)
                {
                    yield return new PassResult();
                    continue;
                }

                var r = attribute.ValidateField(field, validationTarget);
                if (!(r.TargetContext is UnityEngine.Object)) r.TargetContext = lastContext;
                else lastContext = r.TargetContext as UnityEngine.Object;

                yield return r;
                    
                if (attribute.RecursiveValidation == false) continue;
                    
                var fieldValue = field.GetValue(validationTarget);
                if (fieldValue == null) continue;
                if (fieldValue.GetType().GetInterfaces().All(x => x != typeof(ICollection)))
                {
                    //single field
                    if (alreadyValidated.Contains(fieldValue)) continue;
                    foreach (var validationResult in Validate(fieldValue, alreadyValidated, lastContext))
                    {
                        yield return validationResult;
                    }
                }
                else
                {
                    //collection
                    var collection = (ICollection)fieldValue;
                    foreach (var o in collection)
                    {
                        if (o == null) continue;
                        if (alreadyValidated.Contains(o)) continue;
                        foreach (var validationResult in Validate(o, alreadyValidated, lastContext))
                        {
                            yield return validationResult;
                        }
                    }
                }
            }

            var validationMethods = _cachedValidationMethods[validationTargetType];
            for (var i = 0; i < validationMethods.Count; i++)
            {
                var attribute = validationMethods[i].Attribute;
                var method = validationMethods[i].Method;
                
                ValidationResult r;
                try
                {
                    r = attribute.ValidateMethod(method, validationTarget);
                }
                catch (Exception e)
                {
                    r = new FailResult($"Validation Failed with exception:\n {e}", targetContext: validationTarget);
                }
                yield return r;
            }
        }
        
        public static IEnumerable<ValidationResult> Validate(GameObject gameObject)
        {
            if (gameObject == null) yield break;

            foreach (var monoBehaviour in gameObject.GetComponents<MonoBehaviour>())
            {
                foreach (var validationResult in Validate(monoBehaviour))
                {
                    yield return validationResult;
                }
            }

            foreach (Transform t in gameObject.transform)
            {
                foreach (var validationResult in Validate(t.gameObject))
                {
                    yield return validationResult;
                }
            }
        }
        
        #region Scene Validation
        
        /// <summary>
        /// Validates all objects on provided scene
        /// </summary>
        private static IEnumerable<ValidationResult> ValidateScene(Scene scene)
        {
            if (!SceneManager.GetSceneByName(scene.name).isLoaded)
            {
                throw new Exception("Scene must be loaded before validation");
            }

            var rootObjects = scene.GetRootGameObjects();
            
            foreach (var rootObject in rootObjects)
            {
                foreach (var validationResult in Validate(rootObject))
                {
                    yield return validationResult;
                }
            }
        }
        
        /// <summary>
        /// Validates all objects on active scene
        /// </summary>
        public static IEnumerable<ValidationResult> ValidateActiveScene()
        {
            foreach (var validationResult in ValidateScene(SceneManager.GetActiveScene()))
            {
                yield return validationResult;
            }
        }
        
        /// <summary>
        /// Validates all objects on all build scenes
        /// </summary>
        public static IEnumerable<ValidationResult> ValidateInBuildScenes(List<Scene> openedScenes, bool closeScenes = true)
        {
            var activeScene = SceneManager.GetActiveScene();

            var scenePaths = new List<string>();
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                scenePaths.Add(SceneUtility.GetScenePathByBuildIndex(i));
            }
            
            foreach (var scenePath in scenePaths)
            {
                Scene scene = activeScene;
                if (activeScene.path != scenePath)
                {
                    try
                    {
                        scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Could not open scene {scenePath}. Check build settings.");
                        continue;
                    }
                    openedScenes.Add(scene);
                }

                foreach (var validationResult in ValidateScene(scene)) yield return validationResult;

                if (closeScenes)
                {
                    EditorSceneManager.CloseScene(scene, true);
                }
            }
        }
        
        #endregion
        
        #region Assets Validation

        /// <summary>
        /// Validates all objects on all build scenes and all project assets
        /// </summary>
        public static IEnumerable<ValidationResult> ValidateEverything(List<Scene> openedScenes, bool closeScenes = true)
        {
            foreach (var validationResult in ValidateProjectAssets()) yield return validationResult;
            foreach (var validationResult in ValidateInBuildScenes(openedScenes, closeScenes)) yield return validationResult;
        }

        /// <summary>
        /// Validates all project assets (Prefabs and ScriptableObjects)
        /// </summary>
        public static IEnumerable<ValidationResult> ValidateProjectAssets()
        {
            foreach (var result in ValidateScriptableObjects()) yield return result;
            foreach (var result in ValidatePrefabs()) yield return result;
        }

        /// <summary>
        /// Validates all ScriptableObjects
        /// </summary>
        private static IEnumerable<ValidationResult> ValidateScriptableObjects()
        {
            var guids = AssetDatabase.FindAssets("t:ScriptableObject");
            for (var guidIdx = 0; guidIdx < guids.Length; guidIdx++)
            {
                var guid = guids[guidIdx];
                var so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(guid));
                foreach (var validationResult in Validate(so)) yield return validationResult;
            }
        }

        /// <summary>
        /// Validates all Prefabs
        /// </summary>
        private static IEnumerable<ValidationResult> ValidatePrefabs()
        {
            var guids = AssetDatabase.FindAssets("t:Prefab");
            for (var guidIdx = 0; guidIdx < guids.Length; guidIdx++)
            {
                var guid = guids[guidIdx];
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
                foreach (var validationResult in Validate(prefab)) yield return validationResult;
            }
        }

        #endregion
    }
}