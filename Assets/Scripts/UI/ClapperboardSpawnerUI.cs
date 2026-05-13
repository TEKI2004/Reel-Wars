using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ClapperboardSpawnerUI : MonoBehaviour
{
    private GameInputActions inputActions;
    private readonly Dictionary<InputAction, Action<InputAction.CallbackContext>> callbacks = new();

    private Button leftButton1;
    private Button leftButton2;
    private Button leftButton3;

    private Button rightButton1;
    private Button rightButton2;
    private Button rightButton3;

    [SerializeField] private UnitType leftMeleeType;
    [SerializeField] private UnitType leftRangedType;
    [SerializeField] private UnitType leftHeavyType;

    [SerializeField] private UnitType rightMeleeType;
    [SerializeField] private UnitType rightRangedType;
    [SerializeField] private UnitType rightHeavyType;

    private Player leftPlayer;
    private Player rightPlayer;

    private void Awake()
    {
        inputActions = new GameInputActions();
    }

    // ==== SETUP ====
    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        leftButton1 = root.Q<Button>("LeftButton1");
        leftButton2 = root.Q<Button>("LeftButton2");
        leftButton3 = root.Q<Button>("LeftButton3");

        rightButton1 = root.Q<Button>("RightButton1");
        rightButton2 = root.Q<Button>("RightButton2");
        rightButton3 = root.Q<Button>("RightButton3");


        inputActions.Gameplay.Enable();

        BindSpawn(leftButton1, inputActions.Gameplay.SpawnLeftMelee, SpawnLeftMelee);
        BindSpawn(leftButton2, inputActions.Gameplay.SpawnLeftRanged, SpawnLeftRanged);
        BindSpawn(leftButton3, inputActions.Gameplay.SpawnLeftHeavy, SpawnLeftHeavy);

        BindSpawn(rightButton1, inputActions.Gameplay.SpawnRightMelee, SpawnRightMelee);
        BindSpawn(rightButton2, inputActions.Gameplay.SpawnRightRanged, SpawnRightRanged);
        BindSpawn(rightButton3, inputActions.Gameplay.SpawnRightHeavy, SpawnRightHeavy);

        leftPlayer = GameManager.Instance.LeftBase.Player;
        rightPlayer = GameManager.Instance.RightBase.Player;

        ApplyStartingGenres();
    }

    private void OnDisable()
    {
        UnbindSpawn(leftButton1, inputActions.Gameplay.SpawnLeftMelee, SpawnLeftMelee);
        UnbindSpawn(leftButton2, inputActions.Gameplay.SpawnLeftRanged, SpawnLeftRanged);
        UnbindSpawn(leftButton3, inputActions.Gameplay.SpawnLeftHeavy, SpawnLeftHeavy);

        UnbindSpawn(rightButton1, inputActions.Gameplay.SpawnRightMelee, SpawnRightMelee);
        UnbindSpawn(rightButton2, inputActions.Gameplay.SpawnRightRanged, SpawnRightRanged);
        UnbindSpawn(rightButton3, inputActions.Gameplay.SpawnRightHeavy, SpawnRightHeavy);

        inputActions.Gameplay.Disable();
    }

    private void BindSpawn(Button button, InputAction action, Action spawnAction)
    {
        button.clicked += spawnAction;

        Action<InputAction.CallbackContext> callback = _ => spawnAction();

        callbacks[action] = callback;

        action.performed += callback;
    }

    private void UnbindSpawn(Button button, InputAction action, Action spawnAction)
    {
        button.clicked -= spawnAction;

        if (callbacks.TryGetValue(action, out var callback))
        {
            action.performed -= callback;
            callbacks.Remove(action);
        }
    }

    // ==== CALLBACKS ====
    private void SpawnLeftMelee() => SpawnLeft(leftMeleeType);
    private void SpawnLeftRanged() => SpawnLeft(leftRangedType);
    private void SpawnLeftHeavy() => SpawnLeft(leftHeavyType);

    private void SpawnRightMelee() => SpawnRight(rightMeleeType);
    private void SpawnRightRanged() => SpawnRight(rightRangedType);
    private void SpawnRightHeavy() => SpawnRight(rightHeavyType);


    private void SpawnLeft(UnitType unitType)
    {
        if (unitType == null) return;
        GameManager.Instance.LeftBase.SpawnUnit(unitType);
    }

    private void SpawnRight(UnitType unitType)
    {
        if (unitType == null) return;
        GameManager.Instance.RightBase.SpawnUnit(unitType);
    }

    private void ApplyStartingGenres()
    {
        GenreNode basic = Resources.Load<GenreNode>("GenreNodes/Basic");
        if (basic == null)
        {
            Debug.LogError("No Starting Genre!");
            return;
        }
        leftPlayer.SetStartingGenre(basic);
        rightPlayer.SetStartingGenre(basic);
        ApplyLeftGenres();
        ApplyRightGenres();
    }    

    private void ApplyLeftGenres()
    {
        GenreNode genre = leftPlayer.CurrentGenre;

        leftMeleeType = genre.MeleeUnit;
        leftRangedType = genre.RangedUnit;
        leftHeavyType = genre.HeavyUnit;
    }

    private void ApplyRightGenres()
    {
        GenreNode genre = rightPlayer.CurrentGenre;

        rightMeleeType = genre.MeleeUnit;
        rightRangedType = genre.RangedUnit;
        rightHeavyType = genre.HeavyUnit;
    }

    private void OnDestroy()
    {
        inputActions?.Dispose();
    }
}
