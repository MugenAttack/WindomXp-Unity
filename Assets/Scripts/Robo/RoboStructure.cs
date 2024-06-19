using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Assets;
using System.Linq;
public delegate void Update_Event();
public class RoboStructure : MonoBehaviour
{
    public GameObject root;
    public List<GameObject> parts = new List<GameObject>();
    public List<bool> isTop = new List<bool>();
    public hod2v0 hod;
    public ani2 ani;
    public Assimp.AssimpImporter Importer = new Assimp.AssimpImporter();
    public string folder;
    public string filename;
    public CypherTranscoder transcoder;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void buildStructure(string filename)
    {
        ani.load(filename);
        buildStructure(ani.structure);
    }
    public void buildStructure()
    {
        ani = new ani2();
        ani.load(Path.Combine(folder, "Script.ani"));
        buildStructure(ani.structure);
    }

    public void buildStructure(hod2v0 Robo)
    {
        isTop.Clear();
        //find cypher
        string[] files = Directory.GetFiles(folder);
        foreach (string file in files)
        {
            if (transcoder.findCypher(file))
                break;
        }

        hod = Robo;
        

        //build Ani

        parts.Clear();

        if (root != null)
            GameObject.Destroy(root);

        for (int i = 0; i < Robo.parts.Count; i++)
        {

            GameObject part = new GameObject(Robo.parts[i].name);
            if (i == 0)
            {
                root = part;
                part.transform.parent = transform;
            }
            parts.Add(part);
            
            if (i == 0)
            {
                parts[i].transform.localPosition = Robo.parts[i].position;
                parts[i].transform.localRotation = Robo.parts[i].rotation;
                parts[i].transform.localScale = Robo.parts[i].scale;
                isTop.Add(false);
            }
            else
            {
                //find next level higher in tree.
                for (int j = i - 1; j >= 0; j--)
                {
                    if (Robo.parts[i].treeDepth - 1 == Robo.parts[j].treeDepth)
                    {
                        if (j == 0)
                        {
                            parts[i].transform.SetParent(parts[0].transform);
                            parts[i].transform.localPosition = Robo.parts[i].position;
                            parts[i].transform.localRotation = Robo.parts[i].rotation;
                            parts[i].transform.localScale = Robo.parts[i].scale;
                        }
                        else
                        {
                            parts[i].transform.SetParent(parts[j].transform);
                            parts[i].transform.localPosition = Robo.parts[i].position;
                            parts[i].transform.localRotation = Robo.parts[i].rotation;
                            parts[i].transform.localScale = Robo.parts[i].scale;
                            
                        }

                        if (parts[i].name == "Body.x" || isTop[j])
                            isTop.Add(true);
                        else
                            isTop.Add(false);

                        break;
                    }
                }
            }
        }

        for (int i = 0; i < Robo.parts.Count; i++)
        {
            try
            {
                if (i != 0)
                    ImportModelEncrypted(parts[i], Path.Combine(folder, Robo.parts[i].name));

                if (Robo.parts[i].name == "Body_d.x")
                {
                    MeshCollider mc = parts[i].AddComponent<MeshCollider>();
                    mc.sharedMesh = ImportModel(Path.Combine(folder, "Hit.x"));
                    mc.convex = true;
                    parts[i].layer = 7;

                }

                if (Robo.parts[i].name == "Body.x")
                {
                    MeshCollider mc = parts[i].AddComponent<MeshCollider>();
                    mc.sharedMesh = ImportModel(Path.Combine(folder, "HitUp.x"));
                    mc.convex = true;
                    parts[i].layer = 7;
                }
            }
            catch { }
        }
    }

