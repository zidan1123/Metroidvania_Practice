using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Boss_Idle : IState
{
    private float idleTimer;

    private Test_Boss_FSM test_Boss_FSM;
    private Test_Boss_ParameterAndComponent test_Boss_ParameterAndComponent;

    public Test_Boss_Idle(Test_Boss_FSM test_Boss_FSM)
    {
        this.test_Boss_FSM = test_Boss_FSM;
        this.test_Boss_ParameterAndComponent = test_Boss_FSM.test_Boss_ParameterAndComponent;
    }

    public void OnEnter()
    {
        test_Boss_ParameterAndComponent.m_Animator.Play("Test_Boss_Idle");
    }

    public void OnUpdate()
    {
        idleTimer += Time.deltaTime;
        if(idleTimer >= test_Boss_ParameterAndComponent.idleTime)
        {
            //远处的检测器检测主角
            Collider2D collider_0 = Physics2D.OverlapBox(test_Boss_ParameterAndComponent.frontFar_Sensor.position, new Vector2(test_Boss_ParameterAndComponent.frontFar_Sensor_Box_X, test_Boss_ParameterAndComponent.frontFar_Sensor_Box_Y), 0, test_Boss_ParameterAndComponent.whatIsPlayer);
            if(collider_0 != null)
            {
                int randomNum = Random.Range(0, 2);
                if(randomNum == 0)
                {
                    Debug.Log("IdleToJumpToAir");
                    test_Boss_FSM.StateTransition(Test_Boss_State.JumpToAir);
                }
                else
                {
                    Debug.Log("IdleToLongDistanceAttack");
                    test_Boss_FSM.StateTransition(Test_Boss_State.LongDistanceAttack);
                }
            }

            
            

            //近处的检测器检测主角
            Collider2D collider_1 = Physics2D.OverlapBox(test_Boss_ParameterAndComponent.frontNear_Sensor.position, new Vector2(test_Boss_ParameterAndComponent.frontNear_Sensor_Box_X, test_Boss_ParameterAndComponent.frontNear_Sensor_Box_Y), 0, test_Boss_ParameterAndComponent.whatIsPlayer);
            if (collider_1 != null)
            {
                RaycastHit2D raycastHit2D = Physics2D.Raycast(test_Boss_ParameterAndComponent.backWall_Sensor.position, new Vector2(-test_Boss_ParameterAndComponent.facingDirection, 0), 16, test_Boss_ParameterAndComponent.whatIsGround);
                if (raycastHit2D.distance >= 8.25f)
                {
                    int randomNum = Random.Range(0, 3);
                    if (randomNum == 0)
                    {
                        Debug.Log("IdleToJumpToFrontKick");
                        test_Boss_FSM.StateTransition(Test_Boss_State.FrontKick);
                    }
                    else
                    {
                        Debug.Log("IdleToJumpBack");
                        test_Boss_FSM.StateTransition(Test_Boss_State.JumpBack);
                    }
                }
                else
                {
                    Debug.Log("IdleToJumpToFrontKick");
                    test_Boss_FSM.StateTransition(Test_Boss_State.FrontKick);
                }
            }
        }
    }

    public void OnExit()
    {
        idleTimer = 0;
    }
}

public class Test_Boss_JumpBack : IState
{
    private float timer;

    private Test_Boss_FSM test_Boss_FSM;
    private Test_Boss_ParameterAndComponent test_Boss_ParameterAndComponent;

    public Test_Boss_JumpBack(Test_Boss_FSM test_Boss_FSM)
    {
        this.test_Boss_FSM = test_Boss_FSM;
        this.test_Boss_ParameterAndComponent = test_Boss_FSM.test_Boss_ParameterAndComponent;
    }

    public void OnEnter()
    {
        test_Boss_ParameterAndComponent.m_Animator.Play("Test_Boss_Jumpback");

        JumpBack();
    }

    public void OnUpdate()
    {
        timer += Time.deltaTime;
        if (timer >= test_Boss_ParameterAndComponent.m_Animator.GetCurrentAnimatorStateInfo(0).length)
        {
            test_Boss_FSM.StateTransition(Test_Boss_State.Idle);
        }
    }

