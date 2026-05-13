using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Genre Node")]
public class GenreNode : ScriptableObject
{
    public string GenreName;
    public int RequiredTier;

    public UnitType MeleeUnit;
    public UnitType RangedUnit;
    public UnitType HeavyUnit;

    public List<GenreNode> Children = new();

    public bool IsAvailableFor(Player player)
    {
        return player.Popularity.CurrentTier >= RequiredTier;
    }
}