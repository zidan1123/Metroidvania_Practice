using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster002_TouchAttackArea : MonoBehaviour
{
    private Monster002 m_Monster002;

    void Start()
    {
        m_Monster002 = gameObject.transform.parent.GetComponent<Monster002>();
    }

    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            m_Monster002.attackDetails[0] = m_Monster002.Attack;
            m_Monster002.attackDetails[1] = m_Monster002.m_Transform.position.x;
            collision.gameObject.SendMessage("Damage", m_Monster002.attackDetails);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            m_Monster002.attackDetails[0] = m_Monster002.Attack;
            m_Monster002.attackDetails[1] = m_Monster002.m_Transform.position.x;
            collision.gameObject.SendMessage("Damage", m_Monster002.attackDetails);
        }
    }
}
