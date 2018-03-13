using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Net;

using Extant;

[RequireComponent(typeof(InputController))]
[RequireComponent(typeof(GameController))]
public class GlobalController : MonoBehaviour
{
    public GameObject LoginPanel;
    public GameObject InputIPText;
    public GameObject InputPortText;

    private GlobalState state = GlobalState.Login;
    private WorldServerConnection wsConnection;

	void Start ()
    {
        DebugLogger.GlobalDebug.MessageLogged += Debug.Log;
	}
	
	void Update () 
    {
        switch (state)
        {
            case(GlobalState.Login):

                break;
            case(GlobalState.CommunicatingWithMain):
                //TODO this
                break;
            case(GlobalState.InWorld):
                if (wsConnection.IsStopped)
                {
                    state = GlobalState.Login;
                    LoginPanel.SetActive(true);
                }

                if (Input.GetKeyDown(KeyCode.A))
                {
                    wsConnection.Stop();
                }
                break;
        }
	}

    //TEST
    public void _Connect_Player1()
    {
        DebugLogger.GlobalDebug.Log(DebugLogger.LogType.Blank, InputIPText.GetComponent<Text>().text + "/" +
                                                               InputPortText.GetComponent<Text>().text);

        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(InputIPText.GetComponent<Text>().text),
                                                   int.Parse(InputPortText.GetComponent<Text>().text));
        wsConnection = new WorldServerConnection(endPoint, "Player1", 111);
        wsConnection.Start();

        LoginPanel.SetActive(false);
        state = GlobalState.InWorld;
    }
    public void _Connect_Player2()
    {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(InputIPText.GetComponent<Text>().text),
                                                   int.Parse(InputPortText.GetComponent<Text>().text));
        wsConnection = new WorldServerConnection(endPoint, "Player2", 222);
        wsConnection.Start();

        LoginPanel.SetActive(false);
        state = GlobalState.InWorld;
    }
    public void _Connect_Player3()
    {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(InputIPText.GetComponent<Text>().text),
                                                   int.Parse(InputPortText.GetComponent<Text>().text));
        wsConnection = new WorldServerConnection(endPoint, "Player3", 333);
        wsConnection.Start();

        LoginPanel.SetActive(false);
        state = GlobalState.InWorld;
    }
    public void _Connect_Player4()
    {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(InputIPText.GetComponent<Text>().text),
                                                   int.Parse(InputPortText.GetComponent<Text>().text));
        wsConnection = new WorldServerConnection(endPoint, "Player4", 444);
        wsConnection.Start();

        LoginPanel.SetActive(false);
        state = GlobalState.InWorld;
    }
    //~

    public enum GlobalState
    {
        Login,
        CommunicatingWithMain,
        InWorld
    }
}