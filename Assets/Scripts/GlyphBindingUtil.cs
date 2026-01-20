using UnityEngine;
using UnityEngine.InputSystem;

public static class GlyphBindingUtil
{
    public static int FindBindingIndexForDevice(InputAction action, InputDeviceType deviceType)
    {
        for (int i = 0; i < action.bindings.Count; i++)
        {
            var b = action.bindings[i];

            // skip composites and empty
            if (b.isComposite || b.isPartOfComposite) continue;
            if (string.IsNullOrWhiteSpace(b.effectivePath)) continue;

            if (deviceType == InputDeviceType.Gamepad && b.effectivePath.StartsWith("<Gamepad>"))
                return i;

            if (deviceType == InputDeviceType.KeyboardMouse && b.effectivePath.StartsWith("<Keyboard>") || b.effectivePath.StartsWith("<Mouse>"))
                return i;
        }

        return -1;
    }
}
