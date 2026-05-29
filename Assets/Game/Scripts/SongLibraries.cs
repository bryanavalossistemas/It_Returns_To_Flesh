using UnityEngine;
using FMODUnity;
using AYellowpaper.SerializedCollections;

[CreateAssetMenu(fileName = "SongLibrary", menuName = "Scriptable Objects/SongLibraries")]
public class SongLibraries : ScriptableObject
{
    [SerializedDictionary("songName", "EventReference")] public SerializedDictionary<string, EventReference> Events_BGM;
    [SerializedDictionary()] public SerializedDictionary<string, string> Scene_Themes;
    [ParamRef] public string Parameter;
}