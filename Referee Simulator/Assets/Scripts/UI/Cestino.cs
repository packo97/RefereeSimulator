using UnityEngine;
using UnityEngine.EventSystems;

public class Cestino : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject obj = eventData.pointerDrag;
        Destroy(obj.GetComponent<DragDrop>().GetElementInThePitch());
        Destroy(obj);
    }
}
