using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;

using Extant;
using Extant.GameServerShared;

public class UserInterface : MonoBehaviour
{
    private static class Login
    {
        public static string username_text = "Player1";
        public static Rect username_text_position = new Rect(Screen.width / 2 - 100, Screen.height / 2 + (30 + 5) * 0, 200, 30);
        public static string username_label_text = "User:";
        public static Rect username_label_position = new Rect(Screen.width / 2 - 135, Screen.height / 2 + (30 + 5) * 0, 200, 30);

        public static string password_text = "111";
        public static Rect password_text_position = new Rect(Screen.width / 2 - 100, Screen.height / 2 + (30 + 5) * 1, 200, 30);
        public static string password_label_text = "Pass:";
        public static Rect password_label_position = new Rect(Screen.width / 2 - 135, Screen.height / 2 + (30 + 5) * 1, 200, 30);

        public static string targetIp_text = "127.0.0.1";
        public static Rect targetIp_text_position = new Rect(Screen.width / 2 - 100, Screen.height / 2 + (30 + 5) * 2, 200, 30);
        public static string targetIp_label_text = "  IP:";
        public static Rect targetIp_label_position = new Rect(Screen.width / 2 - 135, Screen.height / 2 + (30 + 5) * 2, 200, 30);

        public static string targetPort_text = "3000";
        public static Rect targetPort_text_position = new Rect(Screen.width / 2 - 100, Screen.height / 2 + (30 + 5) * 3, 200, 30);
        public static string targetPort_label_text = "Port:";
        public static Rect targetPort_label_position = new Rect(Screen.width / 2 - 135, Screen.height / 2 + (30 + 5) * 3, 200, 30);

        public static string connectButton_text = "Connect";
        public static Rect connectButton_position = new Rect(Screen.width / 2 - 50, Screen.height / 2 + (30 + 5) * 4, 100, 30);
    }
    private static class InGame
    {
        public static string dayPhase_text = "";
        public static Rect dayPhase_text_position = new Rect(Screen.width / 2 + 32, 0, 200, 30);
        public static string dayPhase_label_text = "DayPhase:";
        public static Rect dayPhase_label_position = new Rect(Screen.width / 2 - 32, 0, 200, 30);

        public static Rect gameTime_text_position = new Rect(50, 0, 200, 30);
        public static string gameTime_label_text = "Time:";
        public static Rect gameTime_label_position = new Rect(0, 0, 200, 30);
    }
    private static class ChatBox
    {
        public static List<string> chatBox_lines = new List<string>();
        public static string chatBox_text = "";
        public static Rect chatBox_text_position = new Rect(0, Screen.height-125, 600, 125);
    }

    private InterfaceState state = InterfaceState.Login;

    private GameController game;
    private GameServerConnection gsConnection;

    void Start()
    {
        game = GameObject.Find("_MasterController").GetComponent<GameController>();
        gsConnection = GameObject.Find("_MasterController").GetComponent<GameServerConnection>();

        DebugLogger.GlobalDebug.MessageLogged += PostMessage;
        DebugLogger.GlobalDebug.MessageLogged += UnityEngine.Debug.Log;
    }

    void Update()
    {
        if (gsConnection.IsConnected)
            state = InterfaceState.Ingame;
        else
            state = InterfaceState.Login;
    }

    void OnGUI()
    {
        switch (state)
        {
            case (InterfaceState.Login):
                {
                    GUI.Label(Login.username_label_position, Login.username_label_text);
                    Login.username_text = GUI.TextArea(Login.username_text_position, Login.username_text);//CleanInput(GUI.TextArea(Login.username_text_position, Login.username_text));

                    GUI.Label(Login.password_label_position, Login.password_label_text);
                    Login.password_text = GUI.TextArea(Login.password_text_position, Login.password_text);//CleanInput(GUI.TextArea(Login.password_text_position, Login.password_text));

                    GUI.Label(Login.targetIp_label_position, Login.targetIp_label_text);
                    Login.targetIp_text = GUI.TextArea(Login.targetIp_text_position, Login.targetIp_text);//CleanInput(GUI.TextArea(Login.targetIp_text_position, Login.targetIp_text));

                    GUI.Label(Login.targetPort_label_position, Login.targetPort_label_text);
                    Login.targetPort_text = GUI.TextArea(Login.targetPort_text_position, Login.targetPort_text);//CleanInput(GUI.TextArea(Login.targetPort_text_position, Login.targetPort_text));

                    if (GUI.Button(Login.connectButton_position, Login.connectButton_text))
                    {
                        gsConnection.SetConnectTarget(Login.targetIp_text, System.Convert.ToInt32(Login.targetPort_text), Login.username_text, System.Convert.ToInt32(Login.password_text));
                    }
                }
                break;

            case(InterfaceState.Ingame):
                {
                    GUI.Label(InGame.dayPhase_label_position, InGame.dayPhase_label_text);
                    GUI.Label(InGame.dayPhase_text_position, InGame.dayPhase_text);

                    GUI.Label(InGame.gameTime_label_position, InGame.gameTime_label_text);
                    GUI.Label(InGame.gameTime_text_position, 
                        System.String.Format("{0:00}:{1:00}",
                        game.ElapsedTime.Minutes, game.ElapsedTime.Seconds));
                }
                break;
        }

        GUI.Label(ChatBox.chatBox_text_position, ChatBox.chatBox_text);
    }

    /*
    private string CleanInput(string s)
    {
        string r = "";
        for (int i = 0; i < s.Length; i++)
        {
            if ((s[i] >= '0' && s[i] <= '9') || (s[i] >= 'A' && s[i] <= 'Z') || (s[i] >= 'a' && s[i] <= 'z') || (s[i] == '.') || (s[i] == ' '))
            {
                r += s[i];
            }
        }
        return r;
    }
    */

    private enum InterfaceState
    {
        Login,
        Ingame
    }

    public void PostMessage(string s)
    {
        ChatBox.chatBox_lines.Add(s);

        ChatBox.chatBox_text = "";
        for (int i = ChatBox.chatBox_lines.Count - 1; i >= ChatBox.chatBox_lines.Count - 10 && i >= 0; i--)
        {
            ChatBox.chatBox_text += ChatBox.chatBox_lines[i] + '\n';
        }
    }

    public DayPhase DayPhase
    {
        set
        {
            InGame.dayPhase_text = value.ToString();
        }
    }
}