using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class PopularityMetersController : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // Config
    // ─────────────────────────────────────────────

    [BoxGroup("Config")]
    [AssetsOnly]
    [Required]
    [LabelText("Stats Config")]
    [SerializeField]
    private StatsConfig statsConfig;


    // ─────────────────────────────────────────────
    // Tier Limits
    // ─────────────────────────────────────────────

    [BoxGroup("Tier Limits")]
    [AssetsOnly]
    [Required]
    [LabelText("Prefab")]
    [SerializeField]
    private Transform tierLimitPrefab;

    [BoxGroup("Tier Limits")]
    [Range(0f, 10f)]
    [SuffixLabel("°", Overlay = true)]
    [LabelText("Tilt")]
    [SerializeField]
    private float tierLimitTilt = 1.5f;

    [HorizontalGroup("Tier Limits/Buttons")]
    [Button("Rebuild Tier Limits", ButtonSizes.Medium)]
    [EnableIf(nameof(CanRebuildTierLimits))]
    private void CreateTierLimits()
    {
        CacheMeterGeometry();
        ClearTierLimits();

        for (int tier = 1; tier < Config.MaxTier; tier++)
        {
            int points = Config.GetTierLimit(tier);
            float y = GetYPosition(points);

            CreateTierLimit(
                leftMaxTierLimit.parent,
                $"LeftTier{tier}Limit",
                y,
                -tierLimitTilt
            );

            CreateTierLimit(
                rightMaxTierLimit.parent,
                $"RightTier{tier}Limit",
                y,
                tierLimitTilt
            );
        }
    }

    [HorizontalGroup("Tier Limits/Buttons")]
    [Button("Clear Tier Limits", ButtonSizes.Medium)]
    [EnableIf(nameof(CanClearTierLimits))]
    private void ClearTierLimits()
    {
        ClearTierLimits(
            leftMaxTierLimit.parent,
            "LeftTier"
        );

        ClearTierLimits(
            rightMaxTierLimit.parent,
            "RightTier"
        );
    }


    // ─────────────────────────────────────────────
    // Meters
    // ─────────────────────────────────────────────

    [HorizontalGroup("Meters")]
    [BoxGroup("Meters/Left Meter")]
    [SceneObjectsOnly]
    [Required]
    [LabelText("Max Tier Limit")]
    [SerializeField]
    private Transform leftMaxTierLimit;

    [BoxGroup("Meters/Left Meter")]
    [SceneObjectsOnly]
    [Required]
    [LabelText("Fill")]
    [SerializeField]
    private Transform leftFill;

    [BoxGroup("Meters/Left Meter")]
    [SceneObjectsOnly]
    [Required]
    [LabelText("Status")]
    [SerializeField] 
    private PopularityStatus leftStatus;


    [HorizontalGroup("Meters")]
    [BoxGroup("Meters/Right Meter")]
    [SceneObjectsOnly]
    [Required]
    [LabelText("Max Tier Limit")]
    [SerializeField]
    private Transform rightMaxTierLimit;

    [BoxGroup("Meters/Right Meter")]
    [SceneObjectsOnly]
    [Required]
    [LabelText("Fill")]
    [SerializeField]
    private Transform rightFill;

    [BoxGroup("Meters/Right Meter")]
    [SceneObjectsOnly]
    [Required]
    [LabelText("Status")]
    [SerializeField] 
    private PopularityStatus rightStatus;

    private const float statusTopOffset = 0.15f;

    private const float statusHeight = 0.3f;

    // ─────────────────────────────────────────────
    // Animation
    // ─────────────────────────────────────────────

    [BoxGroup("Animation")]
    [MinValue(0.01f)]
    [SuffixLabel("s", Overlay = true)]
    [LabelText("Smooth Time")]
    [SerializeField] private float fillSmoothTime = 0.35f;

    private float leftTargetScaleY;
    private float rightTargetScaleY;

    private float leftFillVelocity;
    private float rightFillVelocity;


    // ─────────────────────────────────────────────
    // Runtime Geometry
    // ─────────────────────────────────────────────

    [FoldoutGroup("Runtime Geometry")]
    [ShowInInspector]
    [ReadOnly]
    [LabelText("Bottom Y")]
    private float bottomY;

    [FoldoutGroup("Runtime Geometry")]
    [ShowInInspector]
    [ReadOnly]
    [LabelText("Top Y")]
    private float topY;

    [FoldoutGroup("Runtime Geometry")]
    [ShowInInspector]
    [ReadOnly]
    [LabelText("Full Height")]
    private float fullHeight;


    // ─────────────────────────────────────────────
    // Runtime
    // ─────────────────────────────────────────────

    private Player leftPlayer;
    private Player rightPlayer;

    private PopularityConfig Config =>
        statsConfig.Popularity;

    private bool CanRebuildTierLimits =>
        statsConfig != null &&
        tierLimitPrefab != null &&
        leftMaxTierLimit != null &&
        leftFill != null &&
        rightMaxTierLimit != null &&
        rightFill != null;

    private bool CanClearTierLimits =>
        leftMaxTierLimit != null &&
        rightMaxTierLimit != null;


    private void OnEnable()
    {
        if (!Application.isPlaying)
            return;

        CacheMeterGeometry();
        CreateTierLimits();

        leftPlayer = GameManager.Instance.LeftBase.Player;
        rightPlayer = GameManager.Instance.RightBase.Player;

        leftPlayer.Popularity.OnPointsChanged += UpdateLeftMeter;
        rightPlayer.Popularity.OnPointsChanged += UpdateRightMeter;

        leftPlayer.Popularity.OnTierChanged += UpdateLeftStatus;
        rightPlayer.Popularity.OnTierChanged += UpdateRightStatus;

        SetFillImmediate(leftFill, leftStatus, leftPlayer.Popularity.CurrentPoints, ref leftTargetScaleY);
        SetFillImmediate(rightFill, rightStatus, rightPlayer.Popularity.CurrentPoints, ref rightTargetScaleY);

        UpdateStatus(leftStatus, leftPlayer.Popularity);
        UpdateStatus(rightStatus, rightPlayer.Popularity);
    }

    private void OnDisable()
    {
        if (!Application.isPlaying)
            return;

        leftPlayer.Popularity.OnPointsChanged -= UpdateLeftMeter;
        rightPlayer.Popularity.OnPointsChanged -= UpdateRightMeter;

        leftPlayer.Popularity.OnTierChanged -= UpdateLeftStatus;
        rightPlayer.Popularity.OnTierChanged -= UpdateRightStatus;

        ClearTierLimits();
    }


    // ─────────────────────────────────────────────
    // Meter Geometry
    // ─────────────────────────────────────────────

    private void CacheMeterGeometry()
    {
        bottomY = leftFill.localPosition.y;
        topY = leftMaxTierLimit.localPosition.y;

        fullHeight = topY - bottomY;
    }


    // ─────────────────────────────────────────────
    // Tier Limit Creation
    // ─────────────────────────────────────────────

    private void CreateTierLimit(
        Transform parent,
        string objectName,
        float y,
        float zRotation
    )
    {
        Transform limit = Instantiate(
            tierLimitPrefab,
            parent,
            false
        );

        limit.name = objectName;

        Vector3 position = limit.localPosition;
        position.y = y;
        limit.localPosition = position;

        Vector3 rotation = limit.localEulerAngles;
        rotation.z = zRotation;
        limit.localEulerAngles = rotation;
    }

    private float GetYPosition(int points)
    {
        float t = GetNormalizedPopularity(points);

        return Mathf.Lerp(
            bottomY,
            topY,
            t
        );
    }

    private static void ClearTierLimits(
        Transform parent,
        string prefix)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Transform child = parent.GetChild(i);

            if (!child.name.StartsWith(prefix) ||
                !child.name.EndsWith("Limit"))
            {
                continue;
            }

            if (Application.isPlaying)
                Destroy(child.gameObject);
            else
                DestroyImmediate(child.gameObject);
        }
    }


    // ─────────────────────────────────────────────
    // Fill
    // ─────────────────────────────────────────────

    private void Update()
    {
        AnimateFill(leftFill, leftTargetScaleY, ref leftFillVelocity);
        AnimateFill(rightFill, rightTargetScaleY, ref rightFillVelocity);

        UpdateStatusPosition(leftFill, leftStatus);
        UpdateStatusPosition(rightFill, rightStatus);
    }

    private void AnimateFill(Transform fill, float targetScaleY, ref float velocity)
    {
        Vector3 scale = fill.localScale;
        scale.y = Mathf.SmoothDamp(scale.y, targetScaleY, ref velocity, fillSmoothTime);
        fill.localScale = scale;
    }

    private void UpdateStatusPosition(Transform fill, PopularityStatus status)
    {
        float fillHeight = fill.localScale.y;
        bool shouldBeVisible = fillHeight >= statusHeight + statusTopOffset;

        if (status.gameObject.activeSelf != shouldBeVisible)
            status.gameObject.SetActive(shouldBeVisible);

        Vector3 position = status.transform.localPosition;
        position.y = fill.localPosition.y + fillHeight - statusTopOffset;
        status.transform.localPosition = position;
    }

    private void UpdateLeftMeter(int points)
    {
        leftTargetScaleY = GetFillScaleY(points);
        UpdateLeftStatus();
    }

    private void UpdateRightMeter(int points)
    {
        rightTargetScaleY = GetFillScaleY(points);
        UpdateRightStatus();
    }

    private void UpdateLeftStatus()
    {
        UpdateStatus(leftStatus, leftPlayer.Popularity);
    }

    private void UpdateRightStatus()
    {
        UpdateStatus(rightStatus, rightPlayer.Popularity);
    }

    private void UpdateStatus(PopularityStatus status, Popularity popularity)
    {
        int target = Config.GetTierLimit(popularity.CurrentTier);
        status.SetValue(popularity.CurrentPoints, target);
    }

    private void SetFillImmediate(Transform fill, PopularityStatus status, int points, ref float targetScaleY)
    {
        targetScaleY = GetFillScaleY(points);

        Vector3 scale = fill.localScale;
        scale.y = targetScaleY;
        fill.localScale = scale;

        UpdateStatusPosition(fill, status);
    }

    private float GetFillScaleY(int points)
    {
        return fullHeight * GetNormalizedPopularity(points);
    }


    // ─────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────

    private float GetNormalizedPopularity(int points)
    {
        if (Config.MaxPoints <= 0)
            return 0f;

        return Mathf.Clamp01(
            (float)points / Config.MaxPoints
        );
    }

    public void SetOscarStatus(string ownerId, bool active)
    {
        PopularityStatus status;

        if (ownerId == GameManager.Instance.LeftBase.OwnerId)
            status = leftStatus;
        else if (ownerId == GameManager.Instance.RightBase.OwnerId)
            status = rightStatus;
        else
        {
            Debug.LogError($"Popularity status not found for owner ID: {ownerId}");
            return;
        }

        status.SetOscarSeason(active);
    }
}