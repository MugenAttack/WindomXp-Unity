using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Assimp.Configs;
using Assets;
using System.Linq;
using UnityDds;

public struct PieceData
{
	public Mesh visualMesh;
	public Mesh colliderMesh;
	public Material[] materials;
	public string script;
}


public class Map_Data : MonoBehaviour
{
	public GameObject Map;
	public GameObject HitArea;
	public GameObject SkyMap;
	public Assimp.AssimpImporter Importer = new Assimp.AssimpImporter();
	//public List<Map_PieceBatch> pieceBatchList = new List<Map_PieceBatch>();
	public PieceData[] Pieces;
	public List<string> scripts = new List<string>();
	public string path = "E:\\Downloads\\WindomMapsDecoded\\map\\moon";
	public Material baseMat;
	public Material hitMat;
	public List<Vector3> point_0 = new List<Vector3>();
	public List<Vector3> point_1 = new List<Vector3>();
	public List<Vector3> point_2 = new List<Vector3>();
	public List<Vector3> point_3 = new List<Vector3>();
	public List<Vector3> point_4 = new List<Vector3>();
	public List<Vector3> point_5 = new List<Vector3>();
	public List<Vector3> point_6 = new List<Vector3>();
	public List<Vector3> point_7 = new List<Vector3>();
	public List<Vector3> point_8 = new List<Vector3>();
	public List<Vector3> point_9 = new List<Vector3>();
	public List<Vector3> point_rnd = new List<Vector3>();

	// Start is called before the first frame update
	void Start()
    {

	}

    // Update is called once per frame
    void Update()
    {
        
    }

