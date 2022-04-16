using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //public static PlayerController Instance;

    #region Fields
    //Ability
    [Header("Ability")]
    [SerializeField] bool isObtainDash;
    [SerializeField] bool isObtainDoubleJump;
    [SerializeField] bool isObtainWallSlide;

    //Component
    private Transform m_Transform;
    private Rigidbody2D m_Rigidbody2D;
    private BoxCollider2D m_BoxCollider2D;
    private SpriteRenderer m_SpriteRenderer;
    private Animator m_Animator;
    private AudioSource[] m_AudioSource;
    private Transform ground_Sensor;

    //Move
    [Header("Move")]
    [SerializeField] private float speed = 5f;
    private float moveInputDirection;
    private int facingDirection = 1;
    private bool facingRight = true;
    [SerializeField] private bool canMove = true;
    [SerializeField] private bool canFlip = true;
    [SerializeField] private bool isWalking = true;  //In this project, walk equal run
    //private Transform forwardCheck_FarDown;
    [SerializeField] private Transform forwardCheck_NearUp;
    //[SerializeField] private Transform forwardCheck_Middle;
    [SerializeField] private bool isWall;

    //Jump
    [Header("Jump")]
    [SerializeField] private bool isJumping = false;
    [SerializeField] private float jumpForce = 14.5f;    //Ŀǰ���ܱ�֤��Ծ�����ޣ�Ӧ��Ҫ�Ӷ�һ��timer
    [SerializeField] private float jumpTime;                    //��Ծʱ�ſ��������ж�����ʹ��
    [SerializeField] private float startJumpTime = 0.32f;        //��Ծʱ�ſ��������ж�����ʹ��
    [SerializeField] private bool canJump = true;

    //DoubleJump
    [Header("DoubleJump")]
    [SerializeField] private bool isDoubleJumping = false;
    [SerializeField] private float doubleJumpForce = 12f;
    [SerializeField] private bool canDoubleJump = false;
    [SerializeField] private bool doubleJumpAfterAirDashTimer = false;
    [SerializeField] private bool isAfterDoubleJump = false; //If it is "true", the player already doubleJumped in the air and has not landed, then the "canDoubleJump" will not be set to "true".
    private GameObject doubleJumpEffect;

    //Ground
    [Header("Ground")]
    [SerializeField] private bool isGround = true;
    private LayerMask whatIsGround = 1 << 6;

    //Dash
    [Header("Dash")]
    [SerializeField] private bool isDashing = false;
    [SerializeField] private float dashDuration = 0.13f;
    [SerializeField] private float dashSpeed = 10.875f;
    [SerializeField] private float dashCD = 0.4f;
    [SerializeField] private float lastDashTime;
    [SerializeField] private bool canDash = true;
    [SerializeField] private bool canAirDash = true;
    [SerializeField] private bool canWallSlideDash = true;
    [SerializeField] private bool isAfterAirDash = false; //If it is "true", the player already dashed in the air and has not landed, then the "canAirdash" will not be set to "true".
    private GameObject dashEffect;

    //Attack
    [Header("Attack")]
    [SerializeField] private int attack = 5;
    [SerializeField] private bool canAttack = true;
    [SerializeField] private float attackCD = 0.38f;
    [SerializeField] private Transform attack_Transform;
    [SerializeField] private float attackRadius = 0.57f;
    private LayerMask whatIsMonster = 1 << 7;
    [SerializeField] private bool affectedByRecoil = false;
    [SerializeField] private float recoilForce = 4f;

    //WallSlide and WallSlideJump  *û�б߻�ǽ�߹����Ķ������޷������������
    [Header("WallSlide and WallSlideJump")]
    [SerializeField] private bool isSliding = false;
    [SerializeField] private float wallSlideJumpingDuration = 0.2f;
    [SerializeField] private float wallSlideJumpCounterForce = 8f;
    [SerializeField] private float wallSlideSpeed = 2.5f;
    [SerializeField] private float wallSlideJumpSpeed = 14.4f;
    [SerializeField] private float wallSlideInterruptTimer;
    [SerializeField] private float startWallSlideInterruptTimer = 0.025f;  //�����������һ��ʱ��󣬲��ж�Wall Slide
    private GameObject dustEffect;
    private Transform dust_Transform;

    //Hurt
    [SerializeField] private float startInvincibleTime;
    [SerializeField] private float invincibleTime = 1.6f;
    [SerializeField] private bool affectedByHurt = false;

    //Life
    [Header("Life")]
    private bool isLife = true;
    [SerializeField] private int hp = 5;
    private Image[] hearts;
    private Sprite fullHeart_Sprite;
    private Sprite emptyHeart_Sprite;

    //Audio
    private bool isFootStep = false;

    //Other
    private AudioClip[] m_FootStepAudios;
    private AudioClip attack_SoundEffect;
    private AudioClip dash_SoundEffect;
    private AudioClip doubleJump_SoundEffect;
    #endregion

    #region Attributes
    //Ability
    public bool IsObtainDash { get { return isObtainDash; } set { isObtainDash = value; } }
    public bool IsObtainDoubleJump { get { return isObtainDoubleJump; } set { isObtainDoubleJump = value; } }
    public bool IsObtainWallSlide { get { return isObtainWallSlide; } set { isObtainWallSlide = value; } }

    //Move
    public bool CanMove { get { return canMove; } set { canMove = value; } }
    public bool CanFlip { get { return canFlip; } set { canFlip = value; } }

    //Jump
    public bool CanJump { get { return canJump; } set { canJump = value; } }
    
    //Ground
    public bool IsGround { get { return isGround; } }

    //Attack
    public bool CanAttack { get { return canAttack; } set { canAttack = value; } }

    //DoubleJump
    public bool DoubleJumpAfterAirDashTimer
    {
        get { return doubleJumpAfterAirDashTimer; }
        set
        {
            doubleJumpAfterAirDashTimer = value;
            if (value == false)  //�������ʱ���ر�ʱ��
            {
                canDoubleJump = false;
                m_Animator.SetBool("Jump", true);
                isJumping = false;
                isDoubleJumping = true;
                m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, doubleJumpForce);
                GameObject go = GameObject.Instantiate<GameObject>(doubleJumpEffect, gameObject.transform.position + new Vector3(0, 0.0535261f, 0), Quaternion.identity);
                go.transform.eulerAngles = new Vector3(-78, 0, 0);
            }
        }
    }

    //Attack
    public int Attack
    {
        get { return attack; }
        set { attack = value; }
    }

    //Life
    public int HP
    {
        get { return hp; }
        set
        {
            hp = value;
            if (hp > 0)
            {
                StartCoroutine("Hurt");
                StartCoroutine("GetInvulnerable");
            }
            else if (hp <= 0)
            {
                if (isLife)
                    StartCoroutine("Dead");
            }
            RefreshHPUI();
        }
    }
    #endregion

    //private void Awake()
    //{
    //    if (Instance == null)
    //    {
    //        Instance = this;
    //        DontDestroyOnLoad(gameObject);
    //    }
    //    else
    //    {
    //        Destroy(gameObject);
    //    }
    //}

    void Start()
    {
        Init();
    }

    private void Init()
    {
        //Component
        m_Transform = gameObject.GetComponent<Transform>();
        m_Rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        m_BoxCollider2D = gameObject.GetComponent<BoxCollider2D>();
        m_SpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        m_Animator = gameObject.GetComponent<Animator>();
        m_AudioSource = gameObject.GetComponents<AudioSource>();
        ground_Sensor = m_Transform.Find("Ground_Sensor");
        attack_Transform = m_Transform.Find("Attack_Positon");
        dust_Transform = m_Transform.Find("Dust_Position");
        //forwardCheck_FarDown = m_Transform.Find("Forward Check(FarDown)");
        forwardCheck_NearUp = m_Transform.Find("Forward Check(NearUp)");
        //forwardCheck_Middle = m_Transform.Find("Forward Check(Middle)");

        hearts = GameObject.Find("Canvas/PlayerInfoPanel").GetComponentsInChildren<Image>();  //The Image of the PlayerInfoPanel has been removedand only the Image of the child object is found
        fullHeart_Sprite = Resources.Load<Sprite>("Sprites/FullHeart");
        emptyHeart_Sprite = Resources.Load<Sprite>("Sprites/EmptyHeart");

        m_FootStepAudios = Resources.LoadAll<AudioClip>("StandardAudio/Footstep");
        attack_SoundEffect = Resources.Load<AudioClip>("Audios/Hollow Knight Attack");
        dash_SoundEffect = Resources.Load<AudioClip>("Audios/Hollow Knight Dash");
        doubleJump_SoundEffect = Resources.Load<AudioClip>("Audios/Double Jump");
        doubleJumpEffect = Resources.Load<GameObject>("Prefabs/Effects/BlockFlash");
        dashEffect = Resources.Load<GameObject>("Prefabs/Effects/HeroKnight_BlockNoEffect_0");
        dustEffect = Resources.Load<GameObject>("Prefabs/Effects/SlideDust");

        wallSlideInterruptTimer = startWallSlideInterruptTimer;
    }

    void Update()
    {
        if (isLife == true)
        {
            CheckInput();
            CheckSurroundings();
            UpdateAnimations();
            CheckMovementDirection();
                                                            
            if (Mathf.Abs(m_Rigidbody2D.velocity.x) > 0 && isGround) 
            {
                isWalking = true;

                if (isFootStep == false)  //��·ʱ�����Ų��� (���Ż�)
                {
                    isFootStep = true;
                    //InvokeRepeating("PlayFootStepAudio", 0, 0.4f);
                }
            }
            else if (m_Rigidbody2D.velocity.x == 0 || !isGround)
            {
                isWalking = false;
                isFootStep = false;
                //CancelInvoke("PlayFootStepAudio");
            }

            //fall
            if (m_Animator.GetBool("Jump") && m_Rigidbody2D.velocity.y < 0)  //Falling
            {
                isJumping = false;
                isDoubleJumping = false;
                m_Animator.SetBool("Jump", false);
            }

            //ground
            if (isGround == true)
            {
                canJump = true;
                canDoubleJump = false;
                canAirDash = false;
                canWallSlideDash = false;
                isAfterDoubleJump = false;
                isAfterAirDash = false;
            }

            if (!isGround && canJump && !isSliding)  //��·ʱͻȻ����
            {
                canJump = false;
                canDoubleJump = true;
            }

            if (!isGround)   //������һ˲�仹�ǻ��⵽�ذ壬�ֶ�falseҲû�ã�����ֱ���ڿ��е�ʱ���canDash��false
            {
                canDash = false;
            }

            //dash
            if (Time.time >= lastDashTime + dashCD && isGround == true)
            {
                canDash = true;
            }
            else if (Time.time >= lastDashTime + dashCD && isGround == false && isAfterAirDash == false && isWall == true)
            {
                canAirDash = false;
                canWallSlideDash = true;
            }
            else if (Time.time >= lastDashTime + dashCD && isGround == false && isAfterAirDash == false && isWall == false)
            {
                canAirDash = true;
            }

            //WallSlide
            if (isSliding == false && isGround == false && isWall == true && m_Rigidbody2D.velocity.y < 0  && isObtainWallSlide)  //���뻬ǽ״̬
            {
                isSliding = true;
                isAfterAirDash = false;
                canAirDash = false;
                canJump = true;
                wallSlideInterruptTimer = startWallSlideInterruptTimer;
                m_Animator.SetBool("WallSlide", true);
            }
            else if (isSliding == true && (isGround == true || isWall == false))
            {
                Debug.Log("fdfddfef");
                isSliding = false;
                canJump = false;        // |�ý�ɫ��ǽʱת����Խ��еڶ�����
                canDoubleJump = true;   // |
                m_Animator.SetBool("WallSlide", false);
            }
            //else if (isSliding == true && canJump == false)
            //{
            //    isSliding = false;
            //    canJump = true;
            //    m_Animator.SetBool("WallSlide", false);
            //}
        }
    }

    private void FixedUpdate()
    {
        if (isLife == true)
        {
            //Move
            ApplyMovement(); //Include move and wallSilde

            //Dash
            CheckDash();
        }
    }

    private void CheckInput()
    {
        //Move
        moveInputDirection = Input.GetAxisRaw("Horizontal");

        if ((moveInputDirection > 0 && !facingRight) || (moveInputDirection < 0 && facingRight)) //��ǽʱ�������
        {
            if (isSliding == true)
            {
                if (wallSlideInterruptTimer <= 0)
                {
                    wallSlideInterruptTimer = startWallSlideInterruptTimer;
                    Flip();
                    isWall = false; //ִ��˳�����⣬Flip��isWall����
                }
                else if (wallSlideInterruptTimer > 0)
                {
                    if (facingRight == true && moveInputDirection < 0)
                    {
                        wallSlideInterruptTimer -= Time.deltaTime;
                    }
                    else if (facingRight == false && moveInputDirection > 0)
                    {
                        wallSlideInterruptTimer -= Time.deltaTime;
                    }
                }
            }
        }
        else if (moveInputDirection == 0 && isSliding == true)
        {
            if (wallSlideInterruptTimer < startWallSlideInterruptTimer) //�ɿ������˻�û���жϻ�ǽ�Ļ�������ʱ�����¼��� //WallSlideInterrupt
            {
                wallSlideInterruptTimer = startWallSlideInterruptTimer;
            }
        }

        //Jump and WallSlideJump
        if (isGround == true && Input.GetKeyDown(KeyCode.Space) && canJump == true && !affectedByHurt)  //Jump
        {
            m_Animator.SetBool("Jump", true);
            m_Animator.SetBool("Ground", false); //
            isJumping = true;
            isGround = false;  //������һ˲�仹�ǻ��⵽�ذ壬�����ֶ�falseҲû��
            canJump = false;
            canDash = false;
            m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, jumpForce);

            if (isFootStep == true)
            {
                isFootStep = false;
                //CancelInvoke("PlayFootStepAudio");
            }
        }                                                                                  //����ǽʱͬʱ��������Ծ����ķ�����������ת���˲�������������Ի�ǽ��ʱȷ������ǽ��
        else if (isSliding == true && Input.GetKeyDown(KeyCode.Space) && canJump == true && isWall == true && isObtainWallSlide)  //WallSlideJump
        {
            m_Animator.SetBool("Jump", true);
            m_Animator.SetBool("WallSlide", false);
            isJumping = true;
            isSliding = false;
            Flip(); //ע�⣺��ת����ʩ����ǽ���ٶ�
            canMove = false;
            canFlip = false;
            canDoubleJump = false;
            StartCoroutine("OffWallSlideJumping");
            canJump = false;
            if (facingRight == true)
            {
                m_Rigidbody2D.velocity = new Vector2(wallSlideJumpCounterForce, wallSlideJumpSpeed);
            }
            else if (facingRight == false)
            {
                m_Rigidbody2D.velocity = new Vector2(-wallSlideJumpCounterForce, wallSlideJumpSpeed);
            }
            canWallSlideDash = false;
        }

        if (isJumping == true && Input.GetKeyUp(KeyCode.Space) && m_Rigidbody2D.velocity.y > 0)
        {
            m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);
        }
                                                                                               //����֤��wallsildejumpʱ�ſ�space����ִ�У����ǵ�wallsildejump��Ż�ִ��
        if (isJumping == true && Input.GetKeyUp(KeyCode.Space) && isDoubleJumping == false && canMove)  //�п��ܵ���bug��ʹ�����ʱ���canDoubleJump = true����ע��
        {
            canDoubleJump = true;
        }

        //DoubleJump                                                    //��Ϊ��ȷ������ͨ���Ͳ������������̤ǽʱ������������������ͨ��
        if (Input.GetKeyDown(KeyCode.Space) && canDoubleJump == true && canJump == false && isDashing == false && !affectedByHurt && isObtainDoubleJump)
        {
            PlayDoubleJumpSoundEffect();
            canDoubleJump = false;
            m_Animator.SetBool("Jump", true);
            isJumping = false;
            isDoubleJumping = true;
            isAfterDoubleJump = true;
            m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, doubleJumpForce);
            GameObject go = GameObject.Instantiate<GameObject>(doubleJumpEffect, gameObject.transform.position + new Vector3(0, 0.0535261f, 0), Quaternion.identity);
            go.transform.eulerAngles = new Vector3(-78, 0, 0);
        }
        else if (Input.GetKeyDown(KeyCode.Space) && canDoubleJump == true && canJump == false && isDashing == true && isObtainDoubleJump) //Double Jump After Air Dash Timer
        {
            PlayDoubleJumpSoundEffect();
            doubleJumpAfterAirDashTimer = true;
        }
        
        //Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && isDashing == false && canDash == true && isGround == true && isSliding == false && isObtainDash)  //Ground Dash
        {
            PlayDashSoundEffect();
            isDashing = true;
            canMove = false;  //|
            canFlip = false;  //|���ĸ�����ͳһ��isdashing == trueʱ��ִ�У�������ִ��˳������(slide wall dashʱ�����෴�ķ����ʱ)
            canJump = false;  //|
            canDash = false;  //|
            lastDashTime = Time.time;
        }                                                                                                                                                          
        else if (Input.GetKeyDown(KeyCode.LeftShift) && isDashing == false && canDash == false && canAirDash == true && isGround == false && isSliding == false && isObtainDash)  //Air Dash
        {
            PlayDashSoundEffect();
            canAirDash = false;
            isDashing = true;
            canMove = false;
            canFlip = false;
            canJump = false;
            canDash = false;
            lastDashTime = Time.time;
            isAfterAirDash = true;
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift) && isSliding == true && canWallSlideDash == true && isObtainDash && isObtainWallSlide)  //Slide Wall Dash
        {
            PlayDashSoundEffect();
            Flip();
            canWallSlideDash = false;
            canDoubleJump = true;
            isDashing = true;
            canMove = false;
            canFlip = false;
            canJump = false;
            canDash = false;
            isSliding = false;
            lastDashTime = Time.time;
        }

        //Attack
        if (Input.GetMouseButtonDown(0))
        {
            if (canAttack == true && isSliding == false)
            {
                StartCoroutine("AttackMonster");
            }
        }
    }

    private void CheckMovementDirection()
    {
        if (((facingRight == false && moveInputDirection > 0) || (facingRight == true && moveInputDirection < 0)) && !isSliding)
        {
            Flip();
        }
    }

    private void CheckSurroundings()
    {
        //DetectWall
        isWall = Physics2D.Raycast(forwardCheck_NearUp.position, forwardCheck_NearUp.right, 0.02f, whatIsGround); 
        
        //Ground
        isGround = Physics2D.OverlapCircle(ground_Sensor.position, 0.0001f, whatIsGround);
    }

    private void ApplyMovement()
    {
        if (affectedByRecoil == false && affectedByHurt == false && canMove && !isSliding)
        {
            m_Rigidbody2D.velocity = new Vector2(moveInputDirection * speed, m_Rigidbody2D.velocity.y);
        }

        if (isSliding == true)
        {
            m_Rigidbody2D.velocity = new Vector2(0, -wallSlideSpeed);
        }
    }

    private void UpdateAnimations()
    {
        m_Animator.SetBool("Walk", isWalking);
        m_Animator.SetBool("WallSlide", isSliding);
        m_Animator.SetBool("Ground", isGround);
        m_Animator.SetFloat("yVelocity", m_Rigidbody2D.velocity.y);
    }

    private void CheckDash()
    {
        if (isDashing == true)
        {
            if (Time.time < lastDashTime + dashDuration)
            {
                GameObject go = GameObject.Instantiate<GameObject>(dashEffect, gameObject.transform.position, Quaternion.identity);
                m_Rigidbody2D.velocity = new Vector2(dashSpeed * facingDirection, 0);
                go.transform.rotation = m_Transform.rotation; //�Ѳ�Ӱ��ת
                m_Rigidbody2D.gravityScale = 0; //�����ó�0�Ļ�����̹��̻����µ�
                m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);
                GameObject.Destroy(go, 0.08f);
            }
            else if (Time.time >= lastDashTime + dashDuration)
            {
                DashDone();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Trap"))
        {
            if (Time.time > startInvincibleTime + invincibleTime)
            {
                this.HP--;
            }
        }
    }

    private void Damage(float[] attackDetails) //[1]Ԫ����δʹ��
    {
        if (Time.time > startInvincibleTime + invincibleTime)
        {
            this.HP -= (int)attackDetails[0];
        }
    }

    private IEnumerator GetInvulnerable()
    {
        Physics2D.IgnoreLayerCollision(8, 7, true); //7:monster
        Physics2D.IgnoreLayerCollision(8, 9, true); //9:trap
        Color color = m_SpriteRenderer.material.color;
        color.a = 0.5f;
        m_SpriteRenderer.material.color = color;
        yield return new WaitForSeconds(invincibleTime);
        Physics2D.IgnoreLayerCollision(8, 7, false);
        Physics2D.IgnoreLayerCollision(8, 9, false);
        color.a = 1f;
        m_SpriteRenderer.material.color = color;
    }

    private void PlayFootStepAudio()
    {
        int index = Random.Range(1, m_FootStepAudios.Length);
        m_AudioSource[0].clip = m_FootStepAudios[index];
        m_AudioSource[0].PlayOneShot(m_AudioSource[0].clip);
        m_FootStepAudios[index] = m_FootStepAudios[0];
        m_FootStepAudios[0] = m_AudioSource[0].clip;
    }

    private void Flip()
    {
        if (canFlip)
        {
            facingDirection *= -1;
            m_Transform.Rotate(0, 180, 0);
            facingRight = !facingRight;
        }
    }

    private IEnumerator AttackMonster()
    {
        m_Animator.SetTrigger("Attack");
        canAttack = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attack_Transform.position, attackRadius, whatIsMonster);
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].transform.SendMessage("Damage", attack);
        }
        if (colliders.Length > 0) StartCoroutine("Recoil");
        yield return new WaitForSeconds(attackCD);
        canAttack = true;
    }

    /// <summary>
    /// When attacking, cannot flip
    /// </summary>
    private void SetCanFlip(int canFlip)
    {
        if (canFlip == 0)
            this.canFlip = false;
        else
            this.canFlip = true;
    }

    private IEnumerator Recoil()
    {
        affectedByRecoil = true;
        if (facingRight == true)
        {
            m_Rigidbody2D.velocity = new Vector2(-recoilForce, m_Rigidbody2D.velocity.y);
        }
        else if (facingRight == false)
        {
            m_Rigidbody2D.velocity = new Vector2(recoilForce, m_Rigidbody2D.velocity.y);
        }
        yield return new WaitForSeconds(0.1f);
        affectedByRecoil = false;
    }

    /// <summary>
    /// After seconds, set the bool "isWallSlideJumping" to false.
    /// </summary>
    private IEnumerator OffWallSlideJumping()
    {
        yield return new WaitForSeconds(wallSlideJumpingDuration);
        canMove = true;
        canFlip = true;
        canDoubleJump = true;
    }

    private void DashDone()
    {
        canMove = true;
        canFlip = true;
        isDashing = false;
        m_Rigidbody2D.velocity = new Vector2(0, 0); //��ǿ�ư��ٶȱ��0�Ļ������ʱײ�������ѹ���ײ��(��ѡ�Ż�/��չ�����԰���Ϸ�趨�ɳ��ʱ���ᱻ�����)
        m_Rigidbody2D.gravityScale = 3.5f;
        if (doubleJumpAfterAirDashTimer == true)  //When "dashTime <= 0" and "doubleJumpAfterAirDashTimer == true", off "doubleJumpAfterAirDashTimer" and double jump.
        {
            DoubleJumpAfterAirDashTimer = false;
        }
    }

    private void AE_SlideDust()
    {
        GameObject dust = GameObject.Instantiate<GameObject>(dustEffect, dust_Transform.position, Quaternion.identity);
        if (facingRight == false) dust.transform.eulerAngles = new Vector3(0, 180, 0);
    }

    private IEnumerator Hurt()
    {
        startInvincibleTime = Time.time;
        m_Animator.SetBool("Hurt", true);
        affectedByHurt = true;

        //--------��Ѵ������Jump����һ���ֳ�������------��Ϊ�ܹ�����״̬����
        //StartCoroutine("DelaySecondsAndStartDetectGround");
        isGround = false;
        canJump = false;
        canDash = false;
        if (isFootStep == true)
        {
            isFootStep = false;
            //CancelInvoke("PlayFootStepAudio");
        }

        //���ʱײ������
        if (isDashing == true)
        {
            DashDone();
        }

        //Shock From The Monster   //Ӧ��Ҫ��������ʱ����ɫ�ڹ������ĵ���߻����ұߣ���ߵĻ���������ұ���(��֮��Ÿ��ĳ�����)
        if (facingRight == true)
        {
            m_Rigidbody2D.velocity = new Vector2(-5, 9);
        }
        else if (facingRight == false)
        {
            m_Rigidbody2D.velocity = new Vector2(5, 9);
        }

        //�������ˣ�û����أ���ʹ���������Ҳ�޷�ˢ�¶�����
        if (isAfterDoubleJump == false) canDoubleJump = true;

        yield return new WaitForSeconds(0.273f);
        m_Animator.SetBool("Hurt", false);
        affectedByHurt = false;
    }

    private void RefreshHPUI() //���Ըĳɸı�����ŵİ���ͼƬ�������Ǳ���һ��
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].sprite = emptyHeart_Sprite;
        }
        for (int i = 0; i < this.HP; i++)
        {
            hearts[i].sprite = fullHeart_Sprite;
        }
    }

    private IEnumerator Dead()
    {
        isLife = false;
        m_Animator.SetTrigger("Dead");
        GameObject.Destroy(m_Rigidbody2D);
        GameObject.Destroy(m_BoxCollider2D);
        yield return new WaitForSeconds(3);
        GameObject.Destroy(gameObject);
    }

    private void PlayAttackSoundEffect()
    {
        m_AudioSource[1].PlayOneShot(attack_SoundEffect);
    }

    private void PlayDashSoundEffect()
    {
        m_AudioSource[1].PlayOneShot(dash_SoundEffect);
    }

    private void PlayDoubleJumpSoundEffect()
    {
        m_AudioSource[1].PlayOneShot(doubleJump_SoundEffect);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attack_Transform.position, attackRadius);
        Gizmos.DrawLine(forwardCheck_NearUp.position, forwardCheck_NearUp.position + forwardCheck_NearUp.right * 0.02f);
    }
}