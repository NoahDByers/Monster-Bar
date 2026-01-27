using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScrollListPopulator : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private GameObject itemPrefab;

    void OnEnable()
    {
        string[] names = { "Crossing Fields", "Days of the Week", "Marceline", "Big Black Balls" };
        Build(names);
    }

    public void Build(string[] names)
    {
        for (int i = content.childCount - 1; i >= 0; i--){
            var child = content.GetChild(i).gameObject;
            if (child == itemPrefab) continue;
            Destroy(child);
        }

        foreach (var n in names)
        {
            var go = Instantiate(itemPrefab, content);
            go.SetActive(true);

            // safer: Button/TMP might be on children, not root
            var label = go.GetComponentInChildren<TMP_Text>(true);
            if (label) label.text = n;

            var button = go.GetComponentInChildren<Button>(true);
            if (button) button.onClick.AddListener(() => OnItemClicked(n));
        }
    }

    private void OnItemClicked(string name) => Debug.Log("Clicked: " + name);
}
