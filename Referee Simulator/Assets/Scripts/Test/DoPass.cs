using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoPass : MonoBehaviour
{
    [SerializeField] private GameObject ball;

    [SerializeField] private Transform target;

    private bool ballMovement = false;
    // Start is called before the first frame update
    void Start()
    {
        
        enabled = false;
        StartCoroutine(EnableScript());
    }

    IEnumerator EnableScript()
    {
        yield return new WaitForSeconds(5);
        enabled = true;
        GetComponent<Animator>().SetBool("pass", true);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (GetComponent<AnimatorController>().GetComponent<Animator>().GetBool("pass"))
        {
            ballMovement = true;
        }

        
        
        if (ballMovement)
        {
            float step = 15 * Time.deltaTime;
            ball.transform.position = Vector3.MoveTowards(ball.transform.position, target.position, step);
        }
    }
    
}
