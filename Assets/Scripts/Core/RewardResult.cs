public readonly struct RewardResult
{
    public int Popularity { get; }
    public int Money { get; }

    public RewardResult(int popularity, int money)
    {
        Popularity = popularity;
        Money = money;
    }
}