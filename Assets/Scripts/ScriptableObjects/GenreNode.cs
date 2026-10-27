using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "New Genre Node", menuName = "Game/Genre Node")]
public class GenreNode : ScriptableObject
{
    // ─────────────────────────────────────────────
    // Overview
    // ─────────────────────────────────────────────

    [VerticalGroup("Overview/Layout/Info")]
    [ReadOnly]
    [LabelWidth(95)]
    [LabelText("ID")]
    [SerializeField] private string id;

    public string Id => id;

    [BoxGroup("Overview")]
    [HorizontalGroup("Overview/Layout", Width = 80)]
    [AssetsOnly]
    [Required]
    [PreviewField(80, ObjectFieldAlignment.Center)]
    [HideLabel]
    public Sprite Icon;

    [HorizontalGroup("Overview/Layout")]
    [VerticalGroup("Overview/Layout/Info")]
    [Required]
    [LabelWidth(95)]
    [LabelText("Genre Name")]
    public string GenreName;

    [VerticalGroup("Overview/Layout/Info")]
    [LabelWidth(95)]
    [LabelText("Passive")]
    public string PassiveDescription;


    // ─────────────────────────────────────────────
    // Units
    // ─────────────────────────────────────────────

    [BoxGroup("Units")]
    [AssetsOnly]
    [Required]
    [LabelWidth(70)]
    [LabelText("Melee")]
    public UnitType MeleeUnit;

    [BoxGroup("Units")]
    [AssetsOnly]
    [Required]
    [LabelWidth(70)]
    [LabelText("Ranged")]
    public UnitType RangedUnit;

    [BoxGroup("Units")]
    [AssetsOnly]
    [Required]
    [LabelWidth(70)]
    [LabelText("Heavy")]
    public UnitType HeavyUnit;

    [VerticalGroup("Overview/Layout/Info")]
    [Button("Regenerate ID", ButtonSizes.Small)]
    private void RegenerateId()
    {
        id = ConfigIdUtility.ToKebabCase(GenreName);
    }

    private void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(id) && !string.IsNullOrWhiteSpace(GenreName))
            RegenerateId();
    }

    // ─────────────────────────────────────────────
    // Children Genres
    // ─────────────────────────────────────────────

    [BoxGroup("Children Genres")]
    [AssetsOnly]
    [ListDrawerSettings(
        ShowFoldout = false,
        ShowPaging = false
    )]
    [ValidateInput(
        nameof(HasValidChildCount),
        "A GenreNode csak 0 vagy 2 childot tartalmazhat.",
        InfoMessageType.Warning
    )]
    [HideLabel]
    public List<GenreNode> Children = new();

    private bool HasValidChildCount(List<GenreNode> children)
    {
        return children == null ||
               children.Count == 0 ||
               children.Count == 2;
    }

}