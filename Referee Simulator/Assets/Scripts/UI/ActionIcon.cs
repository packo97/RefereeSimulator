using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionIcon : MonoBehaviour
{
   private int numero;
   private EditorMenu editorMenu;

   private void Start()
   {
      numero = Int32.Parse(gameObject.name.Substring(gameObject.name.Length - 1, 1));
      editorMenu = GetComponentInParent<EditorMenu>();
   }

   public void OpenAction()
   {
      ActionsController.codice = numero;
      editorMenu.SetActions(numero);
      
   }
}
