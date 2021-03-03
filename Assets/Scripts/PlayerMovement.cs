using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerMovement : NetworkBehaviour
{
    readonly static KeyCode USE_KEY = KeyCode.Mouse0;
    readonly static float MAX_DISTANCE = 20f;
    readonly static float SPEED = 12f;
    readonly static float GROUND_DISTANCE = 0.4f;
    readonly static float GRAVITY = -9.81f;
    readonly static float JUMP_HEIGHT = 3f;

    public CharacterController controller;
    public Transform groundCheck;
    public LayerMask groundMask;
    public Camera playerCamera;
    public MeshRenderer playerBodyMeshRenderer;

    [SyncVar]
    public Color playerColor;

    Vector3 velocity;
    bool isGrounded;


    void OnEnable()
    {
        EventManager.onGameStart += onGameStart;
    }

    void OnDisable()
    {
        EventManager.onGameStart -= onGameStart;
    }

    void onGameStart(string gameId)
    {
        if(isLocalPlayer)
        {
            playerCamera.enabled = false;
        }
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            if(!playerCamera.enabled)
            {
                playerCamera.enabled = true;
            }

            if (Input.GetKeyUp(USE_KEY)) {
                RaycastSingle();
            }

            isGrounded = Physics.CheckSphere(groundCheck.position, GROUND_DISTANCE, groundMask);

            if (isGrounded)
            {
                if (Input.GetButton("Jump"))
                {
                    velocity.y = Mathf.Sqrt(JUMP_HEIGHT * -2f * GRAVITY);
                }

                if (velocity.y < 0)
                {
                    velocity.y = -2f;
                }
            }

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;
            controller.Move(move * SPEED * Time.deltaTime);

            velocity.y += GRAVITY * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);

            playerBodyMeshRenderer.material.color = playerColor;
        }
    }

    private void RaycastSingle()
    {
        Vector2 mouseScreenPosition = Input.mousePosition;

        Ray ray = playerCamera.ScreenPointToRay(mouseScreenPosition);
        Debug.DrawRay(ray.origin, ray.direction * MAX_DISTANCE, Color.green);
        bool result = Physics.Raycast(ray, out RaycastHit raycastHit, MAX_DISTANCE);

        Debug.Log(raycastHit.collider.name);
        
        if(result)
        {
            EventManager.FireClickEvent(raycastHit.collider.name);
        }
    }
}