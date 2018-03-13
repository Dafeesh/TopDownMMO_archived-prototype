using Extant;
using Extant.Networking;
using SharedComponents;
using SharedComponents.GameProperties;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private InterfaceController uiController;
    [SerializeField]
    private WorldServerConnection wsConnection;

    private GameTime gameTime;
    private bool isActive;

    private DebugLogger log = new DebugLogger();

    private GameObject sceneManager = null;

    void Start()
    {
        log.AnyLogged += Debug.Log;

        if (uiController == null)
            log.LogError("GameController was not given an InterfaceAccessor.");
        if (wsConnection == null)
            log.LogError("GameController was not given a WorldServerConnection.");

        gameTime = new GameTime();
        isActive = false;
        sceneManager = null;
    }

    void Update()
    {
        if (isActive)
        {
            if (sceneManager == null)
            {
                sceneManager = GameObject.Find("_SceneBase");
                if (sceneManager != null)
                {
                    log.Log("GameController found SceneBase.");
                }
            }
            else
            {
                uiController.GameTime = gameTime.Now;

                HandleIncomingPackets();
            }
        }
    }

    private void HandleIncomingPackets()
    {
        Packet p = null;
        while ((p = wsConnection.GetPacket()) != null)
        {
            switch ((ClientToWorldPackets.PacketType)p.Type)
            {
                case (ClientToWorldPackets.PacketType.Map_MoveTo_c):
                    {
                        LoadNewMap((MapID)(p as ClientToWorldPackets.Map_MoveTo_c).mapNum);
                        log.Log("WorldServer- LoadMap: " + (MapID)(p as ClientToWorldPackets.Map_MoveTo_c).mapNum);
                    }
                    break;

                case (ClientToWorldPackets.PacketType.Character_Add_c):
                    {
                        log.Log("WorldServer- Add Character: " + (p as ClientToWorldPackets.Character_Add_c).charId);
                    }
                    break;

                case (ClientToWorldPackets.PacketType.Character_Position_c):
                    {
                        log.Log("WorldServer- Character position: " + (p as ClientToWorldPackets.Character_Position_c).charId);
                    }
                    break;

                case (ClientToWorldPackets.PacketType.Character_Remove_c):
                    {
                        log.Log("WorldServer- Remove Character: " + (p as ClientToWorldPackets.Character_Remove_c).charId);
                    }
                    break;

                default:
                    {
                        log.LogWarning("WorldServer- Got unknown packet: " + ((ClientToWorldPackets.PacketType)p.Type).ToString());
                    }
                    break;
            }
        }
    }

    private void DeleteOldMap()
    {
        if (sceneManager != null)
        {
            Destroy(sceneManager);
            sceneManager = null;
            log.Log("Deleted the map.");
        }
    }

    private void LoadNewMap(MapID id)
    {
        DeleteOldMap();

        //Load new map
        switch (id)
        {
            case (MapID.TestMap):
                {
                    Application.LoadLevelAdditive("TestMap");
                }
                break;

            default:
                {
                    log.LogError("GameController could not load map: " + id.ToString());
                    return;
                }
                //break;
        }
    }

    public void Stop()
    {
        DeleteOldMap();
        isActive = false;
    }

    public bool IsActive
    {
        get
        {
            return isActive;
        }
        set
        {
            isActive = value;
        }
    }
}
