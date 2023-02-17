using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Ball_RebondLimiter : Ball_State
{
    public int rebondMax;
    public int currentRebond = 0;
    bool isActive;
    public TMP_Text rebond;

    private void OnEnable()
    {
        //sprite.color = color;
        isActive = true;
        rebond.text = (rebondMax).ToString();

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isActive)
        {
            currentRebond++;
            rebond.text = (rebondMax - currentRebond).ToString();
        }
    }
    private void Update()
    {
        //Comportement
        //Reset le compte si un joueur recupere la balle
        if (gameObject.CompareTag("Held"))
        {
            currentRebond = 0;
            rebond.text = (rebondMax).ToString();

        }
        //Ce qui se passe si le nombre de rebond atteind la limite
        if (currentRebond == rebondMax)
        {
            Destroy(gameObject);
        }
        //Transition
    }
    private void OnDisable()
    {
        //sprite.color = colorDefault;
        isActive = false;
        rebond.text = "0";
    }
}
