using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineBehav_EnemyB : MonoBehaviour, IKillable, IDamageable<float>
{
    [Header("Enemy Status")]
    [SerializeField] private float maxHp = 100;
    [SerializeField] public float currentHp;
    [SerializeField] private float damage;

    [SerializeField] private enum State { APPROACH, IDLE, ATTACK };
    [Header("Enum/State/IEnumerator")]
    [SerializeField] private State enemyState;
    [SerializeField] private IEnumerator coroutine;

    [Header("Enemy movement properties")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private int dirX, dirY;
    [SerializeField] private Rigidbody2D rb;

    [Header("Status : APPROACH relate variable")]
    [SerializeField] private float stopDist;
    [Header("Status : IDLE relate variable")]
    [SerializeField] private float idleTime; //define maximum idle time [ Dont let AI idle too long ]
    [Header("Status : ATTACK relate variable")]
    [SerializeField] private bool isAttack; //IsAttack isattack is_Attack
    [SerializeField] private float lastFire;
    [SerializeField] private float firerate;
    [SerializeField] private Vector2 AttackRange;
    [SerializeField] private Transform AttackPos;

    [Header("Component Reference")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform playerShadow;
    [SerializeField] private Transform myselfTransform;
    [SerializeField] private Transform myselfShadow;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer sr;

    [Header("For debug")]
    [SerializeField] private bool isFlipAttackPos = false;

    private void Start()
    {
        currentHp = maxHp;
        enemyState = State.IDLE;
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        playerShadow = GameObject.Find("Player").GetComponent<Transform>().GetChild(1).GetComponent<Transform>();
    }

    private void Update()
    {
        CheckState();
        CheckCondition();
    }

    private void CheckState()
    {
        if (Vector2.Distance(myselfShadow.position, playerShadow.position) > stopDist)
        {
            enemyState = State.APPROACH;
        }
        if (Vector2.Distance(myselfShadow.position, playerShadow.position) <= stopDist)
        {
            enemyState = State.IDLE;
        }
        #region Idle Section
        if (enemyState == State.IDLE)
        {
            idleTime -= Time.deltaTime;
            rb.velocity = Vector2.zero;
            animator.SetFloat("Speed", 0);
        }
        if (idleTime <= 0)
        {
            idleTime = Random.Range(0,0.5f);
            enemyState = State.ATTACK;
        }
        #endregion
        #region Approach Section
        if (enemyState == State.APPROACH)
        {
            Vector2 tempDir = (playerShadow.position - myselfShadow.position).normalized;
            rb.velocity = new Vector2(tempDir.x, tempDir.y) * moveSpeed * Time.deltaTime;
            animator.SetFloat("Speed", 1);
        }
        #endregion
        #region Attack Section
        if (enemyState == State.ATTACK)
        {
            rb.velocity = Vector2.zero;
            if (Time.time - lastFire > 1 / firerate)
            {
                //Debug.Log("Attack at : " + Time.time);
                lastFire = Time.time;
                animator.SetTrigger("Attack");
                StartCoroutine(DelayAttackTrigger());
                //Debug.Log("Attack from Skeleton");
            }
        }
        #endregion
    }

    private void CheckCondition()
    {
        FlipSprite();
        StopMove();
        Kill();
    }

    public IEnumerator DelayAttackTrigger()
    {
        //Debug.Log("Start delay : " + Time.time);
        Time.timeScale = 1;
        yield return new WaitForSeconds(1.02f);
        //Debug.Log("End delay : " + Time.time);
        ActiveCombatCollider();
    }

    public IEnumerator DelayDeathTrigger(Transform myself)
    {
        Time.timeScale = 1;
        yield return new WaitForSeconds(2.0f);
        myself.gameObject.SetActive(false);
    }

    private void ActiveCombatCollider()
    {
        Collider2D[] hitCloseCollider = Physics2D.OverlapBoxAll(AttackPos.position, AttackRange, 0f);
        for (int i = 0; i < hitCloseCollider.Length; i++)
        {
            hitCloseCollider[i].transform.parent.GetComponentInChildren<PlayerController>().TakeDamage(damage);
            //Debug.Log("blank" + hitCloseCollider[i].name);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(AttackPos.position, AttackRange);
    }

    public void TakeDamage(float damage)
    {
        currentHp -= damage;
        animator.SetTrigger("GetHit");
        //Debug.Log("GetHit");
    }

    public void Kill()
    {
        if (currentHp <= 0)
        {
            animator.SetBool("Death", true);
            StartCoroutine(DelayDeathTrigger(myselfTransform));
        }
    }

    private void SetAttackPosWhenFlip(bool flip)
    {
        if (flip)
        {
            AttackPos.localPosition = new Vector3(-0.33f, -0.6f);
            isFlipAttackPos = true;
        }

        if (!flip)
        {
            AttackPos.localPosition = new Vector3(0.33f, -0.6f);
        }

    }

    private void FlipSprite()
    {
        if(!animator.GetBool("Attacking"))
        {
            if (playerTransform.position.x > myselfTransform.position.x)
            {
                sr.flipX = false;
            }
            else if (playerTransform.position.x < myselfTransform.position.x)
            {
                sr.flipX = true;
            }
            SetAttackPosWhenFlip(sr.flipX);
        }
    }    

    private void StopMove()
    {
        if(animator.GetBool("Attacking"))
        {
            rb.velocity = Vector2.zero;
        }
    }
}
