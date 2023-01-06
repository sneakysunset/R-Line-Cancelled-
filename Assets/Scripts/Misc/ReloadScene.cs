using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadScene : MonoBehaviour
{
    LineSound lineSound;

    private void Awake()
    {
        lineSound = FindObjectOfType<LineSound>();
    }


    public void SceneReloader()
    {
        lineSound.sound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        lineSound.sound.release();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void TestScene()
    {
        SceneManager.LoadScene("SceneTest");
    }

    public void PlayGround()
    {
        SceneManager.LoadScene("PlayGround");   
    }

    public void SoundScene()
    {
        SceneManager.LoadScene("LD_MainTest");
    }

    public void LoadScene(string input)
    {
        int intput = int.Parse(input);
        switch (intput)
        {
            case 0:
                lineSound.sound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                lineSound.sound.release();
                SceneManager.LoadScene("SceneTest");
                break;
            case 1:
                lineSound.sound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                lineSound.sound.release();
                SceneManager.LoadScene("PlayGround");
                break;
            case 2:
                lineSound.sound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                lineSound.sound.release();
                SceneManager.LoadScene("Sound Test");
                break;
            case 3:
                lineSound.sound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                lineSound.sound.release();
                SceneManager.LoadScene("LD_MainTest");
                break;
            default:
                break;
        }
    }
}
