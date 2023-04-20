using System.IO.Compression;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;

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
}
