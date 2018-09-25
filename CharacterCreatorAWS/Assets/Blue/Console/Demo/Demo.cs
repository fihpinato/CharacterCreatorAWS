using UnityEngine;

// By @Bullrich
namespace Blue.Console.Demo
{
    public class Demo : MonoBehaviour
    {
        [SerializeField] private bool showLog;

        public void Start()
        {
            AddActions();
        }

        private static void AddActions()
        {
            Debug.Log(Debug.isDebugBuild);
            Debug.Log("Adding methods!");
            GameConsole.AddAction(Ble, "ACTION", 3);
            GameConsole.AddAction(Bla, "This is a bool");
            GameConsole.AddAction(Blu, "Print in console");
            GameConsole.AddAction(error, "Print an error");
            GameConsole.AddAction(warning, "Print a warning");
            GameConsole.AddAction(SeveralErrors, "Throw several errors!");
            GameConsole.AddAction(CustomMessage, "Write a custom message");
        }

        public void Update()
        {
            if (showLog)
            {
                Debug.Log(Time.time);
                showLog = false;
            }
        }

        private static void Ble(int ja)
        {
            Debug.Log(ja + " HOLAAAA");
            GameConsole.RemoveAction("ACTION");
        }

        private static void DeleteAll()
        {
            GameConsole.RemoveAction("ACTION");
            GameConsole.RemoveAction("This is a bool");
            GameConsole.RemoveAction("Print in console");
            GameConsole.RemoveAction("Print an error");
            GameConsole.RemoveAction("Throw several errors!");
        }

        private static void Bla(bool lol)
        {
            Debug.Log("Value is " + lol);
        }

        private static void Blu()
        {
            Debug.Log("This print in console");
        }

        private static void error()
        {
            Debug.LogError("This is an exception!");
        }

        private static void warning()
        {
            Debug.LogWarning("This is a warning");
        }

        private static void CustomMessage()
        {
            GameConsole.WriteMessage("Example custom title", "Here goes a custom message");
        }

        private static void SeveralErrors()
        {
            int randomValues = Random.Range(3, 14);
            for (int i = 0; i < randomValues; i++)
            {
                int newRand = Random.Range(0, 3);
                switch (newRand)
                {
                    case 0:
                        Debug.Log("This is " + i + " a log!");
                        break;
                    case 1:
                        Debug.LogWarning("This is " + i + " warning!");
                        break;
                    case 2:
                        Debug.LogError(i + " | This is an assets known as a kind of error!");
                        break;
                    default:
                        Debug.LogError(i + " = i, this shouldn't happen");
                        break;
                }
            }
        }
    }
}