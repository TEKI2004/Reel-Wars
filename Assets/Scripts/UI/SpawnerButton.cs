using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class SpawnerButton : MonoBehaviour
{
    private UIDocument uiDocument;

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
        // UIDocument lekérése ugyanarról az objektumról
        uiDocument = GetComponent<UIDocument>();

        // Root element
        VisualElement root = uiDocument.rootVisualElement;

        // Gombok lekérése név alapján
        leftButton1 = root.Q<Button>("LeftButton1");
        leftButton2 = root.Q<Button>("LeftButton2");
        leftButton3 = root.Q<Button>("LeftButton3");

        rightButton1 = root.Q<Button>("RightButton1");
        rightButton2 = root.Q<Button>("RightButton2");
        rightButton3 = root.Q<Button>("RightButton3");

        // Események hozzárendelése
        leftButton1.clicked += OnLeftButton1Clicked;
        leftButton2.clicked += OnLeftButton2Clicked;
        leftButton3.clicked += OnLeftButton3Clicked;

        rightButton1.clicked += OnRightButton1Clicked;
        rightButton2.clicked += OnRightButton2Clicked;
        rightButton3.clicked += OnRightButton3Clicked;

        UnitType basicUnitType = Resources.Load<UnitType>("UnitTypes/Basic");

        leftMeleeType = basicUnitType;
        leftRangedType = basicUnitType;
        leftHeavyType = basicUnitType;
        rightMeleeType = basicUnitType;
        rightRangedType = basicUnitType;
        rightHeavyType = basicUnitType;
    }

    private void OnDisable()
    {
        // Események levétele
        leftButton1.clicked -= OnLeftButton1Clicked;
        leftButton2.clicked -= OnLeftButton2Clicked;
        leftButton3.clicked -= OnLeftButton3Clicked;

        rightButton1.clicked -= OnRightButton1Clicked;
        rightButton2.clicked -= OnRightButton2Clicked;
        rightButton3.clicked -= OnRightButton3Clicked;
    }

    // ==== CALLBACK-ek ====
    private void OnLeftButton1Clicked()
    {
        GameManager.Instance.LeftBase.SpawnUnit(leftMeleeType);
        Debug.Log("Left Melee Spawned");
    }

    private void OnLeftButton2Clicked()
    {
        Debug.Log("Left Ranged Spawned");
        GameManager.Instance.LeftBase.SpawnUnit(leftRangedType);
    }

    private void OnLeftButton3Clicked()
    {
        Debug.Log("Left Heavy Spawned");
        GameManager.Instance.LeftBase.SpawnUnit(leftHeavyType);
    }

    private void OnRightButton1Clicked()
    {
        Debug.Log("Right Melee Spawned");
        GameManager.Instance.RightBase.SpawnUnit(rightMeleeType);
    }

    private void OnRightButton2Clicked()
    {
        Debug.Log("Right Ranged Spawned");
        GameManager.Instance.RightBase.SpawnUnit(rightRangedType);
    }

    private void OnRightButton3Clicked()
    {
        Debug.Log("Right Heavy Spawned");
        GameManager.Instance.RightBase.SpawnUnit(rightHeavyType);
    }
}
