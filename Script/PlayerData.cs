using UnityEngine;

public static class PlayerData
{
    public static int Coin = 20;

    public static int attackLevel = 0;
    public static int healthLevel = 0;
    public static int speedLevel = 0;

    public static void AddCoin(int amount)
    {
        Coin += amount;
    }

    public static bool SpendCoin(int amount)
    {
        if (Coin < amount) return false;
        Coin -= amount;
        return true;
    }
}

