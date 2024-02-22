using Atomicrops.Game.DebugTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace AutoShoot
{
    public class MyConsole : MonoBehaviour
    {
        public List<MyDebugCommandBase> cmdList;
        bool showConsole = false;
        bool showHelp = false;
        string input;
        Vector2 scroll;

        void Awake()
        {
            cmdList = new()
            {
                new MyDebugCommand("help", "Show help", "help", () => showHelp = true),
                new MyDebugCommand("test", "descr", "test", () => Debug.Log($"this is test command action call")),
                new MyDebugCommand("runallpopups", "runallpopups descr", "runallpopups", () => DebugCommands.RunAllPopups()),
                new MyDebugCommand<string>("spawnboss", "spawns boss. input should be number 0-7", "spawnboss <int>", (inp) =>
                {
                    if (int.TryParse(inp, out int res))
                        DebugCommands.SpawnBossSimple((DebugCommands.BossOptions)res);
                    else
                        Debug.LogWarning($"couldnt parse input: {inp}");
                }),
                new MyDebugCommand<string>("gun", "gun descr", "gun ??", (s) =>
                {
                    try
                    {
                        DebugCommands.gun(s);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"couldnt load gun: {e.Message}");
                    }
                }),
                new MyDebugCommand("health", "Gives 10 health", "health", () => DebugCommands.GiveHealth()),
                new MyDebugCommand("killall", " descr", "", () => DebugCommands.killall()),
                new MyDebugCommand("NoDayProgressOn", " descr", "", () => DebugCommands.NoDayProgressOn()),
                new MyDebugCommand("NoDayProgressOff", " descr", "", () => DebugCommands.NoDayProgressOff()),
                new MyDebugCommand("GoToNightPhase", " descr", "", () => DebugCommands.GoToNightPhase()),
                new MyDebugCommand("GoToDuskPhase", " descr", "", () => DebugCommands.GoToDuskPhase()),
                new MyDebugCommand("GoToDayPhase", " descr", "", () => DebugCommands.GoToDayPhase()),
                new MyDebugCommand("GoToDawnPhase", " descr", "", () => DebugCommands.GoToDawnPhase()),
                new MyDebugCommand("NextDay", " descr", "", () => DebugCommands.NextDay()),
                new MyDebugCommand("GoToFinalBoss", " descr", "", () => DebugCommands.GoToFinalBoss()),
                new MyDebugCommand("town", " descr", "", () => DebugCommands.town()),
            };

            Debug.Log("MyCobsole is loaded");
        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.BackQuote))
            {
                showConsole = !showConsole;
            }

            if (showConsole && Input.GetKeyUp(KeyCode.Return))
            {
                HandleInput();
                input = string.Empty;
            }
        }

        void OnGUI()
        {
            if (!showConsole) return;

            float y = 0;

            if (showHelp)
            {
                GUI.Box(new Rect(0, y, Screen.width, 100), "");
                Rect viewport = new Rect(0, 0, Screen.width - 30, 20 * cmdList.Count);
                scroll = GUI.BeginScrollView(new Rect(0, y + 5, Screen.width, 90), scroll, viewport);

                for (int i = 0; i < cmdList.Count; i++)
                {
                    MyDebugCommandBase cmd = cmdList[i];
                    string label = $"{cmd.Format} - {cmd.Description}";
                    Rect labelRect = new Rect(5, 20 * i, viewport.width - 100, 20);
                    GUI.Label(labelRect, label);
                }
                GUI.EndScrollView();
                y += 100;
            }


            //GUI.Box(new Rect(0, 0, Screen.width, 30), "");
            input = GUI.TextField(new Rect(10, y + 5, Screen.width - 20, 20), input);
        }

        void HandleInput()
        {
            string[] inp = input.Split(' ');

            if (cmdList.TrueForAll(cmd => !inp[0].Contains(cmd.Id)))
            {
                Debug.LogWarning($"unknown command");
                return;
            }

            foreach (var cmdBase in cmdList)
            {
                if (cmdBase is MyDebugCommand cmd1)
                {
                    if (inp[0] == cmdBase.Id)
                        cmd1.Invoke();
                }
                else if (cmdBase is MyDebugCommand<string> cmd2)
                {
                    if (inp[0] == cmdBase.Id)
                        cmd2.Invoke(inp[1]);
                }
            }
        }

        //private void SetUpBeforeLearningAbtAddingReferences()
        //{
        //    canv = new GameObject("MyCanvas");
        //    var canvCmp = canv.AddComponent<Canvas>();
        //    //canvCmp.sortingLayerID = foundLyrId;
        //    canvCmp.renderMode = RenderMode.ScreenSpaceOverlay;
        //    canvCmp.sortingOrder = 100;
        //    canv.transform.SetParent(transform);

        //    RectTransform trans = canv.GetComponent<RectTransform>();
        //    trans.localScale = Vector3.one;
        //    trans.anchoredPosition = new Vector2(0f, 0f); // setting position, will be on center
        //    trans.sizeDelta = new Vector2(150, 200); // custom size

        //    imgObj = new GameObject("Img");
        //    imgObj.transform.SetParent(canv.transform);

        //    var img = imgObj.AddComponent<Image>();
        //    var tex = new Texture2D(800, 600);
        //    img.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
        //    img.color = Color.blue;
        //    var imgTrans = img.GetComponent<RectTransform>();
        //    imgTrans.sizeDelta = new Vector2(Screen.width / 2, Screen.height / 3);

        //    var verTextFont = GameObject.Find("VersionText").GetComponent<Text>().font;
        //    var textObj = new GameObject("MyText");
        //    text = textObj.AddComponent<Text>();
        //    textObj.transform.SetParent(imgObj.transform);
        //    var textTrans = textObj.GetComponent<RectTransform>();
        //    textTrans.anchoredPosition = new Vector2(0f, 0f); // setting position, will be on center
        //    textTrans.sizeDelta = new Vector2(150, 200);
        //    text.resizeTextForBestFit = true;
        //    //text.font = Resources.GetBuiltinResource("Arial.ttf") as Font;
        //    text.text = "Hello this is Atomicrops console!";
        //}
    }

    public class MyDebugCommandBase
    {

        public string Id { get; private set; }
        public string Description { get; private set; }
        public string Format { get; private set; }

        public MyDebugCommandBase(string id, string descr, string format)
        {
            Id = id;
            Description = descr;
            Format = format;
        }
    }

    public class MyDebugCommand : MyDebugCommandBase
    {
        private Action command;

        public MyDebugCommand(string id, string descr, string format, Action command) : base(id, descr, format)
        {
            this.command = command;
        }

        public void Invoke()
        {
            command.Invoke();
        }
    }

    public class MyDebugCommand<T> : MyDebugCommandBase
    {
        private Action<T> command;

        public MyDebugCommand(string id, string descr, string format, Action<T> command) : base(id, descr, format)
        {
            this.command = command;
        }

        public void Invoke(T val)
        {
            command.Invoke(val);
        }
    }
}
