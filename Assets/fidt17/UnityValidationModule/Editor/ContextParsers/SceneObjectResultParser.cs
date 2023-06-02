using fidt17.UnityValidationModule.Editor.Helpers;
using fidt17.UnityValidationModule.Editor.ValidationResultDrawers;
using fidt17.UnityValidationModule.Runtime;
using fidt17.UnityValidationModule.Runtime.ValidationResults;
using UnityEngine;

namespace fidt17.UnityValidationModule.Editor.ContextParsers
{
    [ValidationResultParser(typeof(Object))]
    public class SceneObjectResultParser : UnityObjectResultParser
    {
        public readonly string SceneName;
        public readonly string Path;
        public readonly AbsoluteGameObjectPath ObjectPath;
        
        public SceneObjectResultParser(ValidationResult vl) : base(vl)
        {
            var sceneObject = UnityExtensions.CastToGameObject(GetUnityContext(vl));
            ObjectPath = new AbsoluteGameObjectPath(sceneObject);
            SceneName = sceneObject.scene.name;
            Path = SceneName + AbsoluteGameObjectPath.FormPathToRoot(sceneObject);
        }

        public override IValidationResultDrawer GetDrawer() => new SceneObjectResultDrawer(this);

        public override bool IsContextValid(object obj)
        {
            var unityObject = (Object)obj;
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

            return !gameObject.IsAssetPrefab();
        }
    }
}