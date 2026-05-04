using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class SpawnerButton : MonoBehaviour
{
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

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        leftButton1 = root.Q<Button>("LeftButton1");
        leftButton2 = root.Q<Button>("LeftButton2");
        leftButton3 = root.Q<Button>("LeftButton3");

        rightButton1 = root.Q<Button>("RightButton1");
        rightButton2 = root.Q<Button>("RightButton2");
        rightButton3 = root.Q<Button>("RightButton3");

        // Subscribe to events
        leftButton1.clicked += OnLeftButton1Clicked;
        leftButton2.clicked += OnLeftButton2Clicked;
        leftButton3.clicked += OnLeftButton3Clicked;

        rightButton1.clicked += OnRightButton1Clicked;
        rightButton2.clicked += OnRightButton2Clicked;
        rightButton3.clicked += OnRightButton3Clicked;

        UnitType basicMeleeType = Resources.Load<UnitType>("UnitTypes/BasicMelee");
        UnitType basicRangedType = Resources.Load<UnitType>("UnitTypes/BasicRanged");
        UnitType basicHeavyType = Resources.Load<UnitType>("UnitTypes/BasicHeavy");

        leftMeleeType = basicMeleeType;
        leftRangedType = basicRangedType;
        leftHeavyType = basicHeavyType;
        rightMeleeType = basicMeleeType;
        rightRangedType = basicRangedType;
        rightHeavyType = basicHeavyType;
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        leftButton1.clicked -= OnLeftButton1Clicked;
        leftButton2.clicked -= OnLeftButton2Clicked;
        leftButton3.clicked -= OnLeftButton3Clicked;

        rightButton1.clicked -= OnRightButton1Clicked;
        rightButton2.clicked -= OnRightButton2Clicked;
        rightButton3.clicked -= OnRightButton3Clicked;
    }

    // ==== CALLBACKS ====
    private void OnLeftButton1Clicked() => SpawnLeft(leftMeleeType);
    private void OnLeftButton2Clicked() => SpawnLeft(leftRangedType);
    private void OnLeftButton3Clicked() => SpawnLeft(leftHeavyType);

    private void OnRightButton1Clicked() => SpawnRight(rightMeleeType);
    private void OnRightButton2Clicked() => SpawnRight(rightRangedType);
    private void OnRightButton3Clicked() => SpawnRight(rightHeavyType);

    private void SpawnLeft(UnitType unitType)
    {
        GameManager.Instance.LeftBase.SpawnUnit(unitType);
    }

    private void SpawnRight(UnitType unitType)
    {
        GameManager.Instance.RightBase.SpawnUnit(unitType);
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.digit1Key.wasPressedThisFrame)
            OnLeftButton1Clicked();

        if (keyboard.digit2Key.wasPressedThisFrame)
            OnLeftButton2Clicked();

        if (keyboard.digit3Key.wasPressedThisFrame)
            OnLeftButton3Clicked();

        if (keyboard.leftArrowKey.wasPressedThisFrame)
            OnRightButton1Clicked();

        if (keyboard.downArrowKey.wasPressedThisFrame)
            OnRightButton2Clicked();

        if (keyboard.rightArrowKey.wasPressedThisFrame)
            OnRightButton3Clicked();
    }
}
