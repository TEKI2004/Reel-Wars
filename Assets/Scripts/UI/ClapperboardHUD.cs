using UnityEngine;
using UnityEngine.UIElements;

public class ClapperboardHUD : MonoBehaviour
{
    private class SideUI
    {
        private Player player;

        private readonly Label playerName;
        private readonly VisualElement genreIcon;
        private readonly Label genreName;
        private readonly Label boxOffice;

        private readonly UnitSlotUI melee;
        private readonly UnitSlotUI ranged;
        private readonly UnitSlotUI heavy;

        private readonly MoneySpentFeedback moneySpentFeedback;

        public SideUI(VisualElement root, string prefix, MoneySpentAnimationConfig moneySpentAnimation)
        {
            playerName = root.Q<Label>($"{prefix}PlayerName");
            genreIcon = root.Q<VisualElement>($"{prefix}CurrentGenreIcon");
            genreName = root.Q<Label>($"{prefix}GenreName");
            boxOffice = root.Q<Label>($"{prefix}BoxOffice");

            VisualElement moneySpentContainer = root.Q<VisualElement>($"{prefix}MoneySpentContainer");
            moneySpentFeedback = new MoneySpentFeedback(moneySpentContainer, moneySpentAnimation);

            melee = new UnitSlotUI(root, $"{prefix}Melee");
            ranged = new UnitSlotUI(root, $"{prefix}Ranged");
            heavy = new UnitSlotUI(root, $"{prefix}Heavy");
        }

        public void Bind(Player player, string ownerId)
        {
            this.player = player;

            player.BoxOffice.OnMoneyChanged += UpdateBoxOffice;
            player.BoxOffice.OnMoneySpent += moneySpentFeedback.Show;
            player.OnGenreChanged += UpdateGenre;

            playerName.text = ownerId;

            UpdateBoxOffice(player.BoxOffice.Money);
            UpdateGenre(player.CurrentGenre);
        }

        public void Unbind()
        {
            player.BoxOffice.OnMoneyChanged -= UpdateBoxOffice;
            player.BoxOffice.OnMoneySpent -= moneySpentFeedback.Show;
            player.OnGenreChanged -= UpdateGenre;

            moneySpentFeedback.Clear();
        }

        private void UpdateGenre(GenreNode genre)
        {
            genreName.text = genre.GenreName;
            genreIcon.style.backgroundImage = new StyleBackground(genre.Icon);

            melee.SetUnit(genre.MeleeUnit);
            ranged.SetUnit(genre.RangedUnit);
            heavy.SetUnit(genre.HeavyUnit);
        }

        private void UpdateBoxOffice(int value)
        {
            boxOffice.text = $"{value:N0}M $";
        }
    }

    private class UnitSlotUI
    {
        private readonly Label name;
        private readonly VisualElement icon;
        private readonly Label cost;

        public UnitSlotUI(VisualElement root, string prefix)
        {
            name = root.Q<Label>($"{prefix}Name");
            icon = root.Q<VisualElement>($"{prefix}Icon");
            cost = root.Q<Label>($"{prefix}Cost");
        }

        public void SetUnit(UnitType unitType)
        {
            name.text = unitType.UnitName;
            cost.text = $"{unitType.Cost:N0}M $";
            icon.style.backgroundImage = new StyleBackground(unitType.Icon);
        }
    }

    [SerializeField] private MoneySpentAnimationConfig moneySpentAnimation;

    private SideUI leftUI;
    private SideUI rightUI;

    private void OnEnable()
    {
        GetUIComponents();

        BaseBehaviour leftBase = GameManager.Instance.LeftBase;
        BaseBehaviour rightBase = GameManager.Instance.RightBase;

        leftUI.Bind(leftBase.Player, leftBase.OwnerId);
        rightUI.Bind(rightBase.Player, rightBase.OwnerId);
    }

    private void OnDisable()
    {
        leftUI.Unbind();
        rightUI.Unbind();
    }

    private void GetUIComponents()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        leftUI = new SideUI(root, "Left", moneySpentAnimation);
        rightUI = new SideUI(root, "Right", moneySpentAnimation);
    }
}