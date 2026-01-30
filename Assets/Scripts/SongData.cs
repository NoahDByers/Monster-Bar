using UnityEngine;


[CreateAssetMenu(menuName="Songs/Song")]
public class SongData : ScriptableObject
{
    public string songId;
    public string displayName;
    public Sprite coverArt;

    [System.Serializable]
    public struct DiffEntry
    {
        public float difficulty;
        public float scrollSpeed;
    }

    public DiffEntry[] difficulties;
}