    public void OnExit()
    {
        timer = 0;

        test_Boss_ParameterAndComponent.m_Rigidbody2D.velocity = Vector2.zero;
    }

    private void JumpBack()
    {
        if (test_Boss_ParameterAndComponent.facingDirection == 1)
        {
            test_Boss_ParameterAndComponent.m_Rigidbody2D.AddForce(new Vector2(-test_Boss_ParameterAndComponent.JumpBackXForce, test_Boss_ParameterAndComponent.JumpBackYForce));
        }
        else
        {
            test_Boss_ParameterAndComponent.m_Rigidbody2D.AddForce(new Vector2(test_Boss_ParameterAndComponent.JumpBackXForce, test_Boss_ParameterAndComponent.JumpBackYForce));
        }
    }
}

public class Test_Boss_JumpToAir : IState
{
    private float jumpToAirDurationtimer;
    private float jumpToAirStaytimer;

    private Test_Boss_FSM test_Boss_FSM;
    private Test_Boss_ParameterAndComponent test_Boss_ParameterAndComponent;

    public Test_Boss_JumpToAir(Test_Boss_FSM test_Boss_FSM)
    {
        this.test_Boss_FSM = test_Boss_FSM;
        this.test_Boss_ParameterAndComponent = test_Boss_FSM.test_Boss_ParameterAndComponent;
    }

    public void OnEnter()
    {
        test_Boss_ParameterAndComponent.m_Animator.Play("Test_Boss_JumpToAir");

        JumpToAir();
    }

    public void OnUpdate()
    {
        jumpToAirDurationtimer += Time.deltaTime;
        if(jumpToAirDurationtimer >= test_Boss_ParameterAndComponent.JumpToAirDurationTime)
        {
            test_Boss_ParameterAndComponent.m_Rigidbody2D.velocity = Vector2.zero;
            test_Boss_ParameterAndComponent.m_Rigidbody2D.gravityScale = 0;

            jumpToAirStaytimer += Time.deltaTime;
            if (jumpToAirStaytimer >= test_Boss_ParameterAndComponent.JumpToAirStayTime)
            {
                test_Boss_FSM.StateTransition(Test_Boss_State.SkyAngleAttack);
            }
        }
    }

    public void OnExit()
    {
        jumpToAirDurationtimer = 0;
        jumpToAirStaytimer = 0;
    }

    private void JumpToAir()
    {
        test_Boss_ParameterAndComponent.m_Rigidbody2D.AddForce(new Vector2(0, test_Boss_ParameterAndComponent.JumpToAirForce));
    }
}

public class Test_Boss_SkyAngleAttack : IState
{
    private float attackGroundStayTimer;
    private float attackWallStayTimer;

    Vector2 targetDirection;

    private Test_Boss_FSM test_Boss_FSM;
    private Test_Boss_ParameterAndComponent test_Boss_ParameterAndComponent;

    public Test_Boss_SkyAngleAttack(Test_Boss_FSM test_Boss_FSM)
    {
        this.test_Boss_FSM = test_Boss_FSM;
        this.test_Boss_ParameterAndComponent = test_Boss_FSM.test_Boss_ParameterAndComponent;
    }

    public void OnEnter()
    {
        test_Boss_ParameterAndComponent.m_Animator.Play("Test_Boss_SkyAngleAttack");

        test_Boss_ParameterAndComponent.canFlip = false;

        targetDirection = ((test_Boss_ParameterAndComponent.target_player.position + new Vector3(0, 1.2f, 0)) - test_Boss_ParameterAndComponent.m_Transform.position).normalized;
        test_Boss_ParameterAndComponent.m_Rigidbody2D.velocity = targetDirection * test_Boss_ParameterAndComponent.SkyAngleAttackSpeed;
    }

