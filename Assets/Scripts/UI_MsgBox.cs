using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_MsgBox : MonoBehaviour
{
    public Text text;

    public void Start()
    {
        //gameObject.SetActive(false);
    }
    public void Show(string message)
    {
        text.text = message;
        gameObject.SetActive(true);
    }
    
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
