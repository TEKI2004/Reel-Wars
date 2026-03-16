using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum UnitDirection
{
    Left,
    Right
}

public class UnitScript : MonoBehaviour
{
    public UnitDirection direction = UnitDirection.Right;

    public int speed = 3;
    private Vector3 DirectionVector 
    {
        get
        {
            return direction == UnitDirection.Right ? Vector3.right : Vector3.left;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Move the unit to the right at a constant speed
        transform.Translate(speed * Time.deltaTime * DirectionVector);
    }
}
