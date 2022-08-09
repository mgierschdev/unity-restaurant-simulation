using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/StoreItemsScriptableObject", order = 1)]
public class StoreItemScriptableObject : ScriptableObject
{
    public string prefabName;
    public string assetFileName;
    public ObjectType type;
}