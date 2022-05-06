using System;
using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private bool _ballRotation;
    private bool _rotationStarted;
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
        while (_ballRotation)
        {
            transform.Rotate(-5f, 0, 0);
            yield return null;
        }

        _rotationStarted = false;
    }

    public void StartPassBallTo(GameObject target)
    {
        StartCoroutine(PassBallTo(target));
    }
    
    private IEnumerator PassBallTo(GameObject target)
    {
        // Move our position a step closer to the target.
        float speed = 6f;
        Vector3 modifiedTarget = new Vector3(target.transform.position.x, 0.17f, target.transform.position.z);
        StartBallRotation();
        while(Vector3.Distance(transform.position, modifiedTarget) > 0.001f)
        {
            //modifiedTarget = new Vector3(target.transform.position.x, 0.17f, target.transform.position.z);
            float step =  speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, modifiedTarget, step);
            yield return null;
        }
        transform.SetParent(target.transform);
        transform.position = transform.parent.TransformPoint(0, 0.17f, 1);
        _ballRotation = false;
        transform.parent.GetComponent<FirstPersonController>().SetBall(this);
    }
}
