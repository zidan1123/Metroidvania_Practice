using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

/// <summary>
/// 来回走怪
/// </summary>
public class Monster001 : MonoBehaviour
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
            if(hp > 0)
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
    [SerializeField] private Vector2 patrol_Left;
    [SerializeField] private Vector2 patrol_Right;
    private Vector2 targetPosition;
    private bool isReachTargetPosition = false;

    void Start()
    {
        m_Transform = gameObject.GetComponent<Transform>();
        m_Rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        m_SkeletonAnimation = gameObject.GetComponent<SkeletonAnimation>();
        bloodEffect = Resources.Load<GameObject>("Prefabs/Effects/Blood");

        //AI
        patrol_Left = m_Transform.Find("Patrol_Left").position;
        patrol_Right = m_Transform.Find("Patrol_Right").position;
        targetPosition = patrol_Right;
        m_SkeletonAnimation.AnimationState.SetAnimation(0, "Walk", true).TimeScale = 1.5f;
    }

    void Update()
    {
        //Move
        if(isLife == true)
        {
            if (Vector2.Distance(m_Transform.position, targetPosition) > 0)
            {
                m_Transform.position = Vector2.MoveTowards(m_Transform.position, targetPosition, speed * Time.deltaTime);
            }
            else if (Vector2.Distance(m_Transform.position, targetPosition) == 0 && isReachTargetPosition == false)
            {
                isReachTargetPosition = true;
                StartCoroutine("ChangeTargetPosition");
            }
        }
        
        //AI
        if (isLife == true)
        {
            //Physics2D.OverlapArea(forwardCheck_FarDown.position, forwardCheck_NearUp.position, whatIsGround);
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
        isLife = false;
        GameObject.Destroy(gameObject.GetComponent<BoxCollider2D>());
        GameObject.Destroy(m_Rigidbody2D);
    }

    private IEnumerator ChangeTargetPosition()
    {
        m_SkeletonAnimation.AnimationState.SetAnimation(0, "Idle", true);
        yield return new WaitForSeconds(1);
        if (targetPosition == patrol_Left)
        {
            targetPosition = patrol_Right;
        }
        else if (targetPosition == patrol_Right)
        {
            targetPosition = patrol_Left;
        }
        isReachTargetPosition = false;
        if(isLife == true)
        {
            m_SkeletonAnimation.AnimationState.SetAnimation(0, "Walk", true).TimeScale = 1.5f;
            Flip();
        }
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
        m_Rigidbody2D.velocity = Vector2.zero; //确保怪物不被主角撞飞
        if (collision.gameObject.tag == "Player")
        {
            attackDetails[0] = attack;
            attackDetails[1] = m_Transform.position.x;
            collision.gameObject.SendMessage("Damage", attackDetails);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(patrol_Left + Vector2.up / 2, new Vector2(1, 1));
        Gizmos.DrawWireCube(patrol_Right + Vector2.up / 2, new Vector2(1, 1));
    }
}
