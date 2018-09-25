using System;

// Mady by @Bullrich

namespace Blue.Console
{
    public class ActionContainer
    {
        public enum ActionType
        {
            _void,
            _bool,
            _int,
            _float
        }

        public ActionType actType;
        public Delegate action;
        public string actionName;

        public bool boolStartStatus;
        public int intDefaultValue;
        public float floatDefaultValue;

        public ActionContainer(ActionType type, Delegate delegateAction, string _actionName)
        {
            actType = type;
            action = delegateAction;
            actionName = _actionName;
        }
    }
}