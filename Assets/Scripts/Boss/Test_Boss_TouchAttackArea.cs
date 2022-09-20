using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Boss_TouchAttackArea : MonoBehaviour
{
    private Test_Boss_ParameterAndComponent m_Test_Boss_ParameterAndComponent;

    void Start()
    {
        m_Test_Boss_ParameterAndComponent = gameObject.transform.parent.GetComponent<Test_Boss_ParameterAndComponent>();
    }

    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            m_Test_Boss_ParameterAndComponent.attackDetails[0] = m_Test_Boss_ParameterAndComponent.Attack;
            m_Test_Boss_ParameterAndComponent.attackDetails[1] = m_Test_Boss_ParameterAndComponent.m_Transform.position.x;
            collision.gameObject.SendMessage("Damage", m_Test_Boss_ParameterAndComponent.attackDetails);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            m_Test_Boss_ParameterAndComponent.attackDetails[0] = m_Test_Boss_ParameterAndComponent.Attack;
            m_Test_Boss_ParameterAndComponent.attackDetails[1] = m_Test_Boss_ParameterAndComponent.m_Transform.position.x;
            collision.gameObject.SendMessage("Damage", m_Test_Boss_ParameterAndComponent.attackDetails);
        }
    }
}
