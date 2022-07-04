using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionIcon : MonoBehaviour
{
   private int numero;
   private EditorMenu editorMenu;
   private Image _image;
   private RecordData rd;
   private void Start()
   {
      numero = Int32.Parse(gameObject.name.Substring(gameObject.name.Length - 1, 1));
      editorMenu = GetComponentInParent<EditorMenu>();
      _image = GetComponent<Image>();
      GameObject currentElementSelected = PosizionamentoMenu.GetCurrentElementSelected();
      
      rd = GameObject.Find("Controller").GetComponent<ActionsController>()
         .GetRecordData(ref currentElementSelected, numero);
      
      if (rd != null && !rd.valid)
         _image.color = new Color32(224,81,81,100);
      else
         _image.color = new Color32(63,200,11,100);
    
         
   }

   public void OpenAction()
   {
      if (rd == null || rd.valid)
      {
         ActionsController.layer = numero;
         editorMenu.SetActions(numero);
      }

   }

   public void DeleteInvalidAction()
   {
      if (rd != null && !rd.valid)
      {
         GameObject.Find("Controller").GetComponent<ActionsController>().RemoveRecording(rd);
         Destroy(gameObject);
      }
         
   }
   
   
}
