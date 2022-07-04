using UnityEngine;
using UnityEngine.EventSystems;

public class Cestino : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        /*
         * Se droppo un icona sul cestino:
         *  - elimino l'elemento corrispondente dal terreno di gioco
         *  - elimino le registrazioni che riguardano quell'elemento
         *  - elimino l'icona
         */
        
        GameObject obj = eventData.pointerDrag;
        GameObject elementInThePitch = obj.GetComponent<DragDrop>().GetElementInThePitch();
        GameObject.Find("Controller").GetComponent<ActionsController>().RemoveAllRecordingsFor(elementInThePitch);
        Destroy(elementInThePitch);
        Destroy(obj);
        
    }
}
