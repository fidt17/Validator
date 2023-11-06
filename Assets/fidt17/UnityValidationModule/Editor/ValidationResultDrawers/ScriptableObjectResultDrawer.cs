using System;
using fidt17.UnityValidationModule.Editor.ContextParsers;
using UnityEditor;

namespace fidt17.UnityValidationModule.Editor.ValidationResultDrawers
{
    internal class ScriptableObjectResultDrawer : BasicResultDrawer
    {
        private ScriptableObjectResultParser _parser;

        public ScriptableObjectResultDrawer(DefaultResultParser parser) : base(parser)
        {
            _parser = parser as ScriptableObjectResultParser;
            if (_parser == null) throw new Exception("Parser should be ScriptableObjectContextParser");
        }

        protected override bool ShouldDrawPingObjectButton() => true;
        protected override void PingObject() => EditorGUIUtility.PingObject(_parser.ScriptableObject);
        protected override string GetContextName() => _parser.ScriptableObject.name;
        protected override string GetScopeInformation() => "ScriptableObject";

    }
}