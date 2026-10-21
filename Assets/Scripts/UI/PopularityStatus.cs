using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class PopularityStatus : MonoBehaviour
{
    [Title("References")]
    [Required][SerializeField] private TextMeshPro currentText;
    [Required][SerializeField] private TextMeshPro dividerText;
    [Required][SerializeField] private TextMeshPro targetText;

    [Title("Materials")]
    [AssetsOnly][Required][SerializeField] private Material defaultMaterial;
    [AssetsOnly][Required][SerializeField] private Material oscarSeasonMaterial;

    private Vector3 dividerBaseScale;

    private void Awake()
    {
        dividerBaseScale = Vector3.one;
    }

    public void SetValue(int current, int target)
    {
        currentText.text = current.ToString();
        targetText.text = target.ToString();

        currentText.ForceMeshUpdate();
        targetText.ForceMeshUpdate();
        dividerText.ForceMeshUpdate();

        float width = Mathf.Max(currentText.preferredWidth, targetText.preferredWidth);
        float dividerWidth = dividerText.preferredWidth;

        if (dividerWidth <= 0f) return;

        Vector3 scale = dividerBaseScale;
        scale.x *= width / dividerWidth;
        dividerText.transform.localScale = scale;
    }

    public void SetOscarSeason(bool active)
    {
        Material material = active ? oscarSeasonMaterial : defaultMaterial;

        currentText.fontSharedMaterial = material;
        dividerText.fontSharedMaterial = material;
        targetText.fontSharedMaterial = material;
    }
}