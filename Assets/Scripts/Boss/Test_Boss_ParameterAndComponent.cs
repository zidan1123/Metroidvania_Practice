using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Boss_ParameterAndComponent : MonoBehaviour
{
    [Header("Basic Value")]
    [SerializeField] private int hp = 15;
    [SerializeField] [Range(1, 10)] private int attack = 1;

    public float[] attackDetails = new float[2];

    public bool canFlip = true;
    public int facingDirection;

    //Component
    public Transform m_Transform;
    public BoxCollider2D m_BoxCollider2D;
    public Rigidbody2D m_Rigidbody2D;
    public SpriteRenderer m_SpriteRenderer;
    public Animator m_Animator;

    public Transform ground_Sensor;
    public Transform wall_Sensor;
    public Transform frontFar_Sensor;
    public Transform frontNear_Sensor;
    public Transform backWall_Sensor;

    public float frontFar_Sensor_Box_X;
    public float frontFar_Sensor_Box_Y;
    public float frontNear_Sensor_Box_X;
    public float frontNear_Sensor_Box_Y;

    public LayerMask whatIsGround = 1 << 6;
    public LayerMask whatIsPlayer = 1 << 8;

    public float idleTime;

    public int JumpBackXForce;
    public int JumpBackYForce;

    public int JumpToAirForce;
    public float JumpToAirDurationTime;
    public float JumpToAirStayTime;

    public float SkyAngleAttackSpeed;
    public float AttackGroundStayTime;
    public float AttackWallStayTime;

    public int wallJumpToGroundXForce;
    public int wallJumpToGroundYForce;

    public GameObject test_Boss_Projectile;
    public float longDistanceAttackShotSpeed;
    public float longDistanceAttackDurationtime;

    public int frontKickXForce;
    public int frontKickYForce;
    public float beforeKickDurationTime;

    public Transform target_player;

    private GameObject bloodEffect;

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

    public int Attack { get { return attack; } }

    void Awake()
    {
        m_Transform = gameObject.GetComponent<Transform>();
        m_BoxCollider2D = gameObject.GetComponent<BoxCollider2D>();
        m_Rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        m_SpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        m_Animator = gameObject.GetComponent<Animator>();

        ground_Sensor = m_Transform.Find("Ground_Sensor");
        wall_Sensor = m_Transform.Find("Wall_Sensor");
        frontFar_Sensor = m_Transform.Find("FrontFar_Sensor");
        frontNear_Sensor = m_Transform.Find("FrontNear_Sensor");
        backWall_Sensor = m_Transform.Find("BackWall_Sensor");

        test_Boss_Projectile = Resources.Load<GameObject>("Prefabs/Projectiles/Test_Boss_Projectile");
        bloodEffect = Resources.Load<GameObject>("Prefabs/Effects/Blood");
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
        GameObject.Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(ground_Sensor.position, 0.001f);
        Gizmos.DrawWireSphere(wall_Sensor.position, 0.001f);
        Gizmos.DrawWireCube(frontFar_Sensor.position, new Vector2(frontFar_Sensor_Box_X, frontFar_Sensor_Box_Y));
        Gizmos.DrawWireCube(frontNear_Sensor.position, new Vector2(frontNear_Sensor_Box_X, frontNear_Sensor_Box_Y));
        Gizmos.DrawRay(backWall_Sensor.position, new Vector2(-facingDirection, 0));
    }
}
