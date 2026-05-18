using UnityEngine;
using UnityEngine.UIElements;

public class ClapperboardHUD : MonoBehaviour
{
    private Label leftBoxOffice;
    private Label rightBoxOffice;

    private Label leftGenreName;
    private Label rightGenreName;

    private Player leftPlayer;
    private Player rightPlayer;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        Label leftPlayerName = root.Q<Label>("LeftPlayerName");
        Label rightPlayerName = root.Q<Label>("RightPlayerName");

        leftBoxOffice = root.Q<Label>("LeftBoxOffice");
        rightBoxOffice = root.Q<Label>("RightBoxOffice");
        leftGenreName = root.Q<Label>("LeftGenreName");
        rightGenreName = root.Q<Label>("RightGenreName");

        leftPlayer = GameManager.Instance.LeftBase.Player;
        rightPlayer = GameManager.Instance.RightBase.Player;

        // Subscribe to UI update events
        leftPlayer.BoxOffice.OnMoneyChanged += UpdateLeftBoxOffice;
        rightPlayer.BoxOffice.OnMoneyChanged += UpdateRightBoxOffice;
        leftPlayer.OnGenreChanged += UpdateLeftGenreName;
        rightPlayer.OnGenreChanged += UpdateRightGenreName;

        // Initial UI update
        leftPlayerName.text = GameManager.Instance.LeftBase.OwnerId;
        rightPlayerName.text = GameManager.Instance.RightBase.OwnerId;

        UpdateLeftBoxOffice(leftPlayer.BoxOffice.Money);
        UpdateRightBoxOffice(rightPlayer.BoxOffice.Money);
        UpdateLeftGenreName(leftPlayer.CurrentGenre);
        UpdateRightGenreName(rightPlayer.CurrentGenre);
    }

    private void OnDisable()
    {
        // Unsubscribe from UI update events
        leftPlayer.BoxOffice.OnMoneyChanged -= UpdateLeftBoxOffice;
        rightPlayer.BoxOffice.OnMoneyChanged -= UpdateRightBoxOffice;
        leftPlayer.OnGenreChanged -= UpdateLeftGenreName;
        rightPlayer.OnGenreChanged -= UpdateRightGenreName;
    }

    private void UpdateLeftBoxOffice(int value) => leftBoxOffice.text = $"{value:N0}M $";
    private void UpdateRightBoxOffice(int value) => rightBoxOffice.text = $"{value:N0}M $";

    private void UpdateLeftGenreName(GenreNode genre) => leftGenreName.text = genre.GenreName;
    private void UpdateRightGenreName(GenreNode genre) => rightGenreName.text = genre.GenreName;
}