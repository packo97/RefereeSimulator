
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PosizionamentoMenu : MonoBehaviour
{
    [SerializeField] private GameObject _iconeInserite;

    [SerializeField] private GameObject actionButton;
    
    private static GameObject currentElementSelected;
    
    private void Update()
    {
            // disabilita/abilita seconda aggiunta arbitro
            foreach (RectTransform obj in GetComponentInChildren<RectTransform>())
            {
                if (obj.name.Contains("AggiungiArbitroButton"))
                {
                    if (GameEvent.isRefereeDropped)
                    {
                        obj.GetComponent<DragDrop>().enabled = false;
                        obj.GetComponent<CanvasGroup>().alpha = .6f;
                    }
                    else
                    {
                        obj.GetComponent<DragDrop>().enabled = true;
                        obj.GetComponent<CanvasGroup>().alpha = 1f;
                    }
                }
            }

            if (currentElementSelected != null)
            {
                if (currentElementSelected.tag.Equals("PlayerA") || currentElementSelected.tag.Equals("PlayerB"))
                    actionButton.SetActive(true);
                else
                    actionButton.SetActive(false);
            }
            else
                actionButton.SetActive(false);
            
            
    }

    public void SwitchComandiPosizionamento()
   {
      
         bool state = gameObject.activeSelf;
         gameObject.SetActive(!state);
      
   }

   public void DeleteAllIconsInserted()
   {
       foreach (RectTransform obj in _iconeInserite.GetComponentsInChildren<RectTransform>())
       {
           if (!obj.gameObject.name.Equals("IconeInserite"))
            Destroy(obj.gameObject);
       }
   }


   public static void SetCurrentElementSelected(GameObject selected)
   {
       currentElementSelected = selected;
   }

   public static GameObject GetCurrentElementSelected()
   {
       return currentElementSelected;
   }

   public void SetActionsOfSelectedPlayer()
   {
       currentElementSelected.GetComponentInChildren<Camera>(true).gameObject.SetActive(true);
       GameEvent.isActionOpen = true;
   }

   public static List<GameObject> GetAllElementsRecordable()
   {
       GameObject elementiInseriti = GameObject.Find("ElementiInseriti");
       List<GameObject> list = new List<GameObject>();
       for (int i = 0; i < elementiInseriti.transform.childCount; ++i)
       {
           if (elementiInseriti.transform.GetChild(i).tag.Equals("PlayerA") ||
               elementiInseriti.transform.GetChild(i).tag.Equals("PlayerB"))
           {
               list.Add(elementiInseriti.transform.GetChild(i).gameObject);
           }
       }

       return list;
   } 
   
}