using System;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
   private Animator _animator;
   private ActionsController.Azione azione;
   
   private void Awake()
   {
       _animator = GetComponent<Animator>();
   }

   private void Update()
   {
       if (_animator.GetCurrentAnimatorStateInfo(0).IsName("idle"))
           azione = ActionsController.Azione.IDLE;
       else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("running"))
           azione = ActionsController.Azione.RUNNING;
       else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("tackle"))
           azione = ActionsController.Azione.TACKLE;
       else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("afterTackle"))
           azione = ActionsController.Azione.FALLEN_AFTER_TACKLE;
       else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("pass"))
           azione = ActionsController.Azione.PASS_BALL;
       else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("receive"))
           azione = ActionsController.Azione.RECEIVE_BALL;
       else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("sideLeft"))
           azione = ActionsController.Azione.SIDE_LEFT;
       else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("sideRight"))
           azione = ActionsController.Azione.SIDE_RIGHT;
       else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("bodyBlockLeft"))
           azione = ActionsController.Azione.BODY_BLOCK_LEFT;
       else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("bodyBlockRight"))
           azione = ActionsController.Azione.BODY_BLOCK_RIGHT;
       
       //Debug.Log(azione + " " + gameObject.name);
   }

   public void SetParameter(string parametro, bool valore)
   {
       _animator.SetBool(parametro, valore);
    }

   public void SetTrigger(string trigger)
   {
       _animator.SetTrigger(trigger);
       
   }

   public void SetParameter(ActionsController.Azione azione, bool valore)
   {
       if (azione == ActionsController.Azione.RUNNING) 
           _animator.SetBool("running", valore);
       if (azione == ActionsController.Azione.IDLE)
           _animator.SetTrigger("idle");
       
       if (azione == ActionsController.Azione.TACKLE)
           _animator.SetTrigger("tackle");
       
       if (azione == ActionsController.Azione.FALLEN_AFTER_TACKLE)
           _animator.SetTrigger("afterTackle");
       
       if (azione == ActionsController.Azione.PASS_BALL)
           _animator.SetTrigger("pass");
       
       if (azione == ActionsController.Azione.RECEIVE_BALL)
           _animator.SetTrigger("receive");
       
       
       if (azione == ActionsController.Azione.SIDE_LEFT)
           _animator.SetBool("sideLeft", valore);
       
       if (azione == ActionsController.Azione.SIDE_RIGHT)
           _animator.SetBool("sideRight", valore);
       
       if (azione == ActionsController.Azione.BODY_BLOCK_LEFT)
           _animator.SetBool("bodyBlockLeft", valore);
       
       if (azione == ActionsController.Azione.BODY_BLOCK_RIGHT)
           _animator.SetBool("bodyBlockRight", valore);
       
   }

   public ActionsController.Azione GetState()
   {
       return azione;
   }

   public void ResetToIdleAnimation()
   {
       _animator.SetBool("running", false);
       _animator.SetTrigger("idle");
       _animator.SetBool("sideLeft", false);
       _animator.SetBool("sideRight", false);
       
   }
   
}
