using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Boss_Projectile : MonoBehaviour
{
    //Component
    private Transform m_Transform;
    private BoxCollider2D m_BoxCollider2D;
    private Rigidbody2D m_Rigidbody2D;

    private float[] attackDetails = new float[2];

    private int attack;

    void Awake()
    {
        m_Transform = gameObject.GetComponent<Transform>();
        m_BoxCollider2D = gameObject.GetComponent<BoxCollider2D>();
        m_Rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
    }

    public void SetProjectile(int attack, Vector2 shotDirection, float shotSpeed)
    {
        this.attack = attack;
        m_Rigidbody2D.velocity = shotDirection * shotSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            attackDetails[0] = attack;
            attackDetails[1] = m_Transform.position.x;
            collision.gameObject.SendMessage("Damage", attackDetails);
        }
    }
}
