using UnityEngine;

public static class TransfomExtension
{
    ///<summary>Converts worldsRotation into local space rotation relative to this transform</summary>
    public static Quaternion InverseTransformRotation(this Transform transform, Quaternion worldRotation)
    {
        return Quaternion.Inverse(transform.rotation) * worldRotation;
    }

    ///<summary>Converts localRotation from local space relative to this transform into absolute world rotation</summary>
    public static Quaternion TransformRotation(this Transform transform, Quaternion localRotation)
    {
        return transform.rotation * localRotation;
    }
}
