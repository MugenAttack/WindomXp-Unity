using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptHook : MonoBehaviour
{
    public RoboStructure robo;
    public MechaAnimator ma;
    public MechaMovement mm;
    public StateManager sm;
    public RunProc2 rp;
    // Start is called before the first frame update
    public void setup()
    {
        ma.scriptRunner = new scriptInterpreter();
        //register scripts
        ma.scriptRunner.registerFunction("Move=", MoveSet);
        ma.scriptRunner.registerFunction("Force=", ForceSet);
        scriptVar v = new scriptVar();
        v.type = scriptVarType.NUM;
        v.num = 0;
        ma.scriptRunner.registerVariable("@int[151]", setDummy, getIsUpperActive);
        ma.scriptRunner.registerFunction("BURNER", Burner);
        ma.scriptRunner.registerFunction("ChangeWeapon", changeWeaponMode);
        ma.scriptRunner.registerVariable("SwordCancel", swordCancel, getDummy);
        rp = GetComponent<RunProc2>();
        ma.scriptRunner.registerFunction("RunProc2", rp.Run);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveSet(scriptVar[] values)
    {
        mm.moveSpeed = new Vector3(values[0].num, values[1].num, values[2].num);
    }

    public void ForceSet(scriptVar[] values)
    {
        mm.forceSpeed = new Vector3(values[0].num, values[1].num, values[2].num);
    }

    public void Burner(scriptVar[] values)
    {
        mm.Thrusters[Mathf.RoundToInt(values[0].num)].SetActive(true);
    }

    public scriptVar getIsUpperActive()
    {
        scriptVar s = new scriptVar();
        if (sm.isUpperActive)
            return ma.scriptRunner.convertFloat(1);
        else
            return ma.scriptRunner.convertFloat(0);
    }

    public void changeWeaponMode(scriptVar[] values)
    {
        if (values[0].str == "GUN")
            sm.gunMode = true;
        if (values[0].str == "SWORD")
            sm.gunMode = false;
        sm.cState.changeWeapon(sm);
        mm.weaponChange(sm.gunMode);
    }
    public scriptVar getDummy()
    {
        return new scriptVar();
    }

    public void setDummy(scriptVar value)
    {
    }

    public void swordCancel(scriptVar value)
    {
        if (value.num > 0)
            sm.attackCancel = true;
        else
            sm.attackCancel = false;
    }

   
}

