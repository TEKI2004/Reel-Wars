using UnityEngine;

public enum UnitRole
{
    Melee,
    Ranged,
    Heavy
}

[CreateAssetMenu(menuName = "Game/Unit Type")]
public class UnitType : ScriptableObject
{
    public string unitName;
    public int cost;
    public int damage;
    public int maxHealth;
    public float moveSpeed;
    public float attackRange;
    public float attackCooldown;
    public UnitRole role;
}