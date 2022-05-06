using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionsMenu : MonoBehaviour
{
    [SerializeField] private Image elementImage;
    [SerializeField] private Text text;
    [SerializeField] private Rotation indicatore;
    private GameObject _currentObjSelected;
    
    [SerializeField] private GameObject listOfActions;

    [SerializeField] private GameObject iconActionPrefab;
    private int numberOfActionInserted;
    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetElement(GameObject obj)
    {
        _currentObjSelected = obj;
        gameObject.SetActive(true);
        indicatore.SetElement(obj);
        if (obj.tag.Equals("PlayerA"))
        {
            elementImage.sprite = Resources.Load<Sprite>("Icons/PlayerA") as Sprite;
        }
        else if (obj.tag.Equals("PlayerB"))
        {
            elementImage.sprite = Resources.Load<Sprite>("Icons/PlayerB") as Sprite;
        }
        else if (obj.tag.Equals("Referee"))
        {
            elementImage.sprite = Resources.Load<Sprite>("Icons/referee") as Sprite;
        }

        //cancello le icone delle azioni nel menu
        for (int i = 0; i < listOfActions.transform.childCount; i++)
        {
            Destroy(listOfActions.transform.GetChild(i).gameObject);
        }
        
        //carico le icone per le corrispondenti azioni già registrate
        int numberOfActionsRegistered = GameObject.Find("Controller").GetComponent<ActionsController>()
            .GetNumberOfActionRegistered(_currentObjSelected);
        Debug.Log("numero di azioni registrate " + numberOfActionsRegistered);
        for(int i=0; i<numberOfActionsRegistered; i++)
        {
            GameObject iconAction = Instantiate(iconActionPrefab, listOfActions.transform);
            iconAction.name = "action" + i;
        }



    }

    public void AddAction()
    {
        numberOfActionInserted = listOfActions.GetComponent<RectTransform>().childCount;
        text.text = "Aggiungi azione... numero di azioni " + numberOfActionInserted;
        
        bool isRecordedTheLastAction;
        if (numberOfActionInserted == 0)
            isRecordedTheLastAction = true;
        else
            isRecordedTheLastAction = GameObject.Find("Controller").GetComponent<ActionsController>().IsRecordedTheLastAction(_currentObjSelected, numberOfActionInserted);

            
        if (numberOfActionInserted < 16 && isRecordedTheLastAction)
        {
            GameObject iconAction = Instantiate(iconActionPrefab, listOfActions.transform);
            iconAction.name = "action" + numberOfActionInserted;
        }
        else
            text.text = "Numero massimo di azioni raggiunto";
    }
    
    public void OpenActionMode(int numero)
    {

        if (numero > 0)
        {
            Debug.Log("Recupera posizione finale della precedente azione");
            Vector3 initialPosition = GameObject.Find("Controller").GetComponent<ActionsController>()
                .GetInitialPositionOfTheNextAction(_currentObjSelected);
            _currentObjSelected.transform.position = initialPosition;
            Vector3 initialAngles = GameObject.Find("Controller").GetComponent<ActionsController>()
                .GetInitialAnglesOfTheNextAction(_currentObjSelected);
            _currentObjSelected.transform.eulerAngles = initialAngles;
            //Debug.Log("initial position richiesta " + initialPosition);
        }
            
       
        
        
        _currentObjSelected.GetComponentInChildren<Camera>(true).gameObject.SetActive(true);
        GameEvent.isActionOpen = true;
    }
    
    public void SetText(string testo)
    {
        text.text = testo;
    }
}
