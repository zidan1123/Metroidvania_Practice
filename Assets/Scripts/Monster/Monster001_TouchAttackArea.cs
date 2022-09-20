using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster001_TouchAttackArea : MonoBehaviour
{
    private Monster001 m_Monster001;

    void Start()
    {
        m_Monster001 = gameObject.transform.parent.GetComponent<Monster001>();
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            m_Monster001.attackDetails[0] = m_Monster001.Attack;
            m_Monster001.attackDetails[1] = m_Monster001.m_Transform.position.x;
            collision.gameObject.SendMessage("Damage", m_Monster001.attackDetails);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            m_Monster001.attackDetails[0] = m_Monster001.Attack;
            m_Monster001.attackDetails[1] = m_Monster001.m_Transform.position.x;
            collision.gameObject.SendMessage("Damage", m_Monster001.attackDetails);
        }
    }
}
