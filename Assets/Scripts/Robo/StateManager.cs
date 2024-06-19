using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void State_Start(StateManager sm);
public delegate void State_Loop(StateManager sm);
public abstract class State
{
    public virtual void Start(StateManager sm)
    {

    }

    public virtual void Update(StateManager sm)
    {

    }


    public virtual void changeWeapon(StateManager sm)
    {

    }
}

public class StateManager : MonoBehaviour
{
    public RoboStructure Robo;
    public MechaAnimator animator;
    public MechaMovement movement;


    public Dictionary<string, State> States = new Dictionary<string, State>();

    //Upper States
    public State cState;
    public State cUpperState;
    public bool isUpperActive = false;

    [Header("State Checks")]
    public bool isGrounded;
    public LayerMask groundMask;
    public bool isMoving;
    public bool isBoostUp;
    public bool isBoostForward;
    public bool isStep;
    public bool gunMode = true;
    public bool drawWeapon = false;
    public bool basicShot = false;
    public bool basicMelee = false;
    public bool attackCancel = false;
    // Start is called before the first frame update
    public void Setup()
    {
        
        animator = GetComponent<MechaAnimator>();
        movement = GetComponent<MechaMovement>();
        States.Add("Stand", new State_Stand());
        States.Add("Walk", new State_Walk());
        States.Add("Fall", new State_Fall());
        States.Add("Land", new State_Land());
        States.Add("BoostUp", new State_BoostUp());
        States.Add("BoostForward", new State_BoostForward());
        States.Add("Step", new State_Step());
        States.Add("DrawWeapon", new State_DrawWeapon());
        States.Add("BasicMelee", new State_BasicMelee());
        ChangeState("Stand");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(transform.position + (new Vector3(0, movement.cc.radius * 0.99f - 0.2f, 0)), movement.cc.radius * 0.99f, groundMask);
        cState.Update(this);
        if (cUpperState != null && isUpperActive)
            cUpperState.Update(this);
    }

    public void setMovementValues(bool UpdateRotation, bool UpdateDirection, bool ForceUpdateDirection = false)
    {
        movement.UpdateRotation = UpdateRotation;
        movement.UpdateDirection = UpdateDirection;
        if (ForceUpdateDirection)
            movement.calculateDirection();
    }
    
    public void ChangeState(string name)
    {
        if (States.ContainsKey(name))
        {
            setMovementValues(true, true);
            movement.forceSpeed = Vector3.zero;
            movement.moveSpeed = Vector3.zero;
            movement.smoothTime = 0.2f;
            movement.useSmoothAngle = true;
            animator.resetMovementPerScript = false;
            cState = States[name];
            cState.Start(this);
            for (int i = 0; i < movement.Thrusters.Count; i++)
                movement.Thrusters[i].SetActive(false);
        }
    }

    public void ChangeUpperState(string name)
    {
        isUpperActive = true;
        cUpperState = States["DrawWeapon"];
        cUpperState.Start(this);
    }

   
}
