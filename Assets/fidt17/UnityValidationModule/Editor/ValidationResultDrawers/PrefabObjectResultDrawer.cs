using fidt17.UnityValidationModule.Editor.ContextParsers;
using fidt17.UnityValidationModule.Editor.Helpers;
using UnityEditor;

namespace fidt17.UnityValidationModule.Editor.ValidationResultDrawers
{
    public class PrefabObjectResultDrawer : BasicResultDrawer
    {
        private readonly PrefabObjectResultParser _parser;

        public PrefabObjectResultDrawer(DefaultResultParser parser) : base(parser)
        {
            _parser = (PrefabObjectResultParser) parser;
        }

        protected override bool ShouldDrawPingObjectButton() => true;
        protected override void PingObject() => EditorGUIUtility.PingObject(_parser.Prefab);

        protected override string GetContextName() => _parser.Prefab.name;
        protected override string GetScopeInformation() => "Prefab";

        protected override string GetResultMessage()
        {
            var path = ValidatorEditorUtils.SplitStringInLinesToFit(_parser.Path, ValidatorEditorConstants.MessageFontSize,
                ValidatorEditorConstants.ValidatorWindowSize.x - 20);
            return $"Path: {path}" +
                   $"\n{base.GetResultMessage()}";
        }
    }
}