using System;
using System.Collections;
using System.Collections.Generic;
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
                        //Debug.Log("pallone preso");
                        _ballCatched = true;
                        GameObject ball = c.gameObject;
                        c.transform.SetParent(gameObject.transform);
                        gameObject.GetComponent<FirstPersonController>().SetBall(ball.GetComponent<Ball>());
                        GameObject.Find("Controller").GetComponent<ActionsController>().AddNewPossessionInRecordingForTheBall(ball);
                        _animatorController.SetTrigger("receive");
                    }
                } 
            }
            else
            {
                GameObject ball = GetComponentInChildren<Ball>().gameObject;
                ball.transform.SetParent(GameObject.Find("ElementiInseriti").transform);
                gameObject.GetComponent<FirstPersonController>().SetBall(null);
                _ballCatched = false;
                _animatorController.SetTrigger("receive");
            }
        }
        else if (Input.GetMouseButton(0) && _ballCatched)
        {
            //Debug.Log("passa il pallone a...");
            //Debug.Log("disegna raycast per trovare un giocatore");
            
            // è il momento di fare un passaggio quindì devo stoppare tutte le coroutine
            // per rendere semplice il passaggio

            ActionsController.pause = true;
            gameObject.GetComponent<FirstPersonController>().enabled = false;
            GameEvent.isBiliardinoOpen = true;
            gameObject.GetComponentInChildren<Camera>().enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            /*RaycastHit hit;
            if (Physics.BoxCast(transform.position, Vector3.one, transform.forward, out hit))
            {
                _animatorController.SetTrigger("pass");
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
            }*/
        }
        else if (Input.GetMouseButtonUp(0) && _ballCatched)
        {
            //passa il pallone nella direzione del player
            //riattiva il movimento
            //riattiva le coroutine in pausa
            //Debug.Log("passa il pallone nella direzione");

            RaycastHit hit;
            Ray ray = GameObject.Find("CameraBiliardino").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                //Debug.Log("passa il pallone nella posizione " + hit.point);
                _animatorController.SetTrigger("pass");
                //Debug.Log("Passa il pallone a " + hit.collider.transform.parent.name);
                GameObject ball = GetComponentInChildren<Ball>().gameObject;
                //GameObject target = hit.collider.transform.parent.gameObject;
                
                ball.GetComponent<Ball>().StartPassBallTo(hit.point);
                ball.transform.SetParent(GameObject.Find("ElementiInseriti").transform);
                gameObject.GetComponent<FirstPersonController>().SetBall(null);
                _ballCatched = false;
                GameObject.Find("Controller").GetComponent<ActionsController>().AddTargetInRecordingForTheBall(ball, gameObject.GetComponent<Player>().id, gameObject.GetComponent<Player>().tag);
            }

            gameObject.GetComponent<FirstPersonController>().enabled = true;
            ActionsController.pause = false;
            GameEvent.isBiliardinoOpen = false;
            gameObject.GetComponentInChildren<Camera>().enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

    }

    private bool _hasCollide = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Body") && !_hasCollide)
        {
            if (gameObject.GetComponent<AnimatorController>().GetState() == ActionsController.Azione.TACKLE && !ReferenceEquals(gameObject, other.transform.parent.gameObject))
            {
                //Debug.Log("collisione con " + other.transform.parent.name);
                other.gameObject.GetComponentInParent<AnimatorController>().SetTrigger("afterTackle");
                other.gameObject.GetComponentInParent<FirstPersonController>().isOnFoot = false;
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

    public void SetBallCatched(bool value)
    {
        this._ballCatched = value;
    }

    public bool GetBallCatched()
    {
        return _ballCatched;
    }
}