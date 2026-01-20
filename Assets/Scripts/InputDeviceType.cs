using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InputDeviceType { KeyboardMouse, Gamepad }

public class InputDeviceTracker : MonoBehaviour
{
    public static InputDeviceTracker Instance { get; private set; }

    public InputDeviceType Current { get; private set; } = InputDeviceType.KeyboardMouse;

    public event Action<InputDeviceType> OnDeviceChanged;

    private IDisposable _subscription;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        // Subscribe to any button press globally
        _subscription = InputSystem.onAnyButtonPress.Subscribe(new AnyButtonObserver(this));
    }

    private void OnDisable()
    {
        // Unsubscribe safely
        _subscription?.Dispose();
        _subscription = null;
    }

    private void SetDeviceFromControl(InputControl control)
    {
        var next = (control.device is Gamepad) ? InputDeviceType.Gamepad : InputDeviceType.KeyboardMouse;
        if (next == Current) return;

        Current = next;
        OnDeviceChanged?.Invoke(Current);
    }

    private sealed class AnyButtonObserver : IObserver<InputControl>
    {
        private readonly InputDeviceTracker _tracker;
        public AnyButtonObserver(InputDeviceTracker tracker) => _tracker = tracker;

        public void OnNext(InputControl value) => _tracker.SetDeviceFromControl(value);
        public void OnError(Exception error) { }
        public void OnCompleted() { }
    }
}
