using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster003 : MonoBehaviour
{
    [Header("Basic Value")]
    [SerializeField] [Range(5, 20)] private int hp = 10;
    [SerializeField] [Range(1, 10)] private int attack = 1;
    [SerializeField] private float speed = 2;
    private float rotateSpeed = 400;
    [HideInInspector] private bool isLife = true;
    [HideInInspector] private bool facingRight;

    public int HP
    {
        get { return hp; }
        set
        {
            hp = value;
            if (hp > 0)
            {
                StartCoroutine("Hurt");
            }
            else if (hp <= 0)
            {
                StartCoroutine("Hurt");
                Dead();
            }
        }
    }

    public int Attack { get { return attack; } set { attack = value; } }
    public float Speed { get { return speed; } set { speed = value; } }
    public bool IsLife { get { return isLife; } set { isLife = value; } }
    public bool FacingRight { get { return facingRight; } set { facingRight = value; } }

    //Component
    private Transform m_Transform;
    private Rigidbody2D m_Rigidbody2D;
    private BoxCollider2D m_BoxCollider2D;
    private GameObject bloodEffect;
    private Animator m_Animator;
    private SpriteRenderer m_SpriteRenderer;

    [HideInInspector] public Transform M_Transform { get { return m_Transform; } }
    [HideInInspector] public Rigidbody2D M_Rigidbody2D { get { return m_Rigidbody2D; } }
    [HideInInspector] public GameObject BloodEffect { get { return bloodEffect; } }

    [SerializeField] private bool isWallCheck = true;
    [SerializeField] private bool wallCheck;
    [SerializeField] private bool isGroundCheck = true;
    [SerializeField] private bool groundCheck;
    [SerializeField] private bool startGroundCheck;
    private LayerMask whatIsGround = 1 << 6;

    [SerializeField] private Transform wallCheck_Transform;
    [SerializeField] private Transform groundCheck_Transform;
    [SerializeField] private Transform startGroundCheck_Transform; //这个地方检测到了地板后，真正决定是否旋转的地板检测才开始检测

    //AI
    private Vector3 targetPos;
    private Quaternion targetRot;
    private bool isReachTargetPos = true;
    private bool isReachTargetRot = true;

    /// <summary>
    /// 
    /// </summary>
    [Tooltip("0:right, 1:up, 2:left, 3:down")] [SerializeField] private int index;

    void Start()
    {
        Init();
    }

    void Update()
    {
        if (isLife)
        {
            //Move
            Move();

            //Rotate
            Rotate();

            //Check
            Check();

            //Already Check
            if (wallCheck) //Wall Detected
            {
                WallDetected();
            }

            if (groundCheck == false) //No Ground Detected
            {
                Debug.Log("groundCheck == false");
                NoGroundDetected();
            }

            //Reach Target
            if (m_Transform.rotation == targetRot && isReachTargetRot == false)
            {
                ReachTargetRot();
            }

            if (m_Transform.position == targetPos)
            {
                ReachTargetPos();
            }
        }

        if (isLife == false)
        {
            if (Physics2D.OverlapBox(groundCheck_Transform.position, new Vector2(0.1f, 0.1f), 0, whatIsGround)) m_Rigidbody2D.bodyType = RigidbodyType2D.Static;
        }
    }

    private void Init()
    {
        m_Transform = gameObject.GetComponent<Transform>();
        m_Rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        m_BoxCollider2D = gameObject.GetComponent<BoxCollider2D>();
        bloodEffect = Resources.Load<GameObject>("Prefabs/Effects/Blood");
        m_Animator = gameObject.GetComponent<Animator>();
        m_SpriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        wallCheck_Transform = m_Transform.Find("Wall Check");
        groundCheck_Transform = m_Transform.Find("Ground Check");
        startGroundCheck_Transform = m_Transform.Find("Start Ground Check");
    }

    private void Move()
    {
        if (isReachTargetPos == true)
        {
            m_Transform.position = Vector2.MoveTowards(m_Transform.position, m_Transform.position + m_Transform.right, speed * Time.deltaTime);
        }
        else if (isReachTargetPos == false)
        {
            m_Transform.position = Vector2.MoveTowards(m_Transform.position, targetPos, speed * Time.deltaTime);
        }
    }

    private void Rotate()
    {
        m_Transform.rotation = Quaternion.RotateTowards(m_Transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
    }

    private void Check()
    {
        if (isWallCheck)
        {
            wallCheck = Physics2D.OverlapBox(wallCheck_Transform.position, new Vector2(0.01f, 0.01f), 0, whatIsGround);
        }

        if (isGroundCheck) groundCheck = Physics2D.OverlapBox(groundCheck_Transform.position, new Vector2(0.1f, 0.1f), 0, whatIsGround);
        startGroundCheck = Physics2D.OverlapBox(startGroundCheck_Transform.position, new Vector2(0.1f, 0.1f), 0, whatIsGround);
    }

    private void WallDetected()
    {
        //Debug.Log("WallDetected");
        isWallCheck = false;
        isGroundCheck = false;
        wallCheck = false;
        Vector3 currentRot = m_Transform.eulerAngles;
        //Debug.Log(currentRot);
        float z = (currentRot.z + 90) % 360;
        //Debug.Log(z);
        targetRot = Quaternion.Euler(new Vector3(currentRot.x, currentRot.y, z));
        index = (index + 1) % 4;
        isReachTargetRot = false;
        SetTargetPos();
    }

    private void NoGroundDetected()
    {
        //Debug.Log("NoGroundDetected");
        isWallCheck = false;
        isGroundCheck = false;
        groundCheck = true;
        Vector3 currentRot = m_Transform.eulerAngles;
        //Debug.Log(currentRot);
        float z = (currentRot.z - 90) % 360;
        //Debug.Log(z);
        targetRot = Quaternion.Euler(new Vector3(currentRot.x, currentRot.y, z));
        //Debug.Log(index);
        index = (index - 1) % 4;
        if (index == -1) index = 3;
        //Debug.Log(index);
        isReachTargetRot = false;
        SetTargetPos();
    }

    /// <summary>
    /// When turn rotation, set the new target position.
    /// </summary>
    private void SetTargetPos()
    {
        //Debug.Log("SetTargetPos");
        switch (index) //0:right, 1:up, 2:left, 3:down
        {
            case 0:
                targetPos = m_Transform.position + Vector3.right * 0.8f; //乘上0.8可以解决此怪物在一层的平台转弯bug。(targetPos太远的话，不能即使开始检测groundCheck_Transform)
                //Debug.Log("RIGHT:" + targetPos);                       //旧版没有这个bug，因为有另外一个bug导致一直执行ReachTargetRot，使每当startGroundCheck_Transform开始检测时就会让groundCheck_Transform开始检测)
                break;
            case 1:
                targetPos = m_Transform.position + Vector3.up * 0.8f;
                //Debug.Log("up:" + targetPos);
                break;
            case 2:
                targetPos = m_Transform.position + Vector3.left * 0.8f;
                //Debug.Log("left:" + targetPos);
                break;
            case 3:
                targetPos = m_Transform.position + Vector3.down * 0.8f;
                //Debug.Log("down:" + targetPos);
                break;
        }
        isReachTargetPos = false;
    }

    private void ReachTargetPos()
    {
        //Debug.Log("ReachTargetPos");
        isWallCheck = true;
        if (startGroundCheck) isGroundCheck = true;
        isReachTargetPos = true;
    }

    private void ReachTargetRot()
    {
        //Debug.Log("ReachTargetRot");
        isWallCheck = true;
        if (startGroundCheck) isGroundCheck = true; 
        isReachTargetRot = true;
    }

    private void Damage(int damage)
    {
        this.HP -= damage;
    }

    private IEnumerator Hurt()
    {
        m_SpriteRenderer.color = Color.red;
        GameObject effect = GameObject.Instantiate<GameObject>(bloodEffect, m_Transform.position, Quaternion.identity);
        GameObject.Destroy(effect, 5);
        yield return new WaitForSeconds(0.075f);
        m_SpriteRenderer.color = Color.white;
    }

    private void Dead()
    {
        m_Animator.SetTrigger("Dead");
        m_Transform.eulerAngles = Vector3.zero;
        m_Rigidbody2D.gravityScale = 1.5f;
        isLife = false;
        GameObject.Destroy(gameObject.GetComponent<BoxCollider2D>());
        //GameObject.Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().HP -= this.Attack;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(wallCheck_Transform.position, new Vector2(0.01f, 0.01f));
        Gizmos.DrawWireCube(groundCheck_Transform.position, new Vector2(0.1f, 0.1f));
        Gizmos.DrawWireCube(startGroundCheck_Transform.position, new Vector2(0.1f, 0.1f));
    }
}