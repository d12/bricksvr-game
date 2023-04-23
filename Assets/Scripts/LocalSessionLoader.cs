using System.IO.Compression;
using Newtonsoft.Json;
using UnityEngine;
using System.Linq;
using System.IO;
using System;

public class LocalSessionLoader : MonoBehaviour
{
    public static BrickData.LocalBrickData[] ReadSave(string file) {
        if(!File.Exists(file)) {
            Debug.LogError("Save not found!");    
            return new BrickData.LocalBrickData[] {};
        }

        using (ZipArchive zip = ZipFile.Open(file, ZipArchiveMode.Read))
        {
            ZipArchiveEntry entry = zip.GetEntry("data.json");

            using (StreamReader reader = new StreamReader(entry.Open()))
            {
                string data = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<BrickData.LocalBrickData[]>(data);
            }
        }
    }

    public static void SaveRoom(string path) {
        BrickStore store = BrickStore.GetInstance();
        BrickData.LocalBrickData[] bricks = store.Values()
            .Where(value => value != null)
            .Select(brick => {
                BrickAttach attach = brick.GetComponent<BrickAttach>();
                
                return new BrickData.LocalBrickData {
                    rot = BrickData.CustomQuaternion.From(brick.transform.rotation),
                    pos = BrickData.CustomVec3.From(brick.transform.position),
                    color = ColorInt.Color32ToInt(attach.Color),
                    type = attach.normalPrefabName
                };
            }).ToArray();

        using (ZipArchive zip = ZipFile.Open(path, ZipArchiveMode.Update))
        {
            ZipArchiveEntry entry = zip.GetEntry("data.json");
            if (entry != null)
                entry.Delete();

            entry = zip.CreateEntry("data.json", System.IO.Compression.CompressionLevel.Optimal);

            using (StreamWriter writer = new StreamWriter(entry.Open()))
            {
                string roomData = JsonConvert.SerializeObject(bricks, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                writer.Write(roomData);
            }
        }
    }

    public static bool CreateRoom(string name) {
        string path = $"{Application.dataPath}/saves/{name}.bricks";
        if(File.Exists(path))
            return false;

        try {
            using (ZipArchive zip = ZipFile.Open(path, ZipArchiveMode.Create))
            {
                ZipArchiveEntry archive = zip.CreateEntry("data.json", System.IO.Compression.CompressionLevel.Optimal);
                using (StreamWriter writer = new StreamWriter(archive.Open()))
                {
                    writer.Write("[]");
                }
                return true;
            }   
        } catch (Exception)
        {
            return false;
        }
    }
}
