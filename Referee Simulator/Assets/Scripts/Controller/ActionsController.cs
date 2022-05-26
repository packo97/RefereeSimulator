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

    public static bool pause = false;
    
    public static int codice = 0;
    
    public enum Azione
    {
        IDLE,
        RUNNING,
        TACKLE,
        FALLEN_AFTER_TACKLE,
        PASS_BALL,
        RECEIVE_BALL
    };

    [System.Serializable]
    public class RecordData
    {
        public bool valid;
        public int codice;
        //public GameObject element;
        public int id_element;
        public string tag_element;
        
        public List<Vector3Surrogate> movements;
        public List<Vector3Surrogate> angles;
        public List<Azione> actions;
        
        public Vector3Surrogate initialPosition;
        public Vector3Surrogate initialRotation;
        
        public List<TargetForElement> targets_kickerID;
        public ElementIdentifier kickerID;
        public Vector3Surrogate finalPosition;
        public Vector3Surrogate finalAngles;

        public Vector3Surrogate initialPositionBall;
        public Vector3Surrogate finalPositionBall;
        
        public RecordData(GameObject element)
        {
            valid = true;
            codice = ActionsController.codice;
            //this.element = element;
            if (element.GetComponent<Player>())
                id_element = element.GetComponent<Player>().id;
            else
                id_element = 0;
            
            tag_element = element.tag;
            movements = new List<Vector3Surrogate>();
            angles = new List<Vector3Surrogate>();
            actions = new List<Azione>();
            initialPosition = new Vector3Surrogate(element.transform.position);
            initialRotation = new Vector3Surrogate(element.transform.eulerAngles);
                
            targets_kickerID = new List<TargetForElement>();
            
        }

        private int indexNextTarget = 0;
        public int countChangeOwn = 0;
        
        public void SetNextTarget(int index)
        {
            indexNextTarget = index;
        }
        
        public Vector3 GetNextTargetForKicker(int kickerID, string kickerTag)
        {
            //soluzione provvisoria
            /*if (indexNextTarget == positions.Count)
                indexNextTarget = 0;*/
            //Debug.Log("get next target " + targets_kicker[indexNextTarget] + "indice " + indexNextTarget);
            //return targets_kicker[indexNextTarget++].Item1;
            //Debug.Log("kicker id " + kickerID + "count: " +  targets_kickerID.Count);
            foreach (TargetForElement i in  targets_kickerID)
            {
                //Debug.Log("il target è" + i.target.GetVector3() + " " + i.alreadyUsed);
                if (i.elementIdentifier.id == kickerID && i.elementIdentifier.tag.Equals(kickerTag) && !i.alreadyUsed)
                {
                    i.alreadyUsed = true;
                    return i.target.GetVector3();
                }
                    
            }
            /*
             * Problema da risolvere fa la return di vector3 zero perchè non ci sono target se avvio il replay da un
             * elemento che non ne ha
             * 
             */
            return Vector3.zero;
        }

        public void ResetTargetForKicker()
        {
            foreach (TargetForElement i in targets_kickerID)
            {
                i.alreadyUsed = false;
            }
        }

        public void SetMovements(List<Vector3Surrogate> movements)
        {
            this.movements = movements;
        }

        public void SetAngles(List<Vector3Surrogate> angles)
        {
            this.angles = angles;
        }

        public void SetActions(List<Azione> actions)
        {
            this.actions = actions;
        }

        public void SetTargetsKicker(List<TargetForElement> targets_kicker)
        {
            this.targets_kickerID = targets_kicker;
        }

        public void SetFinalPosition(Vector3Surrogate finalPosition)
        {
            this.finalPosition = finalPosition;
        }

        public void SetFinalAngles(Vector3Surrogate finalAngles)
        {
            this.finalAngles = finalAngles;
        }
        
    }
    
    [SerializeField] private EditorMenu editorMenu;
    [SerializeField] private InfoBox infoBox;
    
    private GameObject _element;
    private int id_current_element;
    private string tag_current;
    public List<TargetForElement> target_kickerID_layer;
    
    private bool recordingMode;
    private ArrayList recording;
    private bool coroutineReplayStarted;

    public RecordData GetRecordDataFromID_Tag_Codice(int id, string tag)
    {
        for (int i = 0; i < recording.Count; i++)
        {
            RecordData rd = (RecordData) recording[i];
            if (rd.id_element == id && rd.tag_element.Equals(tag) && codice == rd.codice)
                return rd;
        }

        return null;
    }
    
    private void Start()
    {
        recording = new ArrayList();
        recordingMode = false;
        coroutineReplayStarted = false;
        target_kickerID_layer = new List<TargetForElement>();
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
                GameEvent.stopAllCoroutines = false;
                target_kickerID_layer.Clear();
                _element = PosizionamentoMenu.GetCurrentElementSelected();
                id_current_element = _element.GetComponent<Player>().id;
                tag_current = _element.tag;
                /*
                 * Serve per eliminare la registrazione dell'azione già presente in questo layer (se presente)
                 */

                //ElementIdentifier elementIdentifierCurrentElement = new ElementIdentifier(id_current_element, tag_current);
                //ElementIdentifier elementIdentifierBall = new ElementIdentifier(0, "Ball");
                //RemoveBallRecordingOfTheCurrentElement(elementIdentifierCurrentElement, elementIdentifierBall);
                    
                //rimuovo la recording dell'elemento corrente se esiste
                RecordData rd_to_delete = GetRecordDataFromID_Tag_Codice(id_current_element, tag_current);
                recording.Remove(rd_to_delete);
                
                //cerco se qualcuno usava il pallone nello stesso layer per invalidare la recording
                if (rd_to_delete != null)
                {
                    for (int i = 0; i < rd_to_delete.actions.Count; i++)
                    {
                        Azione azione = rd_to_delete.actions[i];
                        if (azione == Azione.RECEIVE_BALL || azione == Azione.PASS_BALL)
                        {
                            for (int j = 0; j < recording.Count; j++)
                            {
                                RecordData rdi = (RecordData)recording[j];
                                if (rd_to_delete.id_element != rdi.id_element ||
                                    !rd_to_delete.tag_element.Equals(rdi.tag_element))
                                {
                                    for (int k = 0; k < rdi.actions.Count; k++)
                                    {
                                        Azione azione_rdi = (Azione)rdi.actions[k];
                                        if (azione_rdi == Azione.RECEIVE_BALL || azione_rdi == Azione.PASS_BALL)
                                            rdi.valid = false;
                                    }
                                    
                                }
                            }
                        }
                    }
                }
                
                /*if (rd_to_delete != null)
                    if (rd_to_delete.tag_element.Equals("Ball"))
                    {
                        for (int j = 0; j < rd_to_delete.targets_kickerID.Count; j++)
                        {
                            if (rd_to_delete.targets_kickerID[j].elementIdentifier.id == _element.GetComponent<Player>().id &&
                                rd_to_delete.targets_kickerID[j].elementIdentifier.tag.Equals(_element.GetComponent<Player>().tag))
                            {
                                //Debug.Log("rimosso");
                                rd_to_delete.targets_kickerID.RemoveAt(j);
                                for (int k = 0; k < recording.Count; k++)
                                {
                                    RecordData rd2 = (RecordData)recording[k];
                                    for (int q = 0; q < rd2.actions.Count; q++)
                                    {
                                        if (rd2.actions[q] == Azione.PASS_BALL || rd2.actions[q] == Azione.RECEIVE_BALL)
                                        {
                                            //recording.RemoveAt(k);
                                            //Debug.Log("usa il pallone, quindì invalida la sequenza e va eliminata");
                                            rd2.valid = false;
                                        }
                                            
                                    }
                                }
                            }
                        }
                    }*/
                
                
                foreach (RecordData rd in recording)
                {
                    if ((rd.id_element != id_current_element || rd.tag_element != tag_current) && rd.codice == 0 && rd.valid)
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
                //registrazione pallone se presente
                /*GameObject ball = GameObject.Find("Controller").GetComponent<PitchController>().GetBall();
                if (ball != null)
                    StartCoroutine(StartRecordingElement(ball));*/

            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameEvent.stopAllCoroutines = true;
                PosizionamentoMenu.GetCurrentElementSelected().GetComponent<FirstPersonController>().enabled = false;
                PosizionamentoMenu.GetCurrentElementSelected().GetComponent<Actions>().enabled = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                editorMenu.OpenComandiPosizionamento();
                GameEvent.isActionOpen = false;
                PosizionamentoMenu.GetCurrentElementSelected().GetComponentInChildren<Camera>().depth = -1;
                recordingMode = false;
                /*
                List<GameObject> allElementsRecordable = PosizionamentoMenu.GetAllElementsRecordable();
                foreach (GameObject elementRec in allElementsRecordable)
                {
                    for (int i = 0; i < recording.Count; i++)
                    {
                        RecordData rd = (RecordData) recording[i];
                        int id_elementRec;
                        if (elementRec.GetComponent<Player>())
                            id_elementRec = elementRec.GetComponent<Player>().id;
                        else
                            id_elementRec = 0;
                        
                        if (rd.id_element == id_elementRec && rd.codice == 0)
                        {
                            elementRec.transform.position = rd.initialPosition;
                            elementRec.transform.eulerAngles = rd.initialRotation;
                        }
                    }
                }*/
                SetAllElementsToInitialPosition(0);
                infoBox.gameObject.SetActive(false);
            }
            else if (Input.GetKeyDown(KeyCode.P))
            {
                GameEvent.stopAllCoroutines = true;
                StopRecording();

                foreach (RecordData rd in recording)
                {
                    rd.ResetTargetForKicker();
                }
                
            }
            else if (!recordingMode && Input.GetKeyDown(KeyCode.R))
            {
                SetAllElementsToInitialPosition(0);
                Debug.Log("numero di recording che partiranno: " + recording.Count);
                foreach (RecordData rd in recording)
                {
                    //replay che inizia sempre dalla prima
                    if (rd.codice == 0 && rd.valid)
                        StartCoroutine(Replay(rd));
                }
                
            }
            
        }

    }

    private void RemoveBallRecordingOfTheCurrentElement(ElementIdentifier elementIdentifierCurrentElement, ElementIdentifier elementIdentifierBall)
    {
        GameObject ball = GameObject.Find("Controller").GetComponent<PitchController>().GetBall();
        if (ball != null)
        {
            RecordData rdBall = GetRecordDataFromID_Tag_Codice(elementIdentifierBall.id, elementIdentifierBall.tag);
            if (rdBall != null)
            {
                Debug.Log("trovata possibile rdball da cancellare " + rdBall.tag_element);

                if (elementIdentifierCurrentElement.Equals(rdBall.kickerID))
                {
                    Debug.Log("è il momento di cancellare");
                    recording.Remove(rdBall);
                }
                
            }
                
        }
    }

    private IEnumerator StartRecordingElement(GameObject element)
    {
        List<Vector3Surrogate> movements = new List<Vector3Surrogate>();
        List<Vector3Surrogate> angles = new List<Vector3Surrogate>();
        List<Azione> actions = new List<Azione>();
        //List<(Vector3, GameObject)> positions = new List<(Vector3, GameObject)>();
        
        Vector3 initialPosition = element.transform.position;
        GameObject ball = GameObject.Find("Controller").GetComponent<PitchController>().GetBall();
        Vector3 initialPositionBall = Vector3.zero;
        if (ball != null) 
            initialPositionBall = ball.transform.position;
        
        RecordData rd = new RecordData(element);
        
        while (recordingMode)
        {
            //if (element.transform.position != initialPosition)
            //{
                //positions.Add(element.transform.position);
            if (!pause)
            {
                if (element.GetComponent<FirstPersonController>())
                    movements.Add(new Vector3Surrogate(element.GetComponent<FirstPersonController>().GetMoveDirection()));
                angles.Add(new Vector3Surrogate(element.transform.eulerAngles));
                if (element.GetComponent<AnimatorController>())
                    actions.Add(element.GetComponent<AnimatorController>().GetState());
            
                //positions.Add((element.transform.position, element));
            }
            
            //}
            
            
            
            yield return null;
        }
        
        rd.SetMovements(movements);
        rd.SetAngles(angles);
        rd.SetActions(actions);
        //rd.SetTargetsKicker(positions);
        rd.SetFinalPosition(new Vector3Surrogate(element.transform.position));
        rd.SetFinalAngles(new Vector3Surrogate(element.transform.eulerAngles));
        
        rd.targets_kickerID.Clear();
        foreach (TargetForElement te in target_kickerID_layer)
        {
            rd.targets_kickerID.Add(new TargetForElement(te.target, te.elementIdentifier)); 
        }

        rd.kickerID = new ElementIdentifier(id_current_element, tag_current);
        //Debug.Log("count rd after recording " + rd.targets_kickerID.Count);
        //Debug.Log("count layer after recording " + target_kickerID_layer.Count);
        /*rd.targets_kickerID = target_kickerID_layer.Select(o =>
            new TargetForElement(o.target, o.elementIdentifier)
            {
                target = o.target, elementIdentifier = o.elementIdentifier
            }).ToList();*/
        rd.initialPositionBall = new Vector3Surrogate(initialPositionBall);
        if (ball != null)
            rd.finalPositionBall = new Vector3Surrogate(ball.transform.position);
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
        StartCoroutine(CoroutineReplay());
        
    }

    public IEnumerator CoroutineReplay()
    {
        yield return new WaitForSeconds(2);
        foreach (RecordData rd in recording)
        {
            if (rd.codice == 0 && rd.valid)
                StartCoroutine(Replay(rd));
        }
    }
    
    private IEnumerator Replay(RecordData rd)
    {
        Debug.Log("coroutine per il replay di: " + rd.id_element + " " + rd.tag_element);
        
        GameEvent.stopAllCoroutines = false;
        coroutineReplayStarted = true;
        int i = 0;
        GameObject el = GameObject.Find("Controller").GetComponent<PitchController>().GetElementFromID(rd.id_element, rd.tag_element);
        GameObject ball = GameObject.Find("Controller").GetComponent<PitchController>().GetBall();
        
        while (i<rd.movements.Count && !GameEvent.stopAllCoroutines)
        {
            if (!pause && el != null)
            {
                //rd.element.GetComponent<CharacterController>().Move(rd.movements[i]);
                //Debug.Log(rd.id_element + " " + rd.actions[i]);
                
                
                
                if (rd.actions[i] == Azione.TACKLE && rd.actions[i-1] != Azione.TACKLE)
                {
                    //Debug.Log("TACKLE");
                    el.GetComponent<AnimatorController>().SetParameter((Azione)rd.actions[i], true);
                }
                
                if (rd.actions[i] == Azione.RECEIVE_BALL && rd.actions[i - 1] != Azione.RECEIVE_BALL)
                {
                    //Debug.Log("RECEIVE THE BALL");
                    el.GetComponent<AnimatorController>().SetParameter(Azione.RECEIVE_BALL, true);

                    bool ballCatched = el.GetComponent<Actions>().GetBallCatched();

                    if (ball != null)
                    {
                        if (!ballCatched)
                            ball.transform.SetParent(el.transform);
                        else
                            ball.transform.SetParent(GameObject.Find("ElementiInseriti").transform);
                    }
                   
                    el.GetComponent<Actions>().SetBallCatched(!ballCatched);
                }
                
                if (rd.actions[i] == Azione.PASS_BALL && rd.actions[i-1] != Azione.PASS_BALL)
                {
                    //Debug.Log("PASS THE BALL");
                    el.GetComponent<AnimatorController>().SetParameter(rd.actions[i], true);

                    Vector3 nextTarget = rd.GetNextTargetForKicker(rd.id_element, rd.tag_element);
                    
                    ball.GetComponent<Ball>().StartPassBallTo(nextTarget);
                    ball.transform.SetParent(GameObject.Find("ElementiInseriti").transform);
                    
                   
                }

                
                el.GetComponent<FirstPersonController>().ReplayMove(rd.movements[i].GetVector3());
                el.transform.eulerAngles = rd.angles[i].GetVector3();
                
                
                
            }
            else
            {
                Debug.Log("coroutine bloccata");
            }

            i++;
            yield return null;
        }
        
        
        coroutineReplayStarted = false;
    
        RecordData nextRecordData = NextRecording(rd);
        if (nextRecordData != null && !GameEvent.stopAllCoroutines)
        {
            StartCoroutine(Replay(nextRecordData));
        }

        GameEvent.stopAllCoroutines = false;
    }

    private RecordData NextRecording(RecordData rd_current)
    {
        GameObject current_el = GameObject.Find("Controller").GetComponent<PitchController>()
            .GetElementFromID(rd_current.id_element, rd_current.tag_element);
        foreach (RecordData rd in recording)
        {
            GameObject el = GameObject.Find("Controller").GetComponent<PitchController>()
                .GetElementFromID(rd.id_element, rd.tag_element);
            if (ReferenceEquals(el, current_el) && rd_current.codice + 1 == rd.codice && rd.valid)
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
        GameObject ball = GameObject.Find("Controller").GetComponent<PitchController>().GetBall();
        List<GameObject> allElementsRecordable = PosizionamentoMenu.GetAllElementsRecordable();
        foreach (GameObject elementRec in allElementsRecordable)
        {
            int id_elementRec = 0;
            if (elementRec.GetComponent<Player>())
                id_elementRec = elementRec.GetComponent<Player>().id;
            
            string tag_elementRec = elementRec.tag;
            for (int i = 0; i < recording.Count; i++)
            {
                RecordData rd = (RecordData) recording[i];
                GameObject el = GameObject.Find("Controller").GetComponent<PitchController>()
                    .GetElementFromID(rd.id_element, rd.tag_element);
                int id_el;
                if (el.GetComponent<Player>())
                    id_el = el.GetComponent<Player>().id;
                else
                    id_el = 0;
                string tag_el = el.tag;
                if (id_el == id_elementRec && tag_el == tag_elementRec && rd.codice == codice)
                {
                    //disattivo e attivo il character controller a causa di un bug interno del character controller
                    if (elementRec.GetComponent<CharacterController>())
                        elementRec.GetComponent<CharacterController>().enabled = false;
                    elementRec.transform.position = rd.initialPosition.GetVector3();
                    elementRec.transform.eulerAngles = rd.initialRotation.GetVector3();
                    if (elementRec.GetComponent<CharacterController>())
                    {
                        elementRec.GetComponent<CharacterController>().enabled = true;
                        elementRec.GetComponent<FirstPersonController>().isOnFoot = true;
                    }
                        
                    /*
                    if (elementRec.GetComponent<Ball>())
                    {
                        rd.SetNextTarget(0);
                        elementRec.transform.SetParent(GameObject.Find("ElementiInseriti").transform);
                    }
                    */

                    if (elementRec.GetComponent<Actions>())
                    {
                        elementRec.GetComponent<Actions>().SetBallCatched(false);
                        elementRec.GetComponent<AnimatorController>().SetParameter(Azione.IDLE, true);
                    }

                    if (ball != null)
                    {
                        ball.transform.SetParent(GameObject.Find("ElementiInseriti").transform);
                        ball.transform.position = rd.initialPositionBall.GetVector3();
                        rd.SetNextTarget(0);
                    }
                        
                }
            }
            
        }
    }

    public Vector3 GetInitialPositionOfTheNextAction(GameObject obj)
    {
        int id_obj = 0;
        if (obj.GetComponent<Player>())
            id_obj = obj.GetComponent<Player>().id;
        
        foreach (RecordData rd in recording)
        {
            
            GameObject el = GameObject.Find("Controller").GetComponent<PitchController>()
                .GetElementFromID(rd.id_element, rd.tag_element);
            Debug.Log(el.name);
            int el_id = 0;
            if (el.GetComponent<Player>())
                el_id = el.GetComponent<Player>().id;
            
            //Debug.Log("rd codice " + rd.codice + " ---- action controller codice " + ActionsController.codice);
            if (el_id == id_obj && el.tag.Equals(obj.tag) && rd.codice == ActionsController.codice - 1)
            {
                if (obj.GetComponent<Player>())
                    return rd.finalPosition.GetVector3();
            }
        }
        return Vector3.zero;
    }
    
    public Vector3 GetInitialAnglesOfTheNextAction(GameObject obj)
    {
        int id_obj = obj.GetComponent<Player>().id;
        foreach (RecordData rd in recording)
        {
            GameObject el = GameObject.Find("Controller").GetComponent<PitchController>()
                .GetElementFromID(rd.id_element, rd.tag_element);
            int el_id = 0;
            if (el.GetComponent<Player>())
                el_id = el.GetComponent<Player>().id;
            if (el_id == id_obj && el.tag.Equals(obj.tag) && rd.codice == ActionsController.codice - 1)
            {
                return rd.finalAngles.GetVector3();
            }
        }

        return Vector3.zero;
    }

    public Vector3 GetInitialPositionBallOfTheNextAction()
    {
        foreach (RecordData rd in recording)
        {
            if (rd.codice == codice - 1)
                return rd.finalPositionBall.GetVector3();
        }
        return Vector3.zero;
    }

    public bool IsRecordedTheLastAction(GameObject obj, int numero)
    {
        int thelastone = numero - 1;
        foreach (RecordData rd in recording)
        {
            GameObject el = GameObject.Find("Controller").GetComponent<PitchController>()
                .GetElementFromID(rd.id_element, rd.tag_element);
            if (rd.codice == thelastone && ReferenceEquals(obj, el))
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
            GameObject el = GameObject.Find("Controller").GetComponent<PitchController>()
                .GetElementFromID(rd.id_element, rd.tag_element);
            //Debug.Log("for " + rd.element.name);
            if (ReferenceEquals(el, currentObjSelected))
                count++;
        }

        return count;
    }

    public ArrayList GetAllActionsRegistered()
    {
        return recording;
    }

    public ArrayList GetActionsRegistered(GameObject currentObjSelected)
    {
        ArrayList recordingOfTheElement = new ArrayList();
        foreach (RecordData rd in recording)
        {
            GameObject el = GameObject.Find("Controller").GetComponent<PitchController>()
                .GetElementFromID(rd.id_element, rd.tag_element);
            //Debug.Log("for " + rd.element.name);
            if (ReferenceEquals(el, currentObjSelected))
                recordingOfTheElement.Add((rd));
        }
        return recordingOfTheElement;
    }

    public void SetActionsRegistered(ArrayList recording)
    {
        this.recording = recording;
    }

    public RecordData GetAction(ref GameObject currentObjSelected, int indice)
    {
        ArrayList recordingOfTheElement = GetActionsRegistered(currentObjSelected);
        
        if (indice < recordingOfTheElement.Count)
        {
            foreach (RecordData rd in recordingOfTheElement)
            {
                GameObject el = GameObject.Find("Controller").GetComponent<PitchController>()
                    .GetElementFromID(rd.id_element, rd.tag_element);
                if (el.GetInstanceID() == currentObjSelected.GetInstanceID() && indice == rd.codice)
                {
                    return (RecordData)rd;
                }
            }
        }
        

        return null;
    }

    public void RemoveAllRecordingsFor(GameObject elementToRemove)
    {
        for (int i = 0; i < recording.Count; i++)
        {
            RecordData rd = (RecordData) recording[i];
            GameObject el = GameObject.Find("Controller").GetComponent<PitchController>()
                .GetElementFromID(rd.id_element, rd.tag_element);
            if (ReferenceEquals(el, elementToRemove))
                recording.RemoveAt(i);
        }
    }

    public void RemoveRecording(RecordData recordData)
    {
        for (int i = 0; i < recording.Count; i++)
        {
            RecordData rd = (RecordData) recording[i];
            if (ReferenceEquals(rd, recordData))
                recording.RemoveAt(i);
        }
        
    }

    public void AddNewPossessionInRecordingForTheBall(GameObject ball)
    {
        foreach (RecordData rdi in recording)
        {
            GameObject el = GameObject.Find("Controller").GetComponent<PitchController>()
                .GetElementFromID(rdi.id_element, rdi.tag_element);
            if (el != null)
                if (el.tag.Equals("Ball"))
                {
                    //rdi.countChangeOwn++;
                    return;
                }
               
        }
        /*
        RecordData rd = new RecordData(ball);
        //rd.countChangeOwn++;
        recording.Add(rd);*/
    }
    
    public void AddTargetInRecordingForTheBall(GameObject ball, int kickerID, string kickerTag)
    {
        /*Debug.Log(recording.Count);
        foreach (RecordData rdi in recording)
        {
            GameObject el = GameObject.Find("Controller").GetComponent<PitchController>()
                .GetBall();
            if (el.tag.Equals(rdi.tag_element))
            {
                ElementIdentifier ei = new ElementIdentifier(kickerID, kickerTag);
                TargetForElement te = new TargetForElement(new Vector3Surrogate(ball.GetComponent<Ball>().GetTarget()), ei);
                Debug.Log("target " + te.target.GetVector3() + " per il kicker " + ei.id + " " + ei.tag);
                //rdi.targets_kickerID.Add(te);
                target_kickerID_layer.Add(te);
                return;
            }
               
        }*/
        //RecordData rd = new RecordData(ball);
        //rd.positions.Add(ball.GetComponent<Ball>().GetTarget());
        //rd.finalPosition = ball.GetComponent<Ball>().GetTarget();
        //recording.Add(rd);
        ElementIdentifier ei = new ElementIdentifier(kickerID, kickerTag);
        TargetForElement te = new TargetForElement(new Vector3Surrogate(ball.GetComponent<Ball>().GetTarget()), ei);
        
        target_kickerID_layer.Add(te);
    }


    public bool AllTheActionsValid(GameObject currentObjSelected)
    {
        foreach (RecordData rdi in recording)
        {
            GameObject el = GameObject.Find("Controller").GetComponent<PitchController>()
                .GetElementFromID(rdi.id_element, rdi.tag_element);
            if (ReferenceEquals(el, currentObjSelected) && rdi.valid == false)
                return false;
        }

        return true;
    }
}
