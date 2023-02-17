using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LineSound : MonoBehaviour
{
    private LineCreator lineC;
    [HideInInspector] public FMOD.Studio.EventInstance sound;
    public float soundUpdateTimer;
    WaitForSeconds waiter;
    private float maxHeight;
    private float minHeight;
    public GameObject visualPrefab;
    private Transform currentVisual;
    IEnumerator soundEnum;
    public bool pingpong;
    public float startTimer;

    private void Start()
    {
        waiter = new WaitForSeconds(soundUpdateTimer);
    }



    //Event pour l'instant activé manuellement avec un bouton.
    //Il lance la mécanique principale sonore (synthétiseur sonore avec pitch dépendant de la courbe créée par la ligne).
    public void PlaySound()
    {
        lineC = FindObjectOfType<LineCreator>();

        //Si la mécanique est déja activé on arrête l'instance de son actuelle. Sinon on instancie la visualisation du lecteur de la courbe.
        if (IsPlaying())
        {
            sound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            sound.release();
            currentVisual.position = lineC.pointList[0].pos;
            StopCoroutine(soundEnum);
            soundEnum = null;
        }
        else
        {
            currentVisual = Instantiate(visualPrefab, lineC.pointList[0].pos, Quaternion.identity).transform;
        }

        soundEnum = SoundControl();
        
        //Création de l'instance du son.
        sound = FMODUnity.RuntimeManager.CreateInstance("event:/MouvementCorde/LineSound");
        sound.start();
        //Méthode actuellement non dynamique. Elle devrait permettre de repérer la hauteur du mur au dessus et en dessous du joueur. Pour l'instant elle récupère des valeurs constantes.
        GetMaxHeight();
        //Coroutine qui fait fonctionner le son.
        StartCoroutine(soundEnum);
    }

    //Méthode qui retourne si le son de la mécanique sonore est entrain de jouer.
    bool IsPlaying()
    {
        FMOD.Studio.PLAYBACK_STATE state;
        sound.getPlaybackState(out state);
        return state != FMOD.Studio.PLAYBACK_STATE.STOPPED;
    }

    //Méthode actuellement non dynamique. Elle devrait permettre de repérer la hauteur du mur au dessus et en dessous du joueur. Pour l'instant elle récupère des valeurs constantes.
    void GetMaxHeight()
    {
        RaycastHit2D hitTop = Physics2D.Raycast((Vector2)lineC.transform.position, Vector2.up, 1000, 15); 
        RaycastHit2D hitBottom = Physics2D.Raycast((Vector2)lineC.transform.position, Vector2.down, 1000, 15);

        //print(hitTop.transform.name + " " + hitTop.point.y);
        //print(hitBottom.transform.name + " " + hitBottom.point.y);
        //maxHeight = hitTop.point.y - hitBottom.point.y;
        //minHeight = hitBottom.point.y;
        maxHeight = 47;
        minHeight = 0;
    }

    IEnumerator SoundControl()
    {
        //Timer servant à attendre la fin du fade in de la piste sonore jouée.
        yield return new WaitForSeconds(startTimer);
        int i = 0;
        
        //De manière itérative on parcours les points de la ligne de gauche à droite. A chaque itération le paramètre locale de FMOD "Pitch" récupère la valeure de la position sur l'axe y du point actuel.
        while(i < lineC.pointList.Count - 1)
        {
            i++;
            sound.setParameterByName("Pitch", (lineC.pointList[i].pos.y - minHeight)/ maxHeight, true);
            currentVisual.position = lineC.pointList[i].pos;

            yield return waiter;
        }
        //Même procédé dans le sens contraire.
        while (i > 0)
        {
            i--;
            sound.setParameterByName("Pitch", (lineC.pointList[i].pos.y - minHeight) / maxHeight, true);
            currentVisual.position = lineC.pointList[i].pos;

            yield return waiter;
        }

        //Si le booléen est activé cette coroutine se répète à l'infini jusqu'à ce que la scène soit changé.
        if (!pingpong)
        {
            sound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            sound.release();
            Destroy(currentVisual.gameObject);
            StopCoroutine(soundEnum);
            soundEnum = null;
        }
        else
        {
            PingPongSoundControl();
        }
    }

    //Méthode permettant de répéter la mécanique sonore à l'infini.
    void PingPongSoundControl()
    {
        startTimer = 0;
        StartCoroutine(SoundControl());
    }

   
}
