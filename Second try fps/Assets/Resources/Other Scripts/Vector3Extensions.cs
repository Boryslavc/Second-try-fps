using UnityEngine;

public static class Vector3Extensions
{
    public static float HorizontalDistance(this Vector3 from, Vector3 to)
    {
        float x = Mathf.Pow(to.x - from.x, 2);
        float z = Mathf.Pow(to.z - from.z, 2);
        float result = Mathf.Sqrt(x + z);
        return result;
    }

    public static float HorizontalSquaredDistanceTo(this Vector3 from, Vector3 to)
    {
        float x = Mathf.Pow(to.x - from.x, 2);
        float z = Mathf.Pow(to.z - from.z, 2);
       
        return x + z;
    }
    public static Vector3 HoriontalDirection(this Vector3 from, Vector3 to)
    {
        float x = to.x - from.x;
        float z = to.z - from.z;

        return new Vector3(x, 0, z);
    }
}
