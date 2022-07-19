using System;
using fidt17.UnityValidationModule.Editor.Helpers;
using fidt17.UnityValidationModule.Editor.ValidationResultDrawers;
using fidt17.UnityValidationModule.Runtime.ValidationResults;

namespace fidt17.UnityValidationModule.Editor.ContextParsers
{
    public class DefaultResultParser : BaseResultParser
    {
        public readonly string ResultMessage;
        
        public DefaultResultParser(ValidationResult vl) : base(vl)
        {
            if (vl == null) throw new ArgumentNullException();
            
            ResultMessage = ValidatorEditorUtils.SplitStringInLinesToFit(vl.Message,
                            ValidatorEditorConstants.MessageFontSize,
                            ValidatorEditorConstants.ValidatorWindowSize.x);
        }

        public override IValidationResultDrawer GetDrawer() => new BasicResultDrawer(this);
        public override bool IsContextValid(object obj) => true;
    }
}