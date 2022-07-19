using fidt17.UnityValidationModule.Editor.ContextParsers;
using UnityEditor;

namespace fidt17.UnityValidationModule.Editor.ValidationResultDrawers
{
    public class PrefabObjectResultDrawer : BasicResultDrawer
    {
        private PrefabObjectResultParser _parser;

        public PrefabObjectResultDrawer(DefaultResultParser parser) : base(parser)
        {
            _parser = (PrefabObjectResultParser) parser;
        }

        protected override bool ShouldDrawPingObjectButton() => true;
        protected override void PingObject() => EditorGUIUtility.PingObject(_parser.Prefab);

        protected override string GetContextName() => _parser.Prefab.name;
        protected override string GetScopeInformation() => "Prefab";
    }
}