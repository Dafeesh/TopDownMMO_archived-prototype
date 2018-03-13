using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Net;

public class LoginMenuController : MonoComponent
{
    [SerializeField]
    Text text_IPAddress = null;
    [SerializeField]
    Text text_PortNumber = null;
    [SerializeField]
    Text text_Username = null;
    [SerializeField]
    Text text_Password = null;

    MasterServerConnection msConnection = null;

    void Start()
    {
        try
        {
            msConnection = GameObject.Find("Connections").GetComponent<MasterServerConnection>();
        }
        finally
        {
            if (msConnection == null)
                Debug.LogError("LoginMenuController could not find MasterServerConnection");
        }
    }

    void InitiateLogin(string username, string password)
    {
        IPEndPoint target = new IPEndPoint(IPAddress.Parse(text_IPAddress.text), int.Parse(text_PortNumber.text));

        msConnection.SetTarget(target, username, password);
    }

    public void OnClick_Login()
    {
        string username = text_Username.text;
        string password = text_Password.text;

        InitiateLogin(username, password);
    }

    public void OnClick_TEST_QuickLogin(int accountNum)
    {
        InitiateLogin("Account" + accountNum.ToString(), "123");
    }
}
