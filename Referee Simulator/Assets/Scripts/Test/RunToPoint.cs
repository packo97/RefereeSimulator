using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunToPoint : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private Vector3 finalPosition;

    [SerializeField] private bool tackle;
    [SerializeField] private Vector3 tacklePosition;
    private CharacterController _character;

    private AnimatorController _animatorController;
    public bool end;


    public bool goalkeeper;
    public Vector3 positionTuffo;
    
    private void Start()
    {
        _character = GetComponent<CharacterController>();
        _animatorController = GetComponent<AnimatorController>();
        enabled = false;
        StartCoroutine(EnableScript());

    }

    IEnumerator EnableScript()
    {
        yield return new WaitForSeconds(5);
        enabled = true;
    }
    
    void Update()
    {
        Vector3 offset = finalPosition - transform.position;
        Vector3 offsetTackle = tacklePosition - transform.position;
        
        if (offset.magnitude > 1f && !end)
        {
            _animatorController.SetParameter("running", true);
            transform.LookAt(finalPosition);
            offset = offset.normalized * moveSpeed;
            _character.Move(offset * Time.deltaTime);
            Debug.Log("muovi");
        }

        

    }

    
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.GetComponent<CharacterController>() && tackle)
        {
            Debug.Log("player colpito " + hit.gameObject.name + " da " + gameObject.name  );
            _animatorController.SetParameter("tackle", tackle);
            hit.gameObject.GetComponent<AnimatorController>().SetParameter("fallAfterTackle", true);
            end = true;
            hit.gameObject.GetComponent<RunToPoint>().end = true;
        }
        /*
        if (goalkeeper && hit.gameObject.GetComponent<CharacterController>())
        {
            _animatorController.SetParameter("left", true);
            end = true;
        }

        if (!goalkeeper && hit.gameObject.GetComponent<CharacterController>() && hit.gameObject
                .GetComponent<RunToPoint>()._animatorController.GetComponent<Animator>().GetBool("left"))
        {
            _animatorController.SetParameter("fallAfterTackle", true);
            end = true;
        }*/

        

        if (!goalkeeper && hit.gameObject.GetComponent<CharacterController>() && hit.gameObject.GetComponent<AnimatorController>().GetComponent<Animator>().GetBool("left"))
        {
            _animatorController.SetParameter("fallAfterTackle", true);
            end = true;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (goalkeeper && other.gameObject.GetComponent<Posizione>())
        {
            _animatorController.SetParameter("left", true);
            end = true;
        }

        if (other.GetComponent<BoxCollider>())
        {
            _animatorController.SetParameter("running", false);
            end = true;
        }
    }
}
