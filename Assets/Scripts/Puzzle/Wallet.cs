public class Wallet
{
    private int coins;

    public Wallet()
    {
    }

    public Wallet(int startCoins)
    {
        coins = startCoins > 0 ? startCoins : 0;
    }

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
