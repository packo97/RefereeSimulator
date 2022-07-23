using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionsMenu : MonoBehaviour
{
    [SerializeField] private Image elementImage;
    [SerializeField] private Text playerNumber;
    [SerializeField] private Rotation indicatore;
    private GameObject _currentObjSelected;
    
    [SerializeField] private GameObject listOfActions;

    [SerializeField] private GameObject iconActionPrefab;
    private int numberOfActionInserted;
    [SerializeField] private Text text;
    [SerializeField] private Button actionButton;

    [SerializeField] private Image goalKeeperImage;
    
    private void Start()
    {
        gameObject.SetActive(false);
    }

    public GameObject GetCurrentObject()
    {
        return _currentObjSelected;
    }
    
    public void SetElement(GameObject obj)
    {
        /*
         * Imposto l'elemento corrente
         */
        if (obj.tag.Equals("Referee"))
        {
            actionButton.gameObject.SetActive(false);
            playerNumber.gameObject.SetActive(false);
            goalKeeperImage.gameObject.SetActive(false);
        }
        else
        {
            actionButton.gameObject.SetActive(true);
            playerNumber.gameObject.SetActive(true);
            goalKeeperImage.gameObject.SetActive(true);
        }
            
        
        _currentObjSelected = obj;
        gameObject.SetActive(true);
        indicatore.SetElement(obj);
        
        if (_currentObjSelected.GetComponent<Player>())
            if (_currentObjSelected.GetComponent<Player>().GetGoalKeeper())
                goalKeeperImage.color = new Color32(234,234,100,255);
            else
                goalKeeperImage.color = new Color32(255,255,255,255);
        
        if (obj.tag.Equals("PlayerA"))
        {
            if (obj.GetComponent<Player>().GetGoalKeeper())
                elementImage.sprite = Resources.Load<Sprite>("Icons/GoalKeeperA") as Sprite;
            else
                elementImage.sprite = Resources.Load<Sprite>("Icons/PlayerA") as Sprite;
            playerNumber.text = obj.GetComponent<Player>().id.ToString();
        }
        else if (obj.tag.Equals("PlayerB"))
        {
            if (obj.GetComponent<Player>().GetGoalKeeper())
                elementImage.sprite = Resources.Load<Sprite>("Icons/GoalKeeperB") as Sprite;
            else
                elementImage.sprite = Resources.Load<Sprite>("Icons/PlayerB") as Sprite;
            playerNumber.text = obj.GetComponent<Player>().id.ToString();
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
        ArrayList actionRegistered = GameObject.Find("Controller").GetComponent<ActionsController>()
            .GetActionsRegistered(_currentObjSelected);
        for(int i=0; i<actionRegistered.Count; i++)
        {
            GameObject iconAction = Instantiate(iconActionPrefab, listOfActions.transform);
            iconAction.name = "action" + i;
        }
        
    }

    public void AddAction()
    {
        /*
         * Aggiungo un azione, solo se le seguenti condizioni sono rispettate:
         *  - la precedente azione è stata effettivamente registrata
         *  - tutte le azioni dell'elemento sono valide
         *  - il numero di azioni dell'elemento è minore di 16
         * 
         */
        
        numberOfActionInserted = listOfActions.GetComponent<RectTransform>().childCount;
        
        bool isRecordedTheLastAction;
        if (numberOfActionInserted == 0)
            isRecordedTheLastAction = true;
        else
            isRecordedTheLastAction = GameObject.Find("Controller").GetComponent<ActionsController>().IsRecordedTheLastAction(_currentObjSelected, numberOfActionInserted);

        bool allTheActionsValid = GameObject.Find("Controller").GetComponent<ActionsController>()
            .AllTheActionsValid(_currentObjSelected);
        
        if (numberOfActionInserted < 16 && isRecordedTheLastAction && allTheActionsValid)
        {
            GameObject iconAction = Instantiate(iconActionPrefab, listOfActions.transform);
            iconAction.name = "action" + numberOfActionInserted;
        }
        else if (numberOfActionInserted >= 16)
            text.text = "Numero massimo di azioni raggiunto";
        else if (!allTheActionsValid)
            text.text = "Cancella prima le azioni invalide con un click su di esse";
    }
    
    public void OpenActionMode(int numero)
    {
        /*
         *  Apro la modalità in cui è possibile compiere azioni e registrarle.
         *  Prima di aprire però devo ripristinare la posizione e angolazione corretta di ogni elemento
         * 
         */
        
        //se sono nel caso in cui non è la prima azione (layer 0) devo preparare la scena con le corrette posizioni e angolazioni
        if (numero > 0)
        {
            GameObject.Find("Controller").GetComponent<PitchController>().SetAllElementsToInitialPositionOfTheLayer(numero);
            GameObject ball = GameObject.Find("Controller").GetComponent<PitchController>().GetBall();
            
            //imposto posizione del pallone ed eventualmente il possessore
            if (ball != null)
            {
                bool isBallCatched = GameObject.Find("Controller").GetComponent<ActionsController>()
                    .IsBallCatchedOnTheLastLayer(numero).Item1;
                if (isBallCatched)
                {
                    GameObject catcher = GameObject.Find("Controller").GetComponent<ActionsController>()
                        .IsBallCatchedOnTheLastLayer(numero).Item2;
                    ball.transform.SetParent(catcher.transform);
                    catcher.GetComponent<Actions>().SetBallCatched(true);
                }
                
                Vector3 initialPositionBall = GameObject.Find("Controller").GetComponent<ActionsController>()
                    .GetInitialPositionBallOfTheNextAction();
                ball.transform.position = initialPositionBall;
            }
        }
        
        //attivo la camera dell'elemento corrente
        _currentObjSelected.GetComponentInChildren<Camera>(true).gameObject.SetActive(true);
        //attivo la modalitò
        GameEvent.isActionOpen = true;
    }
    
    public void SetText(string testo)
    {
        /*
         * Imposto il testo di un Text della UI
         * 
         */
        text.text = testo;
    }

    public void SwitchGoalKeeper()
    {
        PitchController pitchController = GameObject.Find("Controller").GetComponent<PitchController>();
        bool isGoalKeeper = _currentObjSelected.GetComponent<Player>().GetGoalKeeper();
        if (isGoalKeeper)
        {
            if (_currentObjSelected.tag.Equals("PlayerA"))
                elementImage.sprite = Resources.Load<Sprite>("Icons/PlayerA") as Sprite;
            else if (_currentObjSelected.tag.Equals("PlayerB"))
                elementImage.sprite = Resources.Load<Sprite>("Icons/PlayerB") as Sprite;
            
            goalKeeperImage.color = new Color32(255,255,255,255);
            pitchController.SwitchPlayerToGoalKeeper(_currentObjSelected.GetComponent<Player>());
            _currentObjSelected.GetComponent<Player>().SetGoalKeeper(false);
            
        }
        else
        {
            if (_currentObjSelected.tag.Equals("PlayerA"))
                elementImage.sprite = Resources.Load<Sprite>("Icons/GoalKeeperA") as Sprite;
            else if (_currentObjSelected.tag.Equals("PlayerB"))
                elementImage.sprite = Resources.Load<Sprite>("Icons/GoalKeeperB") as Sprite;
            goalKeeperImage.color = new Color32(234,234,100,255);
            pitchController.SwitchPlayerToGoalKeeper(_currentObjSelected.GetComponent<Player>());
            _currentObjSelected.GetComponent<Player>().SetGoalKeeper(true);
        }
    }
    
}
