using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera _freeCamera;
    [SerializeField] private Camera _biliardinoCamera;

    // Update is called once per frame
    void Update()
    {
        /*
         * Aggiorna la camera in base alla modalità aperta
         * 
         */
        
        if (GameEvent.isBiliardinoOpen && !GameEvent.isSimulationOpen)
        {
            _biliardinoCamera.gameObject.SetActive(true);
            _biliardinoCamera.depth = 0;
            _freeCamera.depth = -1;
            if(GameObject.FindWithTag("Referee")!=null)
                GameObject.FindWithTag("Referee").GetComponentInChildren<Camera>().depth = -1;
        }
        else if (!GameEvent.isBiliardinoOpen && !GameEvent.isSimulationOpen)
        {
            _biliardinoCamera.depth = -1;
            _freeCamera.depth = 0;
        }
        else if (GameEvent.isSimulationOpen && !GameEvent.isBiliardinoOpen)
        {
            _biliardinoCamera.depth = -1;
            _freeCamera.depth = -1;
            if(GameObject.FindWithTag("Referee")!=null)
                GameObject.FindWithTag("Referee").GetComponentInChildren<Camera>().depth = 0;
        }
        else if (!GameEvent.isSimulationOpen && !GameEvent.isBiliardinoOpen)
        {
            _biliardinoCamera.depth = -1;
            _freeCamera.depth = 0;
        }
        
    }
    
    
    
}