	public void loadmpd()
    {
		mpd map = new mpd();
		map.load(Path.Combine(path, "map.mpd"));
		point_0 = new List<Vector3>();
		point_1 = new List<Vector3>();
		point_2 = new List<Vector3>();
		point_3 = new List<Vector3>();
		point_4 = new List<Vector3>();
		point_5 = new List<Vector3>();
		point_6 = new List<Vector3>();
		point_7 = new List<Vector3>();
		point_8 = new List<Vector3>();
		point_9 = new List<Vector3>();
		point_rnd = new List<Vector3>();


		Pieces = new PieceData[map.Pieces.Count];
		scripts = map.scripts;

		for (int j = 0; j < Pieces.Length; j++)
		{
			updatePiece(j, map.Pieces[j].visualMesh, map.Pieces[j].collisionMesh, map.Pieces[j].scriptText);
			//if (map.Pieces[j].visualMesh != "")
			//{
				
			//	Mesh visualM = new Mesh();
			//	Mesh colliderM = new Mesh();
			//	Material[] mats = new Material[0];
			//	bool isAlpha = false;
			//	try
			//	{
			//		if (j == 199)
			//			isAlpha = true;

			//		if (map.Pieces[j].visualMesh != "")
			//			ImportModel(Path.Combine(path, map.Pieces[j].visualMesh), ref visualM, ref mats, isAlpha);

			//		if (map.Pieces[j].visualMesh == map.Pieces[j].collisionMesh)
			//		{ colliderM = visualM; Debug.Log("Equals Visual"); }
			//		if (map.Pieces[j].collisionMesh != "")
			//		{ colliderM = ImportModel(Path.Combine(path, map.Pieces[j].collisionMesh)); Debug.Log("New Mesh Loaded"); }
			//	}
   //             catch
   //             {
			//		Debug.Log("Invalid Load: " + map.Pieces[j].visualMesh);
   //             }
			//	visualM.name = map.Pieces[j].visualMesh;
			//	if (colliderM != null)
			//		colliderM.name = map.Pieces[j].collisionMesh;

			//	Pieces[j].visualMesh = visualM;
			//	Pieces[j].colliderMesh = colliderM;
			//	Pieces[j].materials = mats;
			//	Pieces[j].script = map.Pieces[j].scriptText;

				
			//}
		}

		ObjectData od;
		for (int x = 0; x < map.WorldGrid.GetLength(0); x++)
		{
			for (int y = 0; y < map.WorldGrid.GetLength(1); y++)
		{
			mpd_WorldGrid wg2 = map.WorldGrid[x,y];
			foreach (mpd_Object obj in wg2.objects)
			{

				if (Pieces[obj.pieceID].visualMesh != null)
				{
					if (Pieces[obj.pieceID].visualMesh.name.Contains("point"))
					{
							
							if (Pieces[obj.pieceID].visualMesh.name.Contains("point_0"))
								point_0.Add(Utils.GetPosition(obj.transform));
							if (Pieces[obj.pieceID].visualMesh.name.Contains("point_1"))
								point_1.Add(Utils.GetPosition(obj.transform));
							if (Pieces[obj.pieceID].visualMesh.name.Contains("point_2"))
								point_2.Add(Utils.GetPosition(obj.transform));
							if (Pieces[obj.pieceID].visualMesh.name.Contains("point_3"))
								point_3.Add(Utils.GetPosition(obj.transform));
							if (Pieces[obj.pieceID].visualMesh.name.Contains("point_4"))
								point_4.Add(Utils.GetPosition(obj.transform));
							if (Pieces[obj.pieceID].visualMesh.name.Contains("point_5"))
								point_5.Add(Utils.GetPosition(obj.transform));
							if (Pieces[obj.pieceID].visualMesh.name.Contains("point_6"))
								point_6.Add(Utils.GetPosition(obj.transform));
							if (Pieces[obj.pieceID].visualMesh.name.Contains("point_7"))
								point_7.Add(Utils.GetPosition(obj.transform));
							if (Pieces[obj.pieceID].visualMesh.name.Contains("point_8"))
								point_8.Add(Utils.GetPosition(obj.transform));
							if (Pieces[obj.pieceID].visualMesh.name.Contains("point_9"))
								point_9.Add(Utils.GetPosition(obj.transform));
							if (Pieces[obj.pieceID].visualMesh.name.Contains("point_rnd"))
								point_rnd.Add(Utils.GetPosition(obj.transform));
						}
					else
					{
						GameObject go = new GameObject(Pieces[obj.pieceID].visualMesh.name);
						go.layer = 6;
						od = go.AddComponent<ObjectData>();
						od.ModelID = obj.pieceID;
						od.scriptID = obj.scriptIndex;
						go.transform.SetParent(Map.transform);
						go.transform.localPosition = Utils.GetPosition(obj.transform);
						go.transform.localRotation = Utils.GetRotation(obj.transform);
						go.transform.localScale = Utils.GetScale(obj.transform);
						od.data = this;
						od.build();
					}

				}

			}
		}
		}

        HitArea = new GameObject("Hit.x");
        od = HitArea.AddComponent<ObjectData>();
        od.ModelID = 199;
        od.scriptID = -1;
        HitArea.transform.localPosition = new Vector3(1500, 0, 1500);
        od.data = this;
        od.build();
		HitArea.AddComponent<MeshCollider>().sharedMesh = Pieces[199].visualMesh;


		SkyMap = new GameObject("Sky.x");
		od = SkyMap.AddComponent<ObjectData>();
		od.ModelID = 198;
		od.scriptID = -1;
		SkyMap.transform.localPosition = new Vector3(1500, 0, 1500);
		SkyMap.transform.localScale = Vector3.one * 3000;
		od.data = this;
		od.build();

	}

	public void buildMapPart()
    {

    }

	public void save()
    {
		mpd map = new mpd();
		map.scripts = scripts;
		List<mpd_Piece> mpdPieces = new List<mpd_Piece>();
		for (int i = 0; i < Pieces.Length; i++)
        {
			mpd_Piece mpdPiece = new mpd_Piece();
			if (Pieces[i].visualMesh != null)
			{
				mpdPiece.visualMesh = Pieces[i].visualMesh.name;
				if (Pieces[i].colliderMesh != null)
					mpdPiece.collisionMesh = Pieces[i].colliderMesh.name;
				mpdPiece.scriptText = Pieces[i].script;
			}
			mpdPieces.Add(mpdPiece);
        }
		map.Pieces = mpdPieces;
		Transform[] transforms = Map.GetComponentsInChildren<Transform>();
		for (int i = 1; i < transforms.Length; i++)
        {
			Matrix4x4 m = Matrix4x4.TRS(transforms[i].position,transforms[i].rotation,transforms[i].localScale);
			ObjectData od = transforms[i].GetComponent<ObjectData>();
			if (od != null)
				map.addWorldObject(transforms[i].position,m,od.ModelID,od.scriptID);
        }
		map.save(Path.Combine(path, "map.mpd"));
	}