    public void OnUpdate()
    {
        if(Physics2D.OverlapCircle(test_Boss_ParameterAndComponent.ground_Sensor.position, 0.005f, test_Boss_ParameterAndComponent.whatIsGround))
        {
            test_Boss_ParameterAndComponent.m_Animator.Play("Test_Boss_AttackGround");
            test_Boss_ParameterAndComponent.m_Rigidbody2D.velocity = Vector2.zero;

            attackGroundStayTimer += Time.deltaTime;
            if(attackGroundStayTimer >= test_Boss_ParameterAndComponent.AttackGroundStayTime)
            {
                test_Boss_FSM.StateTransition(Test_Boss_State.Idle);
            }
        }

        if (Physics2D.OverlapCircle(test_Boss_ParameterAndComponent.wall_Sensor.position, 0.01f, test_Boss_ParameterAndComponent.whatIsGround))
        {
            test_Boss_ParameterAndComponent.m_Animator.Play("Test_Boss_AttackInWall");
            test_Boss_ParameterAndComponent.m_Rigidbody2D.velocity = Vector2.zero;

            attackWallStayTimer += Time.deltaTime;
            if (attackWallStayTimer >= test_Boss_ParameterAndComponent.AttackWallStayTime)
            {
                test_Boss_FSM.StateTransition(Test_Boss_State.WallJumpToGround);
            }
        }
    }

    public void OnExit()
    {
        attackGroundStayTimer = 0;
        attackWallStayTimer = 0;
        test_Boss_ParameterAndComponent.m_Rigidbody2D.gravityScale = 3.5f;

        test_Boss_ParameterAndComponent.canFlip = true;
    }
}

public class Test_Boss_WallJumpToGround : IState
{
    private Test_Boss_FSM test_Boss_FSM;
    private Test_Boss_ParameterAndComponent test_Boss_ParameterAndComponent;

    public Test_Boss_WallJumpToGround(Test_Boss_FSM test_Boss_FSM)
    {
        this.test_Boss_FSM = test_Boss_FSM;
        this.test_Boss_ParameterAndComponent = test_Boss_FSM.test_Boss_ParameterAndComponent;
    }

    public void OnEnter()
    {
        test_Boss_ParameterAndComponent.m_Animator.Play("Test_Boss_WallJumpToGround");

        if (test_Boss_ParameterAndComponent.facingDirection == 1)
        {
            test_Boss_ParameterAndComponent.m_Rigidbody2D.AddForce(new Vector2(-test_Boss_ParameterAndComponent.wallJumpToGroundXForce, test_Boss_ParameterAndComponent.wallJumpToGroundYForce));
        }
        else
        {
            test_Boss_ParameterAndComponent.m_Rigidbody2D.AddForce(new Vector2(test_Boss_ParameterAndComponent.wallJumpToGroundXForce, test_Boss_ParameterAndComponent.wallJumpToGroundYForce));
        }
    }

    public void OnUpdate()
    {
        if (Physics2D.OverlapCircle(test_Boss_ParameterAndComponent.ground_Sensor.position, 0.005f, test_Boss_ParameterAndComponent.whatIsGround))
        {
            test_Boss_ParameterAndComponent.m_Rigidbody2D.velocity = Vector2.zero;
            test_Boss_FSM.StateTransition(Test_Boss_State.Idle);
        }
    }

    public void OnExit()
    {
        
    }
}

public class Test_Boss_LongDistanceAttack : IState
{
    private bool isShoot;
    private float longDistanceAttackDurationtimer;

    private Test_Boss_FSM test_Boss_FSM;
    private Test_Boss_ParameterAndComponent test_Boss_ParameterAndComponent;

    public Test_Boss_LongDistanceAttack(Test_Boss_FSM test_Boss_FSM)
    {
        this.test_Boss_FSM = test_Boss_FSM;
        this.test_Boss_ParameterAndComponent = test_Boss_FSM.test_Boss_ParameterAndComponent;
    }

    public void OnEnter()
    {
        test_Boss_ParameterAndComponent.m_Animator.Play("Test_Boss_LongDistanceAttack");
    }

