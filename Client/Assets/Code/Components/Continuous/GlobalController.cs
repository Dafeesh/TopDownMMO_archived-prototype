using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Net;

using Extant;
using Extant.Networking;

public class GlobalController : MonoBehaviour
{
    [SerializeField]
    private GameController gameController;
    [SerializeField]
    private InterfaceAccessor uiAccessor;

    private GlobalState state;

    private NetConnection wsConnection;

	void Start ()
    {
        DebugLogger.GlobalDebug.MessageLogged += Debug.Log;

        if (gameController == null)
            Debug.LogError("GlobalController has no reference to gameController.");
        if (uiAccessor == null)
            Debug.LogError("GlobalController has no reference to uiAccessor.");

        SetState(GlobalState.Login);
	}
	
	void Update () 
    {
        switch (state)
        {
            case (GlobalState.InGame):
                {

                    if (wsConnection.IsStopped)
                    {
                        SetState(GlobalState.Login);
                        break;
                    }

                    if (Input.GetKeyUp(KeyCode.A))
                    {
                        wsConnection.Stop();
                        break;
                    }
                }
                break;
        }
	}

    void OnApplicationQuit()
    {
        if (wsConnection != null)
        {
            wsConnection.Stop();
        }
    }

    private void SetState(GlobalState newState)
    {
        //Initiate new state
        switch (newState)
        {
            case (GlobalState.Login):
                {
                    gameController.Stop();
                    Application.LoadLevel("_MainMenu");
                }
                break;

            case (GlobalState.InGame):
                {
                    Application.LoadLevel("_Waiting");
                }
                break;
        }

        state = newState;
    }

    public void SetWorldServerConnection(NetConnection con)
    {
        wsConnection = con;
        SetState(GlobalState.InGame);
        gameController.SetWorldServerConnection(con);
    }

    public enum GlobalState
    {
        Login,
        InGame
    }
}