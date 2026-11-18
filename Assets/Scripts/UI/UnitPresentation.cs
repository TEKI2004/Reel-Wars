using Sirenix.OdinInspector;
using UnityEngine;

public class UnitPresentation : MonoBehaviour
{
    [Required][SerializeField] private AttackedShaker shaker;
    [Required][SerializeField] private FloatingHpBar hpBar;
    [Required][SerializeField] private FloatingUnitTag unitTagPrefab;
    [Required][SerializeField] private Transform moneyRewardSpawnPoint;
    [Required][SerializeField] private Transform popularityRewardSpawnPoint;
    [Required][SerializeField] private Transform visualRoot;
    internal Transform VisualRoot => visualRoot;
    public Vector3 FeedbackPosition => hpBar.transform.position;

    public void MirrorVisual()
    {
        Vector3 scale = visualRoot.localScale;
        scale.z *= -1f;
        visualRoot.localScale = scale;
    }

    public void ShowDamage(int currentHealth, int maxHealth)
    {
        shaker.Shake();
        hpBar.SetPercent((float)currentHealth / maxHealth);
    }

    public void ShowHealing(int currentHealth, int maxHealth)
    {
        hpBar.SetPercent((float)currentHealth / maxHealth);
    }

    public void ShowLastStand(GenrePassiveType passiveType, int currentHealth, int maxHealth)
    {
        shaker.Shake();
        hpBar.SetPercent((float)currentHealth / maxHealth);
        ShowFeedback(passiveType);
    }

    public void ShowFeedback(GenrePassiveType passiveType, int amount = 0)
    {
        FloatingUnitTag unitTag = Instantiate(unitTagPrefab);
        unitTag.ShowFeedback(passiveType, FeedbackPosition, amount);
    }

    public void ShowFeedback(GenrePassiveType passiveType, Vector3 position, int amount = 0)
    {
        FloatingUnitTag unitTag = Instantiate(unitTagPrefab);
        unitTag.ShowFeedback(passiveType, position, amount);
    }

    public void ShowReward(RewardResult reward, Vector3 moveDirection, Material popularityMaterial)
    {
        FloatingUnitTag unitTag = Instantiate(unitTagPrefab);
        unitTag.ShowReward(reward, moneyRewardSpawnPoint.position, popularityRewardSpawnPoint.position, moveDirection, popularityMaterial);
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
    public void EditorAutoWire(FloatingUnitTag sharedUnitTagPrefab)
    {
        visualRoot = FindChild("Visual");
        shaker = GetComponentInChildren<AttackedShaker>(true);
        hpBar = GetComponentInChildren<FloatingHpBar>(true);
        unitTagPrefab = sharedUnitTagPrefab;
        moneyRewardSpawnPoint = FindChild("MoneyRewardSpawnPoint");
        popularityRewardSpawnPoint = FindChild("PopularityRewardSpawnPoint");
    }

    private Transform FindChild(string childName)
    {
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            if (child.name == childName)
                return child;
        }

        return null;
    }
#endif

}