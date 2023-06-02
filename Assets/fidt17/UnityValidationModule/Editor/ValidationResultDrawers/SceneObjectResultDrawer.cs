using System;
using fidt17.UnityValidationModule.Editor.ContextParsers;
using fidt17.UnityValidationModule.Editor.Helpers;
using UnityEditor;

namespace fidt17.UnityValidationModule.Editor.ValidationResultDrawers
{
    public class SceneObjectResultDrawer : BasicResultDrawer
    {
        private readonly SceneObjectResultParser _parser;

        public SceneObjectResultDrawer(DefaultResultParser parser) : base(parser)
        {
            if (parser is SceneObjectResultParser d) _parser = d;
            else throw new ArgumentException();
        }

        protected override bool ShouldDrawPingObjectButton() => true;
        protected override void PingObject() => EditorGUIUtility.PingObject(_parser.ObjectPath.FindGameObject());

        protected override string GetContextName() => _parser.ContextName;
        protected override string GetScopeInformation() => _parser.SceneName;
        
        protected override string GetResultMessage()
        {
            var path = ValidatorEditorUtils.SplitStringInLinesToFit($"Path: {_parser.Path}", ValidatorEditorConstants.MessageFontSize,
                ValidatorEditorConstants.ValidatorWindowSize.x - 20);
            return $"{path}" +
                   $"\n{base.GetResultMessage()}";
        }
    }
}