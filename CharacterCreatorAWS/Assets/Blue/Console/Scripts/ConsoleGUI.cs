using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Mady by @Bullrich

namespace Blue.Console
{
    [RequireComponent(typeof(ConsoleOutput))]
    public class ConsoleGUI : MonoBehaviour
    {
        [Tooltip("Prefab object")] public LogInfo logInfo;
        public Transform logScroll, popUpDetail;
        private Transform logSection, actionSection;
        private Text detailInformation;
        public ConsolePopup popup;
        public ActionButtons actionButtons;
        public Image backgroundImage;

        public LogDetails logDetail;
        private ConsoleGuiManager guiManager;
        private List<ActionContainer> actionsList;
        private SwipeManager openConsoleSettings;
        private string mailSubject;
        private string DefaultMailDirectory = "example@gmail.com";
        private bool _minifiedConsole;

        public void LogMessage(LogType type, string stackTrace, string message)
        {
            LogInfo info = Instantiate(logInfo);
            info.gui = this;
            guiManager.LogMessage(type,
                stackTrace, message, info);
        }

        public void init(SwipeManager swipe, bool minifyOnStart, int logLimit, string defaultMail)
        {
            openConsoleSettings = swipe;
            guiManager = new ConsoleGuiManager(
                logScroll.transform.parent.GetComponent<ScrollRect>(), logDetail, popup, logLimit);
            detailInformation = popUpDetail.GetChild(0).GetChild(0).GetComponent<Text>();
            CleanConsole();
            logSection = logScroll.transform.parent;
            actionSection = actionButtons.actionsContainer.transform.parent.parent;
            GameConsole.listUpdated += AddListElement;
            GameConsole.consoleMessage += WriteToConsole;
            GetComponent<ConsoleOutput>().init(this);
            _minifiedConsole = minifyOnStart;
            DefaultMailDirectory = defaultMail;

            if (!Debug.isDebugBuild)
                Debug.LogWarning("This isn't a development build! You won't be able to read the stack trace!");
        }

        private void AddActionElement(ActionContainer acon)
        {
            ActionButtonBehavior actionButton = actionButtons.actionBtnPrefab;
            Transform parent = actionButtons.actionsContainer.transform;
            if (acon.actType != ActionContainer.ActionType._void)
            {
                actionButton = actionButtons.variablesBtnPrefab;
                parent = actionButtons.variablesContainer.transform;
            }
            ActionButtonBehavior spawnedBtn = Instantiate(actionButton);
            spawnedBtn.transform.SetParent(parent, false);
            spawnedBtn.Init(acon);
            guiManager.AddAction(spawnedBtn);
        }

        private void RemoveActionElement(string _elementName)
        {
            guiManager.RemoveAction(_elementName);
        }

        private void Update()
        {
            if (openConsoleSettings.didSwipe())
                SwitchConsole();
        }

        public void SwitchConsole()
        {
            backgroundImage.enabled = !backgroundImage.enabled;
            // Game console container object
            GameObject child = backgroundImage.gameObject.transform.GetChild(0).gameObject;
            child.SetActive(!child.activeSelf);
            if (_minifiedConsole)
                popup.gameObject.SetActive(!backgroundImage.enabled);
        }

        private void WriteToConsole(string title, string message)
        {
            LogMessage(LogType.Log, message, title);
        }

        private void AddListElement(List<ActionContainer> actions)
        {
            if (actionsList == null)
            {
                actionsList = new List<ActionContainer>(actions);
                foreach (ActionContainer acon in actionsList)
                    AddActionElement(acon);
            }
            else
            {
                int newElements = actions.Count - actionsList.Count;
                if (newElements > 0)
                    for (int i = actionsList.Count; i < actions.Count; i++)
                    {
                        AddActionElement(actions[i]);
                    }
                else if (newElements < 0)
                {
                    int _i = 0;
                    foreach (ActionContainer action in actionsList)
                    {
                        if (action.actionName != actions[_i].actionName)
                        {
                            RemoveActionElement(action.actionName);
                            break;
                        }
                        else if (_i + 1 > actions.Count - 1)
                        {
                            print(action.actionName + " is the last one!");
                            RemoveActionElement(action.actionName);
                        }
                        _i++;
                    }
                }
            }
            actionsList = new List<ActionContainer>(actions);
        }

        #region ButtonFunctions

        public void ToggleActions()
        {
            actionSection.gameObject.SetActive(!actionSection.gameObject.activeSelf);
            logSection.gameObject.SetActive(!logSection.gameObject.activeSelf);
        }

        public void CleanConsole()
        {
            foreach (Transform t in logScroll)
                Destroy(t.gameObject);
            guiManager.ClearList();
        }

        public void ShowDetail(LogInfo.ErrorDetail detail)
        {
            detailInformation.text = detail.logString + "\n\n" + detail.stackTrace;
            mailSubject = string.Format("[{0}] {1}", detail.errorType.ToString(), detail.logString);
            popUpDetail.gameObject.SetActive(true);
        }

        public void ClosePopUp()
        {
            popUpDetail.gameObject.SetActive(false);
        }

        public void MinifiedConsole(bool minifiedStatus)
        {
            _minifiedConsole = minifiedStatus;
        }

        public void CopyTextToClipboard()
        {
            string textToSend = detailInformation.text;
#if !UNITY_ANDROID
            SendEmail(textToSend);
#else
            ShareTextOnAndroid(Application.productName, textToSend);
#endif
        }

        private void SendEmail(string messageBody)
        {
            string email = DefaultMailDirectory;
            string subject = MyEscapeURL(mailSubject);
            string body = MyEscapeURL(messageBody);
            Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
        }

        private static string MyEscapeURL(string url)
        {
            return WWW.EscapeURL(url).Replace("+", "%20");
        }

        private void SetCanvasPosition()
        {
//            if (Screen.width > Screen.height)
//            {
//                CanvasScaler scaler = console.GetComponent<CanvasScaler>();
//                scaler.referenceResolution = new Vector2(1800, 600);
//            }
        }

#if UNITY_ANDROID
        [Obsolete("Deprecated because SendEmail works better and it's multiplatform")]
        private static void ShareTextOnAndroid(string messageTitle, string messageBody)
        {
            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
            intentObject.Call<AndroidJavaObject>("setType", "text/plain");
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"),
                messageTitle);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), messageBody);
            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
            currentActivity.Call("startActivity", intentObject);
        }
#endif

        public void FilterLogs(FilterAction alertButton)
        {
            guiManager.FilterList(alertButton.logType);
            Image buttonSprite = alertButton.transform.GetChild(0).GetComponent<Image>();
            Color defaultColor = Color.white;
            if (alertButton.logType == LogType.Error)
                defaultColor = Color.red;
            else if (alertButton.logType == LogType.Warning)
                defaultColor = Color.yellow;
            buttonSprite.color = buttonSprite.color == defaultColor ? Color.black : defaultColor;
        }

        public void FilterByString(string _filterMessage)
        {
            guiManager.FilterList(_filterMessage);
        }

        public void PauseConsole()
        {
            guiManager.PauseList();
        }

        #endregion

        [Serializable]
        public class ActionButtons
        {
            [Header("Actions")] public VerticalLayoutGroup actionsContainer;
            public ActionButtonBehavior actionBtnPrefab;
            [Header("Variables")] public VerticalLayoutGroup variablesContainer;
            public ActionButtonBehavior variablesBtnPrefab;
        }

        [Serializable]
        public class LogDetails
        {
            public Sprite
                logErrorSprite,
                logWarningSprite,
                logInfoSprite;
        }
    }
}