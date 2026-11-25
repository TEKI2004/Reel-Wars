using UnityEngine;

public class FloatingHpBar : MonoBehaviour
{
    [SerializeField] private Transform fill;

    private Vector3 fullScale;
    private Vector3 fullPosition;

    private void Awake()
    {
        fullScale = fill.localScale;
        fullPosition = fill.localPosition;
    }

    public void SetPercent(float percent)
    {
        percent = Mathf.Clamp01(percent);

        fill.localScale = new Vector3(
            fullScale.x * percent,
            fullScale.y,
            fullScale.z
        );

        float offsetX = -(fullScale.x * (1f - percent)) / 2f;

        fill.localPosition = new Vector3(
            fullPosition.x + offsetX,
            fullPosition.y,
            fullPosition.z
        );
    }
}