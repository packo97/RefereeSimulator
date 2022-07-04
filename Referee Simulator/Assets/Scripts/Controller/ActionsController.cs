using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cursor = UnityEngine.Cursor;

public class ActionsController : MonoBehaviour
{
    public enum Azione
    {
        IDLE,
        RUNNING,
        TACKLE,
        FALLEN_AFTER_TACKLE,
        PASS_BALL,
        RECEIVE_BALL
    };
    
    private List<(int,ElementIdentifier,Task)> tasks = new List<(int,ElementIdentifier,Task)>();
    public static bool pause = false;
    public static int layer = 0;
    
    private GameObject _element;
    private int id_current_element;
    private string tag_current;
    public List<TargetForElement> target_kickerID_layer;
    
    private bool recordingMode;
    private ArrayList recording;
    private bool coroutineReplayStarted;

    [SerializeField] private EditorMenu editorMenu;
    [SerializeField] private InfoBox infoBox;

    private void Start()
    {
        recording = new ArrayList();
        recordingMode = false;
        coroutineReplayStarted = false;
        target_kickerID_layer = new List<TargetForElement>();
    }

    private void Update()
    {
        //se sono nella modalità azione (cioè quella in cui si possono compiere movimenti)
        if (GameEvent.isActionOpen)
        {
            if(!recordingMode)
                infoBox.SetText("Press Enter to move player and start recording\n" +
                                "Press R to replay\n" +
                                "Press P to back to initial position\n" +
                                "Press ESC to exit", InfoBox.TypeOfMessage.INFO, false);
            
            PosizionamentoMenu.GetCurrentElementSelected().GetComponentInChildren<Camera>().depth = 1;
            
            // Se premo il tasto ENTER
            if (Input.GetKeyDown(KeyCode.Return))
            {
                infoBox.SetText("Press P to stop recording", InfoBox.TypeOfMessage.INFO, false);
                
                // abilito il movimento del player corrente
                _element = PosizionamentoMenu.GetCurrentElementSelected();
                _element.GetComponent<FirstPersonController>().enabled = true;
                _element.GetComponent<Actions>().enabled = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                
                // resetto variabili di utilizzo
                recordingMode = true;
                GameEvent.stopAllCoroutines = false;
                target_kickerID_layer.Clear();
                
                // imposto le componenti che identificano il player univocamente sul terreno di gioco
                id_current_element = _element.GetComponent<Player>().id;
                tag_current = _element.tag;

                // rimuovo la recording dell'elemento corrente se esiste già nel layer
                RecordData rd_to_delete = GetRecordDataFromID_Tag_Codice(id_current_element, tag_current);
                recording.Remove(rd_to_delete);
                
                // se ho cancellato qualcosa potrei aver invalidato altre azioni
                if (rd_to_delete != null)
                {
                    SetToInvalidRecordingIfNecessary(rd_to_delete);
                }
                
                // faccio partire il replay dal layer corrente per tutti gli elementi tranne quello selezionato
                foreach (RecordData rd in recording)
                {
                    if ((rd.idElement != id_current_element || rd.tagElement != tag_current) && rd.layer == layer &&
                        rd.valid)
                    {
                        StartCoroutine(Replay(rd));
                    }
                }
                
                // faccio partire la registrazione solo dell'elemento corrente
                StartCoroutine(StartRecordingElement(_element));
            }
            // se premo il tasto ESC
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                /*
                 * Resetto le variabili di utilizzo
                 * Abilito il puntatore del mouse
                 * Switcho i comandi del posizionamento
                 * Mando in background la camera del player selezionato
                 * Imposto gli elementi alla posizione iniziale del layer 0
                 */
                GameEvent.stopAllCoroutines = true;
                PosizionamentoMenu.GetCurrentElementSelected().GetComponent<FirstPersonController>().enabled = false;
                PosizionamentoMenu.GetCurrentElementSelected().GetComponent<Actions>().enabled = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                editorMenu.OpenComandiPosizionamento();
                GameEvent.isActionOpen = false;
                PosizionamentoMenu.GetCurrentElementSelected().GetComponentInChildren<Camera>().depth = -1;
                recordingMode = false;
                
                SetAllElementsToInitialPosition(0);
                infoBox.gameObject.SetActive(false);
            }
            // se premo il tasto P
            else if (Input.GetKeyDown(KeyCode.P))
            {
                /*
                 * Stop della registrazione
                 * Resetto il target for kicker
                 */
                GameEvent.stopAllCoroutines = true;
                StopRecording();

                foreach (RecordData rd in recording)
                {
                    rd.ResetTargetForKicker();
                }
                
            }
            // se non sto registrando e premo il tasto R
            else if (!recordingMode && Input.GetKeyDown(KeyCode.R))
            {
                // imposto tutti gli elementi alla posizione iniziale del layer 0
                SetAllElementsToInitialPosition(0);
                //faccio partire il replay
                StartReplay();
            }
        }
    }
    
    public RecordData GetRecordDataFromID_Tag_Codice(int id, string tag)
    {
        for (int i = 0; i < recording.Count; i++)
        {
            RecordData rd = (RecordData) recording[i];
            if (rd.idElement == id && rd.tagElement.Equals(tag) && layer == rd.layer)
                return rd;
        }

        return null;
    }
    private void SetToInvalidRecordingIfNecessary(RecordData rdToDelete)
    {
        for (int i = 0; i < rdToDelete.actions.Count; i++)
        {
            Azione azione = rdToDelete.actions[i];
            if (azione == Azione.RECEIVE_BALL || azione == Azione.PASS_BALL)
            {
                for (int j = 0; j < recording.Count; j++)
                {
                    RecordData rdi = (RecordData)recording[j];
                    if (rdToDelete.idElement != rdi.idElement ||
                        !rdToDelete.tagElement.Equals(rdi.tagElement))
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

    private IEnumerator StartRecordingElement(GameObject element)
    {
        /*
         * Registro tutte le azioni dell'elemento corrente
         * 
         */
        
        /*
         * Inizializzo la lista per i movimenti, le angolazioni e le azioni
         * 
         */
        List<Vector3Surrogate> movements = new List<Vector3Surrogate>();
        List<Vector3Surrogate> angles = new List<Vector3Surrogate>();
        List<Azione> actions = new List<Azione>();
        
        /*
         * Prendo il pallone e mi salvo la posizione iniziale del pallone
         * 
         */
        GameObject ball = GameObject.Find("Controller").GetComponent<PitchController>().GetBall();
        Vector3 initialPositionBall = Vector3.zero;
        if (ball != null) 
            initialPositionBall = ball.transform.position;
        
        // inizializzo il record data
        RecordData rd = new RecordData(element);
        while (recordingMode)
        {
            /*
             * finchè sono in registrazione:
             *                              - aggiungi alla lista dei movimento, il movimento
             *                              - aggiungi alla lista delle angolazioni, l'angolo
             *                              - aggiungi alla lista delle azioni, l'azione(indicata dallo stato)
             */
            if (!pause)
            {
                if (element.GetComponent<FirstPersonController>())
                    movements.Add(new Vector3Surrogate(element.GetComponent<FirstPersonController>().GetMoveDirection()));
                angles.Add(new Vector3Surrogate(element.transform.eulerAngles));
                if (element.GetComponent<AnimatorController>())
                    actions.Add(element.GetComponent<AnimatorController>().GetState());
                
            }
            
            yield return null;
        }
        
        /*
         * Al termine della registrazione:
         *                              - imposto la lista dei movimenti nel record data
         *                              - imposto la lista delle angolazioni nel record data
         *                              - imposto la lista delle angolazioni nel record data
         *                              - imposto la posizione finale del player
         *                              - imposto l'angolazione finale del player
         *                              - imposto la lista dei target per il passaggio
         *                              - imposto la posizione iniziale e finale del pallone
         *                              - mi salvo se il pallone era in possesso o meno del player alla fine della registrazione
         */
        
        rd.SetMovements(movements);
        rd.SetAngles(angles);
        rd.SetActions(actions);
        
        rd.SetFinalPosition(new Vector3Surrogate(element.transform.position));
        rd.SetFinalAngles(new Vector3Surrogate(element.transform.eulerAngles));
        
        rd.targetsKickerID.Clear();
        foreach (TargetForElement te in target_kickerID_layer)
        {
            rd.targetsKickerID.Add(new TargetForElement(te.target, te.elementIdentifier)); 
        }

        //rd.kickerID = new ElementIdentifier(id_current_element, tag_current);
        
        rd.initialPositionBall = new Vector3Surrogate(initialPositionBall);
        if (ball != null)
        {
            rd.finalPositionBall = new Vector3Surrogate(ball.transform.position);
        }

        if (element.GetComponent<Actions>())
            rd.ballCatched = element.GetComponent<Actions>().GetBallCatched();
        
        // aggiungo la registrazione alla lista di registrazioni
        recording.Add(rd);
        
        // imposto tutti gli elementi alla posizione iniziale
        SetAllElementsToInitialPosition(layer);
        
    }

    private void StopRecording()
    {
        /*
         * Stoppa registrazione, impostando la variabile recordingMode a false
         * Disabilito tutte le azioni del player selezionato
         * Imposto tutti gli elementi alla posizione iniziale in base al layer corrente
         */
        recordingMode = false;
        PosizionamentoMenu.GetCurrentElementSelected().GetComponent<FirstPersonController>().enabled = false;
        PosizionamentoMenu.GetCurrentElementSelected().GetComponent<Actions>().enabled = false;

        StartCoroutine(StartSetAllElementsToInitialPosition(layer));
    }

    private IEnumerator StartSetAllElementsToInitialPosition(int layer)
    {
        /*
         * Coroutine per settare tutti gli elementi alla posizione iniziale dopo un 1 secondo
         */
        yield return new WaitForSeconds(1);
        SetAllElementsToInitialPosition(layer);
    }

    public void StartReplay()
    {
        /*
         * Metodo per far iniziare il Replay
         */
        StartCoroutine(CoroutineReplay());
    }

    private IEnumerator CoroutineReplay()
    {
        /*
         * Coroutine per far iniziare il replay dopo un secondo
         */
        yield return new WaitForSeconds(1);

        // Cerco i layer su cui far partire il replay
        List<int> layers = new List<int>();
        foreach (RecordData rd in recording)
        {
            if (!layers.Contains(rd.layer))
            {
                layers.Add(rd.layer);
            }
        }

        /*
         * Faccio partire il replay le registrazioni layer per layer,
         * aspettando che tutte le registrazioni del layer n finiscano prima
         * di far partire le registrazioni del layer n+1
         */
        
        foreach (int layer in layers )
        {
            foreach (RecordData rd in recording)
            {
                if (rd.layer == layer && rd.valid)
                {
                    //il task mi fa partire la coroutine del replay
                    Task t = new Task(Replay(rd));
                    ElementIdentifier ei = new ElementIdentifier(rd.idElement, rd.tagElement);
                    tasks.Add((rd.layer, ei, t));
                }
            }
            while(AreThereCoroutineInRunningOfLayer(layer))
            {
                //se ci sono replay dello stesso layer ancora in esecuzione, aspetto (rimango bloccato nel while)
                yield return null;
            }
        }
    }
    
    private IEnumerator Replay(RecordData rd)
    {
        //Debug.Log("coroutine per il replay di: " + rd.id_element + " " + rd.tag_element + " durata: " + rd.movements.Count);
        
        /*
         * Coroutine del replay
         *
         * Imposto le variabili di utilizzo
         * Prendo l'elemento corrispondente al record data
         * Prendo il pallone
         * 
         */
        GameEvent.stopAllCoroutines = false;
        coroutineReplayStarted = true;
        int i = 0;
        GameObject el = GameObject.Find("Controller").GetComponent<PitchController>().GetElementFromID(rd.idElement, rd.tagElement);
        GameObject ball = GameObject.Find("Controller").GetComponent<PitchController>().GetBall();
        
        while (i<rd.movements.Count && !GameEvent.stopAllCoroutines)
        {
            /*
             * Imposto ad ogni frame:
             *                      - un azione tra TACKLE, RECEIVE BALL, PASS_BALL
             *                      - il movimento e le angolazioni
             * 
             */
            if (!pause && el != null)
            {
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
                    Vector3 nextTarget = rd.GetNextTargetForKicker(rd.idElement, rd.tagElement);
                    ball.GetComponent<Ball>().StartPassBallTo(nextTarget);
                    ball.transform.SetParent(GameObject.Find("ElementiInseriti").transform);
                    el.GetComponent<Actions>().SetBallCatched(false);
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
        GameEvent.stopAllCoroutines = false;
    }

    private bool AreThereCoroutineInRunningOfLayer(int layer)
    {
        /*
         * Verifico se ci sono coroutine in running con lo stesso layer del parametro
         * 
         */
        foreach ((int, ElementIdentifier, Task) layer_id_task in tasks)
        {
            int current_layer = layer_id_task.Item1;
            ElementIdentifier current_id = layer_id_task.Item2;
            Task current_task = layer_id_task.Item3;
            
            if (current_layer == layer && current_task.Running)
            {
                return true;
            }
        }

        return false;
    }
    
    public void DeleteRecordings()
    {
        /*
         * Cancello tutte le registrazioni
         * 
         */
        recording.Clear();
    }

    public void SetAllElementsToInitialPosition(int layer)
    {
        /*
         * Questo metodo è utilizzato per impostare gli elementi alla posizione iniziale del layer passato come parametro
         * 
         */
        GameObject ball = GameObject.Find("Controller").GetComponent<PitchController>().GetBall();
        List<GameObject> allElementsRecordable = PosizionamentoMenu.GetAllElementsRecordable();
        
        /*
         * Per ogni elemento registrabile
         */
        foreach (GameObject elementRec in allElementsRecordable)
        {
            int id_elementRec = 0;
            if (elementRec.GetComponent<Player>())
                id_elementRec = elementRec.GetComponent<Player>().id;
            
            string tag_elementRec = elementRec.tag;
            
            /*
             * Cerco all'interno delle registrazioni
             */
            for (int i = 0; i < recording.Count; i++)
            {
                //recupero dal record data il gameobject corrispondente utilizzando id e tag
                RecordData rd = (RecordData) recording[i];
                GameObject el = GameObject.Find("Controller").GetComponent<PitchController>()
                    .GetElementFromID(rd.idElement, rd.tagElement);
                
                int id_el = 0;
                if (el.GetComponent<Player>())
                    id_el = el.GetComponent<Player>().id;
                string tag_el = el.tag;
                
                //se l'id e il tag sono uguali tra i due elementi e il layer corrisponde allora ho trovato
                //il record data che contiene le informazioni che devo settare sull'elemento
                if (id_el == id_elementRec && tag_el == tag_elementRec && rd.layer == layer)
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
                    
                    //reset delle variabili di default
                    if (elementRec.GetComponent<Actions>())
                    {
                        elementRec.GetComponent<Actions>().SetBallCatched(false);
                        elementRec.GetComponent<AnimatorController>().SetParameter(Azione.IDLE, true);
                    }
                    //reset delle variabili di default per il pallone
                    if (ball != null)
                    {
                        ball.transform.SetParent(GameObject.Find("ElementiInseriti").transform);
                        ball.transform.position = rd.initialPositionBall.GetVector3();
                        
                    }
                        
                }
            }
            
        }
    }

    public Vector3 GetInitialPositionOfTheLayer(GameObject obj, int layer)
    {
        /*
         * Questo metodo è utilizzato per restituire la posizione initiale di un determinato elemento in un determinato layer
         * 
         */
        
        int id_obj = 0;
        if (obj.GetComponent<Player>())
            id_obj = obj.GetComponent<Player>().id;
        
        //Cerco nelle registrazioni
        foreach (RecordData rd in recording)
        {
            GameObject el = GameObject.Find("Controller").GetComponent<PitchController>()
                .GetElementFromID(rd.idElement, rd.tagElement);
            
            int el_id = 0;
            if (el.GetComponent<Player>())
                el_id = el.GetComponent<Player>().id;
            
            //se id e tag sono uguali ed il layer corrisponde anche ho trovato il record data che contiene le infomrazioni
            if (el_id == id_obj && el.tag.Equals(obj.tag) && rd.layer == layer)
            {
                if (obj.GetComponent<Player>())
                    return rd.initialPosition.GetVector3();
            }
        }
        
        return Vector3.zero;
    }
    public Vector3 GetInitialAnglesOfTheLayer(GameObject obj, int layer)
    {
        /*
         * Questo metodo è utilizzato per restituire gli angoli di un determinato elemento in un determinato layer.
         * 
         */
        
        int id_obj = obj.GetComponent<Player>().id;
        
        //Cerco nelle registrazioni
        foreach (RecordData rd in recording)
        {
            GameObject el = GameObject.Find("Controller").GetComponent<PitchController>()
                .GetElementFromID(rd.idElement, rd.tagElement);
            int el_id = 0;
            if (el.GetComponent<Player>())
                el_id = el.GetComponent<Player>().id;
            
            //Se id e tag sono uguali e il layer corrisponde, ho trovato il record data che contiene le informazioni
            if (el_id == id_obj && el.tag.Equals(obj.tag) && rd.layer == layer)
            {
                return rd.initialRotation.GetVector3();
            }
        }
        
        return Vector3.zero;
    }
    public Vector3 GetFinalPositionOfTheLayer(GameObject obj, int layer)
    {
        /*
         * Questo metodo è utilizzato per restituire la posizione finale di un determinato elemento in un determinato layer
         * 
         */
        
        int id_obj = 0;
        if (obj.GetComponent<Player>())
            id_obj = obj.GetComponent<Player>().id;

        //Cerco nelle registrazioni
        foreach (RecordData rd in recording)
        {
            GameObject el = GameObject.Find("Controller").GetComponent<PitchController>()
                .GetElementFromID(rd.idElement, rd.tagElement);
            int el_id = 0;
            if (el.GetComponent<Player>())
                el_id = el.GetComponent<Player>().id;
            
            //se id e tag sono uguali e il layer corrisponde, ho trovato il record data che contiene le informazioni
            if (el_id == id_obj && el.tag.Equals(obj.tag) && rd.layer == layer)
            {
                if (obj.GetComponent<Player>())
                    return rd.finalPosition.GetVector3();
            }
        }
        
        return Vector3.zero;
    }
    
    public Vector3 GetFinalAnglesOfTheLayer(GameObject obj, int layer)
    {
        /*
         * Questo metodo è utilizzato per restituire gli angoli di un determinato elemento in un determinato layer.
         */
        
        int id_obj = obj.GetComponent<Player>().id;
       
        //Cerco nelle registrazioni
        foreach (RecordData rd in recording)
        {
            GameObject el = GameObject.Find("Controller").GetComponent<PitchController>()
                .GetElementFromID(rd.idElement, rd.tagElement);
            int el_id = 0;
            if (el.GetComponent<Player>())
                el_id = el.GetComponent<Player>().id;
            
            //se id e tag sono uguali e il layer corrisponde, ho trovato il record data che contiene le informazioni
            if (el_id == id_obj && el.tag.Equals(obj.tag) && rd.layer == layer)
            {
                return rd.finalAngles.GetVector3();
            }
        }

        return Vector3.zero;
    }

    public Vector3 GetInitialPositionBallOfTheNextAction()
    {
        /*
         * Questo metodo è utilizzato per ottenere la posizione iniziale del pallone nella prossima azione (ancora da registrare).
         * Se l'azione corrente è nel layer n, verrà restituita la posizione finale del pallone nel layer n-1;
         * 
         */
        
        Vector3 finalPositionBall = Vector3.zero;
        //Cerco nelle registrazione
        foreach (RecordData rd in recording)
        {
            //Se il layer corrisponde e la posizione finale non è nulla
            if (rd.layer == layer - 1 && rd.finalPositionBall!= null)
            {
                finalPositionBall = rd.finalPositionBall.GetVector3();
            }
                
        }

        return finalPositionBall;
    }

    public (bool, GameObject) IsBallCatchedOnTheLastLayer(int layer)
    {
        /*
         * Questo metodo è utilizzato per sapere se in un determinato layer il pallone era stato catturato.
         * Il metodo restituisce una coppia il cui primo elemento indica se la risposta è negativa o positiva,
         * mentre il secondo elemento indica da chi è stato catturato il pallone in quel layer.
         */
        
        //Cerco nelle registrazioni
        foreach (RecordData rd in recording)
        {
           //Se il layer corrisponde e il pallone è stato catturato
            if (rd.layer == layer - 1 && rd.ballCatched)
            {
                GameObject element = GetComponent<PitchController>().GetElementFromID(rd.idElement, rd.tagElement);
                return (rd.ballCatched, element);
            }
        }
        
        return (false, null);
    }
    
    

    public bool IsRecordedTheLastAction(GameObject obj, int numero)
    {
        /*
         * Questo metodo è utilizzato per verificare se, per un determinato oggetto, l'ultima azione è stata registrata
         * 
         */
        
        int thelastone = numero - 1;
        foreach (RecordData rd in recording)
        {
            GameObject el = GameObject.Find("Controller").GetComponent<PitchController>()
                .GetElementFromID(rd.idElement, rd.tagElement);
            if (rd.layer == thelastone && ReferenceEquals(obj, el))
            {
                return true;
            }
        }
        return false;
    }

    public ArrayList GetAllActionsRegistered()
    {
        /*
         * Questo metodo restituisce le registrazioni
         * 
         */
        return recording;
    }

    public ArrayList GetActionsRegistered(GameObject currentObjSelected)
    {
        /*
         * Questo metodo restituisce una lista contenente tutti i record data
         * che riguardano l'elemento passato come parametro
         * 
         */
        
        ArrayList recordingOfTheElement = new ArrayList();
        foreach (RecordData rd in recording)
        {
            GameObject el = GameObject.Find("Controller").GetComponent<PitchController>()
                .GetElementFromID(rd.idElement, rd.tagElement);
            
            if (ReferenceEquals(el, currentObjSelected))
                recordingOfTheElement.Add((rd));
        }
        return recordingOfTheElement;
    }

    public void SetActionsRegistered(ArrayList recording)
    {
        /*
         * Questo metodo è utilizzato per settare la lista delle registrazione con un altra lista (serve per il LOAD SYSTEM)
         */
        this.recording = recording;
    }

    public RecordData GetRecordData(ref GameObject currentObjSelected, int layer)
    {
        /*
         * Questo metodo è utilizzato per ottenere il RecordData di un determinato elemento in un determinato layer
         * 
         */
        ArrayList recordingOfTheElement = GetActionsRegistered(currentObjSelected);
        
        if (layer < recordingOfTheElement.Count)
        {
            foreach (RecordData rd in recordingOfTheElement)
            {
                GameObject el = GameObject.Find("Controller").GetComponent<PitchController>()
                    .GetElementFromID(rd.idElement, rd.tagElement);
                //se l'elemento ha lo stesso id e il layer corrisponde, ho trovato il record data di interesse
                if (el.GetInstanceID() == currentObjSelected.GetInstanceID() && layer == rd.layer)
                {
                    return rd;
                }
            }
        }
        return null;
    }

    public void RemoveAllRecordingsFor(GameObject elementToRemove)
    {
        /*
         * Questo metodo è utilizzato per eliminare tutte le registrazioni che riguardano un determinato elemento
         * 
         */
        for (int i = 0; i < recording.Count; i++)
        {
            RecordData rd = (RecordData) recording[i];
            GameObject el = GameObject.Find("Controller").GetComponent<PitchController>()
                .GetElementFromID(rd.idElement, rd.tagElement);
            //se l'elemento è lo stesso, allora elimino la registrazione nella posizione i
            if (ReferenceEquals(el, elementToRemove))
                recording.RemoveAt(i);
        }
    }

    public void RemoveRecording(RecordData recordData)
    {
        /*
         * Questo metodo è utilizzato per eliminare la registrazione passata come parametro
         * 
         */
        for (int i = 0; i < recording.Count; i++)
        {
            RecordData rd = (RecordData) recording[i];
            if (ReferenceEquals(rd, recordData))
                recording.RemoveAt(i);
        }
    }

   public void AddTargetInRecordingForTheBall(GameObject ball, int kickerID, string kickerTag)
    {
        /*
         * Questo metodo è utilizzato per aggiungere un target alla lista dei target (sistema dei passaggi)
         * 
         */
        ElementIdentifier ei = new ElementIdentifier(kickerID, kickerTag);
        TargetForElement te = new TargetForElement(new Vector3Surrogate(ball.GetComponent<Ball>().GetTarget()), ei);
        target_kickerID_layer.Add(te);
    }


    public bool AllTheActionsValid(GameObject currentObjSelected)
    {
        /*
         * Questo metodo è utilizzato per verificare se tutte le azioni sono valide per un determinato elemento
         * 
         */
        foreach (RecordData rdi in recording)
        {
            GameObject el = GameObject.Find("Controller").GetComponent<PitchController>()
                .GetElementFromID(rdi.idElement, rdi.tagElement);
            if (ReferenceEquals(el, currentObjSelected) && rdi.valid == false)
                return false;
        }

        return true;
    }
    
    
}
