using Sirenix.OdinInspector;
using UnityEngine;

public class UnitPresentation : HealthPresentation
{
    [Required][SerializeField] private FloatingHpBar hpBar;
    [Required][SerializeField] private Transform moneyRewardSpawnPoint;
    [Required][SerializeField] private Transform popularityRewardSpawnPoint;
    [Required][SerializeField] private Transform visualRoot;
    internal Transform VisualRoot => visualRoot;
    public override Vector3 FeedbackPosition => hpBar.transform.position;

    protected override void SetHealth(int currentHealth, int maxHealth)
    {
        hpBar.SetPercent((float)currentHealth / maxHealth);
    }

    public void MirrorVisual()
    {
        Vector3 scale = visualRoot.localScale;
        scale.z *= -1f;
        visualRoot.localScale = scale;
    }
    public void ShowReward(RewardResult reward, Vector3 moveDirection, Material popularityMaterial)
    {
        FloatingTag floatingTag = CreateFloatingTag();
        floatingTag.ShowReward(reward, moneyRewardSpawnPoint.position, popularityRewardSpawnPoint.position, moveDirection, popularityMaterial);
    }

    public void ShowLifesteal(int currentHealth, int maxHealth)
    {
        ShowHealing(currentHealth, maxHealth);
        ShowFeedback(GenrePassiveType.Lifesteal);
    }

    public void CalibrateAnchors(Bounds modelBounds)
    {
        SetWorldY(popularityRewardSpawnPoint, Mathf.Lerp(modelBounds.min.y, modelBounds.max.y, 0.65f));
        SetWorldY(moneyRewardSpawnPoint, Mathf.Lerp(modelBounds.min.y, modelBounds.max.y, 0.85f));
        SetWorldY(hpBar.transform, modelBounds.max.y + 0.2f);
    }

    private static void SetWorldY(Transform target, float y)
    {
        Vector3 position = target.position;
        position.y = y;
        target.position = position;
    }

#if UNITY_EDITOR
    public void EditorAutoWire(FloatingTag sharedFloatingTagPrefab)
    {
        visualRoot = FindChild("Visual");
        shaker = GetComponentInChildren<AttackedShaker>(true);
        hpBar = GetComponentInChildren<FloatingHpBar>(true);
        floatingTagPrefab = sharedFloatingTagPrefab;
        moneyRewardSpawnPoint = FindChild("MoneyRewardSpawnPoint");
        popularityRewardSpawnPoint = FindChild("PopularityRewardSpawnPoint");
    }

    private Transform FindChild(string childName)
    {
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            if (child.name == childName )
                return child;
        }

        return null;
    }
#endif

}