using UnityEngine;
using UnityEngine.UIElements;

public class ClapperboardHUD : MonoBehaviour
{
    private Label leftBoxOffice;
    private Label rightBoxOffice;

    private Player leftPlayer;
    private Player rightPlayer;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        leftBoxOffice = root.Q<Label>("LeftBoxOffice");
        rightBoxOffice = root.Q<Label>("RightBoxOffice");

        leftPlayer = GameManager.Instance.LeftBase.Player;
        rightPlayer = GameManager.Instance.RightBase.Player;

        // Subscribe to UI update events
        leftPlayer.BoxOffice.OnMoneyChanged += UpdateLeftBoxOffice;
        rightPlayer.BoxOffice.OnMoneyChanged += UpdateRightBoxOffice;

        // Initial UI update
        UpdateLeftBoxOffice(leftPlayer.BoxOffice.Money);
        UpdateRightBoxOffice(rightPlayer.BoxOffice.Money);
    }

    private void OnDisable()
    {
        // Unsubscribe from UI update events
        leftPlayer.BoxOffice.OnMoneyChanged -= UpdateLeftBoxOffice;
        rightPlayer.BoxOffice.OnMoneyChanged -= UpdateRightBoxOffice;
    }

    private void UpdateLeftBoxOffice(int value)
    {
        leftBoxOffice.text = $"{value:N0}M $";
    }
    private void UpdateRightBoxOffice(int value)
    {
        rightBoxOffice.text = $"{value:N0}M $";
    }
}