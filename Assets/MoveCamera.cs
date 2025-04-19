using Unity.Netcode;
using UnityEngine;

public class MouseLook : NetworkBehaviour
{
    [Header("Camera Settings")]
    public Transform orientation;
    public Transform cameraFollowTarget;

    public float sensitivityX = 100f;
    public float sensitivityY = 100f;
    public float maxPitch = 80f;
    public float minPitch = -80f;

    private float xRotation;
    private float yRotation;

    private void Start()
    {
        if (!IsOwner)
        {
            gameObject.SetActive(false); // disables camera + script for non-owners 🎥❌
            return;
        }

        LockCursor(true);
    }

    void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.LeftAlt)) LockCursor(false);
        if (Input.GetKeyUp(KeyCode.LeftAlt)) LockCursor(true);

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            float mouseX = Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime;

            yRotation += mouseX;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, minPitch, maxPitch);
        }

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);

        if (orientation != null)
            orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);

        if (cameraFollowTarget != null)
            transform.position = cameraFollowTarget.position;
    }

    void LockCursor(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}