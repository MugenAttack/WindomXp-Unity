using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
public class UI_SelectScreen : MonoBehaviour
{
    public string MechaFolder;
    public string MapFolder;
    public RoboStructure robo;
    public GameObject Template;
    public GameObject Template2;
    public GameObject[] items = new GameObject[9];
    public GameObject[] items2 = new GameObject[9];
    public int index = 0;
    public int index2 = 0;
    public int dirCount;
    Vector2 aPos;
    Vector2 tPos;
    float time = 0;
    bool transition = false;
    PlayerControls input;
    public RectTransform mechaRect;
    public RectTransform mapRect;
    RectTransform rectTransform;
    public float transitionValue = -280.1081f;
    public float transitionValue2 = -300f;
    public bool RoboSelect = true;
    public GameObject mapPanel;
    public Image mapImage;
    // Start is called before the first frame update
    private void Awake()
    {
        input = new PlayerControls();
    }
    void Start()
    {
        robo = GetComponent<RoboStructure>();
        robo.transcoder = new CypherTranscoder();
        generateSelected();
        rectTransform = mechaRect;
        aPos = rectTransform.anchoredPosition;
        DirectoryInfo di = new DirectoryInfo(MechaFolder);
        dirCount = di.GetDirectories().Length;
        input.Menu.Move.performed += ctx => {
            Vector2 v = ctx.ReadValue<Vector2>();
            Debug.Log(v);
            if (v.x > 0)
            {
                if (RoboSelect)
                {
                    index++;
                    if (index == dirCount)
                        index = 0;
                    tPos = aPos + new Vector2(-transitionValue, 0);
                }
                else
                {
                    index2++;
                    if (index2 == dirCount)
                        index2 = 0;
                    tPos = aPos + new Vector2(-transitionValue2, 0);
                }
                transition = true;
                
            }

            if (v.x < 0)
            {
                if (RoboSelect)
                {
                    index--;
                    if (index == -1)
                        index = dirCount - 1;
                    tPos = aPos + new Vector2(transitionValue, 0);
                }
                else
                {
                    index2--;
                    if (index2 == -1)
                        index2 = dirCount - 1;
                    tPos = aPos + new Vector2(transitionValue2, 0);
                }
                transition = true;
                
            }
            
        };
        input.Menu.Enter.performed += ctx =>
        {
            rectTransform = mapRect;
            RoboSelect = false;
            mapPanel.SetActive(true);
            generateSelected();
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        robo.root.transform.Rotate(Vector3.up, 1f);
        if (transition)
        {
            Vector2 v = Vector2.Lerp(aPos, tPos, time);
            rectTransform.anchoredPosition = v;
            time += 0.04f;

            if (time >= 1)
            {
                time = 0;
                generateSelected();
                transition = false;
                rectTransform.anchoredPosition = aPos;
            }

        }
    }
    public void generateSelected()
    {
        //Mecha
        if (RoboSelect)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null)
                    GameObject.Destroy(items[i]);
            }
            DirectoryInfo di = new DirectoryInfo(MechaFolder);
            DirectoryInfo[] dirs = di.GetDirectories();
            for (int i = 0; i < 9; i++)
            {
                int pos = (i - 4) + index;
                while (pos < 0)
                    pos += dirs.Length;
                while (pos >= dirs.Length)
                    pos -= dirs.Length;

                items[i] = GameObject.Instantiate(Template);
                robo.transcoder.findCypher(Path.Combine(dirs[pos].FullName, "select.png"));
                Texture2D tex = Helper.LoadTextureEncrypted(Path.Combine(dirs[pos].FullName, "select.png"), ref robo.transcoder);
                Sprite st = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));

                items[i].GetComponent<Image>().sprite = st;
                items[i].GetComponent<Image>().type = Image.Type.Filled;
                items[i].SetActive(true);
                items[i].transform.SetParent(Template.transform.parent);

            }

            robo.folder = dirs[index].FullName;
            ani2 ani = new ani2();
            ani.load(Path.Combine(dirs[index].FullName, "robo.hod"));
            robo.buildStructure(ani.structure);
        }
        else
        {
            //Map
            for (int i = 0; i < items2.Length; i++)
            {
                if (items[i] != null)
                    GameObject.Destroy(items2[i]);
            }
            DirectoryInfo di = new DirectoryInfo(MapFolder);
            DirectoryInfo[] dirs = di.GetDirectories();
            Texture2D tex;
            Sprite st;
            for (int i = 0; i < 9; i++)
            {
                int pos = (i - 4) + index2;
                while (pos < 0)
                    pos += dirs.Length;
                while (pos >= dirs.Length)
                    pos -= dirs.Length;

                items2[i] = GameObject.Instantiate(Template2);
                robo.transcoder.findCypher(Path.Combine(dirs[pos].FullName, "SelectImage.png"));
                tex = Helper.LoadTextureEncrypted(Path.Combine(dirs[pos].FullName, "SelectImage.png"), ref robo.transcoder);
                st = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));

                items2[i].GetComponent<Image>().sprite = st;
                items2[i].GetComponent<Image>().type = Image.Type.Filled;
                items2[i].SetActive(true);
                items2[i].transform.SetParent(Template2.transform.parent);
            }
            tex = Helper.LoadTextureEncrypted(Path.Combine(dirs[index2].FullName, "SelectImage.png"), ref robo.transcoder);
            st = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
            mapImage.sprite = st;
        }
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
