using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

/// <summary>
/// 停顿冲刺怪
/// </summary>
public class Monster002 : MonoBehaviour
{
    [Header("Basic Value")]
    [SerializeField][Range(5 , 20)] private int hp = 10;
    [SerializeField][Range(1, 10)] private int attack = 1;
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
    private Vector2 targetPosition;
    private bool isReachTargetPosition = false;
    private Transform forwardDistanceDetector;
    private RaycastHit2D forwardWallInfo;
    [SerializeField] private Transform forwardCheck_Middle;
    [SerializeField] private Transform backwardCheck_Middle;

    private bool isDetectPlayer_Forward = false;
    private bool isDetectPlayer_Backward = false;
    private LayerMask whatIsPlayer = 1 << 8;
    private LayerMask whatIsGround = 1 << 6;  //Wall

    private bool isLockPlayer = false;

    //Sprint
    [SerializeField] private float sprintSpeed = 5;
    [SerializeField] private float sprintTime;
    [SerializeField] private float startSprintTime = 0.5f;
    private bool isSprinting = false;

    //Random Walk
    float remainsDistance = 0;

    void Start()
    {
        Init();
    }

    void Update()
    {
        if (isLife == true)
        {
            //Move
            if (isLockPlayer == false)
            {
                Patrol();
            }

            //AI Detect
            DetectWallAndPlayer();

            //Sprint
            if (isSprinting == true)
            {
                Sprint();
            }
        }
    }

    private void Init()
    {
        m_Transform = gameObject.GetComponent<Transform>();
        m_Rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        m_SkeletonAnimation = gameObject.GetComponent<SkeletonAnimation>();
        bloodEffect = Resources.Load<GameObject>("Prefabs/Effects/Blood");

        //AI
        m_SkeletonAnimation.AnimationState.SetAnimation(0, "Walk", true).TimeScale = 1.5f;
        forwardDistanceDetector = m_Transform.Find("Body/ForwardDistanceDetector");
        forwardCheck_Middle = m_Transform.Find("Body/Middle Of Forward Check");
        backwardCheck_Middle = m_Transform.Find("Body/Middle Of Backward Check");

        Physics2D.queriesStartInColliders = false;
        sprintTime = startSprintTime;
        forwardWallInfo = Physics2D.Raycast(forwardDistanceDetector.position, forwardDistanceDetector.right, 75, whatIsGround);
        StartCoroutine("CountAndChangeTargetPosition");
    }

    private void Patrol()
    {
        //↓由于这个算法是经过一些小数运算的，会出现小误差，有时不会准准等于0
        if (Vector2.Distance(forwardDistanceDetector.position, targetPosition) <= 5E-07 && isReachTargetPosition == false) //往前走达到目的地时
        {
            if (remainsDistance > 0) //往前走达到目的地后，还需要继续走，这时肯定前方是墙壁，所以转身后继续走
            {
                Flip();
                if (facingRight == true) //转身后向右
                {
                    targetPosition = new Vector2(forwardDistanceDetector.position.x + remainsDistance, forwardDistanceDetector.position.y);
                }
                else if (facingRight == false) //转身后向左  //转身后x轴加大是向左的,因为子物体坐标系是相对于父物体的
                {
                    targetPosition = new Vector2(forwardDistanceDetector.position.x - remainsDistance, forwardDistanceDetector.position.y);
                }
                remainsDistance = 0;
            }
            else if (remainsDistance == 0) //往前走达到目的地后 ，不需要继续走了
            {
                isReachTargetPosition = true;
                targetPosition = forwardDistanceDetector.position; //Eliminate errors after some decimal operations
                m_SkeletonAnimation.AnimationState.SetAnimation(0, "Idle", true);
                StartCoroutine("CountAndChangeTargetPosition");
            }
        }
        else if (Vector2.Distance(forwardDistanceDetector.position, targetPosition) > 0 && isReachTargetPosition == false) //往前向目的地走 
        {
            if (facingRight == true)
            {
                //↓目标位置怪物的x坐标
                float pivotDistance = targetPosition.x - (forwardDistanceDetector.position.x - m_Transform.position.x);
                m_Transform.position = Vector2.MoveTowards(m_Transform.position, new Vector2(pivotDistance, targetPosition.y - 0.055f), speed * Time.deltaTime);
            }
            else if (facingRight == false) //转身后x轴加大是向左的,因为子物体坐标系是相对于父物体的
            {
                float pivotDistance = targetPosition.x - (forwardDistanceDetector.position.x - m_Transform.position.x);
                m_Transform.position = Vector2.MoveTowards(m_Transform.position, new Vector2(pivotDistance, targetPosition.y - 0.055f), speed * Time.deltaTime);
            }
        }
    }

