using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class GenreChoiceUI : MonoBehaviour
{
    private class ChoiceSide
    {
        public Player Player;
        public VisualElement Frame;
        public VisualElement TopIcon;
        public VisualElement BottomIcon;

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

        BindChoose(side.TopIcon, side.TopAction, () => ChooseGenre(side, side.TopOption) );
        BindChoose(side.BottomIcon, side.BottomAction, () => ChooseGenre(side, side.BottomOption));

        side.Frame.style.display = DisplayStyle.Flex;
    }


    // ─────────────────────────────────────────────
    // Choose
    // ─────────────────────────────────────────────

    private void ChooseGenre(
        ChoiceSide side,
        GenreNode option)
    {
        if (option == null)
            return;

        UnbindSide(side);

        side.Frame.style.display = DisplayStyle.None;
        side.Player.ChooseGenre(option);
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