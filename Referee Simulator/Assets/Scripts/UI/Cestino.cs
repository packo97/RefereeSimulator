using UnityEngine;
using UnityEngine.EventSystems;

public class Cestino : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject obj = eventData.pointerDrag;
        GameObject elementInThePitch = obj.GetComponent<DragDrop>().GetElementInThePitch();
        GameObject.Find("Controller").GetComponent<ActionsController>().RemoveRecording(elementInThePitch);
        Destroy(elementInThePitch);
        Destroy(obj);
        
    }
}
