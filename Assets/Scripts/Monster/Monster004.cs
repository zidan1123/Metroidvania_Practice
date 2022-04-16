using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Pathfinding;

public class Monster004 : MonoBehaviour
{
    [Header("Basic Value")]
    [SerializeField] [Range(5, 20)] private int hp = 15;
    [SerializeField] [Range(1, 10)] private int attack = 1;
    [SerializeField] private float speed = 2;
    [HideInInspector] private bool isLife = true;
    [HideInInspector] private bool facingRight = true;

    private float[] attackDetails = new float[2];

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

    public int Attack
    {
        get { return attack; }
        set { attack = value; }
    }

    //Component
    private Transform m_Transform;
    private Rigidbody2D m_Rigidbody2D;
    private SkeletonAnimation m_SkeletonAnimation;
    private GameObject bloodEffect;

    //AI
    private Transform player_Transform; //不完全是角色的Transform，因为角色中心点太低了，这个读取的是角色的子物体Forward Check(NearUp)
    [SerializeField] private Transform groundCheck_Transform;
    private AIPath m_AIPath;
    private AIDestinationSetter m_AIDestinationSetter;
    private bool isLockPlayer; //没有锁定到角色时，距离3.5才会开始锁定并跟着角色，如果锁定了，角色距离9才会取消锁定和跟着角色
    private LayerMask whatIsGround = 1 << 6;

    void Start()
    {
        Init();
    }

    void Update()
    {
        if (isLife == true)
        { 
            //Move and AI
            if (m_AIPath.remainingDistance <= 3.5f && isLockPlayer == false)
            {
                m_SkeletonAnimation.AnimationState.SetAnimation(0, "Walk", true).TimeScale = 3f;
                isLockPlayer = true;
                m_AIPath.maxSpeed = 2f;
            }
            
            if (isLockPlayer == true)
            {
                if((player_Transform.position.x - m_Transform.position.x) > 0 && m_Transform.eulerAngles != new Vector3(0, 0, 0))
                {
                    m_Transform.eulerAngles = new Vector3(0, 0, 0);
                }
                else if((player_Transform.position.x - m_Transform.position.x) < 0 && m_Transform.eulerAngles != new Vector3(0, 180, 0))
                {
                    m_Transform.eulerAngles = new Vector3(0, 180, 0);
                }

                if (m_AIPath.remainingDistance > 9.5f)
                {
                    m_SkeletonAnimation.AnimationState.SetAnimation(0, "Idle", true).TimeScale = 2f;
                    isLockPlayer = false;
                    m_AIPath.maxSpeed = 0;
                }
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
        m_SkeletonAnimation = m_Transform.Find("Body").GetComponent<SkeletonAnimation>();
        bloodEffect = Resources.Load<GameObject>("Prefabs/Effects/Blood");

        //AI
        player_Transform = GameObject.FindGameObjectWithTag("Player").transform.Find("Forward Check(NearUp)").GetComponent<Transform>(); //不完全是角色的Transform，因为角色中心点太低了，这个读取的是角色的子物体Forward Check(NearUp)
        groundCheck_Transform = m_Transform.Find("Body/Ground Check").GetComponent<Transform>();
        m_AIPath = m_Transform.GetComponent<AIPath>();
        m_AIDestinationSetter = m_Transform.GetComponent<AIDestinationSetter>();

        m_AIDestinationSetter.target = player_Transform;
        m_SkeletonAnimation.AnimationState.SetAnimation(0, "Idle", true).TimeScale = 2f;
    }

    private void Damage(int damage)
    {
        this.HP -= damage;
    }

    private IEnumerator Hurt()
    {
        m_SkeletonAnimation.Skeleton.SetColor(Color.red);
        GameObject effect = GameObject.Instantiate<GameObject>(bloodEffect, m_Transform.position, Quaternion.identity);
        GameObject.Destroy(effect, 5);
        yield return new WaitForSeconds(0.075f);
        m_SkeletonAnimation.Skeleton.SetColor(Color.white);
    }

    private void Dead()
    {
        m_SkeletonAnimation.AnimationState.SetAnimation(0, "Dead", false).TimeScale = 1.5f;
        m_Rigidbody2D.gravityScale = 1.5f;
        isLife = false;
        GameObject.Destroy(gameObject.GetComponent<CircleCollider2D>());
        GameObject.Destroy(m_AIPath);
        GameObject.Destroy(m_AIDestinationSetter);
        
    }

    private void Flip()
    {
        if (facingRight == false)
        {
            m_Transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (facingRight == true)
        {
            m_Transform.eulerAngles = new Vector3(0, 180, 0);
        }
        facingRight = !facingRight;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            attackDetails[0] = attack;
            attackDetails[1] = m_Transform.position.x;
            collision.gameObject.SendMessage("Damage", attackDetails);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck_Transform.position, new Vector2(0.1f, 0.1f));
    }
}