using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineBehav_EnemyA : MonoBehaviour, IKillable, IDamageable<float>
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
    [SerializeField] private Vector2 dirToPlayer;
    [SerializeField] private bool isAttack; //IsAttack isattack is_Attack
    [SerializeField] private int currentBullet;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private float lastFire;
    [SerializeField] private float firerate;

    [Header("Component Reference")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private DynamicPooling bulletPool;
    [SerializeField] private Transform myselfTransform;
    [SerializeField] private Animator animator;

    [Header("Debug")]
    [SerializeField] private int i = 0;
    [SerializeField] bool tempDebug = false;


    private void Start()
    {
        enemyState = State.IDLE;
        currentHp = maxHp;
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        bulletPool = GameObject.Find("PoolingHolder").GetComponent<DynamicPooling>();
    }

    private void Update()
    {
        CheckState();
        CheckCondition();
        //For Debug and Test below
        InputForTakeDamage();
    }

    #region State Logic
    private void CheckState()
    {
        if (Vector2.Distance(myselfTransform.position, playerTransform.position) > stopDist)
        {
            enemyState = State.APPROACH;
        }
        if (Vector2.Distance(myselfTransform.position, playerTransform.position) <= stopDist)
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
            idleTime = Random.Range(3, 6);
            enemyState = State.ATTACK;
        }
        #endregion

        #region Approach Section
        if (enemyState == State.APPROACH)
        {
            Vector2 tempDir = (playerTransform.position - myselfTransform.position).normalized;
            rb.velocity = new Vector2(tempDir.x, tempDir.y) * moveSpeed * Time.deltaTime;
            animator.SetFloat("Speed", 1);
        }
        #endregion
        #region Attack Section
        if (enemyState == State.ATTACK)
        {
            if (Time.time - lastFire > 1 / firerate)
            {
                animator.SetTrigger("Attack");

                if (currentBullet == bulletPool.objectToLists[0].poolingAmount)
                    currentBullet = 0;

                lastFire = Time.time;
                dirToPlayer = (playerTransform.position - myselfTransform.position).normalized;
                float angle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg;
                GameObject bullet = bulletPool.objectToLists[0].list[currentBullet];
                bullet.transform.position = bulletSpawnPoint.position;
                bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                bullet.GetComponent<Bullet>().GetTargetPos(dirToPlayer);
                StartCoroutine(DelayAttackAnim(bullet));

                if (playerTransform.position.x >= myselfTransform.position.x)
                {
                    bullet.GetComponent<SpriteRenderer>().flipX = false;
                }
                else
                    bullet.GetComponent<SpriteRenderer>().flipX = true;

                //Time.timeScale = 0.0f;

                currentBullet++;

                //Debug.Log("Attack");
            }
            #endregion
        }
    }
    #endregion

    #region Condition Logic
    private void CheckCondition()
    {
        Kill();
    }

    public void Kill()
    {
        if (currentHp <= 0)
        {
            Debug.Log("Was Kill");
            animator.SetBool("Death", true);
            //DelayDeathTrigger(myselfTransform);
            StartCoroutine(DelayDeathTrigger(myselfTransform));
            //myselfTransform.gameObject.SetActive(false);
        }
    }
    #endregion

    public void TakeDamage(float damage)
    {
        currentHp -= damage;
        Debug.Log("Successful take damage from player");
    }

    public IEnumerator DelayAttackAnim(GameObject bullet)
    {
        i++;
        //Debug.Log("DelayAttackAnim Active");
        Time.timeScale = 1f;
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(0.4f);

        bullet.SetActive(true);
    }

    public IEnumerator DelayDeathTrigger(Transform myself)
    {
        Time.timeScale = 1f;
        yield return new WaitForSeconds(0.6f);
        Debug.Log("DelayDeathTrigger Active");
        myself.gameObject.SetActive(false);
    }

    #region Test&Dev
    private void InputForTakeDamage()
    {
        if(Input.GetKeyDown(KeyCode.Keypad8))
        {
            TakeDamage(20);
        }    
    }
    #endregion

}
