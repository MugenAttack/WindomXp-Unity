using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_BasicMelee : State
{
    int animID;
    bool hitFail;
    public override void Start(StateManager sm)
    {
        animID = 131;
        sm.animator.run(131);
        sm.basicMelee = false;   
        sm.movement.useSmoothAngle = false;
        sm.movement.CalculateRotation(new Vector2(0,1));
        sm.setMovementValues(false,false);
        sm.animator.resetMovementPerScript = true;
        
    }

    public override void Update(StateManager sm)
    {
        if (sm.attackCancel && sm.basicMelee && animID != 135)
        {
            animID++;
            if (sm.Robo.ani.animations[animID] != null && sm.Robo.ani.animations[animID].scripts.Count > 0)
                sm.animator.run(animID);
            sm.basicMelee = false;
        }

        if (sm.animator.isEnded())
        {
            //grab runner data
            
                if (!sm.isGrounded) sm.ChangeState("Fall");
                else sm.ChangeState("Stand");
            
        }
        
    }

    public override void changeWeapon(StateManager sm)
    {

    }
}

public class State_BasicShot : State
{

}