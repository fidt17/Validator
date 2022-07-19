using System;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace fidt17.UnityValidationModule.Editor.Helpers
{
    public class AbsoluteGameObjectPath
    {
        private readonly string _scenePath;
        private readonly string _parentPath;
        private readonly int _parentRootIndex;
        private readonly int _childIndex;

        public AbsoluteGameObjectPath(GameObject gameObject)
        {
            if (gameObject == null) throw new ArgumentNullException();
            
            _scenePath = gameObject.scene.path;

            if (gameObject.transform.parent != null)
            {
                var obj = gameObject.transform.parent.gameObject;
                string path = "/" + obj.name;
                while (obj.transform.parent != null)
                {
                    obj = obj.transform.parent.gameObject;
                    path = "/" + obj.name + path;
                }

                var rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
                for (int i = 0; i < rootObjects.Length; i++)
                {
                    if (rootObjects[i] == obj)
                    {
                        _parentRootIndex = i;
                        break;
                    }
                }
                _parentPath = path;
            }
            _childIndex = gameObject.transform.GetSiblingIndex();
        }

        /// <summary>
        /// Searches for a GameObject on the calculated previously path 
        /// Will load GameObject's scene if needed
        /// Returns null on fail
        /// </summary>
        public GameObject FindGameObject()
        {
            try
            {
                if (SceneManager.GetActiveScene().path != _scenePath)
                {
                    EditorSceneManager.OpenScene(_scenePath, OpenSceneMode.Single);
                }
                
                var parent = GameObject.Find(_parentPath);
                if (parent == null)
                {
                    return SceneManager.GetActiveScene().GetRootGameObjects()[_childIndex];
                }

                if (parent.transform.parent == null)
                {
                    parent = SceneManager.GetActiveScene().GetRootGameObjects()[_parentRootIndex];
                }

                var r = parent.transform.GetChild(_childIndex).gameObject;
                if (r == null) throw new Exception();
                
                return r;
            }
            catch (Exception e)
            {
                Debug.LogError($"Could not find desired GameObject." +
                               $"Scene path: {_scenePath}," +
                               $"Parent path: {_parentPath}," +
                               $"Parent Root Index: {_parentRootIndex}," +
                               $"Child Index: {_childIndex}");
                return null;
            }
        }
    }
}