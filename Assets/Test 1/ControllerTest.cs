using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerTest : MonoBehaviour
{
    Rigidbody rb;
    float speed = 5f;
    float rotSpeed = 0.5f;
    Vector2 moveDir;
    float turnDir;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        transform.Translate(new Vector3(moveDir.x, 0f, moveDir.y) * Time.fixedDeltaTime * speed);
        if (turnDir != 0f)
        {
            if (turnDir > 0f)
            {
                //turn right
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.right), Time.fixedDeltaTime * rotSpeed);
            }
            else
            {
                //turn left
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(-transform.right), Time.fixedDeltaTime * rotSpeed);
            }
        }
    }

    public void MoveInput(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>();
    }

    public void TurnInput(InputAction.CallbackContext context)
    {
        turnDir = context.ReadValue<float>();
    }
}
