using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Genre Node")]
public class GenreNode : ScriptableObject
{
    [Header("Genre Info")]
    public string GenreName;
    public int RequiredTier;
    
    [Header("Unit Types")]
    public UnitType MeleeUnit;
    public UnitType RangedUnit;
    public UnitType HeavyUnit;

    [Header("Passive")]
    public string PassiveDescription;

    [Header("Children Genres")]
    public List<GenreNode> Children = new();

    public bool IsAvailableFor(Player player)
    {
        return player.Popularity.CurrentTier >= RequiredTier;
    }
}