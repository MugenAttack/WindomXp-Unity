using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject[] Players;
    public int currentPlayer = 0;
    public MechaMovement mi;
    // Start is called before the first frame update
    void Start()
    {
        changePlayer(0);
        for (int i = 0; i < Players.Length; i++)
            Players[i].GetComponent<MechaMovement>().id = i;

        for (int i = 0; i < Players.Length; i++)
        {
            MechaMovement p1 = Players[i].GetComponent<MechaMovement>();
            p1.otherPlayers = new List<GameObject>();
            for (int j = 0; j < Players.Length; j++)
            {

                if (i != j)
                    p1.otherPlayers.Add(Players[j]);
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        if (mi.target != null)
        {
            transform.position = Players[currentPlayer].transform.position;
            transform.LookAt(new Vector3(mi.target.transform.position.x, transform.position.y, mi.target.transform.position.z));
            transform.transform.Translate(mi.centerPoint,Space.Self);
            //transform.position = Players[currentPlayer].transform.position + mi.centerPoint;
            //transform.LookAt(mi.target.transform);
            //transform.position = transform.position - (transform.forward * mi.followDistance);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, Players[currentPlayer].transform.position.y, transform.position.z);
            transform.LookAt(Players[currentPlayer].transform.position);
            transform.position = Players[currentPlayer].transform.position;
            transform.transform.Translate(mi.centerPoint, Space.Self);
        }
    }

    public void changePlayer(int id)
    {
        currentPlayer = id;
        mi = Players[id].GetComponent<MechaMovement>();
    }
}
