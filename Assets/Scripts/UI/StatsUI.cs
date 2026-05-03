using UnityEngine;
using UnityEngine.UIElements;

public class StatsUI : MonoBehaviour
{
    private Label leftBoxOffice;
    private Label leftPopularity;
    private Label rightBoxOffice;
    private Label rightPopularity;

    private Player leftPlayer;
    private Player rightPlayer;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        leftBoxOffice = root.Q<Label>("LeftBoxOffice");
        leftPopularity = root.Q<Label>("LeftPopularity");
        rightBoxOffice = root.Q<Label>("RightBoxOffice");
        rightPopularity = root.Q<Label>("RightPopularity");

        leftPlayer = GameManager.Instance.LeftBase.Player;
        rightPlayer = GameManager.Instance.RightBase.Player;

        // Subscribe to UI update events
        leftPlayer.BoxOffice.OnMoneyChanged += UpdateLeftBoxOffice;
        leftPlayer.Popularity.OnPointsChanged += UpdateLeftPopularity;
        rightPlayer.BoxOffice.OnMoneyChanged += UpdateRightBoxOffice;
        rightPlayer.Popularity.OnPointsChanged += UpdateRightPopularity;

        // Initial UI update
        UpdateLeftBoxOffice(leftPlayer.BoxOffice.Money);
        UpdateLeftPopularity(leftPlayer.Popularity.CurrentPoints);
        UpdateRightBoxOffice(rightPlayer.BoxOffice.Money);
        UpdateRightPopularity(rightPlayer.Popularity.CurrentPoints);
    }

    private void OnDisable()
    {
        // Unsubscribe from UI update events
        leftPlayer.BoxOffice.OnMoneyChanged -= UpdateLeftBoxOffice;
        leftPlayer.Popularity.OnPointsChanged -= UpdateLeftPopularity;
        rightPlayer.BoxOffice.OnMoneyChanged -= UpdateRightBoxOffice;
        rightPlayer.Popularity.OnPointsChanged -= UpdateRightPopularity;
    }

    private void UpdateLeftBoxOffice(int value)
    {
        leftBoxOffice.text = $"BoxOffice: {value:N0} $";
    }
    private void UpdateLeftPopularity(int value)
    {
        leftPopularity.text = $"Popularity: {value:N0}";
    }

    private void UpdateRightBoxOffice(int value)
    {
        rightBoxOffice.text = $"BoxOffice: {value:N0} $";
    }

    private void UpdateRightPopularity(int value)
    {
        rightPopularity.text = $"Popularity: {value:N0}";
    }
}