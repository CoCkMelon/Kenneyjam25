using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    private static readonly string SavePath = "/Saves/savefile.sav";

    public static void SaveGame(SaveData saveData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + SavePath;

        if (!Directory.Exists(Application.persistentDataPath + "/Saves"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Saves");
        }

        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            formatter.Serialize(stream, saveData);
        }
    }

    public static SaveData LoadGame()
    {
        string path = Application.persistentDataPath + SavePath;

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                return formatter.Deserialize(stream) as SaveData;
            }
        }
        else
        {
            Debug.Log("Save file not found in " + path);
            return new SaveData();
        }
    }

    public static void DeleteSaveFile()
    {
        string path = Application.persistentDataPath + SavePath;

        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