    private void DetectWallAndPlayer()
    {
        if (isLife == true) //Detect walls ahead and players in front and behind
        {
            forwardWallInfo = Physics2D.Raycast(forwardDistanceDetector.position, forwardDistanceDetector.right, 75, whatIsGround);
            isDetectPlayer_Forward = Physics2D.OverlapBox(forwardCheck_Middle.position, new Vector2(2.5f, 1f), 0, whatIsPlayer);
            isDetectPlayer_Backward = Physics2D.OverlapBox(backwardCheck_Middle.position, new Vector2(2.5f, 1f), 0, whatIsPlayer);
        }

        if (isLockPlayer == false)
        {
            if (isDetectPlayer_Forward == true)  //Player detected ahead
            {
                isLockPlayer = true;
                StartCoroutine("StartSprint");
            }
            else if (isDetectPlayer_Backward == true)  //Player detected from behind
            {
                isLockPlayer = true;
                Flip();
                StartCoroutine("StartSprint");
            }
        }
    }

    private void Damage(int damage)
    {
        HP -= damage;
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

        StopCoroutine("StartSprint");  //002特有
        StopCoroutine("EndSprint");  //002特有
        StopCoroutine("CountAndChangeTargetPosition");  //002特有
    }

    private IEnumerator CountAndChangeTargetPosition()
    {
        float marchDistance = Random.Range(3f, 7f);
        if (Vector2.Distance(forwardDistanceDetector.position, forwardWallInfo.point) >= marchDistance)  //这两个比较的距离是世界坐标系
        {
            //Debug.Log("Up");
            if (facingRight == true)
            {
                targetPosition = new Vector2(forwardDistanceDetector.position.x + marchDistance, forwardDistanceDetector.position.y);
            }
            else if(facingRight == false)
            {
                targetPosition = new Vector2(forwardDistanceDetector.position.x - marchDistance, forwardDistanceDetector.position.y);
            }
            //Debug.Log("targetPosition:" + targetPosition);
        }
        else if (Vector2.Distance(forwardDistanceDetector.position, forwardWallInfo.point) < marchDistance)
        {
            //Debug.Log("down");
            if (facingRight == true)
            {
                float walkDistance = Vector2.Distance(forwardDistanceDetector.position, forwardWallInfo.point);
                remainsDistance = marchDistance - Vector2.Distance(forwardDistanceDetector.position, forwardWallInfo.point);
                targetPosition = new Vector2(forwardDistanceDetector.position.x + walkDistance, forwardDistanceDetector.position.y);
            }
            else if (facingRight == false)  //转身后x轴加大是向左的
            {
                float walkDistance = Vector2.Distance(forwardDistanceDetector.position, forwardWallInfo.point);
                remainsDistance = marchDistance - Vector2.Distance(forwardDistanceDetector.position, forwardWallInfo.point);
                targetPosition = new Vector2(forwardDistanceDetector.position.x - walkDistance, forwardDistanceDetector.position.y);
                //Debug.Log("targetPosition:" + targetPosition);
                //Debug.Log("marchDistance:" + marchDistance);
            }
        }

        yield return new WaitForSeconds(1);
        isReachTargetPosition = false;
        m_SkeletonAnimation.AnimationState.SetAnimation(0, "Walk", true).TimeScale = 1.5f;
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

    private IEnumerator StartSprint()
    {
        StopCoroutine("CountAndChangeTargetPosition");  //避免这个协程开启的Walk动画
        m_SkeletonAnimation.AnimationState.SetAnimation(0, "Dead", true).TimeScale = 1.5f;
        yield return new WaitForSeconds(0.2f);
        m_SkeletonAnimation.AnimationState.SetAnimation(0, "Walk", true).TimeScale = 4f;
        yield return new WaitForSeconds(0.05f);
        isSprinting = true;
    }

    private void Sprint()
    {
        if(sprintTime > 0)
        {
            sprintTime -= Time.deltaTime;
            if (facingRight == true)
            {
                m_Rigidbody2D.velocity = new Vector2(sprintSpeed, m_Rigidbody2D.velocity.y);
            }
            else if (facingRight == false)
            {
                m_Rigidbody2D.velocity = new Vector2(-sprintSpeed, m_Rigidbody2D.velocity.y);
            }
        }
        else if(sprintTime <= 0)
        {
            StartCoroutine("EndSprint");
        }
    }

    private IEnumerator EndSprint()
    {
        isSprinting = false;
        sprintTime = startSprintTime;
        m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y);
        m_SkeletonAnimation.AnimationState.SetAnimation(0, "Idle", true);
        m_Rigidbody2D.bodyType = RigidbodyType2D.Static; //避免受到角色的碰撞而后退(尝试了很多方法，如果是OnCollisionEnter2D才开启"霸体"，都会受到那一瞬间Enter的力)
        yield return new WaitForSeconds(0.4f);  //连续冲刺CD
        m_Rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        m_SkeletonAnimation.AnimationState.SetAnimation(0, "Walk", true).TimeScale = 1.5f;
        isLockPlayer = false;

        StartCoroutine("CountAndChangeTargetPosition");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().HP -= this.Attack;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(forwardCheck_Middle.position, new Vector2(2.5f, 1f));
        Gizmos.color = new Color(1, 0.5f, 0.5f, 1);  //pink
        Gizmos.DrawWireCube(backwardCheck_Middle.position, new Vector2(2.5f, 1f)); 
    }
}
