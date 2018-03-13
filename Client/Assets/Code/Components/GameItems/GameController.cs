using Extant;
using Extant.Networking;
using SharedComponents;
using SharedComponents.GameProperties;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    InterfaceController uiController = null;
    [SerializeField]
    CharacterListController charListController = null;
    [SerializeField]
    PlayerController plrController = null;

    WorldServerConnection wsConnection;

    DebugLogger log = new DebugLogger();

    GameTime gameTime;
    GameObject sceneManager = null;

    PlayerInfo playerInfo = new PlayerInfo("NULL");

    void Start()
    {
        //log.AnyLogged += Debug.Log;

        if (uiController == null)
            Debug.LogError("GameController was not given a uiController.");
        if (plrController == null)
            Debug.LogError("GameController was not given a plrController.");

        wsConnection = GameObject.Find("_Connections").GetComponent<WorldServerConnection>();
        if (wsConnection == null)
            Debug.LogError("GameController could not find WorldServerConnection.");

        gameTime = new GameTime();
        sceneManager = null;
    }

    void Update()
    {
        if (sceneManager != null)
        {
            HandleIncomingPackets();
        }
        else
        {
            sceneManager = GameObject.Find("_SceneBase");
            if (sceneManager != null)
            {
                log.Log("GameController found SceneBase.");
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
                case (ClientToWorldPackets.PacketType.Player_Info_c):
                    {
                        ClientToWorldPackets.Player_Info_c pp = p as ClientToWorldPackets.Player_Info_c;

                        playerInfo.Name = pp.username;
                        playerInfo.Level = pp.level;

                        uiController.SetPlayerName(pp.username);
                        uiController.SetPlayerLevel(pp.level);

                        log.Log("WorldServer- Got player info: " + playerInfo.Name + "/" + playerInfo.Level);
                    }
                    break;

                case (ClientToWorldPackets.PacketType.Player_SetControl_c):
                    {
                        ClientToWorldPackets.Player_SetControl_c pp = p as ClientToWorldPackets.Player_SetControl_c;

                        plrController.SetCharacterControlled(charListController.GetControllerFromId(pp.id));

                        log.Log("WorldServer- Set control of char: " + pp.id);
                    }
                    break;

                case (ClientToWorldPackets.PacketType.Map_MoveTo_c):
                    {
                        ClientToWorldPackets.Map_MoveTo_c pp = p as ClientToWorldPackets.Map_MoveTo_c;

                        LoadNewMap((MapID)pp.mapNum);
                        log.Log("WorldServer- LoadMap: " + (MapID)pp.mapNum);
                    }
                    break;

                case (ClientToWorldPackets.PacketType.Character_Add_c):
                    {
                        ClientToWorldPackets.Character_Add_c pp = p as ClientToWorldPackets.Character_Add_c;

                        charListController.AddCharacter(pp.charId, pp.charType, pp.modelNumber);

                        log.Log("WorldServer- Add Character: " + pp.charId);
                    }
                    break;

                case (ClientToWorldPackets.PacketType.Character_Position_c):
                    {
                        ClientToWorldPackets.Character_Position_c pp = p as ClientToWorldPackets.Character_Position_c;

                        charListController.SetCharacterPosition(pp.charId, pp.newx, pp.newy);

                        log.Log("WorldServer- Character position: " + pp.charId);
                    }
                    break;

                case (ClientToWorldPackets.PacketType.Character_Remove_c):
                    {
                        ClientToWorldPackets.Character_Remove_c pp = p as ClientToWorldPackets.Character_Remove_c;

                        charListController.RemoveCharacter(pp.charId);

                        log.Log("WorldServer- Remove Character: " + pp.charId);
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
                    Application.LoadLevelAdditive(SceneList.Maps.TestMap);
                    log.Log("Loaded map: " + SceneList.Maps.TestMap);
                }
                break;

            default:
                {
                    log.LogError("GameController could not load map: " + id.ToString());
                }
                break;
        }
    }
}
