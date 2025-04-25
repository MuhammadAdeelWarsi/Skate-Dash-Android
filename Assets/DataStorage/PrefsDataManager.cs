using UnityEngine;


public class PrefsDataManager : MonoBehaviour
{
    private static PrefsDataManager instance;
    public static PrefsDataManager Instance => instance;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public int LoadData(PrefsKeys key, int defaultValue = 0)
    {
        if(!PlayerPrefs.HasKey(key.ToString()))
            SaveData(key, defaultValue);

        return PlayerPrefs.GetInt(key.ToString());
    }
    public float LoadData(PrefsKeys key, float defaultValue = 0f)
    {
        if (!PlayerPrefs.HasKey(key.ToString()))
            SaveData(key, defaultValue);

        return PlayerPrefs.GetFloat(key.ToString());
    }
    public string LoadData(PrefsKeys key, string defaultValue = null)
    {
        if (!PlayerPrefs.HasKey(key.ToString()))
            SaveData(key, defaultValue);

        return PlayerPrefs.GetString(key.ToString());
    }

    public void SaveData(PrefsKeys key, int data)
    {
        PlayerPrefs.SetInt(key.ToString(), data);
    }
    public void SaveData(PrefsKeys key, float data)
    {
        PlayerPrefs.SetFloat(key.ToString(), data);
    }
    public void SaveData(PrefsKeys key, string data)
    {
        PlayerPrefs.SetString(key.ToString(), data);
    }

    public void ForceSave()
    {
        PlayerPrefs.Save();
    }
    public void ClearData(PrefsKeys targetKey)
    {
        PlayerPrefs.DeleteKey(targetKey.ToString());
        ForceSave();
    }
    [ContextMenu("ClearAllData")] public void ClearData()
    {
        PlayerPrefs.DeleteAll();
        ForceSave();
    }

    #region Enum
    public enum PrefsKeys
    {
        GameplayTime,
        EncounteredObstacles,
        Music,
        Sound,
        Vibration,

        None    //For using as a null or empty value
    }
    #endregion
}