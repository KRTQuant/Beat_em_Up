using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour, IDamageable<float>
{
    [Header("Singleton")]
    public static PlayerController instance = null;

    [Header("Walk")]
    [Space(-8)]

    [Header("Player Properties")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private bool canMove;
    [SerializeField] private int moveDirX;
    [SerializeField] private bool isWalk;
    [SerializeField] private Transform player;

    [Header("Player Health")]
    [SerializeField] private float maxHp;
    [SerializeField] public float currentHp;

    [Header("Sprite Renderer")]
    [SerializeField] private int facingDirection = 0;
    [SerializeField] private int YAxisMove = 0;
    [SerializeField] public bool playingAttackAnim;
    [SerializeField] public float stunTime;
    [SerializeField] public float invisTime;

    [Header("Dash")]
    [SerializeField] private float dashTime;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float distanceBtwAfterImage;
    [SerializeField] private float dashCooldown;
    [SerializeField] private float dashTimeLeft;
    [SerializeField] private float lastAfterImageXpos;
    [SerializeField] private float lastDash = -100f;
    [SerializeField] private int dashTrigger = 0;
    [SerializeField] private bool isDash = false;

    [Header("Ultimate Gauge")]
    [SerializeField] private GameObject pentagramCircle;
    [SerializeField] private GameObject pentagramFX;
    [SerializeField] private float maxGauge = 100;
    [SerializeField] private float currentGauge;
    [SerializeField] private bool canUsePersona;
    [SerializeField] public bool usingPersona;
    [SerializeField] private GameObject pentagram;
    [SerializeField] private Animator pentAnim;

    [Header("Component Reference")]
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer spriteRen;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject closeCol;
    [SerializeField] private GameObject medCol;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private GaugeBar gaugeBar;

    [Header("Check and Debugging")]
    [SerializeField] private Vector2 DashVelocity;
    [SerializeField] private float velocityX;
    [SerializeField] private float velocityY;
    [SerializeField] private bool isCheckDead;

    [Header("Attack variable")]
    [SerializeField] private Vector2 closeColliderSize;
    [SerializeField] private Vector2 medColliderSize;
    [SerializeField] private bool isFlipAttackPos;

    [Header("Camera")]
    [SerializeField] private CameraClampAndFollow cam;

    private void Awake()
    {
        currentHp = maxHp;
        healthBar.SetMaxHealth(maxHp);
        currentGauge = 0;
        gaugeBar.SetMaxGauge(maxGauge);


    }

    private void Update()
    {
        InputSystem();
        CheckLogic();
        ForDebugging();
    }

    #region Input
    private void InputSystem()
    {
        Walk();
        Dash();
        Attack_A_Type();
        Attack_B_Type();
        ActivePersona();
    }
    #endregion

    #region Animation and Sprite relate

    private void FacingDetection()
    {
        if(!anim.GetBool("Stun"))
        {
            if (spriteRen.flipX == false && facingDirection == -1)
            {
                spriteRen.flipX = true;
            }

            else if (spriteRen.flipX == true && facingDirection == 1)
            {
                spriteRen.flipX = false;
            }
        }
    }

    private void CalculateWalkVelocity()
    {
        if (isWalk)
            rb.velocity = new Vector2(moveDirX * moveSpeed * Time.deltaTime, YAxisMove * moveSpeed * Time.deltaTime);
    }

    private void ZeroVeloWhilePlayAtkAnim()
    {
        if (anim.GetBool("PlayingAttackAnim") == true)
        {
            //Debug.Log("PlayingAttackAnim is true");
            rb.velocity = Vector2.zero;
        }
    }

    private void ActivePentagramFX()
    {
        if(pentAnim.GetBool("ActivePentagramFX"))
        {
            pentagramFX.SetActive(true);
        }
    }

    private void GetStun()
    {
        if(anim.GetBool("Stun"))
        {
            stunTime -= Time.deltaTime;
            rb.velocity = Vector2.zero;
        }
        if(stunTime <= 0)
        {
            anim.SetBool("Stun", false);
            stunTime = 1.0f;
        }
    }

    #endregion

    #region Walk
    private void Walk()
    {
        if(canMove)
        {
            if (Input.GetKey(KeyCode.D))
            {
                facingDirection = 1;
                moveDirX = 1;
                isWalk = true;
                anim.SetFloat("Speed", 1);
            }

            if (Input.GetKey(KeyCode.A))
            {
                facingDirection = -1;
                moveDirX = -1;
                isWalk = true;
                anim.SetFloat("Speed", 1);
            }

            if (Input.GetKey(KeyCode.W))
            {
                YAxisMove = 1;
                isWalk = true;
                anim.SetFloat("Speed", 1);
            }

            if (Input.GetKey(KeyCode.S))
            {
                YAxisMove = -1;
                isWalk = true;
                anim.SetFloat("Speed", 1);
            }

            if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
            {
                anim.SetFloat("Speed", 0);
                rb.velocity = Vector2.zero;
                isWalk = false;
                moveDirX = 0;
            }

            if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
            {
                anim.SetFloat("Speed", 0);
                isWalk = false;
                YAxisMove = 0;
                rb.velocity = Vector2.zero;
            }

            CalculateWalkVelocity();
            FacingDetection();
        }
    }
    #endregion 

    #region Dash
    private void Dash()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            if(Time.time >= (lastDash + dashCooldown))
            {
                //Debug.Log("Key L was press");
                AttempToDash();
            }
        }
    }

    private void AttempToDash()
    {
        isDash = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;
        dashTrigger = 1;

        AfterImagePool.Instance.GetFromPool();
        lastAfterImageXpos = transform.position.x;
    }

    private void CheckDash()
    {
        if(isDash)
        {
            if (dashTimeLeft > 0)
            {
                canMove = false;
                rb.velocity = new Vector2(dashSpeed * facingDirection * Time.deltaTime * dashTrigger, rb.velocity.y);
                dashTimeLeft -= Time.deltaTime;
                anim.SetBool("isDash", true);

                if (Mathf.Abs(transform.position.x - lastAfterImageXpos) > distanceBtwAfterImage)
                {
                    AfterImagePool.Instance.GetFromPool();
                    lastAfterImageXpos = transform.position.x;
                }
            }

            if (dashTimeLeft <= 0)
            {
                rb.velocity = Vector2.zero;
                canMove = true;
                isDash = false;
                dashTrigger = 0;
                anim.SetBool("isDash", false);
            }
        }
    }
    #endregion

    #region Attack
    private void Attack_A_Type()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            anim.SetTrigger("InputA");
            rb.velocity = Vector2.zero;
            ControlCloseCollider();
        }
    }

    private void Attack_B_Type()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            anim.SetTrigger("InputB");
            rb.velocity = Vector2.zero;
            ControlRangeCollider();
        }
    }
    #endregion

    #region PERU-SO-NA!!!!!!!!
    public void IncreaseGauge() // Call when have a successful hit
    {
        if(Input.GetKeyDown(KeyCode.Keypad9))
        {
            currentGauge += 15;
            //For dev purpose
            if (currentGauge >= maxGauge) //
            {
                currentGauge = maxGauge;
            }
        }
    }

    private void ActivePersona()
    {
        if(Input.GetKeyDown(KeyCode.U))
        {
            if(canUsePersona)
            {
                pentagramCircle.SetActive(true);
                canUsePersona = false;
                usingPersona = true;
            }
        }
    }

    private void CheckUlti()
    {
        if(currentGauge == maxGauge)
        {
            canUsePersona = true;
        }

        if(usingPersona)
        {
            currentGauge -= 2 * Time.deltaTime;
            anim.SetBool("UltimaMode", true);
            canUsePersona = false;
        }

        if(currentGauge <= 0)
        {
            usingPersona = false;
            anim.SetBool("UltimaMode", false);
            pentagramCircle.SetActive(false);
            pentagramFX.SetActive(false);
        }
    }

    private void UpdatePentagramPos()
    {
        float tempPlayerX;
        float tempPlayerY;
        if (spriteRen.flipX)
        {
            //Debug.Log("Flip X : True");
            tempPlayerX = player.position.x + 0.224f;
        }
        else
        {
            tempPlayerX = player.position.x;
        }
        tempPlayerY = player.position.y - 0.881f;
        pentagram.transform.position = new Vector2(tempPlayerX, tempPlayerY);
    }
    #endregion

    #region Combat Collider
    private void ControlCloseCollider()
    {
        Collider2D[] hitCloseCollider = Physics2D.OverlapBoxAll(closeCol.transform.position, closeColliderSize, 0f);
        for(int i = 0; i < hitCloseCollider.Length; i++)
        {
            if(hitCloseCollider[i].GetComponentInChildren<StateMachineBehav_EnemyB>())
            {
                hitCloseCollider[i].GetComponentInChildren<StateMachineBehav_EnemyB>().TakeDamage(50);
                currentGauge += 5;
            }    

            if(hitCloseCollider[i].GetComponentInChildren<StateMachineBehav_EnemyA>())
            {
                currentGauge += 5;
                hitCloseCollider[i].GetComponentInChildren<StateMachineBehav_EnemyA>().TakeDamage(50);
            }
            else
                return;
        }
    }

    private void ControlRangeCollider()
    {
        Collider2D[] hitRangeCollider = Physics2D.OverlapBoxAll(closeCol.transform.position, closeColliderSize, 0f);
        for (int i = 0; i < hitRangeCollider.Length; i++)
        {

            if (hitRangeCollider[i].GetComponentInChildren<StateMachineBehav_EnemyB>())
            {
                currentGauge += 5;
                hitRangeCollider[i].GetComponentInChildren<StateMachineBehav_EnemyB>().TakeDamage(50);
            }
            if (hitRangeCollider[i].GetComponentInChildren<StateMachineBehav_EnemyA>())
            {
                currentGauge += 5;
                hitRangeCollider[i].GetComponentInChildren<StateMachineBehav_EnemyA>().TakeDamage(50);
            }
            else
                return;
        }
    }

    private void SetAttackPosWhenFlip()
    {
        if (spriteRen.flipX)
        {
            medCol.transform.localPosition = new Vector3(-0.84f, -1.1f);
            closeCol.transform.localPosition = new Vector3(-0.52f, -1.1f);
            isFlipAttackPos = true;
        }

        if (!spriteRen.flipX)
        {
            medCol.transform.localPosition = new Vector3(0.84f, -1.1f);
            closeCol.transform.localPosition = new Vector3(0.52f, -1.1f);
        }

    }
    #endregion

    #region Interface fuction
    public void TakeDamage(float damage)
    {
        currentHp -= damage;
        if (damage == 10)
        {
            //Debug.Log("Success collide with bullet");
        }
        if(damage == 20)
        {
            //Debug.Log("Success collide with skeleton's attack");
        }
        anim.SetTrigger("GetHit");
        anim.SetBool("Stun",true);
    }
    #endregion

    #region Logic&Debug
    private void CheckLogic()
    {
        CheckDash();
        ZeroVeloWhilePlayAtkAnim();
        CheckUlti();
        ActivePentagramFX();
        UpdatePentagramPos();
        GetStun();
        LimitPlayerInCam();
        SetAttackPosWhenFlip();
        CheckDead();
    }

    private void CheckDead()
    {
        if(currentHp <= 0 && !isCheckDead)
        {
            isCheckDead = true;
            anim.SetTrigger("Death");
            StartCoroutine(DelayLoadSceneAfterDead());
        }
    }

    private void ForDebugging()
    {
        IncreaseGauge();
        UpdateUI();
        //ActiveFollowCam();
        velocityX = rb.velocity.x;
        velocityY = rb.velocity.y;
        DashVelocity = new Vector2(dashSpeed * facingDirection * Time.deltaTime * dashTrigger, rb.velocity.y);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(closeCol.transform.position, closeColliderSize);
        Gizmos.DrawWireCube(medCol.transform.position, medColliderSize);
    }

    private void LimitPlayerInCam()
    {
        if(!cam.isActive)
        {
            transform.position = new Vector2(Mathf.Clamp(transform.position.x, cam.left.position.x, cam.right.position.x),
                                            Mathf.Clamp(transform.position.y, cam.down.position.y, cam.top.position.y));
        }
    }

    /*    private void ActiveFollowCam()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            cam.GetComponent<CameraClampAndFollow>().isActive = !cam.GetComponent<CameraClampAndFollow>().isActive;
            Debug.Log("Camera's name: " + cam.GetComponent<CameraClampAndFollow>().name);
        }
    }*/

    private void UpdateUI()
    {
        healthBar.SetHealth(currentHp);
        gaugeBar.SetGauge(currentGauge);
    }

    public IEnumerator DelayLoadSceneAfterDead()
    {
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene("MainMenu");
        Destroy(gameObject);
    }
    #endregion
}
