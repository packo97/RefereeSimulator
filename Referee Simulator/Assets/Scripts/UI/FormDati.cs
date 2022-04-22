using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FormDati : MonoBehaviour
{
    private string name_video;
    private string category;
    private string author;
    private string difficulty;
    private string answer;
    private string reason;
    private string state;
    [SerializeField] private InputField inputName;
    [SerializeField] private Dropdown inputCategory;
    [SerializeField] private Dropdown inputDifficulty;
    [SerializeField] private InputField inputAnswer;
    [SerializeField] private InputField inputReason;
    [SerializeField] private Dropdown inputState;

    [SerializeField] private InfoBox _infoBox;
    

    public void CloseFormDati()
    {
        gameObject.SetActive(false);
        
    }

    public void SaveFormDati()
    {
        name_video = inputName.text;
        category = inputCategory.captionText.text;
        author = "Admin";
        difficulty = inputDifficulty.captionText.text;
        answer = inputAnswer.text;
        reason = inputReason.text;
        state = inputState.captionText.text;

        if (name_video != "" && category != "" && difficulty != "" && answer != "" && reason != "" && state != "")
        {
            gameObject.SetActive(false);
            _infoBox.SetText("Simulation saved.", InfoBox.TypeOfMessage.SUCCESS);
        }
        else
        {
            _infoBox.SetText("Pay Attention, some data are not filled.", InfoBox.TypeOfMessage.ERROR);
        }
        
        bool save = SaveLoadManager.SaveSimulation(name_video, category, author, difficulty, answer, reason, state);
        if (!save)
            _infoBox.SetText("There is a simulation with the same name.", InfoBox.TypeOfMessage.ERROR);
    }
    
}
