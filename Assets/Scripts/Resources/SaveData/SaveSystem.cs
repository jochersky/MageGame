using UnityEngine;
using System.IO;
using UnityEngine.XR;

public class SaveSystem
{
    private static bool _release = false;
    private static string _debugFilePath = Application.dataPath + "/Scripts/Resources/SaveData";
    private static string _releaseFilePath = Application.persistentDataPath;
    
    private static SaveData _saveData = new SaveData();
    
    [System.Serializable]
    public struct SaveData
    {
        public PlayerSaveData playerData;
    }

    public static string SaveFileName()
    {
        return _release ? _releaseFilePath + "/save.save" : _debugFilePath + "/save.save";
    }

    public static void Save()
    {
        HandleSaveData();
        
        File.WriteAllText(SaveFileName(), JsonUtility.ToJson(_saveData, true));
    }

    private static void HandleSaveData()
    {
        GameManager.Instance.Player.Save(ref _saveData.playerData);
    }

    public static void Load()
    {
        string saveContext = File.ReadAllText(SaveFileName());
        
        _saveData = JsonUtility.FromJson<SaveData>(saveContext);
        HandleLoadData();
    }

    private static void HandleLoadData()
    {
        GameManager.Instance.Player.Load(ref _saveData.playerData);
    }
}
