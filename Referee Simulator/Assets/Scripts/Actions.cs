using System;
using System.Collections;
using UnityEngine;

public class Actions : MonoBehaviour
{
    private AnimatorController _animatorController;

    private void Start()
    {
        _animatorController = GetComponent<AnimatorController>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _animatorController.SetTrigger("tackle");
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            _animatorController.SetTrigger("afterTackle");
        }
    }

    private bool _hasCollide = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Body") && !_hasCollide)
        {
            if (gameObject.GetComponent<AnimatorController>().GetState() == ActionsController.Azione.TACKLE)
            {
                other.gameObject.GetComponentInParent<AnimatorController>().SetTrigger("afterTackle");
                
                _hasCollide = true;
                StartCoroutine(ResetCollision());
            }
                
        }
    }

    private IEnumerator ResetCollision()
    {
        yield return new WaitForSeconds(1);
        _hasCollide = false;

    }
}
