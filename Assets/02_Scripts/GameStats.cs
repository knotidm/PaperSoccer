using System;

public static class GameStats
{
    public static int playerWinsCount;
    public static int aiWinsCount;
}

[Serializable]
public class GameStatsData
{
    public int playerWinsCount;
    public int aiWinsCount;
}