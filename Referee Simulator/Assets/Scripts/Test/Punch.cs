using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : MonoBehaviour
{
    [SerializeField] public Player target;

    private SphereCollider _sphereCollider;
    // Start is called before the first frame update
    void Start()
    {
        _sphereCollider = GetComponent<SphereCollider>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name.Equals("da colpire"))
        {
            GetComponent<AnimatorController>().SetParameter("punch", true);
            other.GetComponent<AnimatorController>().SetParameter("fallAfterTackle", true);
        }
    }
}
