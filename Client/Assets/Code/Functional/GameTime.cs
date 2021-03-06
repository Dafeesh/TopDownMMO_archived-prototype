﻿using System;
using System.Diagnostics;

public class GameTime
{
    private long gameTime_offset;
    private Stopwatch gameTime_timer = new Stopwatch();

    public GameTime()
    {
        gameTime_timer.Start();
        SetCurrentGameTime(0);
    }

    public long Now
    {
        get
        {
            return gameTime_offset + gameTime_timer.ElapsedMilliseconds;
        }
    }

    public long NowSec
    {
        get
        {
            return (gameTime_offset + gameTime_timer.ElapsedMilliseconds) / 1000;
        }
    }

    public void SetCurrentGameTime(long time)
    {
        gameTime_offset = time;

        if (gameTime_timer.IsRunning)
        {
            gameTime_timer.Stop();
            gameTime_timer.Reset();
            gameTime_timer.Start();
        }
    }
}
