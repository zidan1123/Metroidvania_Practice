using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdieState : IState
{
    private PlayerFSM playerFSM;
    private PlayerParameter playerParameter;

    public PlayerIdieState(PlayerFSM playerFSM)
    {
        this.playerFSM = playerFSM;
        this.playerParameter = playerFSM.playerParameter;
    }

    public void OnEnter()
    {
       
    }

    public void OnUpdate()
    {
        
    }

    public void OnExit()
    {
        
    }
}

public class PlayerRunState : IState
{
    private PlayerFSM playerFSM;
    private PlayerParameter playerParameter;

    public PlayerRunState(PlayerFSM playerFSM)
    {
        this.playerFSM = playerFSM;
        this.playerParameter = playerFSM.playerParameter;
    }

    public void OnEnter()
    {

    }

    public void OnUpdate()
    {

    }

    public void OnExit()
    {

    }
}

public class PlayerJumpState : IState
{
    private PlayerFSM playerFSM;
    private PlayerParameter playerParameter;

    public PlayerJumpState(PlayerFSM playerFSM)
    {
        this.playerFSM = playerFSM;
        this.playerParameter = playerFSM.playerParameter;
    }

    public void OnEnter()
    {

    }

    public void OnUpdate()
    {

    }

    public void OnExit()
    {

    }
}

public class PlayerRushState : IState
{
    private PlayerFSM playerFSM;
    private PlayerParameter playerParameter;

    public PlayerRushState(PlayerFSM playerFSM)
    {
        this.playerFSM = playerFSM;
        this.playerParameter = playerFSM.playerParameter;
    }

    public void OnEnter()
    {

    }

    public void OnUpdate()
    {

    }

    public void OnExit()
    {

    }
}

public class PlayerWallSlideState : IState
{
    private PlayerFSM playerFSM;
    private PlayerParameter playerParameter;

    public PlayerWallSlideState(PlayerFSM playerFSM)
    {
        this.playerFSM = playerFSM;
        this.playerParameter = playerFSM.playerParameter;
    }

    public void OnEnter()
    {

    }

    public void OnUpdate()
    {

    }

    public void OnExit()
    {

    }
}

public class PlayerDoubleJumpState : IState
{
    private PlayerFSM playerFSM;
    private PlayerParameter playerParameter;

    public PlayerDoubleJumpState(PlayerFSM playerFSM)
    {
        this.playerFSM = playerFSM;
        this.playerParameter = playerFSM.playerParameter;
    }

    public void OnEnter()
    {

    }

    public void OnUpdate()
    {

    }

    public void OnExit()
    {

    }
}
