using UnityEngine;

public class CameraConrtoller : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Camera camera;
    [SerializeField] private InputManager inputManager;

    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float minVerticalRotateClamp = -90f;
    [SerializeField] private float maxVerticalRotateClamp = 90f;
    [SerializeField] private float verticalRotation = 0f;
    [SerializeField] private CameraState state;

    //Need to Refactoring
    private void Start()
    {
        switch (state)
        {
            case CameraState.First:
                break;
            case CameraState.Third:
                break;
        }
    }

    private void Update()
    {
        switch (state)
        {
            case CameraState.First:
                FirstPersonConrtoller();
                break;
            case CameraState.Third:
                ThirdPersonConrtoller();
                break;
        }
    }

    public void ChangeState(CameraState newState)
    {
        if (newState == state) return;

        state = newState;

        switch (state)
        {
            case CameraState.First:
                break;
            case CameraState.Third:
                break;
        }
    }

    private void FirstPersonConrtoller()
    {
        float mouseY = inputManager.GetMouseDeltaY * mouseSensitivity * Time.deltaTime;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, minVerticalRotateClamp, maxVerticalRotateClamp);
        transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    private void ThirdPersonConrtoller()
    {
       //float mouseY = inputManager.GetMouseDeltaY * mouseSensitivity * Time.deltaTime;
    }
}
