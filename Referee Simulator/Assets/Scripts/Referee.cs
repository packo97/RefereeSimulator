using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Referee : MonoBehaviour
{
    // This script is used to identify the referee


    public void SetRefereePosition(Vector3 position)
    {
        transform.position = position;
    }

    private void OnDestroy()
    {
        GameEvent.isRefereeDropped = false;
    }
}