    public void OnUpdate()
    {
        longDistanceAttackDurationtimer += Time.deltaTime;

        if((longDistanceAttackDurationtimer >= test_Boss_ParameterAndComponent.longDistanceAttackDurationtime - 0.3f) && !isShoot)
        {
            isShoot = true;
            Test_Boss_Projectile test_Boss_Projectile = GameObject.Instantiate<GameObject>(test_Boss_ParameterAndComponent.test_Boss_Projectile
                , test_Boss_ParameterAndComponent.wall_Sensor.position, Quaternion.identity).GetComponent<Test_Boss_Projectile>();

            if (test_Boss_ParameterAndComponent.facingDirection == 1)
            {
                test_Boss_Projectile.SetProjectile(test_Boss_ParameterAndComponent.Attack, Vector2.right, test_Boss_ParameterAndComponent.longDistanceAttackShotSpeed);
            }
            else
            {
                test_Boss_Projectile.SetProjectile(test_Boss_ParameterAndComponent.Attack, Vector2.left, test_Boss_ParameterAndComponent.longDistanceAttackShotSpeed);
            }
        }

        if (longDistanceAttackDurationtimer >= test_Boss_ParameterAndComponent.longDistanceAttackDurationtime)
        {
            test_Boss_FSM.StateTransition(Test_Boss_State.Idle);
        }
    }

    public void OnExit()
    {
        longDistanceAttackDurationtimer = 0;
        isShoot = false;
    }
}

public class Test_Boss_FrontKick : IState
{
    private bool isKick;
    private float beforeKickDurationTimer;
    private float inAirTimer;
    private bool isStartInAirTimer;

    private Test_Boss_FSM test_Boss_FSM;
    private Test_Boss_ParameterAndComponent test_Boss_ParameterAndComponent;

    public Test_Boss_FrontKick(Test_Boss_FSM test_Boss_FSM)
    {
        this.test_Boss_FSM = test_Boss_FSM;
        this.test_Boss_ParameterAndComponent = test_Boss_FSM.test_Boss_ParameterAndComponent;
    }

    public void OnEnter()
    {
        test_Boss_ParameterAndComponent.m_Animator.Play("Test_Boss_FrontKick");
    }

    public void OnUpdate()
    {
        beforeKickDurationTimer += Time.deltaTime;
        if ((beforeKickDurationTimer >= test_Boss_ParameterAndComponent.beforeKickDurationTime)  && !isKick)
        {
            isKick = true;
            FrontKick();
        }

        if (isStartInAirTimer)
        {
            inAirTimer += Time.deltaTime;
            if(inAirTimer >= test_Boss_ParameterAndComponent.m_Animator.GetCurrentAnimatorStateInfo(0).length / 3) //在空中的时长至少是整个动画的三分之一，才开始检测地面
            {
                if (Physics2D.OverlapCircle(test_Boss_ParameterAndComponent.ground_Sensor.position, 0.005f, test_Boss_ParameterAndComponent.whatIsGround))
                {
                    test_Boss_ParameterAndComponent.m_Rigidbody2D.velocity = Vector2.zero;
                    test_Boss_FSM.StateTransition(Test_Boss_State.Idle);
                }
            }
        }
    }

    public void OnExit()
    {
        isKick = false;
        beforeKickDurationTimer = 0;
        inAirTimer = 0;
        isStartInAirTimer = false;
}

    private void FrontKick()
    {
        if (test_Boss_ParameterAndComponent.facingDirection == 1)
        {
            Debug.Log("1");
            test_Boss_ParameterAndComponent.m_Rigidbody2D.AddForce(new Vector2(test_Boss_ParameterAndComponent.frontKickXForce, test_Boss_ParameterAndComponent.frontKickYForce));
        }
        else
        {
            Debug.Log("-1");
            test_Boss_ParameterAndComponent.m_Rigidbody2D.AddForce(new Vector2(-test_Boss_ParameterAndComponent.frontKickXForce, test_Boss_ParameterAndComponent.frontKickYForce));
        }
        isStartInAirTimer = true;
    }
}
