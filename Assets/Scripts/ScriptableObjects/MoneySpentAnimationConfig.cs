using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(
    fileName = "MoneySpentAnimationConfig",
    menuName = "Game/UI/Money Spent Animation Config"
)]
public class MoneySpentAnimationConfig : ScriptableObject
{
    [BoxGroup("Movement")]
    [LabelText("End Offset")]
    public Vector2 EndOffset = new(12f, 35f);

    [BoxGroup("Movement")]
    [LabelText("End Rotation")]
    [SuffixLabel("°", Overlay = true)]
    public float EndRotation = -6f;


    [BoxGroup("Timing")]
    [LabelText("Duration")]
    [MinValue(0.05f)]
    [SuffixLabel("s", Overlay = true)]
    public float Duration = 0.7f;

    [BoxGroup("Timing")]
    [LabelText("Fade Start")]
    [Range(0f, 1f)]
    public float FadeStart = 0.25f;


    [BoxGroup("Appearance")]
    [LabelText("Start Opacity")]
    [Range(0f, 1f)]
    public float StartOpacity = 0.85f;

    [BoxGroup("Appearance")]
    [LabelText("Color")]
    public Color Color = new Color32(196, 147, 63, 255);
}