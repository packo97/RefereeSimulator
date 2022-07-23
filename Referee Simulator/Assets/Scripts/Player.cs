using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //This script is used to identify the Player

    public int id = 1;
    private float iconX;
    private float iconY;
    private float iconZ;
    private bool isGoalKeeper;
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

    public void SetGoalKeeper(bool value)
    {
        if (value)
        {
            GetComponent<Animator>().runtimeAnimatorController = Resources.Load("Animation/GoalKeeperAnimator") as RuntimeAnimatorController;
            GetComponentInChildren<BoxCollider>().size = new Vector3(1f, 2, 1f);
        }
        else
        {
            GetComponent<Animator>().runtimeAnimatorController = Resources.Load("Animation/PlayerAnimator") as RuntimeAnimatorController;
            GetComponentInChildren<BoxCollider>().size = new Vector3(0.5f, 1.75f, 0.5f);
        }
           
        isGoalKeeper = value;
    }

    public bool GetGoalKeeper()
    {
        return isGoalKeeper;
    }

    public GameObject GetShirt()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name.Contains("Shirt"))
                return transform.GetChild(i).gameObject;
        }

        return null;
    }
    
    public GameObject GetShorts()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name.Contains("Shorts"))
                return transform.GetChild(i).gameObject;
        }

        return null;
    }
    
    public GameObject GetSocks()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name.Contains("Socks"))
                return transform.GetChild(i).gameObject;
        }

        return null;
    }

    private void OnDestroy()
    {
        if (tag.Contains("A"))
            GameEvent.MaxNumberOfPlayerA = false;
        else if (tag.Contains("B"))
            GameEvent.MaxNumberOfPlayerB = false;
    }
}
