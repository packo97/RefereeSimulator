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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Body"))
        {
            if (gameObject.GetComponent<AnimatorController>().GetState() == ActionsController.Azione.TACKLE)
            {
                other.gameObject.GetComponentInParent<AnimatorController>().SetTrigger("afterTackle");
            }
                
        }
        
        
    }
}
