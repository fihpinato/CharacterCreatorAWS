using System;
using UnityEngine;
using UnityEngine.UI;

// by @Bullrich

namespace Blue.Console
{
    public class ConsoleVariableButton : ActionButtonBehavior
    {
        // Variable
        public Text variableText;

        private InputField field;
        private Toggle toggle;
        private Delegate variableAction;
        private bool isIntVariable;

        public override void Init(ActionContainer action)
        {
            SetVariableInputs();
            actionName = action.actionName;
            SetAction(action);
        }

        private void SetVariableInputs()
        {
            foreach (Transform t in transform)
            {
                if (t.GetComponent<InputField>() != null)
                    field = t.GetComponent<InputField>();
                else if (t.GetComponent<Toggle>() != null)
                    toggle = t.GetComponent<Toggle>();
            }
        }

        private void SetAction(ActionContainer action)
        {
            variableText.text = action.actionName;
            variableAction = action.action;
            switch (action.actType)
            {
                case ActionContainer.ActionType._bool:
                    Destroy(field.gameObject);
                    toggle.isOn = action.boolStartStatus;
                    break;
                case ActionContainer.ActionType._int:
                    Destroy(toggle.gameObject);
                    field.contentType = InputField.ContentType.IntegerNumber;
                    isIntVariable = true;
                    field.placeholder.GetComponent<Text>().text = action.intDefaultValue.ToString();
                    break;
                case ActionContainer.ActionType._float:
                    Destroy(toggle.gameObject);
                    field.contentType = InputField.ContentType.DecimalNumber;
                    isIntVariable = false;
                    field.placeholder.GetComponent<Text>().text = action.floatDefaultValue.ToString();
                    break;
            }
        }

        public void BooleanAction(bool boolean)
        {
            variableAction.DynamicInvoke(boolean);
        }

        public void OnInputEnd()
        {
            string value = field.text;
            if (isIntVariable)
                variableAction.DynamicInvoke(int.Parse(value));
            else
                variableAction.DynamicInvoke(float.Parse(value));
        }
    }
}