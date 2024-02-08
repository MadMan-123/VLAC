using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float movementSpeed = 5f;

    [SerializeField] float midpoint = 2.0f;
    [SerializeField] float jumpHeight = 5f;
    [SerializeField] float jumpDelay = 1.5f;
    [SerializeField] private bool isSprinting = false;
    [SerializeField] private float runSpeed = 3.5f;
    [SerializeField] private bool canJump = true;

    private float force = 0.0f;
    private CharacterController characterController;
    [SerializeField] Camera cache;
    private Vector3 moveVector = new Vector3(0, -9.8f, 0);
    private float moveHorizontal;
    private float moveVertical;
    private float speedMultiplier;
    private float yRotation;
    private Vector3 jumpVec;
    private Transform moveTransform;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (TryGetComponent<CharacterController>(out characterController))
        {
        }
    }

    private void Update()
    {
        //get the axis input
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");
        
        //see if the player should run
        speedMultiplier = Input.GetKey(KeyCode.LeftShift) ? runSpeed : 1f;

        //get the transform of the player
        moveTransform = transform;
        //calculate the move vector
        moveVector = (moveTransform.forward * moveVertical + moveTransform.right * moveHorizontal) * (movementSpeed * speedMultiplier);
    
        //sync the rotation of the camera with the player
        yRotation = cache.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0, yRotation, 0);
        

        force -= 9.81f * Time.deltaTime;
        jumpVec = new Vector3(0, force, 0);
        characterController.Move((moveVector + jumpVec) * Time.deltaTime);
    }

    public void Jump()
    {
        if (canJump)
        {
            StartCoroutine(StartJump(jumpDelay));
        }
    }
    
    private IEnumerator StartJump(float delay)
    {
        canJump = false;
        
        force = (2 * jumpHeight / delay) - 9.81f * delay;
        
        yield return new WaitForSeconds(delay);
        
        canJump = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, transform.forward);
    }
}
