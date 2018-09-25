using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Mady by @Bullrich

namespace Blue.Console
{
    public class LoadGameConsole : MonoBehaviour
    {
        public ConsoleGUI gameConsole;
        [SerializeField] public SwipeManager swipeOptions;
        [Tooltip("Show a minified version of the console on start")] public bool startMinified = false;
        [Tooltip("The maximium amount of logs the console can store")] public int limitOfLogs = 100;

        public string defaultMailDirectory = "example@gmail.com";

        public void Awake()
        {
            if (GameObject.Find(gameConsole.gameObject.name) == null)
            {
                StartCoroutine(InitConsole());
            }
            else
                Debug.LogWarning("Tried to spawn console, but it already exists!");
        }

        private IEnumerator InitConsole()
        {
            const string _eventSystemName = "EventSystem";
            GameObject eventSystem = GameObject.Find(_eventSystemName);
            GameObject console = Instantiate(gameConsole.gameObject);
            console.name = gameConsole.gameObject.name;
            ConsoleGUI guiConsole = console.GetComponent<ConsoleGUI>();
            guiConsole.init(swipeOptions, startMinified, limitOfLogs, defaultMailDirectory);
            guiConsole.ToggleActions();
            if (Screen.width > Screen.height)
            {
                CanvasScaler scaler = console.GetComponent<CanvasScaler>();
                scaler.referenceResolution = new Vector2(1800, 600);
            }

            DontDestroyOnLoad(console);
            yield return new WaitForEndOfFrame();
            guiConsole.SwitchConsole();
            guiConsole.ToggleActions();
            if (eventSystem == null)
            {
                GameObject _eventSystem = new GameObject(_eventSystemName);
                _eventSystem.AddComponent<EventSystem>();
                _eventSystem.AddComponent<StandaloneInputModule>();
                _eventSystem.transform.position = Vector3.zero;
                DontDestroyOnLoad(_eventSystem);
            }
            GameConsole.AddAction(guiConsole.MinifiedConsole, "Show minified console", startMinified);
            guiConsole.popup.gameObject.SetActive(startMinified);
            Destroy(gameObject);
        }
    }

    [Serializable]
    public class SwipeManager
    {
        Vector2
            firstPressPos,
            secondPressPos,
            currentSwipe;

        public enum swDirection
        {
            left,
            right,
            down,
            up
        }

        public swDirection swipeDirection = swDirection.down;
        [Range(1, 4)] public int fingersNeed = 2;

        public KeyCode openConsoleKey = KeyCode.Tab;

        public bool didSwipe()
        {
            return Application.isMobilePlatform ? SwipedInDirection() : Input.GetKeyDown(openConsoleKey);
        }


        public bool SwipedInDirection()
        {
            if (Input.touches.Length > fingersNeed - 1)
            {
                Touch t = Input.GetTouch(0);
                if (t.phase == TouchPhase.Began)
                {
                    //save began touch 2d point
                    firstPressPos = new Vector2(t.position.x, t.position.y);
                }
                if (t.phase == TouchPhase.Ended)
                {
                    //save ended touch 2d point
                    secondPressPos = new Vector2(t.position.x, t.position.y);

                    //create vector from the two points
                    currentSwipe =
                        new Vector3(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

                    //normalize the 2d vector
                    currentSwipe.Normalize();

                    //swipe upwards
                    if (swipeDirection == swDirection.up && currentSwipe.y > 0 && currentSwipe.x > -0.5f &&
                        currentSwipe.x < 0.5f)
                    {
                        return true;
                        //Debug.Log("up swipe");
                    }
                    else if (swipeDirection == swDirection.down && currentSwipe.y < 0 && currentSwipe.x > -0.5f &&
                             currentSwipe.x < 0.5f)
                    {
                        //Debug.Log("down swipe");
                        return true;
                    }
                    else if (swipeDirection == swDirection.left && currentSwipe.x < 0 && currentSwipe.y > -0.5f &&
                             currentSwipe.y < 0.5f)
                    {
                        return true;
                        //Debug.Log("left swipe");
                    }
                    else if (swipeDirection == swDirection.right && currentSwipe.x > 0 && currentSwipe.y > -0.5f &&
                             currentSwipe.y < 0.5f)
                    {
                        return true;
                        //Debug.Log("right swipe");
                    }
                }
            }
            return false;
        }
    }
}