using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookBall : MonoBehaviour
{
    [SerializeField] private Transform ball;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(ball);
    }
}
