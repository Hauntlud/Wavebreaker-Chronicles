#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class CleanUpScript : MonoBehaviour
{
    [MenuItem("Tools/Clean Up Assets")]
    static void CleanUpAssets()
    {
        Debug.Log("Starting clean-up process...");
        AssetDatabase.RemoveUnusedAssetBundleNames();
        AssetDatabase.Refresh();
        Debug.Log("Clean-up completed.");
    }
}
#endif