using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private bool _ballRotation;
    private bool _rotationStarted;
    private Vector3 _target;

    [SerializeField] private GameObject ballIndicatorPrefab;
    
    private void Start()
    {
        _ballRotation = false;
        _rotationStarted = false;
    }

    public void SetBallRotation(bool value)
    {
        _ballRotation = value;
    }
    
    
    public void StartBallRotation()
    {
        
        _ballRotation = true;
        if (!_rotationStarted)
            StartCoroutine(BallRotation());
        
    }

    private IEnumerator BallRotation()
    {
        _rotationStarted = true;
        while (_ballRotation && !GameEvent.stopAllCoroutines)
        {
            transform.Rotate(-5f, 0, 0);
            yield return null;
        }
        _rotationStarted = false;
    }
    
    public void StartPassBallTo(Vector3 target)
    {
        _target = target;
        StartCoroutine(PassBallTo(target));
    }
    
    private IEnumerator PassBallTo(Vector3 target)
    {
        //velocità pallone e target modificato per non farlo intersecare col terreno di gioco
        float speed = 12f;
        Vector3 modifiedTarget = new Vector3(target.x, 0.17f, target.z);
        //Instanzia un indicatore sul terreno di gioco che indica la posizione futura del pallone
        
        GameObject ballIndicator = Instantiate(ballIndicatorPrefab, GameObject.Find("ElementiInseriti").transform) as GameObject;
        ballIndicator.transform.position = modifiedTarget;
        
        // Move our position a step closer to the target.
        StartBallRotation();
        while(Vector3.Distance(transform.position, modifiedTarget) > 0.001f && !GameEvent.stopAllCoroutines)
        {
            //modifiedTarget = new Vector3(target.transform.position.x, 0.17f, target.transform.position.z);
            float step =  speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, modifiedTarget, step);
            yield return null;
        }
        //transform.SetParent(target.transform);
        //transform.position = transform.parent.TransformPoint(0, 0.17f, 1);
        _ballRotation = false;
        Destroy(ballIndicator);
        //transform.parent.GetComponent<FirstPersonController>().SetBall(this);
    }

    public Vector3 GetTarget()
    {
        //Debug.Log("get target che vado ad aggiungere" + _target);
        return _target;
    }

    private void OnDestroy()
    {
        GameEvent.MaxNumberOfBall = false;
    }
}
