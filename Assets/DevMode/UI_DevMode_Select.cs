using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
public class UI_DevMode_Select : MonoBehaviour
{
    public Dropdown playerDD;
    public Image playerImage;
    public Dropdown cpuDD;
    public Image cpuImage;
    public Dropdown mapDD;
    public Image mapImage;
    public Button btnStart;
    public List<string> roboList = new List<string>();
    public List<string> mapList = new List<string>();
    public GameObject player;
    public GameObject cpu;
    public GameObject map;
    public UI_DevMode dm;
    // Start is called before the first frame update
    void Start()
    {
        LoadContent();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadContent()
    {
        if (Directory.Exists("Windom_Data"))
        {
            if (Directory.Exists("Windom_Data\\Robo"))
            {
                playerDD.ClearOptions();
                cpuDD.ClearOptions();
                roboList.Clear();
                DirectoryInfo directory = new DirectoryInfo("Windom_Data\\Robo");

                if (directory.GetDirectories().Length > 0)
                {
                    
                    foreach (DirectoryInfo di in directory.GetDirectories())
                    {    
                        roboList.Add(di.Name);
                    }
                    playerDD.AddOptions(roboList);
                    cpuDD.AddOptions(roboList);
                    selectPlayer(0);
                    selectCPU(0);

                }
            }
            //else messagebox and generate required folders

            if (Directory.Exists("Windom_Data\\map"))
            {
                mapDD.ClearOptions();
                mapList.Clear();
                DirectoryInfo directory = new DirectoryInfo("Windom_Data\\map");

                if (directory.GetDirectories().Length > 0)
                {

                    foreach (DirectoryInfo di in directory.GetDirectories())
                    {
                        mapList.Add(di.Name);
                    }
                    mapDD.AddOptions(mapList);
                  
                    selectMap(0);
                    

                }
            }

        }
        //else messagebox and generate required folders
    }

    public void selectPlayer(int index)
    {
        if (File.Exists(Path.Combine("Windom_Data\\Robo", roboList[playerDD.value], "select.png")))
        {
            Texture2D tex = Helper.LoadTexture(Path.Combine("Windom_Data\\Robo", roboList[playerDD.value], "select.png"));
            Sprite st = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));

            playerImage.sprite = st;
        }
        
    }

    public void selectCPU(int index)
    {
        if (File.Exists(Path.Combine("Windom_Data\\Robo", roboList[cpuDD.value], "select.png")))
        {
            Texture2D tex = Helper.LoadTexture(Path.Combine("Windom_Data\\Robo", roboList[cpuDD.value], "select.png"));
            Sprite st = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));

            cpuImage.sprite = st;
        }
    }

    public void selectMap(int index)
    {
        if (File.Exists(Path.Combine("Windom_Data\\map", mapList[mapDD.value], "SelectImage.png")))
        {
            Texture2D tex = Helper.LoadTexture(Path.Combine("Windom_Data\\map", mapList[mapDD.value], "SelectImage.png"));
            Sprite st = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));

            mapImage.sprite = st;
        }
    }

    public void StartDev()
    {
        map.SetActive(true);
        player.SetActive(true);
        cpu.SetActive(true);
        Map_Data md = map.GetComponent<Map_Data>();
        md.path = Path.Combine("Windom_Data\\map", mapList[mapDD.value]);
        md.loadmpd();
        
        RoboLoader rl = player.GetComponent<RoboLoader>();
        
        rl.loadMecha(Path.Combine("Windom_Data\\Robo", roboList[playerDD.value]));
        player.transform.position = md.point_0[0];
        player.transform.LookAt(new Vector3(1500, player.transform.position.y, 1500));
        
        
        rl = cpu.GetComponent<RoboLoader>();
        rl.loadMecha(Path.Combine("Windom_Data\\Robo", roboList[cpuDD.value]));

        cpu.transform.position = md.point_1[0];
        cpu.transform.LookAt(new Vector3(1500, cpu.transform.position.y, 1500));

        dm.setup();
        this.gameObject.SetActive(false);
        Debug.Log("Check");
    }
}
