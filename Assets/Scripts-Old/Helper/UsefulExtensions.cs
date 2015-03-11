using System;
using UnityEngine;

public static class UsefulExtensions
{
    public static Vector3 SetX(this Vector3 vector3, float x)
    {
        return new Vector3(x, vector3.y, vector3.z);
    }

    public static Vector3 SetY(this Vector3 vector3, float y)
    {
        return new Vector3(vector3.x, y, vector3.z);
    }

    public static Vector3 SetZ(this Vector3 vector3, float z)
    {
        return new Vector3(vector3.x, vector3.y, z);
    }

    //Returns the sign of the first non-zero entry
    //If all entries are zero return zero
    public static int Sign(this Vector3 vector3)
    {
        for (int i = 0; i < 3; i++)
        {
            if (vector3[i] > 0)
            {
                return 1;
            }
            if (vector3[i] < 0)
            {
                return -1;
            }
        }
        return 0;
    }

    /// <summary>
    /// Takes a comma separated string of 3 values and attempts to parse it into a vector3
    /// </summary>
    public static Vector3 ParseToVector3(this string vectorString)
    {
        //Kill any any whitespace or parenthesis
        vectorString = vectorString.Trim(new[] { '(', ')' });
        //Split the string on commas
        var vectorValues = vectorString.Split(',');
        //Stick the values in a vector3
        return new Vector3(
            float.Parse(vectorValues[0]), 
            float.Parse(vectorValues[1]), 
            float.Parse(vectorValues[2]));
    }
}

public static class TransformExtension
{
    public static void SetX(this Transform transform, float x)
    {
        Vector3 newPosition = transform.position.SetX(x);
        transform.position = newPosition;
    }

    public static void SetY(this Transform transform, float y)
    {
        Vector3 newPosition = transform.position.SetY(y);
        transform.position = newPosition;
    }

    public static void SetZ(this Transform transform, float z)
    {
        Vector3 newPosition = transform.position.SetZ(z);
        transform.position = newPosition;
    }
}

public static class GameObjectExtension
{
    public static T GetSafeComponent<T>(this GameObject obj) where T : MonoBehaviour
    {
        T component = obj.GetComponent<T>();

        if (component == null)
        {
            Debug.LogError("Expected to find component of type "
                           + typeof(T) + " but found none", obj);
        }

        return component;
    }
}

public static class ColorExtension
{
    public static Color Invert(this Color c)
    {
        return new Color(255 - c.r, 255 - c.g, 255 - c.b, c.a);
    }

    public static Color SetAlpha(this Color c, float a)
    {
        return new Color(c.r, c.g, c.b, a);
    }
}
