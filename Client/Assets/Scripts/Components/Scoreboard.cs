using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Extant;
using Extant.GameServerShared;

public class Scoreboard : MonoBehaviour 
{
    private List<PlayerInfo> players = new List<PlayerInfo>();

    private bool toggle = false;

    private Rect board = new Rect(Screen.width / 2 - 150, Screen.height / 2 - 150, 300, 300);
    private List<Rect> playerSlots = new List<Rect>();

	void Start () 
    {
	    
	}
	
	void Update () 
    {
        toggle = Input.GetKey(KeyCode.Tab);
	}

    void OnGUI()
    {
        if (toggle)
        {
            GUI.Box(board, "Scoreboard");

            for (int i=0; i<playerSlots.Count; i++)
            {
                GUI.Label(playerSlots[i], players[i].Name + "/" + players[i].Clan + "/" + players[i].Level + "/" + players[i].TeamColor.ToString() + " - " + players[i].Kills + "/" + players[i].Deaths + "/" + players[i].Assists);
            }
        }
    }

    private void UpdateScorePositions()
    {
        playerSlots.Clear();
        for (int i=0; i<players.Count; i++)
        {
            playerSlots.Add(new Rect(board.x + 10, board.y + 25 + 25 * i, board.xMax, board.y + 10 + 25 * i + 25));
        }
    }

    public void AddPlayer(PlayerInfo p)
    {
        players.Add(p);
        UpdateScorePositions();
    }

    public void RemovePlayer(string username)
    {
        PlayerInfo del = null;
        foreach (PlayerInfo p in players)
        {
            if (p.Name == username)
                del = p;
        }

        if (del != null)
            players.Remove(del);
    }

    public PlayerInfo GetPlayerInfo(string name)
    {
        foreach (PlayerInfo p in players)
        {
            if (p.Name == name)
                return p;
        }

        DebugLogger.GlobalDebug.LogError("Scoreboard did could not find player: " + name);
        throw new MissingReferenceException();
    }
}
