using UnityEngine;

// Mady by @Bullrich

namespace Blue.Console
{
    public class ConsoleOutput : MonoBehaviour
    {
        private ConsoleGUI _gui;

        public void init(ConsoleGUI cGui)
        {
            _gui = cGui;
            Application.logMessageReceived += HandleLog;
        }

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            _gui.LogMessage(type, stackTrace, logString);
        }
    }
}