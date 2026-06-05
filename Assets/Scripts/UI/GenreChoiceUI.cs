
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class GenreChoiceUI : MonoBehaviour
{
    private GameInputActions inputActions;
    private readonly Dictionary<InputAction, Action<InputAction.CallbackContext>> actionCallbacks = new();
    private readonly Dictionary<InputAction, EventCallback<ClickEvent>> clickCallbacks = new();

    private VisualElement leftGenreChoiceFrame;
    private VisualElement rightGenreChoiceFrame;

    private VisualElement leftTopGenreIcon;
    private VisualElement leftBottomGenreIcon;
    private VisualElement rightTopGenreIcon;
    private VisualElement rightBottomGenreIcon;

    private Player leftPlayer;
    private Player rightPlayer;

    private GenreNode leftTopOption;
    private GenreNode leftBottomOption;
    private GenreNode rightTopOption;
    private GenreNode rightBottomOption;

    private void Awake()
    {
        inputActions = new GameInputActions();
    }

    private void OnEnable()
    {
        GetUIComponents();

        leftPlayer = GameManager.Instance.LeftBase.Player;
        rightPlayer = GameManager.Instance.RightBase.Player;

        inputActions.Gameplay.Enable();

        leftPlayer.OnGenreChoiceAvailable += ShowLeftGenreChoice;
        rightPlayer.OnGenreChoiceAvailable += ShowRightGenreChoice;

        BindChoose(leftTopGenreIcon, inputActions.Gameplay.ChooseLeftTop, ChooseLeftTop);
        BindChoose(leftBottomGenreIcon, inputActions.Gameplay.ChooseLeftBottom, ChooseLeftBottom);
        BindChoose(rightTopGenreIcon, inputActions.Gameplay.ChooseRightTop, ChooseRightTop);
        BindChoose(rightBottomGenreIcon, inputActions.Gameplay.ChooseRightBottom, ChooseRightBottom);
    }

    private void OnDisable()
    {
        UnbindChoose(leftTopGenreIcon, inputActions.Gameplay.ChooseLeftTop);
        UnbindChoose(leftBottomGenreIcon, inputActions.Gameplay.ChooseLeftBottom);
        UnbindChoose(rightTopGenreIcon, inputActions.Gameplay.ChooseRightTop);
        UnbindChoose(rightBottomGenreIcon, inputActions.Gameplay.ChooseRightBottom);
        
        leftPlayer.OnGenreChoiceAvailable -= ShowLeftGenreChoice;
        rightPlayer.OnGenreChoiceAvailable -= ShowRightGenreChoice;
        
        inputActions.Gameplay.Disable();
    }

    private void BindChoose(VisualElement icon, InputAction action, Action spawnAction)
    {

        Action<InputAction.CallbackContext> inputCallback = _ => spawnAction();
        EventCallback<ClickEvent> clickCallback = _ => spawnAction();

        clickCallbacks[action] = clickCallback;
        icon.RegisterCallback<ClickEvent>(clickCallback);

        actionCallbacks[action] = inputCallback;
        action.performed += inputCallback;
    }

    private void UnbindChoose(VisualElement icon, InputAction action)
    {
        icon.UnregisterCallback<ClickEvent>(clickCallbacks[action]);

        if (actionCallbacks.TryGetValue(action, out var inputCallback))
        {
            action.performed -= inputCallback;
            actionCallbacks.Remove(action);
        }
    }

    private void GetUIComponents()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        leftGenreChoiceFrame = root.Q<VisualElement>("LeftGenreChoiceFrame");
        rightGenreChoiceFrame = root.Q<VisualElement>("RightGenreChoiceFrame");

        leftTopGenreIcon = leftGenreChoiceFrame.Q<VisualElement>("LeftTopGenreIcon");
        leftBottomGenreIcon = leftGenreChoiceFrame.Q<VisualElement>("LeftBottomGenreIcon");

        rightTopGenreIcon = rightGenreChoiceFrame.Q<VisualElement>("RightTopGenreIcon");
        rightBottomGenreIcon = rightGenreChoiceFrame.Q<VisualElement>("RightBottomGenreIcon");
    }

    private void ShowLeftGenreChoice(Player player)
    {
        leftTopOption = player.CurrentGenre.Children[0];
        leftBottomOption = player.CurrentGenre.Children[1];

        leftTopGenreIcon.style.backgroundImage = new StyleBackground(leftTopOption.Icon);
        leftBottomGenreIcon.style.backgroundImage = new StyleBackground(leftBottomOption.Icon);

        leftGenreChoiceFrame.style.display = DisplayStyle.Flex;
    }

    private void ShowRightGenreChoice(Player player)
    {
        rightTopOption = player.CurrentGenre.Children[0];
        rightBottomOption = player.CurrentGenre.Children[1];

        rightTopGenreIcon.style.backgroundImage = new StyleBackground(rightTopOption.Icon);
        rightBottomGenreIcon.style.backgroundImage = new StyleBackground(rightBottomOption.Icon);

        rightGenreChoiceFrame.style.display = DisplayStyle.Flex;
    }

    private void ChooseLeftTop()
    {
        if (leftTopOption == null) return;
        if (leftPlayer.TryChooseGenre(leftTopOption))
            leftGenreChoiceFrame.style.display = DisplayStyle.None;
    }

    private void ChooseLeftBottom()
    {
        if (leftBottomOption == null) return;
        if (leftPlayer.TryChooseGenre(leftBottomOption))
            leftGenreChoiceFrame.style.display = DisplayStyle.None;
    }

    private void ChooseRightTop()
    {
        if (rightTopOption == null) return;
        if (rightPlayer.TryChooseGenre(rightTopOption))
            rightGenreChoiceFrame.style.display = DisplayStyle.None;
    }

    private void ChooseRightBottom()
    {
        if (rightBottomOption == null) return;
        if (rightPlayer.TryChooseGenre(rightBottomOption))
            rightGenreChoiceFrame.style.display = DisplayStyle.None;
    }
}

