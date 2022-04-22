using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconaSimulazione : MonoBehaviour
{
    private SimulationDetails simulationDetails;
    private Image _image;
    private ElementData _elementData;
    private string codice;
    private void Start()
    {
        simulationDetails = GameObject.Find("DettagliSimulazione").GetComponent<SimulationDetails>();
        _image = GetComponent<Image>();
    }

    public void LoadDetailsOf()
    {
        /*
         * Quando un icona viene cliccata, carico i dati corrispondenti alla simulazione.
         *
         * Imposto un colore blu per identificare la selezionata e aggiorno il colore delle restanti icone.
         * Cerco la simulazione con il codice dell'icona selezionata nel mio storage.
         *
         * Visualizzo i dettagli nel pannello apposito.
         */
        
        _image.color = new Color32(46, 177, 217, 100);
        ResetColor();

        codice = gameObject.name.Replace("IconaSimulazione", "");
        
        _elementData = SaveLoadManager.LoadSimulation(codice);
        
        ManagerMenu.SetIconSimulazioneSelected(this);

        simulationDetails.name_video.text = _elementData.details.name;
        simulationDetails.category.text = _elementData.details.category;
        simulationDetails.author.text = _elementData.details.author;
        simulationDetails.difficulty.text = _elementData.details.difficulty;
        simulationDetails.answer.text = _elementData.details.answer;
        simulationDetails.reason.text = _elementData.details.reason;
        simulationDetails.state.text = _elementData.details.state;
        simulationDetails.gameObject.SetActive(true);
        simulationDetails.ShowButtons(true);
        
    }

    public void ResetColor()
    {
        /*
         * Resetto il colore delle icone non selezionate
         * 
         */
        
        GameObject contenitore = GameObject.Find("ListaSimulazioni");
        for(int i = 0; i < contenitore.transform.childCount; ++i){
            GameObject child = contenitore.transform.GetChild(i).gameObject;
            if (child != gameObject)
                child.GetComponent<Image>().color =new Color32(224,81,81,100);
        }
    }

    public string GetCodice()
    {
        return codice;
    }
    
    
}
