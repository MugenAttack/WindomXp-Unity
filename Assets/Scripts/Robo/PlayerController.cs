using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    PlayerControls input;
    StateManager stateManager;
    MechaMovement mm;
    bool initialBoostTap = false;
    bool initialStepTap = false;
    void Awake()
    {
        input = new PlayerControls();
        stateManager = GetComponent<StateManager>();
        mm = GetComponent<MechaMovement>();
    }
    // Start is called before the first frame update
    void Start()
    {
        input.Player.Movement.performed += ctx => { 
            mm.directionalInput = ctx.ReadValue<Vector2>(); 
            stateManager.isMoving = true;
            if (initialStepTap)
                stateManager.isStep = true;
        };
        input.Player.Movement.canceled += ctx => { mm.directionalInput = Vector2.zero; stateManager.isMoving = false; stateManager.isStep = false; };
        input.Player.Boost.performed += ctx => {
            stateManager.isBoostUp = true;
            if (initialBoostTap)
                stateManager.isBoostForward = true;
        };
        input.Player.Boost.canceled += ctx => { stateManager.isBoostUp = false; stateManager.isBoostForward = false; };
        input.Player.BoostForward.performed += ctx => { initialBoostTap = true; StartCoroutine(boostTapCancel()); };
        input.Player.MovementStep.performed += ctx => { initialStepTap = true; StartCoroutine(stepTapCancel()); };
        input.Player.Target.performed += ctx =>
        {
            mm.selectTarget();
        };
        input.Player.TargetHold.performed += ctx =>
        {
            mm.target = null;
        };

        input.Player.Range.performed += ctx =>
        {
            if (stateManager.gunMode == false)
                stateManager.drawWeapon = true;
            else
                stateManager.basicShot = true;
        };

        input.Player.Melee.performed += ctx =>
        {
            if (stateManager.gunMode == true)
                stateManager.drawWeapon = true;
            else
                stateManager.basicMelee = true;
        };

    }

    IEnumerator boostTapCancel()
    {
        yield return new WaitForSeconds(0.5f);
        initialBoostTap = false;
    }

    IEnumerator stepTapCancel()
    {
        yield return new WaitForSeconds(0.5f);
        initialStepTap = false;
    }
    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }
}
