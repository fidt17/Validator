using System;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace fidt17.UnityValidationModule.Editor.Helpers
{
    internal class AbsoluteGameObjectPath
    {
        private readonly string _scenePath;
        private readonly string _gameObjectName;
        private readonly List<int> _hierarchyPosition = new List<int>();
        private readonly string _pathToRoot;

        public AbsoluteGameObjectPath(GameObject gameObject)
        {
            if (gameObject == null) throw new ArgumentNullException();
            
            _scenePath = gameObject.scene.path;
            _gameObjectName = gameObject.name;
			
            Transform node = gameObject.transform;
            do
            {
                _pathToRoot = $"/{node.gameObject.name}" + _pathToRoot;
                _hierarchyPosition.Insert(0, node.transform.GetSiblingIndex());
                node = node.parent;
            } while (node != null);
        }

        public static string FormPathToRoot(GameObject gameObject)
        {
            var str = "";
            Transform node = gameObject.transform;
            do
            {
                if (node.parent == null)
                {
                    str = $"{node.gameObject.name}" + str;
                }
                else
                {
                    str = $" -> {node.gameObject.name}" + str;
                }
                node = node.parent;
            } while (node != null);

            return str;
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

                var node = SceneManager.GetActiveScene().GetRootGameObjects()[_hierarchyPosition[0]].transform;
                for (int i = 1; i < _hierarchyPosition.Count; i++)
                {
                    node = node.GetChild(_hierarchyPosition[i]);
                }

                if (node == null) throw new Exception();
                
                return node.gameObject;
            }
            catch (Exception)
            {
                Debug.LogError($"Could not find desired GameObject." +
                               $"Name: {_gameObjectName}," +
                               $"Path: {_scenePath}{_pathToRoot},");
                return null;
            }
        }
    }
}