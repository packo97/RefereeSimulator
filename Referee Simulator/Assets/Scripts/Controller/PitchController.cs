using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PitchController : MonoBehaviour
{
    [SerializeField] private GameObject _elementiInseriti;
    [SerializeField] private GameObject _iconeInserite;


    public List<GameObject> GetAllElementsInThePitch()
    {
        List<GameObject> elements = new List<GameObject>();
        for (int i = 0; i < _elementiInseriti.transform.childCount; i++)
        {
            GameObject element = _elementiInseriti.transform.GetChild(i).gameObject;
            if (element.GetComponent<Player>())
                elements.Add(element);
        }

        return elements;
    }

    public void SetAllElementsToInitialPositionOfTheLayer(int layer)
    {
        /*
         *  Questo metodo serve per impostare tutti gli elementi sul terreno di gioco (escluso il pallone) alla posizione
         *  corretta in base al layer inserito come paramentro.
         *
         */
        
        //prendo tutti gli elementi sul terreno di gioco (escluso il pallone)
        List<GameObject> elements = GetAllElementsInThePitch();
        //prendo l'elemento selezionato nell'editor
        GameObject currentElement = PosizionamentoMenu.GetCurrentElementSelected();
        //inizializzo le variabili di posizione e rotazione
        Vector3 initalPosition = Vector3.zero;
        Vector3 initialRotation = Vector3.zero;
        ActionsController actionsController = GetComponent<ActionsController>();
        
        //per ogni elemento chiedo la sua posizione
        foreach (GameObject element in elements)
        {
            //se l'elemento è quello selezionato nell'editor devo prendere la posizione finale del layer precedente
            if (element.GetComponent<Player>().id == currentElement.GetComponent<Player>().id &&
                element.tag.Equals(currentElement.tag))
            {
                initalPosition = actionsController.GetFinalPositionOfTheLayer(element, layer - 1);
                initialRotation = actionsController.GetFinalAnglesOfTheLayer(element, layer - 1);
            }
            // altrimenti prendo la posizione iniziale del layer passato come parametro
            else
            {
                initalPosition = actionsController.GetInitialPositionOfTheLayer(element, layer);
                initialRotation = actionsController.GetInitialAnglesOfTheLayer(element, layer);
                // se il vector è uguale a zero, significa che non è stato trovata una registrazione per il layer passato come parametro
                // quindi cerco dal layer passato come parametro al layer 0 e utilizzo il primo layer che restituisce una posizione diversa da 0
                if (initalPosition == Vector3.zero)
                {
                    int tmp_layer = layer;
                    while (tmp_layer >= 0)
                    {
                        initalPosition = actionsController.GetFinalPositionOfTheLayer(element, tmp_layer);
                        initialRotation = actionsController.GetFinalAnglesOfTheLayer(element, tmp_layer);
                        if (initalPosition != Vector3.zero)
                            break;
                        tmp_layer -= 1;
                    }
                    
                }
            }
            //imposto la posizione solo se diversa da zero
            if (initalPosition != Vector3.zero)
            {
                element.transform.position = initalPosition;
                element.transform.eulerAngles = initialRotation;
            }
            
        }
    }

    public void LoadElementsInThePitch()
    {
        /*
         * Carica sul terreno di gioco gli elementi della simulazione selezionata
         * 
         */
        
        string codice = ManagerMenu.GetIconSimulazioneSelected().GetCodice();
        ElementData _elementData = SaveLoadManager.LoadSimulation(codice);
        GameObject iconElementPrefab = Resources.Load("Prefabs/IconElement") as GameObject;

        foreach (ElementData.Element element in _elementData.elements)
        {
            string pathPrefab = "";
            if (element.type.Equals("Referee"))
                pathPrefab = "Prefabs/Referee";
            else if (element.type.Equals("PlayerA"))
                pathPrefab = "Prefabs/FootballPlayerA";
            else if (element.type.Equals("PlayerB"))
                pathPrefab = "Prefabs/FootballPlayerB";
            else if (element.type.Equals("Ball"))
                pathPrefab = "Prefabs/Ball";

            GameObject instance = Instantiate(Resources.Load(pathPrefab) as GameObject, _elementiInseriti.transform);
            instance.transform.position = new Vector3(element.positionX, element.positionY, element.positionZ);
            instance.transform.eulerAngles = new Vector3(element.rotationX, element.rotationY, element.rotationZ);
            if (instance.GetComponent<Player>())
                instance.GetComponent<Player>().id = element.id;
            /*
             * Carica sul terreno di gioco le icone corrispondenti agli elementi della simulazione selezionata
             * 
             */
            
            GameObject iconInstance = Instantiate(iconElementPrefab, _iconeInserite.transform);
            iconInstance.transform.position =
                new Vector3(element.iconPositionX, element.iconPositionY, element.iconPositionZ);

            
            foreach (RectTransform rt in iconInstance.GetComponentsInChildren<RectTransform>())
            {
                if (rt.name.Equals("Indicatore"))
                {
                    rt.eulerAngles = new Vector3(element.iconRotationX, element.iconRotationY, element.iconRotationZ);
                }
                
                if (element.type.Equals("Ball") && rt.gameObject != iconInstance.gameObject)
                    Destroy(rt.gameObject);
                    
            }
            
            string iconPath = "";
            if (element.type.Equals("Referee"))
                iconPath = "Icons/referee";
            else if (element.type.Equals("PlayerA"))
                iconPath = "Icons/playerA";
            else if (element.type.Equals("PlayerB"))
                iconPath = "Icons/playerB";
            else if (element.type.Equals("Ball"))
                iconPath = "Icons/ball";

            Sprite sprite = Resources.Load<Sprite>(iconPath) as Sprite;
            iconInstance.GetComponent<Image>().sprite = sprite;
            iconInstance.GetComponent<DragDrop>().SetDropped(true);
            iconInstance.GetComponent<DragDrop>().SetElementInThePitch(instance);
            
            if (element.type.Equals("Referee"))
                GameEvent.isRefereeDropped = true;
            
            
            GameObject.Find("Controller").GetComponent<ActionsController>().SetActionsRegistered(_elementData.recording);
        }
        
        
    }

    public int GetNumberOfElement(string tag)
    {
        int count = 0;
        for (int i = 0; i < _elementiInseriti.transform.childCount; i++)
        {
            if (_elementiInseriti.transform.GetChild(i).tag.Equals(tag))
                count++;
        }

        return count;
    }

    public GameObject GetElementFromID(int id, string tag)
    {
        for (int i = 0; i < _elementiInseriti.transform.childCount; i++)
        {
            GameObject element = _elementiInseriti.transform.GetChild(i).gameObject;
            if (element.GetComponent<Player>())
            {
                if (element.tag.Equals(tag) && element.GetComponent<Player>().id == id)
                {
                    return element;
                } 
            }
            else if (element.GetComponent<Ball>())
            {
                if (element.tag.Equals(tag))
                    return element;
            }

            if (element.GetComponentInChildren<Ball>())
            {
                GameObject ball = element.GetComponentInChildren<Ball>().gameObject;
                if (ball.tag.Equals(tag))
                    return ball;
            }
            
                
        }
        
        

        return null;
    }

    public GameObject GetBall()
    {
        /*
        for (int i = 0; i < _elementiInseriti.transform.childCount; i++)
        {
            GameObject element = _elementiInseriti.transform.GetChild(i).gameObject;
            if (element.tag.Equals("Ball"))
            {
                return element;
            }
        }*/

        Ball ball = _elementiInseriti.GetComponentInChildren<Ball>();
        if (ball != null )
            return ball.gameObject;
        
        return null;
    }

}
