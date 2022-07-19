using System;
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
        
        public PrefabObjectResultParser(ValidationResult vl) : base(vl)
        {
            if (vl == null) throw new ArgumentNullException();
            Prefab = UnityExtensions.CastToGameObject(GetUnityContext(vl));
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

            return gameObject.IsAssetPrefab();
        }
    }
}