    void ImportModel(GameObject GO, string file)
    {
        if (File.Exists(file))
        {
            try
            {
                string Modelpath = Path.GetDirectoryName(file);

                var scen = Importer.ImportFile(file, Helper.PostProcessStepflags);
                
                Mesh mesh = new Mesh();
                mesh.CombineMeshes(scen.Meshes.Select(x => new CombineInstance()
                {
                    mesh = x.ToUnityMesh(),
                    transform = scen.RootNode.Transform.ToUnityMatrix()
                }).ToArray(), false);

                Material[] materials = new Material[scen.Meshes.Length];

                for (int index = 0; index < materials.Length; index++)
                {
                    var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));

                    if (scen.Materials[scen.Meshes[index].MaterialIndex] != null)
                    {
                        mat.name = scen.Materials[scen.Meshes[index].MaterialIndex].Name;
                        var textures = scen.Materials[scen.Meshes[index].MaterialIndex].GetAllTextures();
                        var color = scen.Materials[scen.Meshes[index].MaterialIndex].ColorDiffuse;
                        mat.color = new Color(color.R, color.G, color.B, color.A);
                        mat.SetFloat("_Glossiness", scen.Materials[scen.Meshes[index].MaterialIndex].ShininessStrength);


                        if (textures.Length > 0 && File.Exists(Path.Combine(Modelpath, textures[0].FilePath)))
                        {
                            try
                            {
                                mat.mainTexture = Helper.LoadTexture(Path.Combine(Modelpath, textures[0].FilePath));
                            }
                            catch
                            {
                            }
                        }
                    }

                    materials[index] = mat;
                }

                GO.AddComponent<MeshFilter>().mesh = mesh;
                //part.AddComponent<MeshCollider>().sharedMesh = mesh; 
                GO.AddComponent<MeshRenderer>().materials = materials;
            }
            catch
            {
            }
        }
    }

    void ImportModelEncrypted(GameObject GO, string file)
    {
       
        if (File.Exists(file))
        {
            try
            {
                string Modelpath = Path.GetDirectoryName(file);

                //var scen = Importer.ImportFile(file, Helper.PostProcessStepflags);
                byte[] data = transcoder.Transcode(file);
                MemoryStream ms = new MemoryStream(data);
                var scen = Importer.ImportFileFromStream(ms, Helper.PostProcessStepflags, "x");

                Mesh mesh = new Mesh();
                mesh.CombineMeshes(scen.Meshes.Select(x => new CombineInstance()
                {
                    mesh = x.ToUnityMesh(),
                    transform = scen.RootNode.Transform.ToUnityMatrix()
                }).ToArray(), false);

                Material[] materials = new Material[scen.Meshes.Length];

                for (int index = 0; index < materials.Length; index++)
                {
                    var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));

                    if (scen.Materials[scen.Meshes[index].MaterialIndex] != null)
                    {
                        mat.name = scen.Materials[scen.Meshes[index].MaterialIndex].Name;
                        var textures = scen.Materials[scen.Meshes[index].MaterialIndex].GetAllTextures();
                        var color = scen.Materials[scen.Meshes[index].MaterialIndex].ColorDiffuse;
                        mat.color = new Color(color.R, color.G, color.B, color.A);
                        mat.SetFloat("_Glossiness", scen.Materials[scen.Meshes[index].MaterialIndex].ShininessStrength);


                        if (textures.Length > 0 && File.Exists(Path.Combine(Modelpath, textures[0].FilePath)))
                        {
                            try
                            {
                                mat.mainTexture = Helper.LoadTextureEncrypted(Path.Combine(Modelpath, textures[0].FilePath), ref transcoder);
                            }
                            catch
                            {
                            }
                        }
                    }

                    materials[index] = mat;
                }

                GO.AddComponent<MeshFilter>().mesh = mesh;
                //part.AddComponent<MeshCollider>().sharedMesh = mesh; 
                GO.AddComponent<MeshRenderer>().materials = materials;
            }
            catch
            {
            }
        }
    }

    Mesh ImportModel(string file)
    {
        if (File.Exists(file))
        {
            try
            {
                string Modelpath = Path.GetDirectoryName(file);

                var scen = Importer.ImportFile(file, Helper.PostProcessStepflags);
                Mesh mesh = new Mesh();
                mesh.CombineMeshes(scen.Meshes.Select(x => new CombineInstance()
                {
                    mesh = x.ToUnityMesh(),
                    transform = scen.RootNode.Transform.ToUnityMatrix()
                }).ToArray(), false);


                return mesh;
            }
            catch
            {
            }
        }
        return null;
    }

    public GameObject findPart(string partName)
    {
        foreach (GameObject p in parts)
        {
            if (p.name == partName)
                return p;
        }
        return null;
    }
    public void setPose(hod2v0 pose)
    {
        if (pose.parts.Count == parts.Count)
        {
            for (int i = 0; i < parts.Count; i++)
            {
                parts[i].transform.localPosition = pose.parts[i].position;
                parts[i].transform.localRotation = pose.parts[i].rotation;
                parts[i].transform.localScale = pose.parts[i].scale;
            }
        }
    }

    public void setPose(hod2v1 pose)
    {
        if (pose.parts.Count == parts.Count)
        {
            for (int i = 0; i < parts.Count; i++)
            {
                parts[i].transform.localPosition = pose.parts[i].position;
                parts[i].transform.localRotation = pose.parts[i].rotation;
                parts[i].transform.localScale = pose.parts[i].scale;
            }
        }
    }

    public void setPose(int AnimID, int HodID)
    {
        if (ani != null)
        {
            setPose(ani.animations[AnimID].frames[HodID]);
        }
    }
    public void updateHod(int AnimID, int HodID)
    {
        for (int i = 0; i < parts.Count; i++)
        {
            updatePart(AnimID, HodID, i);
        }
    }
    public void updatePart(int AnimID, int HodID, int prtID)
    {
        if (ani != null)
        {
            hod2v1_Part prt = ani.animations[AnimID].frames[HodID].parts[prtID];
            prt.position = parts[prtID].transform.localPosition;
            prt.rotation = parts[prtID].transform.localRotation;
            prt.scale = parts[prtID].transform.localScale;
            prt.unk1 = parts[prtID].transform.localRotation;
            prt.unk2 = parts[prtID].transform.localRotation;
            prt.unk3 = parts[prtID].transform.localRotation;
            ani.animations[AnimID].frames[HodID].parts[prtID] = prt;
        }
    }
    public void updatePart(int prtID, hod2v1_Part prt, Space space = Space.Self)
    {
        if (space == Space.Self)
        {
            parts[prtID].transform.localPosition = prt.position;
            parts[prtID].transform.localRotation = prt.rotation;
            parts[prtID].transform.localScale = prt.scale;
        }
        else
        {
            parts[prtID].transform.position = prt.position;
            parts[prtID].transform.rotation = prt.rotation;
            parts[prtID].transform.localScale = prt.scale;
        }
    }    
    public void updatePart(int AnimID, int HodID, int prtID, hod2v1_Part prt, Space space = Space.Self)
    {
        if (space == Space.Self)
        {
            parts[prtID].transform.localPosition = prt.position;
            parts[prtID].transform.localRotation = prt.rotation;
            parts[prtID].transform.localScale = prt.scale;
        }
        else
        {
            parts[prtID].transform.position = prt.position;
            parts[prtID].transform.rotation = prt.rotation;
            parts[prtID].transform.localScale = prt.scale;
        }
        updatePart(AnimID, HodID, prtID);
        
    }

    
    public void addPart(string partName, int parent)
    {
        if (ani != null)
        {
            ani.addPart(partName, parent);
            buildStructure(ani.structure);
        }
        else
        {
            for (int i = 0; i < hod.parts.Count; i++)
            {
                hod2v0_Part prt = hod.parts[i];
                prt.position = parts[parent].transform.localPosition;
                prt.rotation = parts[parent].transform.localRotation;
                prt.scale = parts[parent].transform.localScale;
            }
            int level = hod.parts[parent].treeDepth + 1;
            hod2v0_Part pHod = hod.parts[parent];
            pHod.childCount++;
            hod.parts[parent] = pHod;
            hod2v0_Part nPart = new hod2v0_Part();
            nPart.name = partName;
            nPart.treeDepth = level;
            nPart.flag = 1;
            nPart.unk = new Vector3(1, 1, 1);
            nPart.position = new Vector3(0, 0, 0);
            nPart.rotation = new Quaternion();
            nPart.scale = new Vector3(1, 1, 1);
            int j = parent + 1;
            for (; j < hod.parts.Count; j++)
            {
                if (hod.parts[j].treeDepth <= hod.parts[parent].treeDepth)
                {
                    break;
                }
            }
            hod.parts.Insert(j, nPart);

            buildStructure(hod);
        }

    }

    public bool removePart(int index)
    {
        if (ani != null)
        {
            if (ani.removePart(index))
                buildStructure(ani.structure);
            else
                return false;

            return true;
        }
        else if (hod.parts[index].childCount == 0)
        {
            for (int i = 0; i < hod.parts.Count; i++)
            {
                hod2v0_Part prt = hod.parts[i];
                prt.position = parts[index].transform.localPosition;
                prt.rotation = parts[index].transform.localRotation;
                prt.scale = parts[index].transform.localScale;
            }

            int j = index;
            for (; j >= 0; j--)
            {
                if (hod.parts[j].treeDepth < hod.parts[index].treeDepth)
                {
                    hod2v0_Part pHod = hod.parts[j];
                    pHod.childCount--;
                    hod.parts[j] = pHod;
                    hod.parts.RemoveAt(index);
                    break;
                }
            }
            buildStructure(hod);
            return true;
        }
        else
            return false;
    }

    public void renamePart(int index, string name)
    {
        
        if (ani != null)
        {
            hod2v0_Part prt = ani.structure.parts[index];
            prt.name = name;
            ani.structure.parts[index] = prt;
            buildStructure(ani.structure);
        }
        else
        {
            hod2v0_Part prt = hod.parts[index];
            prt.name = name;
            hod.parts[index] = prt;
            buildStructure(hod);
        }
        
    }

    public hod2v1 createHod2v1()
    {
        hod2v1 Pose = new hod2v1("Copy");
        Pose.parts = new List<hod2v1_Part>();
        for (int i = 0; i < parts.Count; i++)
        {
            hod2v1_Part prt = new hod2v1_Part();
            prt.position = parts[i].transform.localPosition;
            prt.rotation = parts[i].transform.localRotation;
            prt.scale = parts[i].transform.localScale;
            prt.unk1 = parts[i].transform.localRotation;
            prt.unk2 = parts[i].transform.localRotation;
            prt.unk3 = parts[i].transform.localRotation;
            Pose.parts.Add(prt);

        }
        return Pose;
    }


    public void saveAni()
    {
        ani.save();
    }

    public void saveHOD1()
    {
        
        hod1 sHOD = new hod1("");
        for (int i = 0; i < parts.Count; i++)
        {
            hod2v0_Part prt = hod.parts[i];
            prt.position = parts[i].transform.localPosition;
            prt.rotation = parts[i].transform.localRotation;
            prt.scale = parts[i].transform.localScale;
            hod.parts[i] = prt;
        }
        sHOD.createFromHod2v0(hod);
        BinaryWriter bw = new BinaryWriter(File.Open(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite));
        sHOD.saveToBinary(ref bw);
        bw.Close();
    }
    
}
