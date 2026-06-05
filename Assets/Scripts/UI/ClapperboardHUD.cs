using GLTFast.Schema;
using UnityEngine;
using UnityEngine.UIElements;



public class ClapperboardHUD : MonoBehaviour
{
    private class UnitSlotUI
    {
        public Label Name;
        public VisualElement Icon;
        public Label Cost;

        public UnitSlotUI(VisualElement root, string prefix)
        {
            Name = root.Q<Label>($"{prefix}Name");
            Icon = root.Q<VisualElement>($"{prefix}Icon");
            Cost = root.Q<Label>($"{prefix}Cost");
        }

        public void SetUnit(UnitType unitType)
        {
            Name.text = unitType.UnitName;
            Cost.text = $"{unitType.Cost}M $";
            Icon.style.backgroundImage = new StyleBackground(unitType.Icon);
        }
    }

    Label leftPlayerName;
    Label rightPlayerName;

    private VisualElement leftCurrentGenreIcon;
    private VisualElement rightCurrentGenreIcon;

    private Label leftGenreName;
    private Label rightGenreName;

    private Label leftBoxOffice;
    private Label rightBoxOffice;

    private UnitSlotUI leftMelee;
    private UnitSlotUI leftRanged;
    private UnitSlotUI leftHeavy;

    private UnitSlotUI rightMelee;
    private UnitSlotUI rightRanged;
    private UnitSlotUI rightHeavy;

    private Player leftPlayer;
    private Player rightPlayer;

    private void OnEnable()
    {
        GetUIComponents();

        leftPlayer = GameManager.Instance.LeftBase.Player;
        rightPlayer = GameManager.Instance.RightBase.Player;

        // Subscribe to UI update events
        leftPlayer.BoxOffice.OnMoneyChanged += UpdateLeftBoxOffice;
        rightPlayer.BoxOffice.OnMoneyChanged += UpdateRightBoxOffice;
        leftPlayer.OnGenreChanged += UpdateLeftGenre;
        rightPlayer.OnGenreChanged += UpdateRightGenre;

        // Initial UI update
        leftPlayerName.text = GameManager.Instance.LeftBase.OwnerId;
        rightPlayerName.text = GameManager.Instance.RightBase.OwnerId;

        UpdateLeftBoxOffice(leftPlayer.BoxOffice.Money);
        UpdateRightBoxOffice(rightPlayer.BoxOffice.Money);
        UpdateLeftGenre(leftPlayer.CurrentGenre);
        UpdateRightGenre(rightPlayer.CurrentGenre);
    }

    private void OnDisable()
    {
        // Unsubscribe from UI update events
        leftPlayer.BoxOffice.OnMoneyChanged -= UpdateLeftBoxOffice;
        rightPlayer.BoxOffice.OnMoneyChanged -= UpdateRightBoxOffice;
        leftPlayer.OnGenreChanged -= UpdateLeftGenre;
        rightPlayer.OnGenreChanged -= UpdateRightGenre;
    }

    private void GetUIComponents()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        leftPlayerName = root.Q<Label>("LeftPlayerName");
        rightPlayerName = root.Q<Label>("RightPlayerName");

        leftBoxOffice = root.Q<Label>("LeftBoxOffice");
        rightBoxOffice = root.Q<Label>("RightBoxOffice");

        leftCurrentGenreIcon = root.Q<VisualElement>("LeftCurrentGenreIcon");
        rightCurrentGenreIcon = root.Q<VisualElement>("RightCurrentGenreIcon");

        leftGenreName = root.Q<Label>("LeftGenreName");
        rightGenreName = root.Q<Label>("RightGenreName");

        leftMelee = new UnitSlotUI(root, "LeftMelee");
        leftRanged = new UnitSlotUI(root, "LeftRanged");
        leftHeavy = new UnitSlotUI(root, "LeftHeavy");

        rightMelee = new UnitSlotUI(root, "RightMelee");
        rightRanged = new UnitSlotUI(root, "RightRanged");
        rightHeavy = new UnitSlotUI(root, "RightHeavy");

    }

    private void UpdateLeftGenre(GenreNode genre)
    {
        leftGenreName.text = genre.GenreName;

        leftCurrentGenreIcon.style.backgroundImage = new StyleBackground(genre.Icon);

        leftMelee.SetUnit(genre.MeleeUnit);
        leftRanged.SetUnit(genre.RangedUnit);
        leftHeavy.SetUnit(genre.HeavyUnit);
    }

    private void UpdateRightGenre(GenreNode genre)
    {
        rightGenreName.text = genre.GenreName;

        rightCurrentGenreIcon.style.backgroundImage = new StyleBackground(genre.Icon);

        rightMelee.SetUnit(genre.MeleeUnit);
        rightRanged.SetUnit(genre.RangedUnit);
        rightHeavy.SetUnit(genre.HeavyUnit);
    }

    private void UpdateLeftBoxOffice(int value) => leftBoxOffice.text = $"{value:N0}M $";
    private void UpdateRightBoxOffice(int value) => rightBoxOffice.text = $"{value:N0}M $";

}