using UnityEngine;
using UnityEngine.EventSystems;

using Extant;
using Extant.Networking;

using SharedComponents.Global;
using SharedComponents.Global.GameProperties;

public class GameController : MonoComponent
{
    [SerializeField]
    InterfaceController uiController = null;
    [SerializeField]
    CharacterListController charListController = null;
    [SerializeField]
    MapController mapController = null;

    InstanceServerConnection instConnection;

    GameTime gameTime;

    void Start()
    {
        //log.AnyLogged += Debug.Log;

        if (uiController == null)
            Debug.LogError("GameController was not given a uiController.");
        if (charListController == null)
            Debug.LogError("GameController was not given a charListController.");
        if (mapController == null)
            Debug.LogError("GameController was not given a mapController.");

        instConnection = InstanceServerConnection.Main;
        if (instConnection == null)
            Debug.LogError("GameController could not find InstanceServerConnection.");

        gameTime = new GameTime();
    }

    void Update()
    {
        
    }

    public void Command_MoveTo(float x, float y)
    {
        throw new System.NotImplementedException("Need to implement ability to contact InstCon.");
        //wsConnection.SendPacket(new ClientToWorldPackets.Player_MovementRequest_w(x, y));
    }
}
