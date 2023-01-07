using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class UI_Pause : MonoBehaviour
{
    bool paused;

    //Ne fonctionne pas pour l'instant.
    //Mais pause au jeu.
    public void OnPause(InputAction.CallbackContext context)
    {
        print(0);
        if(paused && context.started)
        {
            print(1);
            paused = false;
            Time.timeScale = 1;
        }
        else if(!paused && context.started)
        {
            print(2);

            paused = true;
            Time.timeScale = 0; 
        }
    }
}
