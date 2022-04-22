using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ManagerMenu : MonoBehaviour
{
    [SerializeField] private GameObject iconaSimulazionePrefab;
    [SerializeField] private GameObject contenitoreSimulazioni;
    [SerializeField] private SimulationDetails simulationDetails;
    [SerializeField] private Dropdown selectCategory;
    [SerializeField] private Button _exitButton;
    [SerializeField] private InfoBox _infoBox;
    private static IconaSimulazione icon_selected;

    private void Start()
    {
        _exitButton.gameObject.SetActive(false);
    }

    private void Update()
    {
        /*
         * Usa il tasco ESC per uscire dalla simulation Mode
         * 
         */
        
        if (GameEvent.isSimulationOpen)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseSimulation();
            }
        }
    }
    
    public void OpenManager()
    {
        gameObject.SetActive(true);
        _exitButton.gameObject.SetActive(false);
        GameEvent.isManagerOpen = true;
        CleanManager();
        LoadAllSimulation();
    }
    
    
    public void CloseManager()
    {
        selectCategory.value = 0;
        CleanManager();
        gameObject.SetActive(false);
        simulationDetails.CleanDetails();
        simulationDetails.ShowButtons(false);
    }

    public void LoadAllSimulation()
    {
        ArrayList namesFile = SaveLoadManager.GetFilesName();
        foreach (string name in namesFile)
        {
            GameObject icona = Instantiate(iconaSimulazionePrefab, contenitoreSimulazioni.transform);
            icona.name = "IconaSimulazione" + Path.GetFileNameWithoutExtension(name);
        }
    }

    public void LoadSimulationForCategory()
    {
        CleanManager();
        simulationDetails.CleanDetails();
        string category = selectCategory.captionText.text;
        if (!category.Equals("ALL"))
        {
            ArrayList filesname = SaveLoadManager.GetFilesName();
            foreach (string name in filesname)
            {
                string codice = Path.GetFileNameWithoutExtension(name);
                ElementData elementData = SaveLoadManager.LoadSimulation(codice);
                if (elementData.details.category.Equals(category))
                {
                    GameObject icona = Instantiate(iconaSimulazionePrefab, contenitoreSimulazioni.transform);
                    icona.name = "IconaSimulazione" + codice;
                }
            }
            
        }
        else
        {
            LoadAllSimulation();
        }
        
    }
    

    private void CleanManager()
    {
        foreach (RectTransform iconaSimulazione in contenitoreSimulazioni.GetComponentsInChildren<RectTransform>())
        {
            if (iconaSimulazione.gameObject != contenitoreSimulazioni) 
                Destroy(iconaSimulazione.gameObject);
        }
    }
    

    public static IconaSimulazione GetIconSimulazioneSelected()
    {
        return icon_selected;
    }

    public static void SetIconSimulazioneSelected(IconaSimulazione selected)
    {
        icon_selected = selected;
    }

    public void PlaySimulation()
    {
        if (GameEvent.isRefereeDropped)
        {
            GameEvent.isSimulationOpen = true;
            FindReferee().GetComponent<FirstPersonController>().enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            _infoBox.SetText("Referee is not added in the pitch.", InfoBox.TypeOfMessage.ERROR);
        }
        _exitButton.gameObject.SetActive(true);
    }

    public void CloseSimulation()
    {
        GameEvent.isSimulationOpen = false;
        _exitButton.gameObject.SetActive(false);
        FindReferee().GetComponent<FirstPersonController>().enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    private Referee FindReferee()
    {
        GameObject obj =  GameObject.FindWithTag("Referee");
        if (obj != null)
            return obj.GetComponent<Referee>();
        
        return null;
    }
}
