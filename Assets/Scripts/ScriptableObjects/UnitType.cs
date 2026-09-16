using Sirenix.OdinInspector;
using UnityEngine;

public enum UnitRole
{
    Melee,
    Ranged,
    Heavy
}

[CreateAssetMenu(fileName = "New Unit Type", menuName = "Game/Unit Type")]
public class UnitType : ScriptableObject
{
    // ─────────────────────────────────────────────
    // Overview
    // ─────────────────────────────────────────────

    [BoxGroup("Overview")]
    [HorizontalGroup("Overview/Layout", Width = 75)]
    [AssetsOnly]
    [Required]
    [PreviewField(60, ObjectFieldAlignment.Center)]
    [HideLabel]
    public Sprite Icon;

    [HorizontalGroup("Overview/Layout")]
    [VerticalGroup("Overview/Layout/Info")]
    [Required]
    [LabelWidth(80)]
    [LabelText("Unit Name")]
    public string UnitName;

    [VerticalGroup("Overview/Layout/Info")]
    [EnumToggleButtons]
    [LabelWidth(80)]
    [LabelText("Role")]
    public UnitRole Role;

    [VerticalGroup("Overview/Layout/Info")]
    [AssetsOnly]
    [Required]
    [LabelWidth(80)]
    [LabelText("Prefab")]
    public GameObject VisualPrefab;


    // ─────────────────────────────────────────────
    // Stats
    // ─────────────────────────────────────────────

    [HorizontalGroup("Stats")]
    [BoxGroup("Stats/Core")]
    [MinValue(0)]
    [LabelWidth(85)]
    public int Cost;

    [BoxGroup("Stats/Core")]
    [MinValue(0f)]
    [LabelWidth(85)]
    [LabelText("Move Speed")]
    public float MoveSpeed;


    [HorizontalGroup("Stats")]
    [BoxGroup("Stats/Power")]
    [MinValue(0)]
    [LabelWidth(85)]
    public int Damage;

    [BoxGroup("Stats/Power")]
    [MinValue(1)]
    [LabelWidth(85)]
    [LabelText("Max Health")]
    public int MaxHealth;


    [HorizontalGroup("Stats")]
    [BoxGroup("Stats/Attack")]
    [MinValue(0f)]
    [LabelWidth(85)]
    [LabelText("Range")]
    public float AttackRange;

    [BoxGroup("Stats/Attack")]
    [MinValue(0.01f)]
    [LabelWidth(85)]
    [LabelText("Cooldown")]
    [SuffixLabel("s", Overlay = true)]
    public float AttackCooldown;
}