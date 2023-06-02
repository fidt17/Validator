using fidt17.UnityValidationModule.Editor.ContextParsers;
using fidt17.UnityValidationModule.Editor.Helpers;
using UnityEditor;
using UnityEngine;

namespace fidt17.UnityValidationModule.Editor.ValidationResultDrawers
{
    public class BasicResultDrawer : IValidationResultDrawer
    {
        private readonly DefaultResultParser _parser;

        public BasicResultDrawer(DefaultResultParser parser)
        {
            _parser = parser;
        }

        public virtual void DrawValidationResult()
        {
            EditorGUILayout.Space(10);

            EditorGUILayout.BeginVertical(ValidatorEditorUtils.GetBackgroundStyle(ValidatorEditorConstants.DarkBackgroundColor));

                EditorGUILayout.BeginHorizontal();

                    if (ShouldDrawPingObjectButton())
                    {
                        if (DrawShowObjectButton())
                        {
                            PingObject();
                        }
                    }
                            
                    GUILayout.Label(GetContextName(), new GUIStyle(GUI.skin.label)
                    {
                        fontSize = 14,
                        fontStyle = FontStyle.Bold,
                        alignment = TextAnchor.MiddleLeft
                    });
                            
                    GUILayout.Label(GetScopeInformation(), new GUIStyle(GUI.skin.label)
                    {
                        fontSize = 14,
                        alignment = TextAnchor.MiddleRight
                    });

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginVertical(ValidatorEditorUtils.GetBackgroundStyle(ValidatorEditorConstants.LightRedDarkBackgroundColor));
                    
                    GUILayout.Label(GetResultMessage(), new GUIStyle(GUI.skin.label)
                    {
                        fontSize = ValidatorEditorConstants.MessageFontSize
                    });

                EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndVertical();
        }

        private static bool DrawShowObjectButton()
        {
            var wasShowButtonPressed = GUILayout.Button((Texture)Resources.Load(ValidatorEditorConstants.PingButtonTextureName), new GUIStyle(GUI.skin.button)
            {
                fixedWidth = 25,
                fixedHeight = 25,
            });
            return wasShowButtonPressed;
        }

        protected virtual bool ShouldDrawPingObjectButton() => false;
        protected virtual void PingObject() {}

        protected virtual string GetContextName() => "Unidentified context";
        protected virtual string GetScopeInformation() => "Unidentified scope";
        
        protected virtual string GetResultMessage() => _parser.ResultMessage;
    }
}