    public void updatePiece(int index, string visualMesh, string collisionMesh, string scriptText)
    {
		if (visualMesh != "")
		{

			Mesh visualM = new Mesh();
			Mesh colliderM = new Mesh();
			Material[] mats = new Material[0];
			bool isAlpha = false;
			try
			{
				if (index == 199)
					isAlpha = true;

				if (visualMesh != "")
					ImportModel(Path.Combine(path, visualMesh), ref visualM, ref mats, isAlpha);

				if (visualMesh == collisionMesh)
				{ colliderM = visualM; }
				if (collisionMesh != "")
				{ colliderM = ImportModel(Path.Combine(path, collisionMesh));  }
			}
			catch
			{
				Debug.Log("Invalid Load: " + visualMesh);
			}
			visualM.name = visualMesh;
			if (colliderM != null)
				colliderM.name = collisionMesh;

			Pieces[index].visualMesh = visualM;
			Pieces[index].colliderMesh = colliderM;
			if (index == 199)
            {
				for (int i = 0; i < mats.Length; i++)
                {
					Texture tex = mats[i].mainTexture;
					mats[i] = new Material(hitMat.shader);
					mats[i].mainTexture = tex;
                }
            }
			Pieces[index].materials = mats;
			Pieces[index].script = scriptText;
		}
	}
    //public void build()
    //{

    //	for (int i = 0; i < pieceBatchList.Length; i++)
    //	{
    //		if (pieceBatchList[i] != null)
    //		{
    //			Debug.Log("Building");
    //			pieceBatchList[i].destroyGameObjects();
    //			pieceBatchList[i].generateGameObjects();
    //		}
    //	}
    //}

    void ImportModel(GameObject GO, string file, bool isAlpha = false)
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
						if (isAlpha)
							mat.CopyPropertiesFromMaterial(baseMat);
						mat.name = scen.Materials[scen.Meshes[index].MaterialIndex].Name;
						var textures = scen.Materials[scen.Meshes[index].MaterialIndex].GetAllTextures();
						var color = scen.Materials[scen.Meshes[index].MaterialIndex].ColorDiffuse;
						mat.color = new Color(color.R, color.G, color.B, color.A);
						//mat.SetFloat("_Glossiness", scen.Materials[scen.Meshes[index].MaterialIndex].ShininessStrength);


						if (textures.Length > 0 && File.Exists(Path.Combine(Modelpath, textures[0].FilePath)))
						{
							try
							{
								FileInfo f = new FileInfo(Path.Combine(Modelpath, textures[0].FilePath));
								if (f.Extension == ".dds")
									mat.mainTexture = DdsTextureLoader.LoadTexture(f.FullName);
								else
									mat.mainTexture = Helper.LoadTexture(f.FullName);
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

	void ImportModel(string file, ref Mesh mesh, ref Material[] materials, bool isAlpha = false)
	{
		
		if (File.Exists(file))
		{
		
			try
			{
				string Modelpath = Path.GetDirectoryName(file);

				var scen = Importer.ImportFile(file, Helper.PostProcessStepflags);
				mesh.CombineMeshes(scen.Meshes.Select(x => new CombineInstance()
				{
					mesh = x.ToUnityMesh(),
					transform = scen.RootNode.Transform.ToUnityMatrix()
				}).ToArray(), false);
				materials = new Material[scen.Meshes.Length];
				Debug.Log(materials.Length);
				for (int index = 0; index < materials.Length; index++)
				{
					var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));

					if (scen.Materials[scen.Meshes[index].MaterialIndex] != null)
					{
						if (isAlpha)
							mat.CopyPropertiesFromMaterial(baseMat);
						mat.name = scen.Materials[scen.Meshes[index].MaterialIndex].Name;
						var textures = scen.Materials[scen.Meshes[index].MaterialIndex].GetAllTextures();



						if (textures.Length > 0 && File.Exists(Path.Combine(Modelpath, textures[0].FilePath)))
						{
							try
							{
								FileInfo f = new FileInfo(Path.Combine(Modelpath, textures[0].FilePath));
								if (f.Extension == ".dds")
								{
									mat.mainTexture = DdsTextureLoader.LoadTexture(f.FullName);
									mat.SetTextureScale("_MainTex", new Vector2(1, -1));
								}
								else
									mat.mainTexture = Helper.LoadTexture(f.FullName);
							}
							catch
							{
							}
						}
					}

					materials[index] = mat;
				}
			}
			catch
			{
			}
		}
		else
			Debug.Log("File Doesn't Exist");
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

	//Texture2D LoadDDS(string file)
 //   {
	//	byte[] data = File.ReadAllBytes(file);
		
 //   }
}
