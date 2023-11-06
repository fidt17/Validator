using UnityEngine;

namespace fidt17.UnityValidationModule.Editor.Helpers
{
    internal static class ValidatorEditorConstants
    {
        public static readonly Vector2 ValidatorWindowSize = new Vector2(550, 700);
        public static readonly Vector2 ValidatorSettingsWindowSize = new Vector2(240, 150);
        
        public const int MessageFontSize = 12;
        public const int MaxLengthOfContextName = 45;
        
        public static readonly Color DarkBackgroundColor = new Color(0.12f, 0.12f, 0.12f);
        public static readonly Color LightDarkBackgroundColor = new Color(0.15f, 0.15f, 0.15f);
        public static readonly Color LightRedDarkBackgroundColor = new Color(0.25f, 0.10f, 0.08f);

        public const string PingButtonTextureName = "InArrows";
        public const string NextPageButtonTextureName = "ArrowRight";
        public const string PreviousPageButtonTextureName = "ArrowLeft";

        public const string ValidatorSettingsName = "ValidatorSettings";
        
        public const int MaxResultsForPage = 12;
    }
}