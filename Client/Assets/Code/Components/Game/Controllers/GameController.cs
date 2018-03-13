using Extant;
using Extant.Networking;
using SharedComponents;
using SharedComponents.GameProperties;
using UnityEngine;

public class GameController : MonoBehaviour , ILogging
{
    [SerializeField]
    InterfaceController uiController = null;
    [SerializeField]
    CharacterListController charListController = null;
    [SerializeField]
    PlayerController plrController = null;

    WorldServerConnection wsConnection;
    DebugLogger log = new DebugLogger();

    //GameMap map = new GameMap();
    PlayerInfo playerInfo = new PlayerInfo("NULL");
    GameTime gameTime;

    void Start()
    {
        //log.AnyLogged += Debug.Log;

        if (uiController == null)
            Debug.LogError("GameController was not given a uiController.");
        if (plrController == null)
            Debug.LogError("GameController was not given a plrController.");
        try
        {
            wsConnection = GameObject.Find("Connections").GetComponent<WorldServerConnection>();
        }
        finally
        {
            if (wsConnection == null)
                Debug.LogError("GameController could not find WorldServerConnection.");
        }

        gameTime = new GameTime();
    }

    void Update()
    {
        HandleIncomingPackets();
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

                case (ClientToWorldPackets.PacketType.Map_Reset_c):
                    {
                        //ClientToWorldPackets.Map_Reset_c pp = p as ClientToWorldPackets.Map_Reset_c;


                        log.Log("WorldServer- Reset Map.");
                    }
                    break;

                case (ClientToWorldPackets.PacketType.Map_TerrainBlock_c):
                    {
                        //ClientToWorldPackets.Map_TerrainBlock_c pp = p as ClientToWorldPackets.Map_TerrainBlock_c;


                        log.Log("WorldServer- Terrain Block.");
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

    public DebugLogger Log
    {
        get
        {
            return log;
        }
    }
}
