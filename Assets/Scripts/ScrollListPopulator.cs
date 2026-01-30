using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScrollListPopulator : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private ShiftSetLibrary shiftSetLibrary;
    [SerializeField] private TMP_Text titleText;

    void OnEnable() => Build(shiftSetLibrary.sets);

    public void Build(ShiftSet[] sets)
    {
        for (int i = content.childCount - 1; i >= 0; i--){
            var child = content.GetChild(i).gameObject;
            if (child == itemPrefab) continue;
            Destroy(child);
        }

        foreach (var set in sets)
        {
            var go = Instantiate(itemPrefab, content);
            go.SetActive(true);

            // safer: Button/TMP might be on children, not root
            var label = go.GetComponentInChildren<TMP_Text>(true);
            if (label) label.text = set.displayName;

            var button = go.GetComponentInChildren<Button>(true);
            if (button) button.onClick.AddListener(() => OnItemClicked(set));
        }
    }

    private void OnItemClicked(ShiftSet set)
    {
        ShiftSession.I.Select(set);
        titleText.text = set.displayName;
    }
}
