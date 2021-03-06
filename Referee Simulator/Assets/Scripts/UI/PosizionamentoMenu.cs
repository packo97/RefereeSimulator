
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class PosizionamentoMenu : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject _iconeInserite;
    private static GameObject currentElementSelected;
    [SerializeField] private GameObject actionMenu;
    
    
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
            else if (obj.name.Contains("AggiungiGiocatoreSquadraAButton"))
            {
                if (GameEvent.MaxNumberOfPlayerA)
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
            else if (obj.name.Contains("AggiungiGiocatoreSquadraBButton"))
            {
                if (GameEvent.MaxNumberOfPlayerB)
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
            else if (obj.name.Contains("AggiungiPallone"))
            {
                if (GameEvent.MaxNumberOfBall)
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
    }

    public void SwitchComandiPosizionamento()
    {
        bool state = gameObject.activeSelf;
        gameObject.SetActive(!state);
        actionMenu.gameObject.SetActive(false);
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
    
    public static ref GameObject GetCurrentElementSelected()
    {
       return ref currentElementSelected;
    }
    
   public static List<GameObject> GetAllElementsRecordable()
   {
       bool ballFound = false;
       GameObject elementiInseriti = GameObject.Find("ElementiInseriti");
       List<GameObject> list = new List<GameObject>();
       for (int i = 0; i < elementiInseriti.transform.childCount; ++i)
       {
           if (elementiInseriti.transform.GetChild(i).tag.Equals("PlayerA") ||
               elementiInseriti.transform.GetChild(i).tag.Equals("PlayerB") ||
               elementiInseriti.transform.GetChild(i).tag.Equals("Ball"))
           {
               list.Add(elementiInseriti.transform.GetChild(i).gameObject);
           }

           if (elementiInseriti.transform.GetChild(i).tag.Equals("Ball"))
               ballFound = true;
       }

       if (!ballFound)
       {
           Ball ball = elementiInseriti.GetComponentInChildren<Ball>();
           if (ball != null)
               list.Add(ball.gameObject);
       }

       return list;
   }


   public void OnPointerClick(PointerEventData eventData)
   {
       if (eventData.pointerCurrentRaycast.gameObject == gameObject)
           actionMenu.gameObject.SetActive(false);
   }
}