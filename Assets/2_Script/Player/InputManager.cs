using UnityEngine;

public class InputManager : MonoBehaviour
{
    private float mouseDeltaX;
    private float mouseDeltaY;
    private float moveHorizontal;
    private float moveVertical;
    private bool space;
    private bool IsE;
    public float GetMouseDeltaX => mouseDeltaX;
    public float GetMouseDeltaY => mouseDeltaY;
    public float GetMoveHorizontal => moveHorizontal;
    public float GetMoveVertical => moveVertical;
    public bool GetSpace => space;
    public bool GetIsE => IsE;

    void Update()
    {
        mouseDeltaX = Input.GetAxis("Mouse X");

        mouseDeltaY = Input.GetAxis("Mouse Y");

        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");

        space = Input.GetButtonDown("Jump");

        IsE = Input.GetKey(KeyCode.E);
    }
}
