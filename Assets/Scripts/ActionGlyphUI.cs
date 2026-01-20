using UnityEngine;
using UnityEngine.InputSystem;

// Alias fixes the ambiguous Image error even if UI Toolkit is in the project
using UIImage = UnityEngine.UI.Image;

public class ActionGlyphUI : MonoBehaviour
{
    [Header("Action")]
    [SerializeField] private InputActionReference actionRef;

    [Header("Glyph Libraries")]
    [SerializeField] private GlyphLibrary keyboardGlyphs;
    [SerializeField] private GlyphLibrary gamepadGlyphs;

    [Header("UI")]
    [SerializeField] private UIImage targetImage;

    // Optional: if you’re using the InputDeviceTracker from earlier
    [SerializeField] private InputDeviceTracker deviceTracker;

    private void Awake()
    {
        if (!targetImage) targetImage = GetComponent<UIImage>();
        if (!deviceTracker) deviceTracker = FindFirstObjectByType<InputDeviceTracker>();
    }

    private void OnEnable()
    {
        Refresh();

        if (deviceTracker != null)
            deviceTracker.OnDeviceChanged += OnDeviceChanged;
    }

    private void OnDisable()
    {
        if (deviceTracker != null)
            deviceTracker.OnDeviceChanged -= OnDeviceChanged;
    }

    private void OnDeviceChanged(InputDeviceType _) => Refresh();


    public void Refresh()
    {
        if (actionRef == null || actionRef.action == null || targetImage == null)
            return;

        var action = actionRef.action;

        InputDeviceType deviceType =
            deviceTracker != null ? deviceTracker.Current : InputDeviceType.KeyboardMouse;

        int bindingIndex = FindBindingIndexForDevice(action, deviceType);
        if (bindingIndex < 0)
        {
            targetImage.enabled = false;
            return;
        }

        // Use effectivePath as the source of truth
        string effectivePath = action.bindings[bindingIndex].effectivePath; // e.g. "<Keyboard>/d"
        string key = ExtractControlName(effectivePath);                     // e.g. "d"

        GlyphLibrary lib = (deviceType == InputDeviceType.Gamepad) ? gamepadGlyphs : keyboardGlyphs;
        Sprite sprite = lib != null ? lib.Get(key) : null;

        targetImage.sprite = sprite;
        targetImage.enabled = sprite != null;

        // Turn this on while building your library:
        // Debug.Log($"{action.name} [{deviceType}] effectivePath={effectivePath} key={key} sprite={(sprite ? sprite.name : "NULL")}");
    }

    private static string ExtractControlName(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;

        // path looks like "<Keyboard>/d" or "<Gamepad>/buttonSouth"
        int slash = path.LastIndexOf('/');
        if (slash >= 0 && slash < path.Length - 1)
            return path.Substring(slash + 1);

        return path;
    }


    private static int FindBindingIndexForDevice(InputAction action, InputDeviceType deviceType)
    {
        for (int i = 0; i < action.bindings.Count; i++)
        {
            var b = action.bindings[i];
            if (b.isComposite || b.isPartOfComposite) continue;
            if (string.IsNullOrEmpty(b.effectivePath)) continue;

            if (deviceType == InputDeviceType.Gamepad && b.effectivePath.StartsWith("<Gamepad>"))
                return i;

            if (deviceType == InputDeviceType.KeyboardMouse &&
                (b.effectivePath.StartsWith("<Keyboard>") || b.effectivePath.StartsWith("<Mouse>")))
                return i;
        }

        return -1;
    }
}
