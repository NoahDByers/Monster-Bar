using System.Collections.Generic;
using UnityEngine;

public class NotePool : MonoBehaviour
{
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private int prewarmCount = 200;
    [SerializeField] private Transform poolParent;

    private readonly Stack<NoteController> pool = new Stack<NoteController>();

    private void Awake()
    {
        if (poolParent == null) poolParent = transform;

        // Prewarm
        for (int i = 0; i < prewarmCount; i++)
        {
            var note = CreateNew();
            ReturnToPool(note);
        }
    }

    private NoteController CreateNew()
    {
        var go = Instantiate(notePrefab, poolParent);
        go.SetActive(false);

        var ctrl = go.GetComponent<NoteController>();
        if (ctrl == null)
            ctrl = go.AddComponent<NoteController>();

        ctrl.SetPool(this);
        return ctrl;
    }

    public NoteController Get()
    {
        if (pool.Count > 0)
        {
            var note = pool.Pop();
            note.gameObject.SetActive(true);
            return note;
        }

        var created = CreateNew();
        created.gameObject.SetActive(true);
        return created;
    }

    public void ReturnToPool(NoteController note)
    {
        note.gameObject.SetActive(false);
        note.transform.SetParent(poolParent, false);
        pool.Push(note);
    }
}
