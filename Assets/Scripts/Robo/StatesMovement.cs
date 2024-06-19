using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Stand : State
{
    public override void Start(StateManager sm)
    {
        
        sm.animator.run(0, true);
        changeWeapon(sm);
        sm.setMovementValues(false, false);
        sm.movement.boostCost = 10;
    }

    public override void Update(StateManager sm)
    {
        if (!sm.isUpperActive)
        {
            if (sm.isMoving) sm.ChangeState("Walk");
            if (!sm.isGrounded) sm.ChangeState("Fall");
            if (sm.isBoostUp) sm.ChangeState("BoostUp");
            if (sm.drawWeapon) sm.ChangeUpperState("DrawWeapon");
            if (sm.basicMelee) sm.ChangeState("BasicMelee");
        }
    }

    public override void changeWeapon(StateManager sm)
    {
        if (sm.gunMode)
            sm.animator.changeAnim(0);
        else
            sm.animator.changeAnim(50);
    }
}

public class State_Walk : State
{
    public override void Start(StateManager sm)
    {
        sm.animator.run(1, true);
        changeWeapon(sm);
        sm.movement.forceSpeed = sm.movement.gravity;
        sm.movement.boostCost = 10;
    }

    public override void Update(StateManager sm)
    {
        if (!sm.isUpperActive)
        {
            if (!sm.isMoving) sm.ChangeState("Stand");
            if (!sm.isGrounded) sm.ChangeState("Fall");
            if (sm.isBoostUp) sm.ChangeState("BoostUp");
            if (sm.isStep) sm.ChangeState("Step");
            if (sm.drawWeapon) sm.ChangeUpperState("DrawWeapon");
            if (sm.basicMelee) sm.ChangeState("BasicMelee");
        }
    }

    public override void changeWeapon(StateManager sm)
    {
        if (sm.gunMode)
            sm.animator.changeAnim(1);
        else
            sm.animator.changeAnim(51);
    }
}

public class State_Fall : State
{
    public override void Start(StateManager sm)
    {
        sm.animator.run(8);
        changeWeapon(sm);
        sm.setMovementValues(true, false);
        sm.movement.forceSpeed = sm.movement.gravity;
        sm.movement.boostCost = 0;
    }

    public override void Update(StateManager sm)
    {
        
        if (sm.isGrounded) sm.ChangeState("Land");
        if (!sm.isUpperActive)
        {
            if (sm.isBoostUp && sm.movement.Generator > 0) sm.ChangeState("BoostUp");
            if (sm.isBoostForward && sm.movement.Generator > 0) sm.ChangeState("BoostForward");
            if (sm.isStep) sm.ChangeState("Step");
            if (sm.drawWeapon) sm.ChangeUpperState("DrawWeapon");
            if (sm.basicMelee) sm.ChangeState("BasicMelee");
        }
        
    }

    public override void changeWeapon(StateManager sm)
    {
        if (sm.gunMode)
            sm.animator.changeAnim(8);
        else
            sm.animator.changeAnim(58);
    }
}

public class State_Land : State
{
    public override void Start(StateManager sm)
    {
        sm.animator.run(5);
        changeWeapon(sm);
        sm.setMovementValues(false, false);
    }

    public override void Update(StateManager sm)
    {
        if (sm.animator.isEnded()) sm.ChangeState("Stand");
        if (!sm.isUpperActive)
        {
            if (sm.isBoostForward) sm.ChangeState("BoostForward");
            if (sm.isStep) sm.ChangeState("Step");
            if (sm.drawWeapon) sm.ChangeUpperState("DrawWeapon");
            if (sm.basicMelee) sm.ChangeState("BasicMelee");
        }
    }

    public override void changeWeapon(StateManager sm)
    {
        if (sm.gunMode)
            sm.animator.changeAnim(5);
        else
            sm.animator.changeAnim(55);
    }
}

public class State_BoostUp : State
{
    public override void Start(StateManager sm)
    {
        sm.animator.run(7);
        changeWeapon(sm);
        sm.movement.boostCost = -5;
    }

    public override void Update(StateManager sm)
    {
        if (sm.isBoostUp == false || sm.movement.Generator == 0)
        {
            sm.ChangeState("Fall");
            sm.isBoostForward = false;
            sm.isBoostUp = false;
        }
        if (!sm.isUpperActive)
        {
            if (sm.isBoostForward && sm.movement.Generator > 0) sm.ChangeState("BoostForward");
            if (sm.drawWeapon) sm.ChangeUpperState("DrawWeapon");
            if (sm.basicMelee) sm.ChangeState("BasicMelee");
        }
    }

