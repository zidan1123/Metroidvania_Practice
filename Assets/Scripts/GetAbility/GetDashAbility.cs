using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetDashAbility : GetAbilityBase
{
    protected override void Start()
    {
        base.Start();

        if (m_PlayerController.IsObtainDash)
            Destroy(gameObject);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            m_PlayerController.IsObtainDash = true;
            WhenGetAbility();
        }
    }
}
