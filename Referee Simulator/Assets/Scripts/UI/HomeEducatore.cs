using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeEducatore : MonoBehaviour
{
    [SerializeField] private EditorMenu _editorMenu;
    [SerializeField] private ManagerMenu _managerMenu;

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
            //UnityEditor.EditorApplication.isPlaying = false;
        }
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
    
    
    public void OpenEditor()
    {
        gameObject.SetActive(false);
        _managerMenu.gameObject.SetActive(false);
        _editorMenu.OpenEditor();
    }
    
    public void CloseEditor()
    {
        gameObject.SetActive(true);
        _editorMenu.CloseEditor();
    }
    
    public void OpenManager()
    {
        _editorMenu.DestroyAllElementsInserted();
        gameObject.SetActive(false);
        GameEvent.isManagerOpen = true;
        _managerMenu.OpenManager();
    }

    public void CloseManager()
    {
        gameObject.SetActive(true);
        GameEvent.isManagerOpen = false;
        _managerMenu.CloseManager();
    }

    public void OpenSimulation()
    {
        _editorMenu.PlaySimulation();
    }

    public void ExitSimulation()
    {
        GameEvent.isManagerOpen = false;
        _editorMenu.DestroyAllElementsInserted();
        OpenManager();
        ExitSimulation();
    }
    
}
