using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball_State : MonoBehaviour
{
    public Color color;

    protected Color colorDefault = new Color(238, 255, 0);
    protected Rigidbody2D rb;
    protected SpriteRenderer sprite;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();

    }
}
