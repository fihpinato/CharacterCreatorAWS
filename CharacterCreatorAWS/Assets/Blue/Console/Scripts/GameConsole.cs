using System.Collections.Generic;
using Blue.Console;
using UnityEngine;

// Mady by @Bullrich

namespace Blue
{
    public class GameConsole
    {
        public delegate void booleanAction(bool boolInput);

        public delegate void voidAction();

        public delegate void intAction(int intInput);

        public delegate void floatAction(float floatInput);

        private static List<ActionContainer> actions;

        public delegate void ListUpdated(List<ActionContainer> actionList);

        public static event ListUpdated listUpdated;

        public delegate void ConsoleMessage(string title, string message);

        public static event ConsoleMessage consoleMessage;

        private static List<ActionContainer> getActions()
        {
            if (actions == null)
                actions = new List<ActionContainer>();
            return actions;
        }

        private static void AddActionToList(ActionContainer container)
        {
            if (!getActions().Contains(container))
                getActions().Add(container);
            else
                Debug.LogWarning(string.Format("Action {0} has already being added!", container.actionName));

            listUpdated(getActions());
        }

        public static void AddAction(booleanAction action, string actionName, bool defaultBooleanState = false)
        {
            ActionContainer acon = new ActionContainer(ActionContainer.ActionType._bool, action, actionName);
            acon.boolStartStatus = defaultBooleanState;
            AddActionToList(acon);
        }

        public static void AddAction(voidAction action, string actionName)
        {
            AddActionToList(new ActionContainer(ActionContainer.ActionType._void, action, actionName));
        }

        public static void AddAction(floatAction action, string actionName, float defaultFloatValue = 0f)
        {
            ActionContainer acon = new ActionContainer(ActionContainer.ActionType._float, action, actionName);
            acon.floatDefaultValue = defaultFloatValue;
            AddActionToList(acon);
        }

        public static void AddAction(intAction action, string actionName, int defaultIntValue = 0)
        {
            ActionContainer acon = new ActionContainer(ActionContainer.ActionType._int, action, actionName);
            acon.intDefaultValue = defaultIntValue;
            AddActionToList(acon);
        }

        public static void WriteMessage(string title, string message)
        {
            consoleMessage(title, message);
        }

        public static void RemoveAction(string _actionName)
        {
            foreach (ActionContainer action in getActions())
            {
                if (action.actionName == _actionName)
                {
                    getActions().Remove(action);
                    listUpdated(getActions());
                    break;
                }
            }
        }
    }

    public abstract class ActionButtonBehavior : MonoBehaviour
    {
        protected string actionName = null;
        public abstract void Init(ActionContainer action);

        public string GetActionName()
        {
            return actionName;
        }
    }
}