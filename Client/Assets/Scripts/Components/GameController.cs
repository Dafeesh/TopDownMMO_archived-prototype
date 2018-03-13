using UnityEngine;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

using Extant;
using Extant.Networking;
using Extant.GameServerShared;

public class GameController : MonoBehaviour
{
    private GameServerConnection connection_gs;
    private Scoreboard scoreBoard;
    private UserInterface userInterface;

    private Dictionary<string, CharacterController> characters = new Dictionary<string, CharacterController>();
    private PlayerController thisPlayer;

    private DayPhase dayPhase = DayPhase.Day;

    private Stopwatch gameTime = new Stopwatch();
    private System.TimeSpan gameTime_start = new System.TimeSpan();

    private void Start()
    {
        connection_gs = this.GetComponent<GameServerConnection>();
        scoreBoard = this.GetComponent<Scoreboard>();
        userInterface = this.GetComponent<UserInterface>();
        
        //Test
        gameTime.Start();
        //~
    }

    private void Update()
    {
        Packet p = null;
        while ((p = connection_gs.GetPacket()) != null)
        {
            switch((GameServerPackets.PacketType)p.Type)
            {
                case (GameServerPackets.PacketType.Game_Info_c):
                    {
                        GameServerPackets.Game_Info_c pp = p as GameServerPackets.Game_Info_c;

                        gameTime_start = new System.TimeSpan(0,0,0,0,pp.gameTime);
                        gameTime.Reset();
                        gameTime.Start();

                        //pp.mapPreset;

                        break;
                    }

                case (GameServerPackets.PacketType.Player_Add_c):
                    {
                        GameServerPackets.Player_Add_c pp = p as GameServerPackets.Player_Add_c;
                        scoreBoard.AddPlayer(new PlayerInfo() { Name = pp.name, Clan = pp.clan, Level = pp.level, TeamColor = TeamColor.Neutral });
                        break;
                    }

                case (GameServerPackets.PacketType.Player_Score_c):
                    {
                        GameServerPackets.Player_Score_c pp = p as GameServerPackets.Player_Score_c;
                        PlayerInfo pi = scoreBoard.GetPlayerInfo(pp.username);
                        if (pi != null)
                        {
                            pi.Kills = pp.kill;
                            pi.Deaths = pp.death;
                            pi.Assists = pp.assist;
                            pi.TeamColor = pp.teamColor;
                        }
                        break;
                    }

                case (GameServerPackets.PacketType.Game_DayPhase_c):
                    {
                        GameServerPackets.Game_DayPhase_c pp = p as GameServerPackets.Game_DayPhase_c;

                        dayPhase = (DayPhase)pp.dayPhase;
                        userInterface.DayPhase = dayPhase;

                        break;
                    }

                default:
                    DebugLogger.GlobalDebug.LogCatch("Unsupported packet: " + ((GameServerPackets.PacketType)p.Type).ToString());
                    break;
            }
        }
    }

    public System.TimeSpan ElapsedTime
    {
        get
        {
            return gameTime.Elapsed.Add(gameTime_start);
        }
    }
}