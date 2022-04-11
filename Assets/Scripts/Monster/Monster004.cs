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
    private Transform player_Transform; //����ȫ�ǽ�ɫ��Transform����Ϊ��ɫ���ĵ�̫���ˣ������ȡ���ǽ�ɫ��������Forward Check(NearUp)
    private Transform parent_Transform; 
    private AIPath m_AIPath;
    private AIDestinationSetter m_AIDestinationSetter;
    private bool isLockPlayer; //û����������ɫʱ������3.5�ŻῪʼ���������Ž�ɫ����������ˣ���ɫ����9�Ż�ȡ�������͸��Ž�ɫ
    [SerializeField] private bool afterDeadGroundCheck; //��groundCheckͬλ��
    private LayerMask whatIsGround = 1 << 6;

    void Start()
    {
        m_Transform = gameObject.GetComponent<Transform>();
        m_Rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        m_SkeletonAnimation = gameObject.GetComponent<SkeletonAnimation>();
        bloodEffect = Resources.Load<GameObject>("Prefabs/Effects/Blood");

        //AI
        player_Transform = GameObject.FindGameObjectWithTag("Player").transform.Find("Forward Check(NearUp)").GetComponent<Transform>(); //����ȫ�ǽ�ɫ��Transform����Ϊ��ɫ���ĵ�̫���ˣ������ȡ���ǽ�ɫ��������Forward Check(NearUp)
        parent_Transform = m_Transform.parent;
        m_AIPath = m_Transform.parent.parent.GetComponent<AIPath>();
        m_AIDestinationSetter = m_Transform.parent.parent.GetComponent<AIDestinationSetter>();

        m_AIDestinationSetter.target = player_Transform;
        m_SkeletonAnimation.AnimationState.SetAnimation(0, "Idle", true).TimeScale = 2f;
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
                if((player_Transform.position.x - parent_Transform.position.x) > 0 && parent_Transform.eulerAngles != new Vector3(0, 0, 0))
                {
                    parent_Transform.eulerAngles = new Vector3(0, 0, 0);
                }
                else if((player_Transform.position.x - parent_Transform.position.x) < 0 && parent_Transform.eulerAngles != new Vector3(0, 180, 0))
                {
                    parent_Transform.eulerAngles = new Vector3(0, 180, 0);
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
            afterDeadGroundCheck = Physics2D.OverlapBox(m_Transform.position, new Vector2(0.1f, 0.1f), 0, whatIsGround);

            if (afterDeadGroundCheck == true) m_Rigidbody2D.bodyType = RigidbodyType2D.Static;
        }
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
        GameObject.Destroy(gameObject.GetComponent<BoxCollider2D>());
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().HP -= this.Attack;
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireCube(patrol_Left + Vector2.up / 2, new Vector2(1, 1));
    //    Gizmos.DrawWireCube(patrol_Right + Vector2.up / 2, new Vector2(1, 1));
    //}
}