using System;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stats Config", menuName = "Game/Stats Config")]
public class StatsConfig : ScriptableObject
{
    // ─────────────────────────────────────────────
    // Base + Box Office
    // ─────────────────────────────────────────────

    [HorizontalGroup("Top")]
    [BoxGroup("Top/Base Health")]
    [HideLabel]
    [MinValue(1)]
    public int BaseHealth;

    [HorizontalGroup("Top")]
    [BoxGroup("Top/Starting Money")]
    [HideLabel]
    [MinValue(0)]
    public int StartingMoney;


    // ─────────────────────────────────────────────
    // Starting Genre
    // ─────────────────────────────────────────────

    [BoxGroup("Starting Genre")]
    [HideLabel]
    [AssetsOnly]
    [Required]
    public GenreNode StartingGenre;


    // ─────────────────────────────────────────────
    // Rewards
    // ─────────────────────────────────────────────

    [BoxGroup("Rewards")]
    [InlineProperty]
    [HideLabel]
    public RewardConfig Rewards;


    // ─────────────────────────────────────────────
    // Popularity
    // ─────────────────────────────────────────────

    [BoxGroup("Popularity")]
    [InlineProperty]
    [HideLabel]
    public PopularityConfig Popularity;
}


[Serializable]
public class RewardConfig
{
    // ─────────────────────────────────────────────
    // Death Rewards
    // ─────────────────────────────────────────────

    [HorizontalGroup("Death Rewards")]
    [BoxGroup("Death Rewards/Melee")]
    [HideLabel]
    [MinValue(0)]
    public int MeleeDeathPoints;

    [HorizontalGroup("Death Rewards")]
    [BoxGroup("Death Rewards/Ranged")]
    [HideLabel]
    [MinValue(0)]
    public int RangedDeathPoints;

    [HorizontalGroup("Death Rewards")]
    [BoxGroup("Death Rewards/Heavy")]
    [HideLabel]
    [MinValue(0)]
    public int HeavyDeathPoints;


    // ─────────────────────────────────────────────
    // Cost Multiplier
    // ─────────────────────────────────────────────

    [PropertySpace(SpaceBefore = 5)]
    [MinValue(0f)]
    [LabelWidth(105)]
    [LabelText("Cost Multiplier")]
    [SuffixLabel("×", Overlay = true)]
    public float CostMultiplier;
}

[Serializable]
public class PopularityConfig
{
    // ─────────────────────────────────────────────
    // General
    // ─────────────────────────────────────────────

    [HorizontalGroup("General")]
    [MinValue(0)]
    [LabelWidth(90)]
    [LabelText("Starting Points")]
    public int StartingPoints;

    [HorizontalGroup("General")]
    [MinValue(1)]
    [LabelWidth(55)]
    [LabelText("Max Tier")]
    public int MaxTier;


    // ─────────────────────────────────────────────
    // Tier Limit Growth
    // ─────────────────────────────────────────────

    [BoxGroup("Tier Limit Growth")]
    [HorizontalGroup("Tier Limit Growth/Values")]
    [MinValue(0)]
    [LabelWidth(35)]
    [LabelText("Base")]
    public int BaseLimit;

    [HorizontalGroup("Tier Limit Growth/Values")]
    [MinValue(0)]
    [LabelWidth(45)]
    [LabelText("Linear")]
    public int LinearLimitGrowth;

    [HorizontalGroup("Tier Limit Growth/Values")]
    [MinValue(0)]
    [LabelWidth(65)]
    [LabelText("Quadratic")]
    public int QuadraticLimitGrowth;


    // ─────────────────────────────────────────────
    // Calculated Values
    // ─────────────────────────────────────────────

    [FoldoutGroup("Calculated Values")]
    [ShowInInspector]
    [DisplayAsString]
    [LabelWidth(80)]
    [LabelText("Max Points")]
    public int MaxPoints => GetTierLimit(MaxTier);

    [FoldoutGroup("Calculated Values")]
    [ShowInInspector]
    [DisplayAsString]
    [LabelWidth(80)]
    [LabelText("Tier Limits")]
    private string TierLimitsPreview => BuildTierLimitsPreview();


    public int GetTierLimit(int tier)
    {
        return BaseLimit
             + tier * LinearLimitGrowth
             + tier * tier * QuadraticLimitGrowth;
    }

    private string BuildTierLimitsPreview()
    {
        if (MaxTier <= 1)
            return "—";

        string[] limits = new string[MaxTier - 1];

        for (int tier = 1; tier < MaxTier; tier++)
        {
            limits[tier - 1] = $"T{tier}: {GetTierLimit(tier)}";
        }

        return string.Join("     ", limits);
    }
}