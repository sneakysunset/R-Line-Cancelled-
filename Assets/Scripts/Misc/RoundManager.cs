using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RoundManager : MonoBehaviour
{
    public float roundTimer = 15;
    private float timer;
    public bool roundEnded;
    public Slider timerSlider;
    bool flag;
    void Start()
    {
        timer = roundTimer;
    }

    private void Update()
    {
        timerSlider.value = timer / roundTimer;
        if (timer > 0) timer -= Time.deltaTime;
        else if (timer <= 0 && !flag) RoundEnd();
    }


    private void RoundEnd()
    {
        Physics2D.gravity *= -1;
        roundEnded = true;
        flag = true;
    }
}
