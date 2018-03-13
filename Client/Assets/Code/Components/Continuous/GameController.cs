using Extant;
using Extant.Networking;
using SharedComponents;
using SharedComponents.GameProperties;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private InterfaceAccessor uiAccessor;

    [SerializeField]
    private int packetCount = 0;

    private NetConnection wsCon;
    private GameTime gameTime;
    private bool isActive;

    private GameObject baseMapObject;

    void Start()
    {
        if (uiAccessor == null)
        {
            Debug.LogError("GameController was not given an InterfaceAccessor.");
        }

        gameTime = new GameTime();
        isActive = false;
        baseMapObject = null;
    }

    void Update()
    {
        if (isActive)
        {
            if (baseMapObject == null)
            {
                baseMapObject = GameObject.Find("_SceneBase");
                if (baseMapObject != null)
                {
                    Debug.Log("GameController found SceneBase.");
                }
            }
            else
            {
                uiAccessor.GameTime = gameTime.Now;

                HandleIncomingPackets();
            }
        }
    }

    private void HandleIncomingPackets()
    {
        Packet p = null;
        while ((p = wsCon.GetPacket()) != null)
        {
            packetCount++;
            switch ((ClientToWorldPackets.PacketType)p.Type)
            {
                case (ClientToWorldPackets.PacketType.Map_MoveTo_c):
                    {
                        LoadNewMap((MapID)(p as ClientToWorldPackets.Map_MoveTo_c).mapNum);
                        DebugLogger.GlobalDebug.Log(DebugLogger.LogType.Networking, "WorldServer- LoadMap: " + (MapID)(p as ClientToWorldPackets.Map_MoveTo_c).mapNum);
                    }
                    break;

                case (ClientToWorldPackets.PacketType.Character_Add_c):
                    {
                        DebugLogger.GlobalDebug.Log(DebugLogger.LogType.Networking, "WorldServer- Add Character: " + (p as ClientToWorldPackets.Character_Add_c).charId);
                    }
                    break;

                case (ClientToWorldPackets.PacketType.Character_Position_c):
                    {
                        DebugLogger.GlobalDebug.Log(DebugLogger.LogType.Networking, "WorldServer- Character position: " + (p as ClientToWorldPackets.Character_Position_c).charId);
                    }
                    break;

                case (ClientToWorldPackets.PacketType.Character_Remove_c):
                    {
                        DebugLogger.GlobalDebug.Log(DebugLogger.LogType.Networking, "WorldServer- Remove Character: " + (p as ClientToWorldPackets.Character_Remove_c).charId);
                    }
                    break;

                default:
                    {
                        DebugLogger.GlobalDebug.Log(DebugLogger.LogType.Catch, "WorldServer- Got unknown packet: " + ((ClientToWorldPackets.PacketType)p.Type).ToString());
                    }
                    break;
            }
        }
    }

    private void LoadNewMap(MapID id)
    {
        switch (id)
        {
            case (MapID.TestMap):
                {
                    Application.LoadLevel("TestMap");
                }
                break;

            default:
                {
                    Debug.Log("GameController could not load map: " + id.ToString());
                }
                break;
        }
    }

    public void Stop()
    {
        IsActive = false;
    }

    public bool IsActive
    {
        get
        {
            return isActive;
        }

        private set
        {
            isActive = value;

            if (isActive)
            {
                baseMapObject = GameObject.Find("_SceneBase");
                if (baseMapObject == null)
                {
                    Debug.LogError("GameController could not find SceneBase.");
                }
            }
            else
            {
                if (baseMapObject != null)
                {
                    Debug.Log("GameController deleted SceneBase.");
                    Destroy(baseMapObject);
                }
                baseMapObject = null;
            }
        }
    }

    public void SetWorldServerConnection(NetConnection wsc)
    {
        wsCon = wsc;
        IsActive = true;
    }
}
