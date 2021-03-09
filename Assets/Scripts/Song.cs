using UnityEngine;

[CreateAssetMenu (fileName = "New Song", menuName = "Song")]
public class Song : ScriptableObject {

    public string Name;
    public string Author;
    [TextArea (3, 10)] public string Description;
    public ChartDifficultyToGameObjectDictionary Charts;

}