    public override void changeWeapon(StateManager sm)
    {
        if (sm.gunMode)
            sm.animator.changeAnim(7);
        else
            sm.animator.changeAnim(57);
    }
}

public class State_BoostForward : State
{
    public override void Start(StateManager sm)
    {
        sm.animator.run(22);
        changeWeapon(sm);
        //sm.movement.rb.velocity = Vector3.zero;
        sm.movement.boostCost = -5;
    }

    public override void Update(StateManager sm)
    {
        if (sm.isBoostForward == false || sm.movement.Generator == 0)
        {
            sm.ChangeState("Fall");
            sm.isBoostForward = false;
            sm.isBoostUp = false;
        }
        if (sm.drawWeapon && !sm.isUpperActive) sm.ChangeUpperState("DrawWeapon");
        if (sm.basicMelee && !sm.isUpperActive) sm.ChangeState("BasicMelee");
    }

    public override void changeWeapon(StateManager sm)
    {
        if (sm.gunMode)
            sm.animator.changeAnim(22);
        else
            sm.animator.changeAnim(72);
    }
}

public class State_Step : State
{
    float angle;
    public override void Start(StateManager sm)
    {
        sm.setMovementValues(false, true, true);
        angle = Vector3.SignedAngle(sm.transform.forward, sm.Robo.root.transform.forward, Vector3.up);
        if (Mathf.Abs(angle) > 90)
        {
            angle = Vector3.SignedAngle(-sm.transform.forward, sm.movement.Direction, Vector3.up);
            if (angle >= 0)
            {
                //back to left

                sm.animator.run(new int[] { 12, 9 }, Mathf.Abs(angle) / 90);

            }
            else
            {
                //back to right
                sm.animator.run(new int[] { 12, 10 }, Mathf.Abs(angle) / 90);
            }

        }
        else
        {
            if (angle >= 0)
            {
                //forward to right
                sm.animator.run(new int[] { 11, 10 }, Mathf.Abs(angle) / 90);
            }
            else
            {
                //forward to left
                sm.animator.run(new int[] { 11, 9 }, Mathf.Abs(angle) / 90);

            }
        }
        changeWeapon(sm);
    }

    public override void Update(StateManager sm)
    {
        if ((sm.animator.isEnded() || sm.isStep == false) && sm.isGrounded)
            sm.ChangeState("Land"); 
        if ((sm.animator.isEnded() || sm.isStep == false) && !sm.isGrounded)
            sm.ChangeState("Fall");
        if (sm.drawWeapon && !sm.isUpperActive) sm.ChangeUpperState("DrawWeapon");
    }

    public override void changeWeapon(StateManager sm)
    {
        if (sm.gunMode)
        {
            if (Mathf.Abs(angle) > 90)
            {
                if (angle >= 0)
                {
                    //back to left

                    sm.animator.changeAnim(new int[] { 12, 9 });

                }
                else
                {
                    //back to right
                    sm.animator.changeAnim(new int[] { 12, 10 });
                }

            }
            else
            {
                if (angle >= 0)
                {
                    //forward to right
                    sm.animator.changeAnim(new int[] { 11, 10 });
                }
                else
                {
                    //forward to left
                    sm.animator.changeAnim(new int[] { 11, 9 });

                }
            }
        }
        else
        {
            if (Mathf.Abs(angle) > 90)
            {
                if (angle >= 0)
                {
                    //back to left

                    sm.animator.changeAnim(new int[] { 62, 59 });

                }
                else
                {
                    //back to right
                    sm.animator.changeAnim(new int[] { 62, 60 });
                }

            }
            else
            {
                if (angle >= 0)
                {
                    //forward to right
                    sm.animator.changeAnim(new int[] { 61, 60 });
                }
                else
                {
                    //forward to left
                    sm.animator.changeAnim(new int[] { 61, 59 });

                }
            }
        }
    }
}


public class State_DrawWeapon : State
{
    public override void Start(StateManager sm)
    {
        if (sm.gunMode)
            sm.animator.runUpper(18);
        else
            sm.animator.runUpper(68);
        sm.drawWeapon = false;
    }

    public override void Update(StateManager sm)
    {
        if (sm.animator.isUpperEnded())
        {
            sm.animator.playTop = false;
            sm.isUpperActive = false;
        }

    }

    public override void changeWeapon(StateManager sm)
    {
        
    }
}


