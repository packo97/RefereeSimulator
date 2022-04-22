using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoBox : MonoBehaviour
{
    public enum TypeOfMessage
    {
        INFO, SUCCESS, ERROR
    }
    
    private Text _text;
    private Image _image;
    
    void Start()
    {
        _text = GetComponentInChildren<Text>();
        _image = GetComponentInChildren<Image>();
        gameObject.SetActive(false);
    }

    public void SetText(string text, TypeOfMessage typeOfMessage)
    {
        gameObject.SetActive(true);
        if (typeOfMessage == TypeOfMessage.INFO)
            _image.color = new Color32(78, 210, 167, 100);
        else if (typeOfMessage == TypeOfMessage.ERROR)
            _image.color = new Color32(226, 28, 7, 100);
        else if (typeOfMessage == TypeOfMessage.SUCCESS)
            _image.color = new Color32(7, 226, 68, 100);

        _text.text = text;

        StartCoroutine(DeleteMessageAfter3Seconds());
    }

    private IEnumerator DeleteMessageAfter3Seconds()
    {
        yield return new WaitForSeconds(3);
        _text.text = "";
        _image.color = new Color(255f, 255f, 255f, 100f);
        gameObject.SetActive(false);
    }
}
