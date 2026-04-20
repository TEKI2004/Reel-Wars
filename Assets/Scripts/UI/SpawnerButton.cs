using UnityEngine;
using UnityEngine.UIElements;

public class SpawnerButton : MonoBehaviour
{
    private UIDocument uiDocument;

    [SerializeField] private GameManager gameManager;

    private Button leftButton1;
    private Button leftButton2;
    private Button leftButton3;

    private Button rightButton1;
    private Button rightButton2;
    private Button rightButton3;

    [SerializeField] private UnitType leftUnitType1;
    [SerializeField] private UnitType leftUnitType2;
    [SerializeField] private UnitType leftUnitType3;

    [SerializeField] private UnitType rightUnitType1;
    [SerializeField] private UnitType rightUnitType2;
    [SerializeField] private UnitType rightUnitType3;

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

        leftUnitType1 = basicUnitType;
        leftUnitType2 = basicUnitType;
        leftUnitType3 = basicUnitType;
        rightUnitType1 = basicUnitType;
        rightUnitType2 = basicUnitType;
        rightUnitType3 = basicUnitType;
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
        Debug.Log("Bal 1 gomb megnyomva");
        gameManager.SpawnUnit(FacingDirection.Right, leftUnitType1);
    }

    private void OnLeftButton2Clicked()
    {
        Debug.Log("Bal 2 gomb megnyomva");
        gameManager.SpawnUnit(FacingDirection.Right, leftUnitType2);
    }

    private void OnLeftButton3Clicked()
    {
        Debug.Log("Bal 3 gomb megnyomva");
        gameManager.SpawnUnit(FacingDirection.Right, leftUnitType3);
    }

    private void OnRightButton1Clicked()
    {
        Debug.Log("Jobb 1 gomb megnyomva");
        gameManager.SpawnUnit(FacingDirection.Left, rightUnitType1);
    }

    private void OnRightButton2Clicked()
    {
        Debug.Log("Jobb 2 gomb megnyomva");
        gameManager.SpawnUnit(FacingDirection.Left, rightUnitType2);
    }

    private void OnRightButton3Clicked()
    {
        Debug.Log("Jobb 3 gomb megnyomva");
        gameManager.SpawnUnit(FacingDirection.Left, rightUnitType3);
    }
}
