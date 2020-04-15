using UnityEngine;

public class UIButtonManager : MonoBehaviour
{
    //Public fields
    public FixedJoystick fixedJoystick;

    //Private fields
    private bool delta;
    private bool circle;

    //Custom methods
    public float Horizontal()
    {
        return fixedJoystick.Horizontal;
    }

    public float Vertical()
    {
        return fixedJoystick.Vertical;
    }

    public bool IsDeltaPressed()
    {
        return delta;
    }

    public bool IsCirclePressed()
    {
        return circle;
    }

    public void DeltaPointerDown()
    {
        delta = true;
    }

    public void DeltaPointerUp()
    {
        delta = false;
    }

    public void CirclePointerDown()
    {
        circle = true;
    }

    public void CirclePointerUp()
    {
        circle = false;
    }

}
