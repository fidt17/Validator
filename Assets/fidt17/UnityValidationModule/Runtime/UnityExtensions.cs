using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace fidt17.UnityValidationModule.Runtime
{
    public static class UnityExtensions
    {
        /// <summary>
        /// Verify that object is either null or "None" or "Missing"
        /// </summary>
        /// Sometimes values on fields can go "missing"
        /// which cannot be tracked by comparison to null.
        public static bool IsUnityNull(System.Object value)
        {
            if (value == null)
            {
                return true;
            }
            
            if (value is UnityEngine.Object unityObject)
            {
                if (unityObject.GetInstanceID() == 0)
                {
                    return true;
                }
                
                try
                {
                    var t = unityObject.name;
                }
                catch (Exception)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Analyses unityEvent's subscribers and detects if any of them are invalid
        /// </summary>
        public static bool IsUnityEventValid(UnityEvent unityEvent)
        {
            if (unityEvent == null) return false;

            for (int i = 0; i < unityEvent.GetPersistentEventCount(); i++)
            {
                var target = unityEvent.GetPersistentTarget(i);
                if (IsUnityNull(target)) return false;

                var methodName = unityEvent.GetPersistentMethodName(i);
                if (string.IsNullOrEmpty(methodName)) return false;
                
                var type = Type.GetType(target.GetType().FullName);
                if (type == null) return false;

                var method = type.GetMethod(methodName);
                if (method == null) return false;
            }
            
            return true;
        }

        #if UNITY_EDITOR
        /// <summary>
        /// Checks if object is a part of a prefab that wasn't instantiated (is an asset)
        /// </summary>
        public static bool IsAssetPrefab(this UnityEngine.Object value)
        {
            if (value == null) return false;
            return PrefabUtility.IsPartOfAnyPrefab(value) && !PrefabUtility.IsPartOfPrefabInstance(value);
        }
        #endif

        /// <summary>
        /// Returns GameObject if provided value is a GameObject or a MonoBehaviour component on a GameObject
        /// Returns null otherwise
        /// </summary>
        public static GameObject CastToGameObject(UnityEngine.Object unityObject)
        {
            if (unityObject is GameObject go) return go;
            if (unityObject is Component component)
            {
                try
                {
                    return component.gameObject;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return null;
        }
    }
}