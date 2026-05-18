using UnityEngine;

public class PopularityMetersController : MonoBehaviour
{
    [Header("Tier limits (left)")]
    [SerializeField] private Transform leftTier1Limit;
    [SerializeField] private Transform leftTier2Limit;
    
    [Header("Tier limits (right)")]
    [SerializeField] private Transform rightTier1Limit;
    [SerializeField] private Transform rightTier2Limit;

    [Header("Fill bars")]
    [SerializeField] private Transform leftFill;
    [SerializeField] private Transform rightFill;

    [SerializeField] private float maxHeight = 6.3f;
    [SerializeField] private float groundHeight = -3.5f;

    private Player leftPlayer;
    private Player rightPlayer;

    private void OnEnable()
    {
        ConfigureTierLimits();

        leftPlayer = GameManager.Instance.LeftBase.Player;
        rightPlayer = GameManager.Instance.RightBase.Player;

        leftPlayer.Popularity.OnPointsChanged += UpdateLeftMeter;
        rightPlayer.Popularity.OnPointsChanged += UpdateRightMeter;

        UpdateLeftMeter(leftPlayer.Popularity.CurrentPoints);
        UpdateRightMeter(rightPlayer.Popularity.CurrentPoints);
    }

    private void OnDisable()
    {
        leftPlayer.Popularity.OnPointsChanged -= UpdateLeftMeter;
        rightPlayer.Popularity.OnPointsChanged -= UpdateRightMeter;
    }

    private void ConfigureTierLimits()
    {
        int[] limits = Popularity.GetPopularityThresholds();
        SetTierLimitPosition(leftTier1Limit, limits[0]);
        SetTierLimitPosition(leftTier2Limit, limits[1]);
        SetTierLimitPosition(rightTier1Limit, limits[0]);
        SetTierLimitPosition(rightTier2Limit, limits[1]);
    }

    private void SetTierLimitPosition(Transform limit, int points)
    {
        float t = Mathf.Clamp01((float) points / GameManager.Instance.Config.Popularity.MaxPoints);
        Vector3 position = limit.localPosition;
        position.y = t * maxHeight + groundHeight;
        limit.localPosition = position;
    }

    private void UpdateLeftMeter(int points)
    {
        UpdateFill(leftFill, points);
    }

    private void UpdateRightMeter(int points)
    {
        UpdateFill(rightFill, points);
    }

    private void UpdateFill(Transform fill, int points)
    {
        float t = Mathf.Clamp01((float)points / GameManager.Instance.Config.Popularity.MaxPoints);

        Vector3 scale = fill.localScale;
        scale.y = t * maxHeight;
        fill.localScale = scale;
    }
}