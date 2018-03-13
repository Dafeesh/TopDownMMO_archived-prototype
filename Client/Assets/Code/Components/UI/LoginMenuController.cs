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
    GameObject canvas_Waiting_text_Active = null;
    [SerializeField]
    GameObject canvas_Waiting_text_FailedToConnect = null;
    [SerializeField]
    GameObject canvas_Waiting_text_InvalidLogin = null;

    [SerializeField]
    Text text_IPAddress = null;
    [SerializeField]
    Text text_PortNumber = null;
    [SerializeField]
    Text text_Username = null;
    [SerializeField]
    Text text_Password = null;

    MasterServerConnection msConnection = null;
    bool _isWaiting;

    void Awake()
    {
        msConnection = MasterServerConnection.Main;
        if (msConnection == null)
            Debug.LogError("LoginMenuController could not find MasterServerConnection.");
    }

    void Start()
    {
        IsWaiting = false;
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
        if (IsWaiting && state == ConnectionState.NoConnection)
        {
            canvas_Waiting_text_Active.SetActive(false);
            canvas_Waiting_text_FailedToConnect.SetActive(true);
        }
        else if (!IsWaiting && state != ConnectionState.NoConnection)
        {
            IsWaiting = true;
        }
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
        IsWaiting = false;
    }

    public void OnClick_TEST_QuickLogin(int accountNum)
    {
        InitiateLogin("Account" + accountNum.ToString(), "123");
    }

    public bool IsWaiting
    {
        get
        {
            return _isWaiting;
        }
        private set
        {
            if (_isWaiting != value)
            {
                _isWaiting = value;
                
                if (_isWaiting)
                {
                    canvas_Main.SetActive(false);
                    canvas_Waiting.SetActive(true);
                }
                else
                {
                    canvas_Waiting_text_Active.SetActive(true);
                    canvas_Waiting_text_FailedToConnect.SetActive(false);
                    canvas_Waiting_text_InvalidLogin.SetActive(false);

                    canvas_Main.SetActive(true);
                    canvas_Waiting.SetActive(false);
                    
                }
            }
        }
    }
}
