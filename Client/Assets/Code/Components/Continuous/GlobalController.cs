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
    private InterfaceController uiController;

    private GlobalState state;
    private DebugLogger log = new DebugLogger();

	void Start ()
    {
        log.AnyLogged += Debug.Log;

        if (gameController == null)
            log.LogError("GlobalController has no reference to gameController.");
        if (uiController == null)
            log.LogError("GlobalController has no reference to uiAccessor.");

        SetState(GlobalState.Login);
	}
	
	void Update () 
    {
        switch (state)
        {
            case (GlobalState.InGame):
                {

                }
                break;
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
                    Application.LoadLevelAdditive("_MainMenu");
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

    public enum GlobalState
    {
        Login,
        InGame
    }
}