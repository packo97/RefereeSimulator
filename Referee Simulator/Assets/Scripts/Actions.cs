using System;
using System.Collections;
using UnityEngine;

public class Actions : MonoBehaviour
{
    private AnimatorController _animatorController;
    private bool _ballCatched;

    private void Start()
    {
        _animatorController = GetComponent<AnimatorController>();
        _ballCatched = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !_ballCatched)
        {
            //Esegui tackle
            _animatorController.SetTrigger("tackle");
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            //Esegui simulazione
            _animatorController.SetTrigger("afterTackle");
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            //Prendi il pallone se è vicino
            //Libera il pallone se è in possesso
            
            if (!_ballCatched)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, 1);
                foreach (Collider c in colliders)
                {
                    if (c.tag.Equals("Ball"))
                    {
                        Debug.Log("pallone preso");
                        _ballCatched = true;
                        GameObject ball = c.gameObject;
                        c.transform.SetParent(gameObject.transform);
                        gameObject.GetComponent<FirstPersonController>().SetBall(ball.GetComponent<Ball>());
                    }
                } 
            }
            else
            {
                GameObject ball = GetComponentInChildren<Ball>().gameObject;
                ball.transform.SetParent(GameObject.Find("ElementiInseriti").transform);
                gameObject.GetComponent<FirstPersonController>().SetBall(null);
                _ballCatched = false;
            }
        }
        else if (Input.GetMouseButtonDown(0) && _ballCatched)
        {
            Debug.Log("passa il pallone a...");
            Debug.Log("disegna raycast per trovare un giocatore");
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit))
            {
                Debug.Log("Passa il pallone a " + hit.collider.transform.parent.name);
                GameObject ball = GetComponentInChildren<Ball>().gameObject;
                
                GameObject target = hit.collider.transform.parent.gameObject;
                ball.GetComponent<Ball>().StartPassBallTo(target);
                ball.transform.SetParent(GameObject.Find("ElementiInseriti").transform);
                gameObject.GetComponent<FirstPersonController>().SetBall(null);
                _ballCatched = false;
            }
            else
            {
                Debug.Log("nessuno colpito");
            }
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