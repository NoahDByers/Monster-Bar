using UnityEngine;

[CreateAssetMenu(menuName="Songs/Shift Set")]
public class ShiftSet : ScriptableObject
{
    public string shiftId;
    public string displayName;
    public Sprite coverArt;
    public SongData[] songSet;
    //public ShiftModifier[] allowedModifiers;
    
}
