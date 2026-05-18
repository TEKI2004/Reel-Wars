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
    [Header("Identity")]
    public string UnitName;
    public UnitRole Role;

    [Header("Visual")]
    public Sprite Icon;
    public GameObject VisualPrefab;

    [Header("Economy")]
    public int Cost;

    [Header("Combat")]
    public int Damage;
    public int MaxHealth;
    public float AttackRange;
    public float AttackCooldown;

    [Header("Movement")]
    public float MoveSpeed;
}