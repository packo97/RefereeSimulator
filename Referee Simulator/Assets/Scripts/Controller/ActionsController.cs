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
    
    
    public static int codice = 0;
    
    public enum Azione
    {
        IDLE,
        RUNNING,
        TACKLE,
        FALLEN_AFTER_TACKLE,
        PASS_BALL
    };
    
    
    private class RecordData
    {
        public int codice;
        public GameObject element;
        public List<Vector3> movements;
        public List<Vector3> angles;
        public List<Azione> actions;

        public Vector3 initialPosition;
        public Vector3 initialRotation;

        public List<Vector3> positions;

        public Vector3 finalPosition;
        public Vector3 finalAngles;
        
        public RecordData(GameObject element)
        {
            codice = ActionsController.codice;
            this.element = element;
            movements = new List<Vector3>();
            angles = new List<Vector3>();
            actions = new List<Azione>();
            initialPosition = element.transform.position;
            initialRotation = element.transform.eulerAngles;
                
            positions = new List<Vector3>();
            
        }

        public void SetMovements(List<Vector3> movements)
        {
            this.movements = movements;
        }

        public void SetAngles(List<Vector3> angles)
        {
            this.angles = angles;
        }

        public void SetActions(List<Azione> actions)
        {
            this.actions = actions;
        }

        public void SetPositions(List<Vector3> positions)
        {
            this.positions = positions;
        }

        public void SetFinalPosition(Vector3 finalPosition)
        {
            this.finalPosition = finalPosition;
        }

        public void SetFinalAngles(Vector3 finalAngles)
        {
            this.finalAngles = finalAngles;
        }
        
    }
    
    [SerializeField] private EditorMenu editorMenu;
    [SerializeField] private InfoBox infoBox;
    
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
            if(!recordingMode)
                infoBox.SetText("Press Enter to move player and start recording\n" +
                                "Press R to replay\n" +
                                "Press P to back to initial position\n" +
                                "Press ESC to exit", InfoBox.TypeOfMessage.INFO, false);
            
            PosizionamentoMenu.GetCurrentElementSelected().GetComponentInChildren<Camera>().depth = 1;
            if (Input.GetKeyDown(KeyCode.Return))
            {
                infoBox.SetText("Press P to stop recording", InfoBox.TypeOfMessage.INFO, false);
                
                PosizionamentoMenu.GetCurrentElementSelected().GetComponent<FirstPersonController>().enabled = true;
                PosizionamentoMenu.GetCurrentElementSelected().GetComponent<Actions>().enabled = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                
                recordingMode = true;
                
                _element = PosizionamentoMenu.GetCurrentElementSelected();
       
                /*
                 * Serve per eliminare la registrazione dell'azione già presente in questo layer (se presente)
                 */
                for (int i = 0; i < recording.Count; i++)
                {
                    RecordData rd = (RecordData) recording[i];
                    if (ReferenceEquals(rd.element, _element) && rd.codice == codice)
                        recording.RemoveAt(i);
                }
                
                foreach (RecordData rd in recording)
                {
                    if (!ReferenceEquals(rd.element, _element) && rd.codice == 0)
                        StartCoroutine(Replay(rd));
                        
                }

                //prima facevo partire la registrazione per tutti gli elementi registrabili
                /*
                List<GameObject> allElementsRecordable = PosizionamentoMenu.GetAllElementsRecordable();
                foreach (GameObject obj in allElementsRecordable)
                {
                    StartCoroutine(StartRecordingElement(obj));
                }*/
                //registrazione solo elemento corrente
                StartCoroutine(StartRecordingElement(_element));
                

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
                
                List<GameObject> allElementsRecordable = PosizionamentoMenu.GetAllElementsRecordable();
                foreach (GameObject elementRec in allElementsRecordable)
                {
                    for (int i = 0; i < recording.Count; i++)
                    {
                        RecordData rd = (RecordData) recording[i];
                        if (ReferenceEquals(rd.element, elementRec) && rd.codice == 0)
                        {
                            elementRec.transform.position = rd.initialPosition;
                            elementRec.transform.eulerAngles = rd.initialRotation;
                        }
                    }
                }
                
            }
            else if (Input.GetKeyDown(KeyCode.P))
            {
                StopRecording();
            }
            else if (!recordingMode && Input.GetKeyDown(KeyCode.R))
            {
                SetAllElementsToInitialPosition(0);
                
                foreach (RecordData rd in recording)
                {
                    //replay che inizia sempre dalla prima
                    if (rd.codice == 0)
                        StartCoroutine(Replay(rd));
                }
                
            }
            
        }
        else
        {
            infoBox.gameObject.SetActive(false);
        }
        
    }
    

    private IEnumerator StartRecordingElement(GameObject element)
    {
        List<Vector3> movements = new List<Vector3>();
        List<Vector3> angles = new List<Vector3>();
        List<Azione> actions = new List<Azione>();
        List<Vector3> positions = new List<Vector3>();
        
        Vector3 initialPosition = element.transform.position;
        

            RecordData rd = new RecordData(element);
        
        while (recordingMode)
        {
            //if (element.transform.position != initialPosition)
            //{
                //positions.Add(element.transform.position);
            movements.Add(element.GetComponent<FirstPersonController>().GetMoveDirection());
            angles.Add(element.transform.eulerAngles);
            actions.Add(element.GetComponent<AnimatorController>().GetState());
            
            positions.Add(element.transform.position);
            //}
            
            
            
            yield return null;
        }
        
        rd.SetMovements(movements);
        rd.SetAngles(angles);
        rd.SetActions(actions);
        rd.SetPositions(positions);
        
        rd.SetFinalPosition(element.transform.position);
        rd.SetFinalAngles(element.transform.eulerAngles);
        
        recording.Add(rd);
       
        //rd.element.transform.position = rd.initialPosition;
        //rd.element.transform.eulerAngles = rd.initialRotation;
        SetAllElementsToInitialPosition(codice);
    }

    private void StopRecording()
    {
        recordingMode = false;
        PosizionamentoMenu.GetCurrentElementSelected().GetComponent<FirstPersonController>().enabled = false;
        PosizionamentoMenu.GetCurrentElementSelected().GetComponent<Actions>().enabled = false;

        SetAllElementsToInitialPosition(codice);
        
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
        for (int i = 0; i < rd.movements.Count; i++)
        {
            //rd.element.GetComponent<CharacterController>().Move(rd.movements[i]);
            rd.element.GetComponent<FirstPersonController>().ReplayMove(rd.movements[i]);
            rd.element.transform.eulerAngles = rd.angles[i];

            if (rd.actions[i] == Azione.TACKLE && rd.actions[i-1] != Azione.TACKLE)
            {
                Debug.Log("TACKLE");
                rd.element.GetComponent<AnimatorController>().SetParameter((Azione)rd.actions[i], true);
            }
            
            
            if (rd.actions[i] == Azione.PASS_BALL && rd.actions[i-1] != Azione.PASS_BALL)
            {
                Debug.Log("PASS THE BALL");
                rd.element.GetComponent<AnimatorController>().SetParameter(rd.actions[i], true);
            }
            
            //Per ora commentato
            /*
            if (!(rd.actions[i] == Azione.TACKLE && rd.actions[i-1] == Azione.TACKLE))
            {
                rd.element.GetComponent<AnimatorController>().SetParameter((Azione) rd.actions[i], true);
            }
            */
            //Debug.Log(rd.actions[i]);

            yield return null;
        }
        coroutineReplayStarted = false;

        RecordData nextRecordData = NextRecording(rd);
        if (nextRecordData != null)
        {
            StartCoroutine(Replay(nextRecordData));
        }
    }

    private RecordData NextRecording(RecordData rd_current)
    {
        foreach (RecordData rd in recording)
        {
            if (ReferenceEquals(rd.element, rd_current.element) && rd_current.codice + 1 == rd.codice)
                return rd;
        }

        return null;
    }
    

    public void DeleteRecordings()
    {
        recording.Clear();
    }

    public void SetAllElementsToInitialPosition(int codice)
    {
        /*foreach (RecordData rd in recording)
        {
            rd.element.transform.position = rd.initialPosition;
            rd.element.transform.eulerAngles = rd.initialRotation;
        }*/
        List<GameObject> allElementsRecordable = PosizionamentoMenu.GetAllElementsRecordable();
        foreach (GameObject elementRec in allElementsRecordable)
        {
            for (int i = 0; i < recording.Count; i++)
            {
                RecordData rd = (RecordData) recording[i];
                if (ReferenceEquals(rd.element, elementRec) && rd.codice == codice)
                {
                    //disattivo e attivo il character controller a causa di un bug interno del character controller
                    elementRec.GetComponent<CharacterController>().enabled = false;
                    elementRec.transform.position = rd.initialPosition;
                    elementRec.transform.eulerAngles = rd.initialRotation;
                    elementRec.GetComponent<CharacterController>().enabled = true;
                }
            }
        }
    }

    public List<Vector3> GetPositionsOfElement(GameObject element)
    {
        foreach (RecordData rd in recording)
        {
            if (ReferenceEquals(rd.element, element))
            {
                return rd.positions;
            }
        }

        return null;
    }

    public Vector3 GetInitialPositionOfTheNextAction(GameObject obj)
    {
        
        foreach (RecordData rd in recording)
        {
            //Debug.Log("rd codice " + rd.codice + " ---- action controller codice " + ActionsController.codice);
            if (ReferenceEquals(rd.element, obj) && rd.codice == ActionsController.codice - 1)
            {
                return rd.finalPosition;
            }
        }

        return Vector3.zero;
    }
    
    public Vector3 GetInitialAnglesOfTheNextAction(GameObject obj)
    {

        foreach (RecordData rd in recording)
        {
            
            if (ReferenceEquals(rd.element, obj) && rd.codice == ActionsController.codice - 1)
            {
                return rd.finalAngles;
            }
        }

        return Vector3.zero;
    }

    public bool IsRecordedTheLastAction(GameObject obj, int numero)
    {
        int thelastone = numero - 1;
        foreach (RecordData rd in recording)
        {
            if (rd.codice == thelastone && ReferenceEquals(obj, rd.element))
            {
                return true;
            }
        }

        return false;

    }

    public int GetNumberOfActionRegistered(GameObject currentObjSelected)
    {
        //Debug.Log(currentObjSelected.name);
        int count = 0;
        foreach (RecordData rd in recording)
        {
            //Debug.Log("for " + rd.element.name);
            if (ReferenceEquals(rd.element, currentObjSelected))
                count++;
        }

        return count;
    }

    public void RemoveRecording(GameObject elementToRemove)
    {
        for (int i = 0; i < recording.Count; i++)
        {
            RecordData rd = (RecordData) recording[i];
            if (ReferenceEquals(rd.element, elementToRemove))
                recording.RemoveAt(i);
        }
    }
}
