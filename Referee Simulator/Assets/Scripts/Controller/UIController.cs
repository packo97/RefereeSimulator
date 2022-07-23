using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private HomeEducatore _homeEducatore;
    [SerializeField] private EditorMenu _editorMenu;
    [SerializeField] private ManagerMenu _managerMenu;

    [SerializeField] private Image yellowOrRedCardImage;
    
    private void Start()
    {
        _editorMenu.CloseEditor();
        _managerMenu.CloseManager();
        _homeEducatore.Open();
    }

    private void Update()
    {
        /*
         * When simulation mode is open, listening on ESC to escape from this mode
         * 
         */
        
        //Debug.Log("SIMULATION " + GameEvent.isSimulationOpen + " MANAGER " + GameEvent.isManagerOpen + " EDITOR " + GameEvent.isEditorOpen);
        //Debug.Log("coroutine: " + GameEvent.stopAllCoroutines);
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            if (!GameEvent.isSimulationOpen && GameEvent.isManagerOpen && !GameEvent.isEditorOpen)
            {
                CloseSimulationFromManager();
            }
            else if (GameEvent.isSimulationOpen && GameEvent.isManagerOpen && GameEvent.isEditorOpen)
            {
                _editorMenu.CloseSimulation();
                gameObject.GetComponent<ActionsController>().SetAllElementsToInitialPosition(0);
                OpenEditor();
            }
            else if (GameEvent.isSimulationOpen && !GameEvent.isManagerOpen && GameEvent.isEditorOpen)
            {
                _editorMenu.CloseSimulation();
                gameObject.GetComponent<ActionsController>().SetAllElementsToInitialPosition(0);
            }
            else if (GameEvent.isSimulationOpen && GameEvent.isManagerOpen && !GameEvent.isEditorOpen)
            {
                gameObject.GetComponent<ActionsController>().SetAllElementsToInitialPosition(0);
                CloseSimulationFromManager();
            }
            GameEvent.stopAllCoroutines = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            yellowOrRedCardImage.enabled = false;
        }
/*
        if (GameEvent.isSimulationOpen)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (GameEvent.isManagerOpen)
                {
                    CloseSimulationFromManager();
                    GameEvent.stopAllCoroutines = true;
                    //_editorMenu.DestroyAllElementsInserted();
                    //GetComponent<ActionsController>().DeleteRecordings();
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else if (GameEvent.isEditorOpen)
                {
                    _editorMenu.CloseSimulation();
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                gameObject.GetComponent<ActionsController>().SetAllElementsToInitialPosition(0);
                yellowOrRedCardImage.enabled = false;
            }
        }
*/
    }

    public void OpenHomeEducatore()
    {
        _homeEducatore.Open();
    }

    public void CloseHomeEducatore()
    {
        _homeEducatore.Close();
    }

    public void OpenEditor()
    {
        _homeEducatore.Close();
        _managerMenu.CloseManager();
        _editorMenu.OpenEditor();
    }

    public void CloseEditor()
    {
        _editorMenu.CloseEditor();
    }

    public void OpenManager()
    {
        _homeEducatore.Close();
        _editorMenu.CloseEditor();
        _managerMenu.OpenManager();
        GameEvent.isManagerOpen = true;
    }

    public void CloseManager()
    {
        _managerMenu.CloseManager();
    }

    public void OpenSimulationFromManager()
    {
        _managerMenu.CloseManager();
        GetComponent<PitchController>().LoadElementsInThePitch();
        _managerMenu.PlaySimulation();
    }
    
    
    public void CloseSimulationFromManager()
    {
        _editorMenu.DestroyAllElementsInserted();
        GetComponent<ActionsController>().DeleteRecordings();
        _managerMenu.CloseSimulation();
        _managerMenu.OpenManager();
        
    }
    
}
