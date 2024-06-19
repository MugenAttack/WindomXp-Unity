using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_Tabs : MonoBehaviour
{
    public GameObject[] Tabs;
    public GameObject[] Panels;
    public Color selected;
    public Color deselected;
    
    // Start is called before the first frame update
    void Start()
    {
        //select(0);
    }

    

    public void select(int i)
    {
        Debug.Log(i); 
        for (int j = 0; j < Tabs.Length; j++)
        {
            ColorBlock cb = Tabs[j].GetComponent<Button>().colors;
            if (j == i)
                cb.normalColor = selected;
            else
                cb.normalColor = deselected;
            Tabs[j].GetComponent<Button>().colors = cb;
        }

        for (int k = 0; k < Panels.Length; k++)
        {
            if (k == i)
                Panels[k].SetActive(true);
            else
                Panels[k].SetActive(false);
        }
        
    }

}
