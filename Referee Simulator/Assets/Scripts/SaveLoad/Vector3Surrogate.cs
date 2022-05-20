using UnityEngine;

[System.Serializable]
public class Vector3Surrogate
{
    public float x;
    public float y;
    public float z;

    public Vector3Surrogate(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3Surrogate(Vector3 vector3)
    {
        this.x = vector3.x;
        this.y = vector3.y;
        this.z = vector3.z;
    }

    public Vector3 GetVector3()
    {
        return new Vector3(x, y, z);
    }

 
    
    
}
