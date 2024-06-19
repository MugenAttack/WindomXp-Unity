using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
public struct mpd_Piece
{
    public string visualMesh;
    public string collisionMesh;
    public string scriptText;
}

public struct mpd_Object
{
    public Matrix4x4 transform;
    public short pieceID;
    //public string scriptText;
    public int scriptIndex;
}

public class mpd_WorldGrid
{
    public List<mpd_Object> objects;
}
public class mpd
{
    public List<mpd_Piece> Pieces;
    public mpd_WorldGrid[,] WorldGrid;
    public List<string> scripts;
    public float unk = 0.0f; //grid size

    public mpd()
    {
        WorldGrid = new mpd_WorldGrid[100,100];
        unk = 30.0f;
    }
    public bool load(string filename)
    {
        scripts = new List<string>();
        BinaryReader br = new BinaryReader(File.Open(filename, FileMode.Open, FileAccess.Read));
        string signature = new string(br.ReadChars(3));
        if (signature == "MPD")
        {
            int WorldParts = br.ReadInt32();
            int PiecesCount = br.ReadInt16();
            Debug.Log(PiecesCount);
            Pieces = new List<mpd_Piece>();
            for (int i = 0; i < PiecesCount; i++)
            {
                mpd_Piece Piece = new mpd_Piece();
                byte[] txt = br.ReadBytes(256);
                //scan
                int b = 0;
                for (; b < txt.Length; b++)
                {
                    if (txt[b] == 0)
                        break;
                }
                byte[] txt2 = new byte[b];
                System.Array.Copy(txt, 0, txt2, 0, b);
                Piece.visualMesh = USEncoder.ToEncoding.ToUnicode(txt2).TrimEnd('\0');
                txt = br.ReadBytes(256);
                //scan
                b = 0;
                for (; b < txt.Length; b++)
                {
                    if (txt[b] == 0)
                        break;
                }
                txt2 = new byte[b];
                System.Array.Copy(txt, 0, txt2, 0, b);
                Piece.collisionMesh = USEncoder.ToEncoding.ToUnicode(txt2).TrimEnd('\0');
                br.BaseStream.Seek(3, SeekOrigin.Current);
                int txtCount = br.ReadInt32();
                Piece.scriptText = USEncoder.ToEncoding.ToUnicode(br.ReadBytes(txtCount));
                Pieces.Add(Piece);
            }

            Debug.Log(br.BaseStream.Position.ToString());
            int x = br.ReadInt16();
            int y = br.ReadInt16();
            WorldGrid = new mpd_WorldGrid[100,100];
            unk = br.ReadInt32();

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    int ReadCount = br.ReadInt32();
                    WorldGrid[i, j] = new mpd_WorldGrid();
                    WorldGrid[i, j].objects = new List<mpd_Object>();
                    for (int k = 0; k < ReadCount; k++)
                    {
                        mpd_Object obj = new mpd_Object();
                        obj.transform = new Matrix4x4();
                        obj.transform.m00 = br.ReadSingle();
                        obj.transform.m10 = br.ReadSingle();
                        obj.transform.m20 = br.ReadSingle();
                        obj.transform.m30 = br.ReadSingle();
                        obj.transform.m01 = br.ReadSingle();
                        obj.transform.m11 = br.ReadSingle();
                        obj.transform.m21 = br.ReadSingle();
                        obj.transform.m31 = br.ReadSingle();
                        obj.transform.m02 = br.ReadSingle();
                        obj.transform.m12 = br.ReadSingle();
                        obj.transform.m22 = br.ReadSingle();
                        obj.transform.m32 = br.ReadSingle();
                        obj.transform.m03 = br.ReadSingle();
                        obj.transform.m13 = br.ReadSingle();
                        obj.transform.m23 = br.ReadSingle();
                        obj.transform.m33 = br.ReadSingle();
                        obj.pieceID = br.ReadInt16();
                        int rCount = br.ReadInt32();
                        string script = USEncoder.ToEncoding.ToUnicode(br.ReadBytes(rCount));
                        if (scripts.Contains(script))
                            obj.scriptIndex = scripts.FindIndex(s => s == script);
                        else
                        {
                            scripts.Add(script);
                            obj.scriptIndex = scripts.Count - 1;
                        }
                        //obj.scriptText = ASCIIEncoding.ASCII.GetString(br.ReadBytes(rCount));
                        WorldGrid[i, j].objects.Add(obj);
                    }
                }
            }
        }
        br.Close();

        return false;
    }

    public void save(string filename)
    {
        BinaryWriter bw = new BinaryWriter(File.Open(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite));
        bw.Write(ASCIIEncoding.ASCII.GetBytes("MPD"));
        bw.Write(20000);
        bw.Write((short)200);
        for (int i = 0; i < Pieces.Count; i++)
        {
            byte[] shiftjistext;
            byte[] blankText = new byte[256];
            if (Pieces[i].visualMesh != null)
            {
                shiftjistext = USEncoder.ToEncoding.ToSJIS(Pieces[i].visualMesh);
                bw.Write(shiftjistext);
                bw.BaseStream.Seek(256 - shiftjistext.Length, SeekOrigin.Current);
            }
            else
                bw.Write(blankText);

            if (Pieces[i].collisionMesh != null)
            {
                shiftjistext = USEncoder.ToEncoding.ToSJIS(Pieces[i].collisionMesh);
                bw.Write(shiftjistext);
                bw.BaseStream.Seek(256 - shiftjistext.Length, SeekOrigin.Current);
            }
            else
                bw.Write(blankText);
            bw.Write((short)-1);
            bw.BaseStream.Seek(1, SeekOrigin.Current);

            if (Pieces[i].scriptText != null)
            {
                List<byte> list = new List<byte>();
                list.AddRange(USEncoder.ToEncoding.ToSJIS(Pieces[i].scriptText));
                for (int j = 0; j < list.Count; j++)
                {
                    if (list[j] == 0x0A && list[j - 1] != 0x0D)
                        list.Insert(j, 0x0D);
                }
                bw.Write(list.Count);
                bw.Write(list.ToArray());
            }
            else
            {
                bw.Write(1);
                bw.Write((byte)0x00);
            }
        }
        bw.Write((short)100);
        bw.Write((short)100);
        bw.Write(unk);
        for (int x = 0; x < 100; x++)
        {
            for (int y = 0; y < 100; y++)
            {
                if (WorldGrid[x, y] != null)
                {
                    int count = WorldGrid[x, y].objects.Count;
                    bw.Write(count);
                    for (int z = 0; z < count; z++)
                    {
                        bw.Write(WorldGrid[x, y].objects[z].transform.m00);
                        bw.Write(WorldGrid[x, y].objects[z].transform.m10);
                        bw.Write(WorldGrid[x, y].objects[z].transform.m20);
                        bw.Write(WorldGrid[x, y].objects[z].transform.m30);
                        bw.Write(WorldGrid[x, y].objects[z].transform.m01);
                        bw.Write(WorldGrid[x, y].objects[z].transform.m11);
                        bw.Write(WorldGrid[x, y].objects[z].transform.m21);
                        bw.Write(WorldGrid[x, y].objects[z].transform.m31);
                        bw.Write(WorldGrid[x, y].objects[z].transform.m02);
                        bw.Write(WorldGrid[x, y].objects[z].transform.m12);
                        bw.Write(WorldGrid[x, y].objects[z].transform.m22);
                        bw.Write(WorldGrid[x, y].objects[z].transform.m32);
                        bw.Write(WorldGrid[x, y].objects[z].transform.m03);
                        bw.Write(WorldGrid[x, y].objects[z].transform.m13);
                        bw.Write(WorldGrid[x, y].objects[z].transform.m23);
                        bw.Write(WorldGrid[x, y].objects[z].transform.m33);
                        bw.Write(WorldGrid[x, y].objects[z].pieceID);
                        List<byte> list = new List<byte>();
                        list.AddRange(USEncoder.ToEncoding.ToSJIS(scripts[WorldGrid[x, y].objects[z].scriptIndex]));
                        for (int j = 0; j < list.Count; j++)
                        {
                            if (list[j] == 0x0A && list[j - 1] != 0x0D)
                                list.Insert(j, 0x0D);
                        }
                        bw.Write(list.Count);
                        bw.Write(list.ToArray());
                    }
                }
                else
                    bw.Write(0);
            }
        }

        bw.Close();
    }

    public void addWorldObject(Vector3 position, Matrix4x4 matrix, int pieceID, int scriptID)
    {

        int x = Mathf.FloorToInt(position.x / unk);
        int y = Mathf.FloorToInt(position.z / unk);
        
        mpd_Object mo;
        mo.transform = matrix;
        mo.pieceID = (short)pieceID;
        mo.scriptIndex = scriptID;

        if (WorldGrid[y, x] == null)
        {
            WorldGrid[y, x] = new mpd_WorldGrid();
            WorldGrid[y, x].objects = new List<mpd_Object>();
        }
        WorldGrid[y, x].objects.Add(mo);
    }
}
