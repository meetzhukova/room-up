public class Wallet
{
    private int coins;

    public int GetCoins()
    {
        return coins;
    }

    public void AddCoins(int amount)
    {
        if (amount > 0)
        {
            coins += amount;
        }
    }

    public bool TrySpend(int amount)
    {
        if (amount < 0 || amount > coins)
        {
            return false;
        }

        coins -= amount;
        return true;
    }
}
