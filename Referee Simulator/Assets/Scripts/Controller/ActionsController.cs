using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionsController : MonoBehaviour
{
    
    /*
     * Devo distinguere il tackle da contatto effettivo:
     * - collider attiva animazione
     * - da comandi per simulazione
     * 
     */
    
    
    private static int codice = 0;
    public enum Azione
    {
        IDLE,
        RUNNING,
        TACKLE,
        FALLEN_AFTER_TACKLE
    };
    
    
    private class RecordData
    {
        public int codice;
        public GameObject element;
        public List<Vector3> positions;
        public List<Vector3> angles;
        public List<Azione> actions;

        public Vector3 initialPosition;
        public Vector3 initialRotation;

        public RecordData(GameObject element)
        {
            codice = ActionsController.codice++;
            this.element = element;
            positions = new List<Vector3>();
            angles = new List<Vector3>();
            actions = new List<Azione>();
            initialPosition = element.transform.position;
            initialRotation = element.transform.eulerAngles;
        }

        public void SetPositions(List<Vector3> positions)
        {
            this.positions = positions;
        }

        public void SetAngles(List<Vector3> angles)
        {
            this.angles = angles;
        }

        public void SetActions(List<Azione> actions)
        {
            this.actions = actions;
        }
    }
    
    [SerializeField] private EditorMenu editorMenu;

    private GameObject _element;

    private bool recordingMode;
    private ArrayList recording;
    private bool coroutineReplayStarted;
    
    private void Start()
    {
        recording = new ArrayList();
        recordingMode = false;
        coroutineReplayStarted = false;

    }

    private void Update()
    {
        if (GameEvent.isActionOpen)
        {
            PosizionamentoMenu.GetCurrentElementSelected().GetComponentInChildren<Camera>().depth = 1;
            if (Input.GetKeyDown(KeyCode.Return))
            {
                PosizionamentoMenu.GetCurrentElementSelected().GetComponent<FirstPersonController>().enabled = true;
                PosizionamentoMenu.GetCurrentElementSelected().GetComponent<Actions>().enabled = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                
                recordingMode = true;
                
                _element = PosizionamentoMenu.GetCurrentElementSelected();
       
                for (int i = 0; i < recording.Count; i++)
                {
                    RecordData rd = (RecordData)recording[i];
                    if (ReferenceEquals(rd.element, _element))
                        recording.RemoveAt(i);
                }
                
                foreach (RecordData rd in recording)
                {
                    if (!ReferenceEquals(rd.element, _element))
                        StartCoroutine(Replay(rd));
                        
                }

                List<GameObject> allElementsRecordable = PosizionamentoMenu.GetAllElementsRecordable();
                foreach (GameObject obj in allElementsRecordable)
                {
                    StartCoroutine(StartRecordingElement(obj));
                }

            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                PosizionamentoMenu.GetCurrentElementSelected().GetComponent<FirstPersonController>().enabled = false;
                PosizionamentoMenu.GetCurrentElementSelected().GetComponent<Actions>().enabled = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                editorMenu.OpenComandiPosizionamento();
                GameEvent.isActionOpen = false;
                PosizionamentoMenu.GetCurrentElementSelected().GetComponentInChildren<Camera>().depth = -1;
                recordingMode = false;
                
                
            }
            else if (Input.GetKeyDown(KeyCode.P))
            {
                StopRecording();
            }
            else if (!recordingMode && Input.GetKeyDown(KeyCode.R))
            {
                foreach (RecordData rd in recording)
                {
                    StartCoroutine(Replay(rd));
                }
                
            }
            
        }
        
        
    }
    

    private IEnumerator StartRecordingElement(GameObject element)
    {
        List<Vector3> positions = new List<Vector3>();
        List<Vector3> angles = new List<Vector3>();
        List<Azione> actions = new List<Azione>();

        Vector3 initialPosition = element.transform.position;

        RecordData rd = new RecordData(element);
        
        while (recordingMode)
        {
            if (element.transform.position != initialPosition)
            {
                positions.Add(element.transform.position);
                angles.Add(element.transform.eulerAngles);
                actions.Add(element.GetComponent<AnimatorController>().GetState());
                Debug.Log(element.name + " " + element.GetComponent<AnimatorController>().GetState());
            }
            
            yield return null;
        }
        
        rd.SetPositions(positions);
        rd.SetAngles(angles);
        rd.SetActions(actions);
        recording.Add(rd);
       
        rd.element.transform.position = rd.initialPosition;
        rd.element.transform.eulerAngles = rd.initialRotation;
    }

    private void StopRecording()
    {
        recordingMode = false;
        PosizionamentoMenu.GetCurrentElementSelected().GetComponent<FirstPersonController>().enabled = false;
        PosizionamentoMenu.GetCurrentElementSelected().GetComponent<Actions>().enabled = false;
    }

    public void StartReplay()
    {
        foreach (RecordData rd in recording)
        {
            StartCoroutine(Replay(rd));
        }
    }
    
    private IEnumerator Replay(RecordData rd)
    {
        coroutineReplayStarted = true;
        for (int i = 0; i < rd.positions.Count; i++)
        {
            rd.element.transform.position = rd.positions[i];
            rd.element.transform.eulerAngles = rd.angles[i];
            

            if (!(rd.actions[i] == Azione.TACKLE && rd.actions[i-1] == Azione.TACKLE))
            {
                rd.element.GetComponent<AnimatorController>().SetParameter((Azione) rd.actions[i], true);
            }
            

            yield return null;
        }
        
        rd.element.transform.position = rd.initialPosition;
        rd.element.transform.eulerAngles = rd.initialRotation;
        coroutineReplayStarted = false;
    }
    
}
