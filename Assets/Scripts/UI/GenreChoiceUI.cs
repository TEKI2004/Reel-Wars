using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class GenreChoiceUI : MonoBehaviour
{
    [Title("Animation")]
    [MinValue(0.01f)][SuffixLabel("s", Overlay = true)][SerializeField] private float introGrowDuration = 0.36f;
    [MinValue(0.01f)][SuffixLabel("s", Overlay = true)][SerializeField] private float introSettleDuration = 0.1f;

    [MinValue(0.01f)][SuffixLabel("s", Overlay = true)][SerializeField] private float outroBounceDuration = 0.1f;
    [MinValue(0.01f)][SuffixLabel("s", Overlay = true)][SerializeField] private float outroShrinkDuration = 0.18f;

    [MinValue(0.01f)][SuffixLabel("s", Overlay = true)][SerializeField] private float flyDuration = 1f;
    private const float bounceScale = 1.1f;

    private class ChoiceSide
    {
        public Player Player;
        public VisualElement Frame;
        public VisualElement TopIcon;
        public VisualElement BottomIcon;

        public VisualElement HudGenreIcon;
        public Label HudGenreName;

        public InputAction TopAction;
        public InputAction BottomAction;

        public GenreNode TopOption;
        public GenreNode BottomOption;

        public Action ShowAction;
    }

    private GameInputActions inputActions;

    private readonly Dictionary<InputAction, Action<InputAction.CallbackContext>> actionCallbacks = new();

    private readonly Dictionary<InputAction, EventCallback<ClickEvent>> clickCallbacks = new();

    private ChoiceSide left;
    private ChoiceSide right;


    private void Awake()
    {
        inputActions = new GameInputActions();
    }

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        left = CreateSide(
            root,
            "Left",
            GameManager.Instance.LeftBase.Player,
            inputActions.Gameplay.ChooseLeftTop,
            inputActions.Gameplay.ChooseLeftBottom
        );

        right = CreateSide(
            root,
            "Right",
            GameManager.Instance.RightBase.Player,
            inputActions.Gameplay.ChooseRightTop,
            inputActions.Gameplay.ChooseRightBottom
        );

        inputActions.Gameplay.Enable();

        left.ShowAction = () => ShowGenreChoice(left);
        right.ShowAction = () => ShowGenreChoice(right);

        left.Player.OnGenreChoiceAvailable += left.ShowAction;
        right.Player.OnGenreChoiceAvailable += right.ShowAction;
    }

    private void OnDisable()
    {
        UnbindSide(left);
        UnbindSide(right);

        left.Player.OnGenreChoiceAvailable -= left.ShowAction;
        right.Player.OnGenreChoiceAvailable -= right.ShowAction;

        inputActions.Gameplay.Disable();
    }


    // ─────────────────────────────────────────────
    // Setup
    // ─────────────────────────────────────────────

    private static ChoiceSide CreateSide(
        VisualElement root, 
        string prefix, 
        Player player, 
        InputAction topAction, 
        InputAction bottomAction
    )
    {
        var frame = root.Q<VisualElement>($"{prefix}GenreChoiceFrame");

        return new ChoiceSide
        {
            Player = player,
            Frame = frame,

            TopIcon = frame.Q<VisualElement>($"{prefix}TopGenreIcon"),
            BottomIcon = frame.Q<VisualElement>($"{prefix}BottomGenreIcon"),

            HudGenreIcon = root.Q<VisualElement>($"{prefix}CurrentGenreIcon"),
            HudGenreName = root.Q<Label>($"{prefix}GenreName"),

            TopAction = topAction,
            BottomAction = bottomAction
        };
    }


    // ─────────────────────────────────────────────
    // Show
    // ─────────────────────────────────────────────

    private void ShowGenreChoice(ChoiceSide side)
    {
        side.TopOption = side.Player.CurrentGenre.Children[0];
        side.BottomOption = side.Player.CurrentGenre.Children[1];

        side.TopIcon.style.backgroundImage = new StyleBackground(side.TopOption.Icon);
        side.BottomIcon.style.backgroundImage = new StyleBackground(side.BottomOption.Icon);

        StartCoroutine(ShowGenreChoiceRoutine(side));
    }

    private IEnumerator ShowGenreChoiceRoutine(ChoiceSide side)
    {
        side.Frame.style.display = DisplayStyle.Flex;
        side.Frame.style.scale = new Scale(Vector2.zero);

        yield return AnimateScale(side.Frame, 0f, bounceScale, introGrowDuration);
        yield return AnimateScale(side.Frame, bounceScale, 1f, introSettleDuration);

        BindChoose(side.TopIcon, side.TopAction, () => ChooseGenre(side, side.TopOption, side.TopIcon));
        BindChoose(side.BottomIcon, side.BottomAction, () => ChooseGenre(side, side.BottomOption, side.BottomIcon));
    }

    // ─────────────────────────────────────────────
    // Choose
    // ─────────────────────────────────────────────

    private void ChooseGenre(ChoiceSide side, GenreNode option, VisualElement selectedIcon)
    {
        if (option == null) return;

        UnbindSide(side);
        StartCoroutine(ChooseGenreRoutine(side, option, selectedIcon));
    }

    private IEnumerator ChooseGenreRoutine(ChoiceSide side, GenreNode option, VisualElement selectedIcon)
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        side.HudGenreIcon.style.visibility = Visibility.Hidden;
        side.HudGenreName.style.visibility = Visibility.Hidden;

        VisualElement flyingIcon = CreateFlyingIcon(root, selectedIcon, option);

        selectedIcon.style.visibility = Visibility.Hidden;

        yield return AnimateScale(side.Frame, 1f, bounceScale, outroBounceDuration);
        yield return AnimateScale(side.Frame, bounceScale, 0f, outroShrinkDuration);

        side.Frame.style.display = DisplayStyle.None;
        side.Frame.style.scale = new Scale(Vector2.one);

        selectedIcon.style.visibility = Visibility.Visible;

        yield return AnimateToHud(root, flyingIcon, side.HudGenreIcon);

        side.Player.ChooseGenre(option);

        side.HudGenreIcon.style.visibility = Visibility.Visible;
        side.HudGenreName.style.visibility = Visibility.Visible;

        flyingIcon.RemoveFromHierarchy();
    }

    // ─────────────────────────────────────────────
    // Animation helpers
    // ─────────────────────────────────────────────

    private static VisualElement CreateFlyingIcon(VisualElement root, VisualElement source, GenreNode genre)
    {
        Vector2 position = source.ChangeCoordinatesTo(root, Vector2.zero);

        VisualElement icon = new()
        {
            name = "FlyingGenreIcon"
        };

        icon.AddToClassList("genre-flying-icon");

        icon.style.position = Position.Absolute;
        icon.style.left = position.x;
        icon.style.top = position.y;
        icon.style.width = source.resolvedStyle.width;
        icon.style.height = source.resolvedStyle.height;
        icon.style.backgroundImage = new StyleBackground(genre.Icon);

        root.Add(icon);
        icon.BringToFront();

        return icon;
    }

    private IEnumerator AnimateToHud(VisualElement root, VisualElement icon, VisualElement target)
    {
        Vector2 startPosition = new(icon.resolvedStyle.left, icon.resolvedStyle.top);

        Vector2 targetPosition = target.ChangeCoordinatesTo(root, Vector2.zero);

        float startWidth = icon.resolvedStyle.width;
        float startHeight = icon.resolvedStyle.height;

        float targetWidth = target.resolvedStyle.width;
        float targetHeight = target.resolvedStyle.height;

        float elapsed = 0f;

        while (elapsed < flyDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / flyDuration);
            float movementT = Mathf.SmoothStep(0f, 1f, t);

            Vector2 position = Vector2.Lerp(startPosition, targetPosition, movementT);

            icon.style.left = position.x;
            icon.style.top = position.y;

            icon.style.width = Mathf.Lerp(startWidth, targetWidth, movementT);
            icon.style.height = Mathf.Lerp(startHeight, targetHeight, movementT);

            yield return null;
        }

        icon.style.left = targetPosition.x;
        icon.style.top = targetPosition.y;
        icon.style.width = targetWidth;
        icon.style.height = targetHeight;
    }

    private static IEnumerator AnimateScale(VisualElement element, float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float value = Mathf.Lerp(from, to, Mathf.SmoothStep(0f, 1f, t));

            element.style.scale = new Scale(new Vector2(value, value));

            yield return null;
        }

        element.style.scale = new Scale(new Vector2(to, to));
    }

    // ─────────────────────────────────────────────
    // Binding
    // ─────────────────────────────────────────────

    private void BindChoose(
        VisualElement icon,
        InputAction action,
        Action chooseAction)
    {
        UnbindChoose(icon, action);

        Action<InputAction.CallbackContext> inputCallback = _ => chooseAction();

        EventCallback<ClickEvent> clickCallback = _ => chooseAction();

        actionCallbacks[action] = inputCallback;
        clickCallbacks[action] = clickCallback;

        action.performed += inputCallback;
        icon.RegisterCallback<ClickEvent>(clickCallback);
    }

    private void UnbindSide(ChoiceSide side)
    {
        UnbindChoose(side.TopIcon, side.TopAction);
        UnbindChoose(side.BottomIcon, side.BottomAction);
    }

    private void UnbindChoose(
        VisualElement icon,
        InputAction action)
    {
        if (clickCallbacks.Remove(action, out var clickCallback))
            icon.UnregisterCallback<ClickEvent>(clickCallback);

        if (actionCallbacks.Remove(action, out var inputCallback))
            action.performed -= inputCallback;
    }
}