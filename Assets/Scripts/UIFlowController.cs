using System.Collections.Generic;
using UnityEngine;

public enum UIState
{
    Title,
    Hub,
    BarMenu,
    ShiftMenu,
    DungeonMenu
}

public class UIFlowController : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private GameObject screenTitle;
    [SerializeField] private GameObject screenHub;
    [SerializeField] private GameObject screenBarMenu;
    [SerializeField] private GameObject screenShiftMenu;
    [SerializeField] private GameObject screenDungeonMenu;

    private readonly Stack<UIState> history = new();
    private UIState current;

    void Start()
    {
        GoTo(UIState.Title, clearHistory: true);
    }

    public void GoTo(UIState next, bool clearHistory = false)
    {
        if (clearHistory)
        {
            history.Clear();
        }
        else
        {
            history.Push(current);
        }

        current = next;
        ShowOnly(current);
    }

    public void Back()
    {
        if (history.Count == 0) return;
        current = history.Pop();
        ShowOnly(current);
    }

    private void ShowOnly(UIState state)
    {
        screenTitle.SetActive(state == UIState.Title);
        screenHub.SetActive(state == UIState.Hub);
        screenBarMenu.SetActive(state == UIState.BarMenu);
        screenShiftMenu.SetActive(state == UIState.ShiftMenu);
        screenDungeonMenu.SetActive(state == UIState.DungeonMenu);
    }

    public void GoTitle() => GoTo(UIState.Title, true);
    public void GoHub() => GoTo(UIState.Hub, true);
    public void GoBarMenu() => GoTo(UIState.BarMenu, false);
    public void GoDungeonMenu() => GoTo(UIState.DungeonMenu, false);
    public void GoShiftMenu() => GoTo(UIState.ShiftMenu, false);
}
