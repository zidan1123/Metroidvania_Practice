using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerStateType
{
    Idle, Run, Jump, Rush, WallSlide, DoubleJump
}

[Serializable]
public class PlayerParameter
{
    //Move
    [SerializeField] private float speed = 5f;
    private int facingDirection = 1;

    //Jump
    [Header("Jump")]
    [SerializeField] private float jumpForce = 14.5f;
    [SerializeField] private float startJumpTime = 0.32f;

    //DoubleJump
    [Header("DoubleJump")]
    [SerializeField] private float doubleJumpForce = 12f;

    //Ground
    [Header("Ground")]
    [SerializeField] private LayerMask whatIsGround = 1 << 6;

    //Dash
    [Header("Dash")]
    [SerializeField] private float dashDuration = 0.13f;
    [SerializeField] private float dashSpeed = 10.875f;
    [SerializeField] private float dashCD = 0.4f;

    //Attack
    [Header("Attack")]
    [SerializeField] private int attack = 5;
    [SerializeField] private float attackCD = 0.38f;
    [SerializeField] private float attackRadius = 0.57f;
    private LayerMask whatIsMonster = 1 << 7;
    [SerializeField] private float recoilForce = 4f;

    //WallSlide and WallSlideJump  *没有边滑墙边攻击的动画，无法制作这个功能
    [Header("WallSlide and WallSlideJump")]
    [SerializeField] private float wallSlideJumpingDuration = 0.2f;
    [SerializeField] private float wallSlideJumpCounterForce = 8f;
    [SerializeField] private float wallSlideSpeed = 2.5f;
    [SerializeField] private float wallSlideJumpSpeed = 14.4f;
    [SerializeField] private float startWallSlideInterruptTimer = 0.025f;  //按方向键持续一段时间后，才中断Wall Slide

    //Hurt
    [SerializeField] private float invincibleTime = 1.6f;

    //Life
    [SerializeField] private int hp = 5;
}

public class PlayerFSM : MonoBehaviour
{
    private IState currentState;

    private Dictionary<PlayerStateType, IState> statesDictionary = new Dictionary<PlayerStateType, IState>();

    public PlayerParameter playerParameter = new PlayerParameter();

    void Start()
    {
        statesDictionary.Add(PlayerStateType.Idle, new PlayerIdieState(this));

        TransitionState(PlayerStateType.Idle);
    }

    void Update()
    {
        currentState.OnUpdate();
    }

    public void TransitionState(PlayerStateType type)
    {
        if(currentState != null)
        {
            currentState.OnExit();
        }
        currentState = statesDictionary[type];
        currentState.OnEnter();
    }
}
