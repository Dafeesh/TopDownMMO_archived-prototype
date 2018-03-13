using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Net;

public class LoginMenuController : MonoComponent
{
    [SerializeField]
    GameObject canvas_Main = null;
    [SerializeField]
    GameObject canvas_Waiting = null;

    [SerializeField]
    Text text_Waiting = null;
    private const string waitingText_Connecting = "Connecting...";
    private const string waitingText_Authorizing = "Authorizing...";
    private const string waitingText_Failed = "Failed to connect.";

    [SerializeField]
    Text text_IPAddress = null;
    [SerializeField]
    Text text_PortNumber = null;
    [SerializeField]
    Text text_Username = null;
    [SerializeField]
    Text text_Password = null;

    MasterServerConnection msConnection = null;
    ConnectionState _currentConnectionStep;

    void Awake()
    {
        msConnection = MasterServerConnection.Main;
        if (msConnection == null)
            Debug.LogError("LoginMenuController could not find MasterServerConnection.");
    }

    void Start()
    {
        CurrentConnectionStep = ConnectionState.Null;
        msConnection.StateChanged += OnStateChange_MasterServer;
    }

    void OnDestroy()
    {
        msConnection.StateChanged -= OnStateChange_MasterServer;
    }

    void InitiateLogin(string username, string password)
    {
        IPEndPoint target = new IPEndPoint(IPAddress.Parse(text_IPAddress.text), int.Parse(text_PortNumber.text));

        msConnection.SetTarget(target, username, password);
    }

    public void OnStateChange_MasterServer(ConnectionState state)
    {
        CurrentConnectionStep = state;
    }

    public void OnClick_Login()
    {
        string username = text_Username.text;
        string password = text_Password.text;

        InitiateLogin(username, password);
    }

    public void OnClick_Cancel()
    {
        msConnection.CloseConnection();
        CurrentConnectionStep = ConnectionState.Null;
    }

    public void OnClick_TEST_QuickLogin(int accountNum)
    {
        InitiateLogin("Account" + accountNum.ToString(), "123");
    }

    public ConnectionState CurrentConnectionStep
    {
        get
        {
            return _currentConnectionStep;
        }

        private set
        {
            if (_currentConnectionStep != value)
            {
                switch (value)
                {
                    case (ConnectionState.Null):
                        {
                            canvas_Main.SetActive(true);

                            canvas_Waiting.SetActive(false);
                        }
                        break;

                    case (ConnectionState.Connecting):
                        {
                            canvas_Main.SetActive(false);

                            canvas_Waiting.SetActive(true);
                            text_Waiting.text = waitingText_Connecting;
                        }
                        break;

                    case (ConnectionState.Authorizing):
                        {
                            text_Waiting.text = waitingText_Authorizing;
                        }
                        break;

                    case (ConnectionState.Connected):
                        {
                            Application.LoadLevel(ResourceList.Scenes.CharSelect);
                        }
                        break;

                    case (ConnectionState.NoConnection):
                        {
                            text_Waiting.text = waitingText_Failed;
                        }
                        break;
                }
                _currentConnectionStep = value;
            }
        }
    }
}
