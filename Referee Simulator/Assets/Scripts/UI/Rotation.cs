using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    private GameObject elementInThePitch;
    [SerializeField] private GameObject indicator;
    private void Start()
    {
        //elementInThePitch = GetComponentInParent<DragDrop>().GetElementInThePitch();
        elementInThePitch = PosizionamentoMenu.GetCurrentElementSelected();
    }

    public void SetElement(GameObject obj)
    {
        elementInThePitch = obj;
        float numero = (360 - elementInThePitch.transform.eulerAngles.y + 90) % 360;
        indicator.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, numero);
        
    }
    
    public void RightRotation()
    {
        elementInThePitch.transform.Rotate(0,10,0, Space.Self);
        indicator.transform.Rotate(0,0,-10, Space.Self);
    }
    
    public void LeftRotation()
    {
        elementInThePitch.transform.Rotate(0,-10,0, Space.Self);
        indicator.transform.Rotate(0,0,10, Space.Self);
    }
}
