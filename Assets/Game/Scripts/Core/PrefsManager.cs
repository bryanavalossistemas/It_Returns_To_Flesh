using System;
using UnityEngine;
using static BehaviourPlus;

public class PrefsManager : MonoBehaviour
{
    #region Setters_Getters
    private void SetEnum<T>(string key, T value) where T : Enum => PlayerPrefs.SetInt(key, Convert.ToInt32(value));
    private T GetEnum<T>(string key, T _default = default) where T : Enum => (T)Enum.ToObject(typeof(T), PlayerPrefs.GetInt(key, Convert.ToInt32(_default)));

    public void SetInt(string key, int value) => PlayerPrefs.SetInt(key, value);
    public int GetInt(string key, int _default) => PlayerPrefs.GetInt(key, _default);

    public void SetFloat(string key, float value) => PlayerPrefs.SetFloat(key, value);
    public float GetFloat(string key, float _default) => PlayerPrefs.GetFloat(key, _default);

    public void SetString(string key, string value) => PlayerPrefs.SetString(key, value);
    public string GetString(string key, string _default) => PlayerPrefs.GetString(key, _default);
    #endregion

    public void LoadPlayerPrefs()
    {
        audioManager.SetMasterVolume(GetInt("vol_master", 100));
        audioManager.SetMusicVolume(GetInt("vol_music", 100));
        audioManager.SetSfxVolume(GetInt("vol_sfx", 100));
    }

    public void SavePlayerPrefs() => PlayerPrefs.Save();
}