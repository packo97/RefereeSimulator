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
       
       
       Debug.Log(gameObject.name + " " + azione);
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
       else if (azione == ActionsController.Azione.IDLE)
           _animator.SetBool("running", false);
       
       if (azione == ActionsController.Azione.TACKLE)
           _animator.SetTrigger("tackle");
       
       if (azione == ActionsController.Azione.FALLEN_AFTER_TACKLE)
           _animator.SetTrigger("afterTackle");
       
   }

   public ActionsController.Azione GetState()
   {
       return azione;
   }
}
