using System;
using System.Collections.Generic;
using fidt17.UnityValidationModule.Runtime.ValidationResults;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace fidt17.UnityValidationModule.Editor
{
    internal class ValidationUtility
    {
        private readonly Validator _validator = new Validator();

        public IEnumerable<ValidationResult> ValidateGameObject(GameObject gameObject)
        {
            if (gameObject == null) yield break;

            foreach (var monoBehaviour in gameObject.GetComponents<MonoBehaviour>())
            {
                foreach (var validationResult in _validator.Validate(monoBehaviour))
                {
                    yield return validationResult;
                }
            }

            foreach (Transform t in gameObject.transform)
            {
                foreach (var validationResult in ValidateGameObject(t.gameObject))
                {
                    yield return validationResult;
                }
            }
        }
        
        public IEnumerable<ValidationResult> ValidateEverything(List<Scene> openedScenes, bool closeScenes = true)
        {
            foreach (var validationResult in ValidateProjectAssets()) yield return validationResult;
            foreach (var validationResult in ValidateInBuildScenes(openedScenes, closeScenes)) yield return validationResult;
        }
        
        public IEnumerable<ValidationResult> ValidateProjectAssets()
        {
            foreach (var result in ValidateScriptableObjects()) yield return result;
            foreach (var result in ValidatePrefabs()) yield return result;
        }
        
        public IEnumerable<ValidationResult> ValidateInBuildScenes(List<Scene> openedScenes, bool closeScenes = true)
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
                    catch (Exception)
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
        
        public IEnumerable<ValidationResult> ValidateActiveScene() => ValidateScene(SceneManager.GetActiveScene());
        
        private IEnumerable<ValidationResult> ValidatePrefabs()
        {
            var guids = AssetDatabase.FindAssets("t:Prefab");
            for (var guidIdx = 0; guidIdx < guids.Length; guidIdx++)
            {
                var guid = guids[guidIdx];
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
                foreach (var validationResult in ValidateGameObject(prefab)) yield return validationResult;
            }
        }

        private IEnumerable<ValidationResult> ValidateScriptableObjects()
        {
            var guids = AssetDatabase.FindAssets("t:ScriptableObject");
            for (var guidIdx = 0; guidIdx < guids.Length; guidIdx++)
            {
                var guid = guids[guidIdx];
                var so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(guid));
                foreach (var validationResult in _validator.Validate(so)) yield return validationResult;
            }
        }

        private IEnumerable<ValidationResult> ValidateScene(Scene scene)
        {
            if (!SceneManager.GetSceneByName(scene.name).isLoaded)
            {
                throw new Exception("Scene must be loaded before validation");
            }

            var rootObjects = scene.GetRootGameObjects();
            
            foreach (var rootObject in rootObjects)
            {
                foreach (var validationResult in ValidateGameObject(rootObject))
                {
                    yield return validationResult;
                }
            }
        }
    }
}