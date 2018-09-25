using UnityEngine;
using UnityEngine.UI;

// Mady by @Bullrich

namespace Blue.Console
{
    public class LogInfo : MonoBehaviour
    {
        public Text logMessage;
        public Image logType;
        private ErrorDetail logDetail;
        [HideInInspector] public ConsoleGUI gui;

        public void LoadLog(ErrorDetail eDetail)
        {
            logDetail = eDetail;

            PopulateLog(eDetail.logSprite, eDetail.logString);
            logType.color = ColorOfLog(eDetail.errorType);
        }

        public LogType GetLogType()
        {
            return logDetail.errorType;
        }

        private static Color ColorOfLog(LogType type)
        {
            Color logColor = Color.white;
            if (type == LogType.Assert || type == LogType.Error || type == LogType.Exception)
                logColor = Color.red;
            else if (type == LogType.Warning)
                logColor = Color.yellow;
            return logColor;
        }

        private void PopulateLog(Sprite type, string message)
        {
            logType.sprite = type;
            logMessage.text = message;
        }

        public void EnterDetailMode()
        {
            gui.ShowDetail(logDetail);
        }

        public LogType GetFilterLogType()
        {
            if (logDetail.errorType == LogType.Warning)
                return LogType.Warning;
            else if (logDetail.errorType == LogType.Log)
                return LogType.Log;
            else
                return LogType.Error;
        }

        public struct ErrorDetail
        {
            public ErrorDetail(
                string log,
                string stack,
                LogType eType,
                Sprite sprite)
            {
                logString = log;
                stackTrace = stack;
                errorType = eType;
                logSprite = sprite;
            }

            public string logString,
                stackTrace;

            public LogType errorType;
            public Sprite logSprite;
        }
    }
}