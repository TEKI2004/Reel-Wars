using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NeonPlayerName : MonoBehaviour
{
    [SerializeField] private TextMeshPro letterPrefab;
    [SerializeField] private int maxLetters = 7;
    [SerializeField] private float letterSpacing = 0.05f;

    [SerializeField] private Material neonOnMaterial;
    [SerializeField] private Material neonOffMaterial;

    private readonly List<TextMeshPro> letters = new();

    public void SetName(string playerName)
    {
        if (playerName.Length > maxLetters)
        {
            playerName = playerName.Substring(0, maxLetters);
            Debug.LogWarning($"Player name too long, truncating to {maxLetters} characters.");
        }

        playerName = playerName.Trim().ToUpper();

        foreach (var letter in letters)
        {
            Destroy(letter.gameObject);
        }
        letters.Clear();

        float startX = -((playerName.Length - 1) * letterSpacing) / 2f;

        for (int i = 0; i < playerName.Length; i++)
        {
            TextMeshPro letter = Instantiate(letterPrefab, transform);

            letter.text = playerName[i].ToString();
            letter.fontMaterial = neonOnMaterial;
            letter.transform.localPosition = new Vector3(startX + i * letterSpacing, 0, 0);

            letters.Add(letter);
        }
    }

    public void SetLitPercent(float percent)
    {
        int litCount = Mathf.CeilToInt(letters.Count * Mathf.Clamp01(percent));

        for (int i = 0; i < letters.Count; i++)
        {
            letters[i].fontMaterial = i < litCount
                ? neonOnMaterial
                : neonOffMaterial;
        }
    }
}