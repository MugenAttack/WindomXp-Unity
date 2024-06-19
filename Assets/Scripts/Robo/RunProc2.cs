using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunProc2 : MonoBehaviour
{
    public MechaMovement mm;
    public GameObject BeamSwordTemplate;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Run(scriptVar[] values)
    {
  
        switch ((int)values[1].num)
        {
            case 55:
                BeamSwordRun((int)values[2].num, values[3].num);
                break;
        }
    }

    public void BeamSwordRun(int output, float length)
    {
        if (mm.weaponPoints[output].GetComponents<Transform>().Length < 2)
        {
            GameObject bs = Instantiate<GameObject>(BeamSwordTemplate);
            BeamSword bsc = bs.GetComponent<BeamSword>();
            bsc.length = length / 120;
            bs.transform.SetParent(mm.weaponPoints[output].transform, false);
            bs.transform.localPosition = Vector3.zero;
            bs.transform.localRotation = Quaternion.identity;
            bsc.create();
            bs.SetActive(true);
        }
    }
}
