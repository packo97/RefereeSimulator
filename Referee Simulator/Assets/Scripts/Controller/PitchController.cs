using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PitchController : MonoBehaviour
{
    [SerializeField] private GameObject _elementiInseriti;
    [SerializeField] private GameObject _iconeInserite;
    


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
        }
        
        
    }

}
