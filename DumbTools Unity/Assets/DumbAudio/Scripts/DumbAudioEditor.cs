using UnityEngine;
using UnityEditor;

public static class DumbAudioEditor
{
    [MenuItem("GameObject/Audio/Dumb Ambient Area", false, 10)]
    public static void CreateAudioAmbientArea(MenuCommand menuCommand)
    {
        GameObject go = new ("Audio Ambient Area");
        go.AddComponent<DumbAmbientArea>();
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        Selection.activeObject = go;
    }
    
    [MenuItem("GameObject/Audio/Dumb Audio Manager", false, 9)]
    public static void CreateDumbAudioManager(MenuCommand menuCommand)
    {
        GameObject go = new ("Audio Manager");
        go.AddComponent<DumbAudio>();
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        Selection.activeObject = go;
    }
}
