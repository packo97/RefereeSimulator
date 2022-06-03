using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //This script is used to identify the Player

    public int id;
    private float iconX;
    private float iconY;
    private float iconZ;

    public void SetPositionIcon(Vector3 transformPosition)
    {
        iconX = transformPosition.x;
        iconY = transformPosition.y;
        iconZ = transformPosition.z;
    }

    public Vector3 GetPositionIcon()
    {
        return new Vector3(iconX, iconY, iconZ);
    }
    
}
