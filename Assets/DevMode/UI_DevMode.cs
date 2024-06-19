using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_DevMode : MonoBehaviour
{
    [Header("Player Tab")]
    public Dropdown Players;
    public Text hpText;
    public Text genText;
    public Text enText;
    public MechaMovement mmP;
    public MechaMovement mmC;

    [Header("Ani Tab")]
    public Dropdown Anims;
    public Dropdown scripts;
    public InputField scriptField;
    public RoboStructure robo;
    public windomAnimation cAnim;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Players.value == 0)
        {
            hpText.text = mmP.HP.ToString() + "/" + mmP.MaxHP.ToString();
            genText.text = mmP.Generator.ToString() + "/" + mmP.MaxGenerator.ToString();
            enText.text = mmP.Energy.ToString() + "/" + mmP.MaxEnergy.ToString();
        }
        else
        {
            hpText.text = mmC.HP.ToString() + "/" + mmC.MaxHP.ToString();
            genText.text = mmC.Generator.ToString() + "/" + mmC.MaxGenerator.ToString();
            enText.text = mmC.Energy.ToString() + "/" + mmC.MaxEnergy.ToString();
        }
    }

    public void setup()
    {

        int aCount = robo.ani.animations.Count;
        List<string> aList = new List<string>();
        for (int i = 0; i < aCount; i++)
        {
            aList.Add((i + 1).ToString() + ". " +robo.ani.animations[i].name);
        }
        Anims.ClearOptions();
        Anims.AddOptions(aList);
        selectAnim(0);
    }

    
    public void selectAnim(int i)
    {
        cAnim = robo.ani.animations[i];
        if (cAnim.scripts != null && cAnim.scripts.Count != 0)
        {
            List<string> sList = new List<string>();
            for (int s = 0; s < cAnim.scripts.Count; s++)
                sList.Add(s.ToString());
            scripts.ClearOptions();
            scripts.AddOptions(sList);
            scripts.value = 0;
            selectScript(0);
        }
    }

    public void selectScript(int i)
    {
        scriptField.text = cAnim.scripts[i].squirrel;
    }

    public void scriptEndEdit()
    {
        script s = cAnim.scripts[scripts.value];
        s.squirrel = scriptField.text;
        cAnim.scripts[scripts.value] = s;
    }

    public void saveAni()
    {
        robo.ani.save();
    }
}
