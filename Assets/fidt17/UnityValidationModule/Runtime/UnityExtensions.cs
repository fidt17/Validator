using System;
using UnityEditor;
using UnityEngine;

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

        #if UNITY_EDITOR
        /// <summary>
        /// Checks if object is a part of a prefab that wasn't instantiated (is an asset)
        /// </summary>
        public static bool IsAssetPrefab(this UnityEngine.Object value)
        {
            if (value == null) return false;
            return PrefabUtility.IsPartOfPrefabAsset(value) && !PrefabUtility.IsPartOfPrefabInstance(value);
        }
        #endif

        /// <summary>
        /// Returns GameObject if provided value is a GameObject or a MonoBehaviour component on a GameObject
        /// Throws an exception otherwise
        /// </summary>
        public static GameObject CastToGameObject(UnityEngine.Object unityObject)
        {
            if (unityObject is GameObject go) return go;
            if (unityObject is MonoBehaviour mono)
            {
                try
                {
                    return mono.gameObject;
                }
                catch (Exception)
                {
                    throw new Exception($"Could get GameObject from {unityObject.GetType()}");
                }
            }
            throw new Exception($"Could get GameObject from {unityObject.GetType()}");
        }
    }
}