using UnityEngine;

public enum Difficulty {Easy, Normal, Hard, Expert}


public class ShiftSession : MonoBehaviour
{
    public static ShiftSession I {get; private set;}
    public ShiftSet SelectedShift {get; private set;}
    public Difficulty SelectedDifficulty {get; private set;}

    public bool HasSelection() => SelectedShift != null;

    void Awake()
    {
        if (I != null) {Destroy(gameObject); return;}
        I = this;
        DontDestroyOnLoad(gameObject);  
    }

    public void Select(ShiftSet shift)
    {
        SelectedShift = shift;
    }

}
