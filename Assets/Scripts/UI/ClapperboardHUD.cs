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
        private IVisualElementScheduledItem boxOfficePenaltyAnimation;

        private readonly UnitSlotUI melee;
        private readonly UnitSlotUI ranged;
        private readonly UnitSlotUI heavy;

        private readonly MoneySpentTag moneySpentTag;

        public SideUI(VisualElement root, string prefix)
        {
            playerName = root.Q<Label>($"{prefix}PlayerName");
            genreIcon = root.Q<VisualElement>($"{prefix}CurrentGenreIcon");
            genreName = root.Q<Label>($"{prefix}GenreName");
            boxOffice = root.Q<Label>($"{prefix}BoxOffice");

            VisualElement moneySpentContainer = root.Q<VisualElement>($"{prefix}MoneySpentContainer");
            moneySpentTag = new MoneySpentTag(moneySpentContainer);

            melee = new UnitSlotUI(root, $"{prefix}Melee");
            ranged = new UnitSlotUI(root, $"{prefix}Ranged");
            heavy = new UnitSlotUI(root, $"{prefix}Heavy");
        }

        public void Bind(Player player, string ownerId)
        {
            this.player = player;

            player.BoxOffice.OnMoneyChanged += UpdateBoxOffice;
            player.BoxOffice.OnMoneySpent += moneySpentTag.ShowCost;
            player.BoxOffice.OnMoneyPenalty += moneySpentTag.ShowPenalty;
            player.BoxOffice.OnMoneyPenalty += FlashBoxOfficePenalty;
            player.OnGenreChanged += UpdateGenre;
            player.OnUnitPurchaseFailed += FlashUnitPurchaseFailed;

            playerName.text = ownerId;

            UpdateBoxOffice(player.BoxOffice.Money);
            UpdateGenre(player.CurrentGenre);
        }

        public void Unbind()
        {
            player.BoxOffice.OnMoneyChanged -= UpdateBoxOffice;
            player.BoxOffice.OnMoneySpent -= moneySpentTag.ShowCost;
            player.BoxOffice.OnMoneyPenalty -= moneySpentTag.ShowPenalty;

            player.BoxOffice.OnMoneyPenalty -= FlashBoxOfficePenalty;
            boxOfficePenaltyAnimation?.Pause();
            boxOffice.RemoveFromClassList("penalty-flash");

            player.OnGenreChanged -= UpdateGenre;
            player.OnUnitPurchaseFailed -= FlashUnitPurchaseFailed;

            moneySpentTag.Clear();
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

        private void FlashUnitPurchaseFailed(UnitType unitType)
        {
            GetUnitSlot(unitType.Role)?.FlashInsufficientFunds();
        }
        private UnitSlotUI GetUnitSlot(UnitRole role)
        {
            return role switch
            {
                UnitRole.Melee => melee,
                UnitRole.Ranged => ranged,
                UnitRole.Heavy => heavy,
                _ => null
            };
        }
        private void FlashBoxOfficePenalty(int _)
        {
            boxOfficePenaltyAnimation = ClapperboardHUD.FlashElements(boxOfficePenaltyAnimation, "negative-feedback", boxOffice);
        }
    }

    private static IVisualElementScheduledItem FlashElements(IVisualElementScheduledItem currentAnimation, string className, params VisualElement[] elements)
    {
        currentAnimation?.Pause();

        foreach (VisualElement element in elements)
            element.AddToClassList(className);

        bool active = true;
        int toggles = 0;

        IVisualElementScheduledItem animation = null;

        animation = elements[0].schedule.Execute(() =>
        {
            active = !active;

            foreach (VisualElement element in elements)
            {
                if (active)
                    element.AddToClassList(className);
                else
                    element.RemoveFromClassList(className);
            }

            toggles++;

            if (toggles >= 4)
            {
                foreach (VisualElement element in elements)
                    element.RemoveFromClassList(className);

                animation.Pause();
            }
        }).StartingIn(120).Every(120);

        return animation;
    }

    private class UnitSlotUI
    {
        private readonly Label name;
        private readonly VisualElement icon;
        private readonly Label cost;

        private IVisualElementScheduledItem insufficientFundsAnimation;

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

        public void FlashInsufficientFunds()
        {
            insufficientFundsAnimation = ClapperboardHUD.FlashElements(insufficientFundsAnimation, "negative-feedback", name, cost);
        }
    }

    private SideUI leftUI;
    private SideUI rightUI;

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        leftUI = new SideUI(root, "Left");
        rightUI = new SideUI(root, "Right");

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
}