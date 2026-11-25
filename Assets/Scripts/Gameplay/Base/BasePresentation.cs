using Sirenix.OdinInspector;
using UnityEngine;

public class BasePresentation : HealthPresentation
{
    [Title("Neon")]
    [Required][SerializeField] private Material neonOnMaterial;
    [Required][SerializeField] private Material neonOffMaterial;
    [Required][SerializeField] private Material rewardNeonMaterial;
    [Required][SerializeField] private NeonPlayerName playerNameDisplay;
    [Required][SerializeField] private BaseEmissionFader emissionFader;

    [Title("Feedback")]
    [Required][SerializeField] private Transform feedbackPoint;

    public Material NeonMaterial => rewardNeonMaterial;
    public override Vector3 FeedbackPosition => feedbackPoint.position;

    public void Initialize(string ownerId, int maxHealth)
    {
        playerNameDisplay.Initialize(neonOnMaterial, neonOffMaterial);
        playerNameDisplay.SetName(ownerId);
        SetHealth(maxHealth, maxHealth);
    }

    protected override void SetHealth(int currentHealth, int maxHealth)
    {
        float percent = (float)currentHealth / maxHealth;

        playerNameDisplay.SetLitPercent(percent);
        emissionFader.SetLitPercent(percent);
    }
}