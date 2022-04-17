using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetDoubleJumpAbility : GetAbilityBase
{
    protected override void Start()
    {
        base.Start();

        if (m_PlayerController.IsObtainDoubleJump)
            Destroy(gameObject);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            m_PlayerController.IsObtainDoubleJump = true;
            WhenGetAbility();
        }
    }
}