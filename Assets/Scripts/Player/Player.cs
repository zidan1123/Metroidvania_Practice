using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private int isObtainDash;
    private int isObtainDoubleJump;
    private int isObtainWallSlide;

    public int IsObtainDash { get { return isObtainDash; } set { isObtainDash = value; } }
    public int IsObtainDoubleJump { get { return isObtainDoubleJump; } set { isObtainDoubleJump = value; } }
    public int IsObtainWallSlide { get { return isObtainWallSlide; } set { isObtainWallSlide = value; } }

    public Player() { }
    public Player(int isObtainDash, int isObtainDoubleJump, int isObtainWallSlide)
    {
        this.IsObtainDash = isObtainDash;
        this.IsObtainDoubleJump = isObtainDoubleJump;
        this.IsObtainWallSlide = isObtainWallSlide;
    }
}
