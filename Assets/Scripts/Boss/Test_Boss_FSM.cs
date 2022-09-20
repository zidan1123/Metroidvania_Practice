using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Test_Boss_State
{
    Idle, JumpBack, JumpToAir, SkyAngleAttack, WallJumpToGround, LongDistanceAttack, FrontKick
}

public class Test_Boss_FSM : MonoBehaviour
{
    public Test_Boss_ParameterAndComponent test_Boss_ParameterAndComponent;
    
    private IState currentState;
    private Dictionary<Test_Boss_State, IState> test_Boss_StateDictionary = new Dictionary<Test_Boss_State, IState>();

    void Start()
    {
        test_Boss_ParameterAndComponent = gameObject.GetComponent<Test_Boss_ParameterAndComponent>();

        test_Boss_StateDictionary.Add(Test_Boss_State.Idle, new Test_Boss_Idle(this));
        test_Boss_StateDictionary.Add(Test_Boss_State.JumpBack, new Test_Boss_JumpBack(this));
        test_Boss_StateDictionary.Add(Test_Boss_State.JumpToAir, new Test_Boss_JumpToAir(this));
        test_Boss_StateDictionary.Add(Test_Boss_State.SkyAngleAttack, new Test_Boss_SkyAngleAttack(this));
        test_Boss_StateDictionary.Add(Test_Boss_State.WallJumpToGround, new Test_Boss_WallJumpToGround(this));
        test_Boss_StateDictionary.Add(Test_Boss_State.LongDistanceAttack, new Test_Boss_LongDistanceAttack(this));
        test_Boss_StateDictionary.Add(Test_Boss_State.FrontKick, new Test_Boss_FrontKick(this));

        StateTransition(Test_Boss_State.Idle);
    }

    void Update()
    {
        CheckFlip();
        currentState.OnUpdate();
    }

    public void StateTransition(Test_Boss_State state)
    {
        if (currentState != null)
        {
            currentState.OnExit();
        }
        currentState = test_Boss_StateDictionary[state];
        currentState.OnEnter();
    }

    private void CheckFlip()
    {
        if (test_Boss_ParameterAndComponent.canFlip)
        {
            if (test_Boss_ParameterAndComponent.target_player.position.x > test_Boss_ParameterAndComponent.m_Transform.position.x)
            {
                test_Boss_ParameterAndComponent.m_Transform.localScale = new Vector3(1, 1, 1);
                test_Boss_ParameterAndComponent.facingDirection = 1;
            }
            else if (test_Boss_ParameterAndComponent.target_player.position.x < test_Boss_ParameterAndComponent.m_Transform.position.x)
            {
                test_Boss_ParameterAndComponent.m_Transform.localScale = new Vector3(-1, 1, 1);
                test_Boss_ParameterAndComponent.facingDirection = -1;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            test_Boss_ParameterAndComponent.attackDetails[0] = test_Boss_ParameterAndComponent.Attack;
            test_Boss_ParameterAndComponent.attackDetails[1] = test_Boss_ParameterAndComponent.m_Transform.position.x;
            collision.gameObject.SendMessage("Damage", test_Boss_ParameterAndComponent.attackDetails);
        }
    }
}
