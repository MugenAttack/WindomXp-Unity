using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MechaMovement : MonoBehaviour
{
    [Header("Mecha Data")]
    public int id;
    public Dictionary<int, GameObject> Thrusters = new Dictionary<int, GameObject>();
    public int HP;
    public int MaxHP;
    public int Generator;
    public int MaxGenerator;
    public int Energy;
    public int MaxEnergy;
    public Text HpText;
    public Text EnergyText;
    public int boostCost = 0;
    public List<GameObject> otherPlayers;
    public GameObject[] Guns = new GameObject[10];
    public GameObject[] Swords = new GameObject[10];
    public GameObject[] weaponPoints = new GameObject[50];
    [Header("Movement")]
    public Rigidbody rb;
    public CapsuleCollider cc;
    public RoboStructure robo;
    public Vector3 gravity = new Vector3(0, -1, 0);
    public float forceMulitplier = 1.5f;
    public float moveMultiplier = 2f;
    float turnVelocity;
    public Vector2 directionalInput;
    public GameObject target;
    public bool UpdateRotation;
    public bool UpdateDirection;
    public Vector3 Direction;
    public Vector3 moveSpeed;
    public Vector3 forceSpeed;
    public bool useSmoothAngle = false;
    public float smoothTime = 0.2f;
    [Header("Camera Info")]
    public Camera cam;
    public float followDistance;
    public Vector3 centerPoint;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CapsuleCollider>();
        rb.freezeRotation = true;
        //HP = MaxHP;
        //Energy = MaxEnergy;
        //HpText.text = "HP: " + HP.ToString();
        //EnergyText.text = "EN: " + Energy.ToString();
    }

    private void Update()
    {
        
    }
    private void FixedUpdate()
    {

        
        Generator += boostCost;
        if (Generator > MaxGenerator)
            Generator = MaxGenerator;
        if (Generator < 0)
            Generator = 0;
        

        if (UpdateRotation)
        {
            CalculateRotation(directionalInput);

        }

     
        Vector3 worldV = transform.TransformVector((moveSpeed + forceSpeed) * moveMultiplier);
        if (forceSpeed.y != 0)
        {
            rb.velocity = new Vector3(worldV.x,rb.velocity.y,worldV.z);
            rb.AddRelativeForce(forceSpeed * forceMulitplier, ForceMode.Impulse);
        }
        else
            rb.velocity = new Vector3(worldV.x, 0, worldV.z);


    }
    
    public void CalculateRotation(Vector2 dInput)
    {
        if (dInput.magnitude > 0.1f && UpdateDirection)
        {
            calculateDirection(dInput);
        }

        float angle = Mathf.Atan2(Direction.x, Direction.z) * Mathf.Rad2Deg;
        float smoothAngle = Mathf.SmoothDampAngle(robo.root.transform.rotation.eulerAngles.y, angle, ref turnVelocity, smoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
        if (useSmoothAngle)
            robo.root.transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
        else
            robo.root.transform.rotation = Quaternion.Euler(0f, angle, 0f);
       ;
      
    }
    public void calculateDirection()
    {
        calculateDirection(directionalInput);
    }
    public void calculateDirection(Vector2 dInput)
    {
        if (target != null)
        {
            Direction = calculateDirection(transform.position, target.transform.position, dInput);
        }
        else
        {
            //Vector3 lookOffset = calculateDirection(transform.position + transform.forward, transform.position, directionalInput);
            //Vector3 lookTarget = transform.position + transform.forward + lookOffset;
            //Direction = (transform.position - lookTarget).normalized;
            Direction = calculateDirection(transform.position, cam.transform.position, -dInput);
        }
        Quaternion rootRot = robo.root.transform.rotation;
        float angle = Mathf.Atan2(Direction.x, Direction.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
        robo.root.transform.rotation = rootRot;
    }
    public static Vector3 calculateDirection(Vector3 objectPos, Vector3 targetPos, Vector2 Axis)
    {
        objectPos.y = 0;
        targetPos.y = 0;
        Vector3 difpos = (objectPos - targetPos).normalized;
        Vector3 cross = Vector3.Cross(Vector3.up, difpos);
        Vector3 toward = difpos * -Axis.y;
        Vector3 perpendicular = cross * -Axis.x;
        return (toward + perpendicular);
    }

    public void selectTarget()
    {
        if (otherPlayers.Count != 0)
        {
            if (otherPlayers.Count == 1)
            {
                target = otherPlayers[0];
            }
        }
    }
    
    public void weaponChange(bool gunMode)
    {
        if (gunMode)
        {
            for (int i = 0; i < Swords.Length; i++)
            {
                if (Swords[i] != null)
                    Swords[i].SetActive(false);
            }
            if (Guns[0] != null)
                Guns[0].SetActive(true);
            if (Guns[1] != null)
                Guns[1].SetActive(true);
            if (Guns[8] != null)
                Guns[8].SetActive(true);
            if (Guns[9] != null)
                Guns[9].SetActive(true);
        }
        else
        {
            for (int i = 0; i < Guns.Length; i++)
            {
                if (Guns[i] != null)
                    Guns[i].SetActive(false);
            }
            if (Swords[0] != null)
                Swords[0].SetActive(true);
            if (Swords[1] != null)
                Swords[1].SetActive(true);
            if (Swords[8] != null)
                Swords[8].SetActive(true);
            if (Swords[9] != null)
                Swords[9].SetActive(true);

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == target)
            Debug.Log("Hit Success");
        
    }
}
