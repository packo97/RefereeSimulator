using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SimulationDetails : MonoBehaviour
{
    [SerializeField] private Button play;
    [SerializeField] private Button modify;
    [SerializeField] private Button delete;
    [SerializeField] private Button report;
    
    [SerializeField] public Text name_video;
    [SerializeField] public Text category;
    [SerializeField] public Text author;
    [SerializeField] public Text difficulty;
    [SerializeField] public Text answer;
    [SerializeField] public Text reason;
    [SerializeField] public Text state;

    private ElementData _elementData;


    private void Start()
    {
        ResetDetails();
    }

    public void ResetDetails()
    {
        play.gameObject.SetActive(false);
        modify.gameObject.SetActive(false);
        delete.gameObject.SetActive(false);
        report.gameObject.SetActive(false);
        CleanDetails();
    }
    
    public void CleanDetails()
    {
        name_video.text = "";
        category.text = "";
        author.text = "";
        difficulty.text = "";
        answer.text = "";
        reason.text = "";
        state.text = "";
    }

    public void ShowButtons(bool show)
    {
        play.gameObject.SetActive(show);
        modify.gameObject.SetActive(show);
        delete.gameObject.SetActive(show);
        report.gameObject.SetActive(show);
    }

    
    public void DeleteSimulation()
    {
        string codice = ManagerMenu.GetIconSimulazioneSelected().GetCodice();
        Destroy(GameObject.Find("IconaSimulazione" + codice));
        SaveLoadManager.DeleteSimulation(codice);
        ResetDetails();
    }
    
}
