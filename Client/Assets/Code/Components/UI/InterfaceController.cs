#pragma warning disable 

using UnityEngine;
using UnityEngine.UI;

public class InterfaceController : MonoComponent
{
    [SerializeField]
    private Text gameTime_text = null;
    private GameTime gameTime = new GameTime();
    private long gameTime_lastSec = 0;

    [SerializeField]
    private Text mapName_text = null;
    private string mapName = "";

    [SerializeField]
    private Text player_name_text = null;
    [SerializeField]
    private Text player_level_text = null;

    void Start()
    {
        if (gameTime_text == null)
            Debug.LogError("InterfaceAccessor has no reference to gameTime_text.");
        if (mapName_text == null)
            Debug.LogError("InterfaceAccessor has no reference to mapName_text.");
        if (player_name_text == null)
            Debug.LogError("InterfaceAccessor has no reference to player_name_text.");
        if (player_level_text == null)
            Debug.LogError("InterfaceAccessor has no reference to player_level_text.");

        SetMapName(Application.loadedLevelName);
    }

    void Update()
    {
        //GameTime
        long nowSec = gameTime.NowSec;
        if (nowSec > gameTime_lastSec)
        {
            gameTime_lastSec = nowSec;

            string newText = "";

            if (nowSec / 60 < 10)
                newText += "0";
            newText += nowSec / 60 + ":";

            if (nowSec % 60 < 10)
                newText += "0";
            newText += nowSec % 60;

            gameTime_text.text = newText;
        }
    }

    public void SetGameTime(long timeMilliSec)
    {
        gameTime.SetCurrentGameTime(timeMilliSec);
        gameTime_lastSec = 0;
    }

    public void SetMapName(string name)
    {
        mapName = name;
        mapName_text.text = mapName;
    }

    public void SetPlayerName(string name)
    {
        player_name_text.text = name;
    }

    public void SetPlayerLevel(int level)
    {
        player_level_text.text = level.ToString();
    }
}
