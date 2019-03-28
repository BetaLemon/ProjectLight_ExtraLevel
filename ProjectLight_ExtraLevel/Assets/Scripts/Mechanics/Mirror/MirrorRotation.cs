using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorRotation : MonoBehaviour
{
    /// <summary>
    /// This script controls mirror rotation. It's very very delicate. Please don't touch the prefab much.
    /// It has two functions for rotation, one horizontal, and one vertical.
    /// </summary>

    public Transform MirrorCenter;
    public Transform VerticalCenter;
    public Transform Frame;
    public Transform[] BottomGear;
    public Transform[] SideGear;

    private Vector3 Rotation;
    private Vector3 prevRotation;
    private float[] verticalClamp = { -80, 80 };
    private float[] horizontalClamp = { -40, 40 };

    void Start()
    {
        verticalClamp[0] += MirrorCenter.localEulerAngles.y;
        verticalClamp[1] += MirrorCenter.localEulerAngles.y;

        horizontalClamp[0] += MirrorCenter.localEulerAngles.x;
        horizontalClamp[1] += MirrorCenter.localEulerAngles.x;

        Rotation = MirrorCenter.localEulerAngles;
    }

    public void RotateHorizontal(float angle)
    {
        RotateHorizontalAxis(MirrorCenter, angle);
        RotateHorizontalAxisGear(SideGear[0], -angle);
        RotateHorizontalAxis(SideGear[1], angle);
        prevRotation.x = Rotation.x;
    }

    public void RotateVertical(float angle)
    {
        RotateVerticalAxisAlt(VerticalCenter, angle);
        RotateVerticalAxis(Frame, angle);
        foreach (Transform t in BottomGear) { RotateVerticalAxis(t, angle); }
        foreach (Transform t in SideGear) { RotateAroundVerticalAxis(t, angle); }
        prevRotation.y = Rotation.y;  // This NEEDS to be after the SideGear Rotation.
    }

    #region InternFunctions
    void RotateVerticalAxisAlt(Transform obj, float angle)
    {
        float clamp = Clamp(Rotation.y + angle, verticalClamp[0], verticalClamp[1]);
        obj.localEulerAngles = new Vector3(obj.localEulerAngles.x, clamp, obj.localEulerAngles.z);
        //Debug.Log(clamp);
        Rotation.y = clamp;
    }

    void RotateVerticalAxis(Transform obj, float angle)
    {
        float clamp = Clamp(Rotation.y + angle, verticalClamp[0], verticalClamp[1]);
        obj.localEulerAngles = new Vector3(obj.localEulerAngles.x, obj.localEulerAngles.y, clamp);
        //Debug.Log(clamp);
        Rotation.y = clamp;
    }

    void RotateAroundVerticalAxis(Transform obj, float angle)
    {
        obj.RotateAround(MirrorCenter.position, Vector3.up, Rotation.y - prevRotation.y);
    }

    void RotateHorizontalAxis(Transform obj, float angle)
    {
        float clamp = Clamp(Rotation.x + angle, horizontalClamp[0], horizontalClamp[1]);
        obj.localEulerAngles = new Vector3(clamp, obj.localEulerAngles.y, obj.localEulerAngles.z);
        Rotation.x = clamp;
    }

    void RotateHorizontalAxisGear(Transform obj, float angle)
    {
        obj.localEulerAngles = new Vector3(obj.localEulerAngles.x, obj.localEulerAngles.y, -Rotation.x);
    }

    float Clamp(float value, float min, float max)
    {
        if(value > max) { value = max; }
        if(value < min) { value = min; }
        return value;
    }
    #endregion
}