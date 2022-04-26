using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Camera cameraBiliardino;
    [SerializeField] private GameObject objPrefab;
    [SerializeField] private GameObject directionPrefab;
    
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    
    private bool _isDropped = false;
    private GameObject obj;
    private Vector3 positionBeforeDrag;
    
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();

       
    }

    private void Start()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        cameraBiliardino = GameObject.Find("CameraBiliardino").GetComponent<Camera>();
    }
    

    public void OnBeginDrag(PointerEventData eventData)
    {
        /*
         * All'inizio del drag creo un icona nello stessa position dell'icona draggata
         * 
         */
        
        if (!_isDropped)
        {
            Transform parent = gameObject.transform.parent;
            Instantiate(gameObject, parent);
        }
        
        /*
         * Oscuro leggermente l'icona draggata
         *
         * Salvo la posizione iniziale che potrebbe servirmi nel caso in cui il dragEnd non andasse a buon fine
         * Diminuisco la dimensione dell'icona draggata
         */
        _canvasGroup.alpha = .6f;
        _canvasGroup.blocksRaycasts = false;
        positionBeforeDrag = transform.position;
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(15, 15);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        /*
         * Ripristino il colore dell'icona
         */
        
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
        
        /*
         * Genero un raggio a partire dalla posizione del mouse, se colpisce qualcosa (il terreno di gioco), creo un elemento
         * (arbitro, calciatore A o B, pallone) nella posizione di collisione.
         * Aggiungo l'elemento al gameobject "ElementiInseriti".
         * Aggiungo l'icona corrispondente al gameobject "IconeInserite".
         * Aggiungo all'icona i comandi per la rotazione (tranne se è un pallone).
         * Se l'elemento era stato già droppato, aggiorno solo la posizione dell'elemento.
         *          * 
         */
        
        RaycastHit hit;
        Ray ray = cameraBiliardino.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out hit)) {
            Transform objectHit = hit.transform;
            
            if (!_isDropped)
            {
                obj = Instantiate(objPrefab, GameObject.Find("ElementiInseriti").transform) as GameObject;
                obj.transform.position = hit.point;
                if (obj.tag.Equals("Ball"))
                    obj.transform.position = new Vector3(hit.point.x, 0.17f, hit.point.z);
                _isDropped = true;
                if (obj.name.Contains("Referee"))
                {
                    GameEvent.isRefereeDropped = true;
                }
                
                gameObject.transform.SetParent(GameObject.Find("IconeInserite").transform);
                if (directionPrefab != null)
                    Instantiate(directionPrefab, gameObject.transform);
            }
            else
            {
                if (obj.tag.Equals("Ball"))
                    obj.transform.position = new Vector3(hit.point.x, 0.17f, hit.point.z);
                else
                    obj.transform.position = hit.point;
            }
            
        }
        
        else
        {
            /*
             * Se il raggio non colpisce niente:
             * - se non è mai stato droppato -> distruggo l'icona draggata
             * - se era stato droppato -> ripristino la posizione dell'icona draggata a quella prima del drag.
             */
            
            if(!_isDropped)
                Destroy(gameObject);
            else
            {
                transform.position = positionBeforeDrag;
            }
        }
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        /*
         * Per spostare l'icona insieme al puntatore del mouse
         */
        
        _rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
    
    public GameObject GetElementInThePitch()
    {
        return obj;
    }

    public void SetElementInThePitch(GameObject obj)
    {
        this.obj = obj;
    }
    
    public void SetDropped(bool dropped)
    {
        _isDropped = dropped;
    }

    public void SetSelected()
    {
        PosizionamentoMenu.SetCurrentElementSelected(obj);
    }
}
