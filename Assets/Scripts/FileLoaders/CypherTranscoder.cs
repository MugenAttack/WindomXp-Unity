using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

public struct ctFileType
{
    public string fileExt;
    public uint signature;  
}
public class CypherTranscoder
{
    public uint cypher = 0x0B7E7759;
    public List<ctFileType> filetypes = new List<ctFileType>();

    public CypherTranscoder()
    {
        registerFileType(".png", 1196314761);
        registerFileType(".x", 543584120);
    }
        
    public void registerFileType(string fileExt, uint signature)
    {
        ctFileType fileType = new ctFileType();
        fileType.fileExt = fileExt;
        fileType.signature = signature;
        filetypes.Add(fileType);
    }

    public bool findCypher(string name)
    {
        FileInfo fi = new FileInfo(name);
        byte[] bytes = File.ReadAllBytes(name);
        //check if encrypted
        uint signature = BitConverter.ToUInt32(bytes, 0);
            
        for (int i = 0; i < filetypes.Count; i++)
        {
            if (filetypes[i].fileExt == fi.Extension)
            {
                uint cypherF = filetypes[i].signature ^ signature;
                signature ^= cypherF;
                if (signature == filetypes[i].signature)
                {
                    cypher = cypherF;
                    return true;
                }
                break;
            }
        }
        return false;
    }

    public byte[] Transcode(string name)
    {
        byte[] bytes = File.ReadAllBytes(name);
        for (int i = 0; i < bytes.Length; i += 4)
        {
            byte[] cypherBytes = BitConverter.GetBytes(cypher);
            for (int b = 0; b < cypherBytes.Length; b++)
            {
                if (i + 3 < bytes.Length)
                    bytes[i + b] ^= cypherBytes[b];
            }
        }
        return bytes;
    }

    public byte[] Transcode(byte[] bytes)
    {
        for (int i = 0; i < bytes.Length; i += 4)
        {
            byte[] cypherBytes = BitConverter.GetBytes(cypher);
            for (int b = 0; b < cypherBytes.Length; b++)
            {
                if (i + 3 < bytes.Length)
                    bytes[i + b] ^= cypherBytes[b];
            }
        }
        return bytes;
    }
}

