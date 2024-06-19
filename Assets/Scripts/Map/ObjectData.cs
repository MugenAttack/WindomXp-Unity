using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectData : MonoBehaviour
{
    // Start is called before the first frame update
    public int ModelID;
    public int scriptID;
    public Map_Data data;

    public void build()
    {
        if (data.Pieces[ModelID].visualMesh != null)
        {
            gameObject.name = data.Pieces[ModelID].visualMesh.name;
            if (gameObject.GetComponent<MeshFilter>() == null)
                gameObject.AddComponent<MeshFilter>().mesh = data.Pieces[ModelID].visualMesh;
            else
                gameObject.GetComponent<MeshFilter>().mesh = data.Pieces[ModelID].visualMesh;

            if (gameObject.GetComponent<MeshRenderer>() == null)
                gameObject.AddComponent<MeshRenderer>().materials = data.Pieces[ModelID].materials;
            else
                gameObject.GetComponent<MeshRenderer>().materials = data.Pieces[ModelID].materials;

            if (data.Pieces[ModelID].colliderMesh != null)
            {
                if (gameObject.GetComponent <MeshCollider>() == null)
                    gameObject.AddComponent<MeshCollider>().sharedMesh = data.Pieces[ModelID].colliderMesh;
                else
                    gameObject.GetComponent<MeshCollider>().sharedMesh = data.Pieces[ModelID].colliderMesh;    
            }
        }
    }
}
