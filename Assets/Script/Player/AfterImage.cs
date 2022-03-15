using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImage : MonoBehaviour
{
    [Header("Component Reference")]
    [SerializeField] private Transform player;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private SpriteRenderer playerSR;
    [SerializeField] private Color color;
    [SerializeField] private AfterImagePool afterImagePool;

    [Header("Variable")]
    [SerializeField] private float activeTime = 0.25f;
    [SerializeField] private float timeActivated;
    [SerializeField] private float alpha;
    [SerializeField] private float alphaSet = 0.8f;
    [SerializeField] private float alphaMultiplier = 0.5f;

    private void OnEnable()
    {
        sr = GetComponent<SpriteRenderer>();
        player = GameObject.Find("Player_SpriteRenderer").GetComponent<SpriteRenderer>().transform;
        playerSR = player.GetComponent<SpriteRenderer>();

        alpha = alphaSet;
        sr.sprite = playerSR.sprite;
        transform.position = player.position;
        transform.rotation = player.rotation;
        timeActivated = Time.time;
    }

    private void Start()
    {
        if (playerSR.flipX)
        {
            sr.flipX = true;
        }
        else if (!playerSR.flipX)
        {
            sr.flipX = false;
        }
    }

    private void Update()
    {
        if(playerSR.flipX)
        {
            sr.flipX = true;
        }
        else if(!playerSR.flipX)
        {
            sr.flipX = false;
        }
        alpha *= alphaMultiplier;
        color = new Color(1f, 1f, 1f,alpha);
        sr.color = color;

        if (Time.time >= (timeActivated + activeTime))
        {
            AfterImagePool.Instance.AddToPool(gameObject);
        }
    }
}
