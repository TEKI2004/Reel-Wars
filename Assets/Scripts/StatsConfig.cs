using UnityEngine;

[CreateAssetMenu(menuName = "Game/Stats Config")]
public class StatsConfig : ScriptableObject
{
    [Header("Base Stats")]
    public int baseHealth;

    [Header("Rewards")]
    public int meleeDeathPoints;
    public int rangedDeathPoints;
    public int heavyDeathPoints;
    public float costMultiplier;

    [Header("Popularity")]
    public int startingPoints;
    public int startingTier;

    [Header("Popularity Tier Limits")]
    public int baseLimit;
    public int linearLimitGrowth;
    public int quadraticLimitGrowth;

    [Header("Debug Tier Limits")]
    [SerializeField, Tooltip("Calculated")] private int tier1;
    [SerializeField, Tooltip("Calculated")] private int tier2;
    [SerializeField, Tooltip("Calculated")] private int tier3;
    [SerializeField, Tooltip("Calculated")] private int tier4;

    [Header("Box Office")]
    public int startingMoney;

    private void OnValidate()
    {
        tier1 = GetLimit(1);
        tier2 = GetLimit(2);
        tier3 = GetLimit(3);
        tier4 = GetLimit(4);
    }

    private int GetLimit(int tier)
    {
        return baseLimit
             + tier * linearLimitGrowth
             + tier * tier * quadraticLimitGrowth;
    }
}