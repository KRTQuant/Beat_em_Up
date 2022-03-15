using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] float moveSpeed;
    [SerializeField] Vector2 tempPos;
    [SerializeField] bool isGetPos;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }
    void Update()
    {
        if(isGetPos)
            rb.velocity = tempPos * moveSpeed;
    }

    public void GetTargetPos(Vector2 targetPos)
    {
        tempPos = targetPos;
        isGetPos = true;
    }

    private void OnDisable()
    {
        isGetPos = false;
        tempPos = Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("PlayerBulletCollider"))
        {
            other.GetComponentInParent<PlayerController>().TakeDamage(10);
            
        }
    }
}
