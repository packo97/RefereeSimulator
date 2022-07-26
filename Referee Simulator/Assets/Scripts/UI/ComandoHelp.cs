using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComandoHelp : MonoBehaviour
{
    [SerializeField] private InfoBox infoBox;
    
    // Update is called once per frame
    void Update()
    {
        if (GameEvent.isSimulationOpen)
        {
            if (Input.GetKey(KeyCode.H))
            {
                infoBox.SetText("PRESS LEFT BUTTON MOUSE TO WHISTLE\n" +
                                "PRESS RIGHT BUTTON MOUSE TO USE YELLOW CARD\n" +
                                "PRESS CTRL + RIGHT BUTTON MOUSE TO USE RED CARD\n", InfoBox.TypeOfMessage.INFO, true);
            }
        }
        else if (GameEvent.isActionOpen)
        {
            if (Input.GetKey(KeyCode.H))
            {
                infoBox.SetText("Press Enter to move player and start recording\n" +
                                "Press R to replay\n" +
                                "Press P to stop recording and back to initial position\n" +
                                "Press ESC to exit", InfoBox.TypeOfMessage.INFO, true);
                
                //infoBox.SetText("Press P to stop recording", InfoBox.TypeOfMessage.INFO, false);
            }
        }
}
    
    
}
