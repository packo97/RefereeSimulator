using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class EditorMenu : MonoBehaviour
{
    [SerializeField] private PosizionamentoMenu _posizionamentoMenu;
    [SerializeField] private Camera _cameraBiliardino;
    private bool statoCameraBiliardino;
    [SerializeField] private FormDati _formDati;
    [SerializeField] private InfoBox _infoBox;

    [SerializeField] private Button exitSimulationButton;

    [SerializeField] private Button cameraButton;
    [SerializeField] private Button confermaButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button playButton;
    
    private void Start()
    {
        
        foreach (RectTransform obj in gameObject.GetComponentInChildren<RectTransform>())
        {
            obj.gameObject.SetActive(false);   
        }
        _cameraBiliardino.gameObject.SetActive(false);
        
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            exitSimulationButton.gameObject.SetActive(false);
        }
        
    }

    public void OpenEditor()
    {
        GameEvent.isEditorOpen = true;
        gameObject.SetActive(true);
        foreach (RectTransform obj in gameObject.GetComponentInChildren<RectTransform>())
        { 
            if(!obj.gameObject.GetComponent<PosizionamentoMenu>() && !obj.gameObject.GetComponent<FormDati>())
                obj.gameObject.SetActive(true);
            if(obj.gameObject.name.Equals("ExitSimulationButton"))
                obj.gameObject.SetActive(false);
        }
    }

    public void CloseEditor()
    {
        gameObject.SetActive(false);
        GameEvent.isEditorOpen = false;
        DestroyAllElementsInserted();
    }

    public void SwitchComandiPosizionamento()
    {
        GameEvent.isBiliardinoOpen = !GameEvent.isBiliardinoOpen;
        _posizionamentoMenu.SwitchComandiPosizionamento();
        statoCameraBiliardino = _cameraBiliardino.gameObject.activeSelf;
        _cameraBiliardino.gameObject.SetActive(!statoCameraBiliardino);
        
        foreach (RectTransform obj in gameObject.GetComponentInChildren<RectTransform>())
        {
            if (obj.gameObject.GetComponent<ComandoEditor>() && !obj.gameObject.name.Equals("CameraButton"))
            {
                bool stato = obj.gameObject.activeSelf;
                obj.gameObject.SetActive(!stato);
            }

            if (obj.GetComponent<ComandoSimulazione>())
            {
                if(obj.gameObject.name.Equals("PlayButton"))
                    obj.gameObject.SetActive(!GameEvent.isBiliardinoOpen);
                else if (obj.gameObject.name.Equals("ExitSimulationButton"))
                    obj.gameObject.SetActive(false);
            }
                
        }
    }

    public void OpenComandiPosizionamento()
    {
        GameEvent.isBiliardinoOpen = true;
        _cameraBiliardino.gameObject.SetActive(true);
        _posizionamentoMenu.SwitchComandiPosizionamento();
        cameraButton.gameObject.SetActive(true);
    }
    
    public void PlaySimulation()
    {
        if (GameEvent.isRefereeDropped)
        {
            GameEvent.isSimulationOpen = true;
        
            foreach (RectTransform obj in gameObject.GetComponentInChildren<RectTransform>())
            {
                if (obj.gameObject.GetComponent<ComandoEditor>() || obj.gameObject.name.Equals("CameraButton") || obj.gameObject.name.Equals("PlayButton"))
                {
                    obj.gameObject.SetActive(false);
                }
                
                if(obj.gameObject.name.Equals("ExitSimulationButton"))
                    obj.gameObject.SetActive(true);
            
            }

            FindReferee().GetComponent<FirstPersonController>().enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            GameObject.Find("Controller").GetComponent<ActionsController>().StartReplay();
            
        }
        else
        {
            _infoBox.SetText("Referee is not added in the pitch.", InfoBox.TypeOfMessage.ERROR);
        }
        
    }

    public void CloseSimulation()
    {
        GameEvent.isSimulationOpen = false;

        if (GameEvent.isEditorOpen)
        {
            foreach (RectTransform obj in gameObject.GetComponentInChildren<RectTransform>())
            {
                if (obj.gameObject.GetComponent<ComandoEditor>() || obj.gameObject.name.Equals("CameraButton") || obj.gameObject.name.Equals("PlayButton"))
                {
                    obj.gameObject.SetActive(true);
                }
            
                if(obj.gameObject.name.Equals("ExitSimulationButton"))
                    obj.gameObject.SetActive(false);
            
            }    
        }

        FindReferee().GetComponent<FirstPersonController>().enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
    }

    public void ConfermaSimulazione()
    {
        _formDati.gameObject.SetActive(true);
    }

    private Referee FindReferee()
    {
        GameObject obj =  GameObject.FindWithTag("Referee");
        if (obj != null)
            return obj.GetComponent<Referee>();
        
        return null;
    }

    public void DestroyAllElementsInserted()
    {
        Referee referee = FindReferee();
        if (referee != null)
        {
            Destroy(FindReferee().gameObject);
            GameEvent.isRefereeDropped = false;
        }
            
        
        foreach (GameObject playerA in FindAllPlayerOfTeamA())
        {
            Destroy(playerA);
        }
        
        foreach (GameObject playerB in FindAllPlayerOfTeamB())
        {
            Destroy(playerB);
        }

        foreach (GameObject ball in FindAllBall())
        {
            Destroy(ball);
        }
        
        _posizionamentoMenu.DeleteAllIconsInserted();
        
    }

    private ArrayList FindAllPlayerOfTeamA()
    {
        ArrayList teamA = new ArrayList(GameObject.FindGameObjectsWithTag("PlayerA"));
        return teamA;
    }
    
    private ArrayList FindAllPlayerOfTeamB()
    {
        ArrayList teamB = new ArrayList(GameObject.FindGameObjectsWithTag("PlayerB"));
        return teamB;
    }

    private ArrayList FindAllBall()
    {
        ArrayList balls = new ArrayList(GameObject.FindGameObjectsWithTag("Ball"));
        return balls;
    }


    public void SetActions()
    {
        SwitchComandiPosizionamento();
        playButton.gameObject.SetActive(false);
        cameraButton.gameObject.SetActive(false);
        confermaButton.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
        _posizionamentoMenu.SetActionsOfSelectedPlayer();
    }
}
