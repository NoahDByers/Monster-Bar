using UnityEngine;
using UnityEngine.InputSystem;

public partial class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    private Vector2 move;

    private void Update()
    {
        transform.position += (Vector3)(move * (speed * Time.deltaTime));
    }   

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }
}

