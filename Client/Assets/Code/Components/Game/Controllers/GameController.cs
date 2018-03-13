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
    MapController mapController = null;
    [SerializeField]
    PlayerController plrController = null;

    WorldServerConnection wsConnection;
    DebugLogger log;

    //GameMap map = new GameMap();
    PlayerInfo playerInfo = new PlayerInfo();
    GameTime gameTime;

    void Start()
    {
        log = new DebugLogger("GameController");
        //log.AnyLogged += Debug.Log;

        if (uiController == null)
            Debug.LogError("GameController was not given a uiController.");
        if (charListController == null)
            Debug.LogError("GameController was not given a charListController.");
        if (mapController == null)
            Debug.LogError("GameController was not given a mapController.");
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
                case (ClientToWorldPackets.PacketType.Error_c):
                    {
                        ClientToWorldPackets.Error_c pp = p as ClientToWorldPackets.Error_c;

                        Debug.LogError("ERROR from WorldServer: " + pp.error.ToString());
                        log.Log("ERROR: " + pp.error.ToString());
                    }
                    break;

                case (ClientToWorldPackets.PacketType.Player_Info_c):
                    {
                        ClientToWorldPackets.Player_Info_c pp = p as ClientToWorldPackets.Player_Info_c;

                        playerInfo.Name = pp.username;
                        playerInfo.Level = pp.level;

                        uiController.SetPlayerName(pp.username);
                        uiController.SetPlayerLevel(pp.level);

                        log.Log("Got player info: " + playerInfo.Name + "/" + playerInfo.Level);
                    }
                    break;

                case (ClientToWorldPackets.PacketType.Player_SetControl_c):
                    {
                        ClientToWorldPackets.Player_SetControl_c pp = p as ClientToWorldPackets.Player_SetControl_c;

                        plrController.SetCharacterControlled(charListController.GetControllerFromId(pp.id));

                        log.Log("Set control of char: " + pp.id);
                    }
                    break;

                case (ClientToWorldPackets.PacketType.Map_Reset_c):
                    {
                        ClientToWorldPackets.Map_Reset_c pp = p as ClientToWorldPackets.Map_Reset_c;

                        mapController.ResetMap(pp.newNumBlocksX, pp.newNumBlocksY);

                        log.Log("Reset Map.");
                    }
                    break;

                case (ClientToWorldPackets.PacketType.Map_TerrainBlock_c):
                    {
                        ClientToWorldPackets.Map_TerrainBlock_c pp = p as ClientToWorldPackets.Map_TerrainBlock_c;

                        mapController.SetTerrainBlock(pp.blockX, pp.blockY, pp.heightMap);

                        log.Log("Terrain Block.");
                    }
                    break;

                case (ClientToWorldPackets.PacketType.Character_Add_c):
                    {
                        ClientToWorldPackets.Character_Add_c pp = p as ClientToWorldPackets.Character_Add_c;

                        charListController.AddCharacter(pp.charId, pp.charType, pp.modelNumber);

                        log.Log("Add Character: " + pp.charId);
                    }
                    break;

                case (ClientToWorldPackets.PacketType.Character_Position_c):
                    {
                        ClientToWorldPackets.Character_Position_c pp = p as ClientToWorldPackets.Character_Position_c;

                        charListController.SetCharacter_Position(pp.charId, pp.newx, pp.newy);

                        log.Log("Character position: " + pp.charId);
                    }
                    break;

                case (ClientToWorldPackets.PacketType.Character_Movement_c):
                    {
                        ClientToWorldPackets.Character_Movement_c pp = p as ClientToWorldPackets.Character_Movement_c;

                        charListController.SetCharacter_MovePoint(pp.charId, pp.movePoint);

                        log.Log("Character movement: " + pp.charId + "- " + pp.movePoint.start.ToString() + "->" + pp.movePoint.end.ToString());
                    }
                    break;

                case (ClientToWorldPackets.PacketType.Character_Remove_c):
                    {
                        ClientToWorldPackets.Character_Remove_c pp = p as ClientToWorldPackets.Character_Remove_c;

                        charListController.RemoveCharacter(pp.charId);

                        log.Log("Remove Character: " + pp.charId);
                    }
                    break;

                case (ClientToWorldPackets.PacketType.Character_UpdateStats_c):
                    {
                        ClientToWorldPackets.Character_UpdateStats_c pp = p as ClientToWorldPackets.Character_UpdateStats_c;

                        charListController.SetCharacter_Stats(pp.charId, pp.stats);

                        log.Log("Character stats: " + pp.charId);
                    }
                    break;

                default:
                    {
                        log.Log("Got unknown packet: " + ((ClientToWorldPackets.PacketType)p.Type).ToString());
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

    public void Command_MoveTo(float x, float y)
    {
        wsConnection.SendPacket(new ClientToWorldPackets.Player_MovementRequest_w(x, y));
    }
}
