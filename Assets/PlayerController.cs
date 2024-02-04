using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float movementSpeed = 5f;

    [SerializeField] float midpoint = 2.0f;
    [SerializeField] float fJumpHeight = 5f;
    [SerializeField] float fJumpDelay = 1.5f;
    [SerializeField] private bool isSprinting = false;
    [SerializeField] private float fRunSpeed = 3.5f;
    [SerializeField] private bool bCanJump = true;

    private float force = 0.0f;
    private CharacterController characterController;
    [SerializeField] Camera cache;
    private float timer = 0.0f;
    private Vector3 moveVector = new Vector3(0, -9.8f, 0);

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (TryGetComponent(out characterController))
        {
            // Initialization code for characterController if needed
        }
    }

    private void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        float speedMultiplier = Input.GetKey(KeyCode.LeftShift) ? fRunSpeed : 1f;

        moveVector = (transform.forward * moveVertical + transform.right * moveHorizontal) * (movementSpeed * speedMultiplier);

        float yRotation = cache.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0, yRotation, 0);

        isSprinting = Input.GetKey(KeyCode.LeftShift);


        if (Input.GetKey(KeyCode.Space) && bCanJump)
        {
            StartCoroutine(Jump(fJumpDelay));
        }

        force -= 9.81f * Time.deltaTime;
        Vector3 jumpVec = new Vector3(0, force, 0);
        characterController.Move((moveVector + jumpVec) * Time.deltaTime);
    }

    IEnumerator Jump(float delay)
    {
        bCanJump = false;
        force = (2 * fJumpHeight / fJumpDelay) - 9.81f * fJumpDelay;
        yield return new WaitForSeconds(delay);
        bCanJump = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, transform.forward);
    }
}
