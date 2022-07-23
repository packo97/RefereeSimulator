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

        if (GameObject.Find("ActionMenu"))
        {
            GameObject currentElement = GameObject.Find("ActionMenu").GetComponent<ActionsMenu>().GetCurrentObject();
            Player player = elementInThePitch.GetComponent<Player>();
            Referee referee = elementInThePitch.GetComponent<Referee>();
        
            if (player != null && currentElement.GetComponent<Player>())
            {
                if (player.id == currentElement.GetComponent<Player>().id && player.tag.Equals(currentElement.tag))
                    GameObject.Find("ActionMenu").SetActive(false);
            }
            else if (referee != null && currentElement.GetComponent<Referee>())
            {
            
                if (referee.tag.Equals(currentElement.tag))
                    GameObject.Find("ActionMenu").SetActive(false);
            }
        }
        
        Destroy(elementInThePitch);
        Destroy(obj);
        
        
    }
}
