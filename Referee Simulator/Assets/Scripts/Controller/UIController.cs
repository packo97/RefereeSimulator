using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private HomeEducatore _homeEducatore;
    [SerializeField] private EditorMenu _editorMenu;
    [SerializeField] private ManagerMenu _managerMenu;

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
        
        if (GameEvent.isSimulationOpen)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (GameEvent.isManagerOpen)
                {
                    CloseSimulationFromManager();
                    GameEvent.stopAllCoroutines = true;
                    _editorMenu.DestroyAllElementsInserted();
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
            }
            
            
        }
        
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
        _managerMenu.CloseSimulation();
        _managerMenu.OpenManager();
        
    }
    
}
