using System;
using fidt17.UnityValidationModule.Editor.Helpers;
using fidt17.UnityValidationModule.Editor.ValidationResultDrawers;
using fidt17.UnityValidationModule.Runtime;
using fidt17.UnityValidationModule.Runtime.ValidationResults;
using UnityEngine;

namespace fidt17.UnityValidationModule.Editor.ContextParsers
{
    [ValidationResultParser(typeof(UnityEngine.Object))]
    public class PrefabObjectResultParser : UnityObjectResultParser
    {
        public readonly GameObject Prefab;
        public readonly string Path;
        
        public PrefabObjectResultParser(ValidationResult vl) : base(vl)
        {
            if (vl == null) throw new ArgumentNullException();
            var go = UnityExtensions.CastToGameObject(GetUnityContext(vl));

            var node = go.transform.parent;
            if (node != null)
            {
                while (node.parent != null)
                {
                    node = node.parent;
                }
                Prefab = node.gameObject;
            }
            else
            {
                Prefab = go;
            }

            Path = AbsoluteGameObjectPath.FormPathToRoot(go);
        }

        public override IValidationResultDrawer GetDrawer() => new PrefabObjectResultDrawer(this);
        
        public override bool IsContextValid(object obj)
        {
            var unityObject = (UnityEngine.Object)obj;
            if (unityObject == null) return false;
                
            GameObject gameObject;
            if (unityObject is GameObject o)
            {
                gameObject = o;
            }
            else if (unityObject is MonoBehaviour monoBehaviour)
            {
                gameObject = monoBehaviour.gameObject;
            }
            else
            {
                return false;
            }

            var root = gameObject;
            while (root.transform.parent != null)
            {
                root = root.transform.parent.gameObject;
            }

            return root.IsAssetPrefab();
        }
    }
}