using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerMovement : NetworkBehaviour
{
    readonly static KeyCode USE_KEY = KeyCode.Mouse0;
    readonly static KeyCode EXIT_GAME_KEY = KeyCode.Escape;
    readonly static float MAX_DISTANCE = 20f;
    readonly static float SPEED = 12f;
    readonly static float GROUND_DISTANCE = 0.4f;
    readonly static float GRAVITY = -9.81f;
    readonly static float JUMP_HEIGHT = 3f;

    public CharacterController controller;
    public Transform groundCheck;
    public LayerMask groundMask;
    public GameObject playerCamera;
    public MeshRenderer playerBodyMeshRenderer;
    public bool isMovementDisabled;

    [SyncVar]
    public Color playerColor;

    private Vector3 velocity;
    private bool isGrounded;
    public PlayerModelScript player;
    public TextMesh displayNameTextMesh;
    
    int currentGameId = -1;

    void OnEnable()
    {
        EventManager.OnPrepareToGame += OnPrepareToGame;
        EventManager.OnReadyToExitGame += OnReadyToExitGame;
        EventManager.OnPlayerLogin += OnPlayerLogin;
    }

    void OnDisable()
    {
        EventManager.OnPrepareToGame -= OnPrepareToGame;
        EventManager.OnReadyToExitGame -= OnReadyToExitGame;
        EventManager.OnPlayerLogin -= OnPlayerLogin;
    }

    void OnPlayerLogin(PlayerModelScript player)
    {
        this.player = player;
        displayNameTextMesh.text = player.UserName;
    }

    void OnPrepareToGame(int intanceId)
    {
        if (isLocalPlayer && !IsInGame())
        {
            isMovementDisabled = true;
            currentGameId = intanceId;
            Cursor.lockState = CursorLockMode.Confined;
            playerCamera.SetActive(false);
            EventManager.FireReadyToGameEvent(intanceId);
        }
    }

    void OnReadyToExitGame(int intanceId)
    {
        if (isLocalPlayer && IsInGame())
        {
            isMovementDisabled = false;
            currentGameId = -1;
            Cursor.lockState = CursorLockMode.Locked;
            playerCamera.SetActive(true);
        }
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            HandleExitGame();
            HandleCamera();
            if(playerCamera.activeSelf)
            {
                HandleRaycasting();
                HandlePlayerMovement();
            }
        }
    }

    void HandleExitGame()
    {
        if (isLocalPlayer && IsInGame() && Input.GetKeyUp(EXIT_GAME_KEY))
        {
            EventManager.FirePrepareToExitGameEvent(currentGameId);
        }
    }

    void HandleCamera()
    {
        if (!IsInGame() && !playerCamera.activeSelf)
        {
            playerCamera.SetActive(true);
        }
    }

    void HandlePlayerMovement()
    {
        if (!isMovementDisabled)
        {
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
        }
    }

    void HandleRaycasting()
    {
        if (Input.GetKeyUp(USE_KEY))
        {
            Vector2 mouseScreenPosition = Input.mousePosition;

            Ray ray = playerCamera.GetComponent<Camera>().ScreenPointToRay(mouseScreenPosition);
            Debug.DrawRay(ray.origin, ray.direction * MAX_DISTANCE, Color.green);
            bool result = Physics.Raycast(ray, out RaycastHit raycastHit, MAX_DISTANCE);

            if (result)
            {
                EventManager.FireClickEvent(raycastHit.transform.GetInstanceID());
                Debug.Log(raycastHit.collider.name);
            }
        }
    }

    bool IsInGame()
    {
        return currentGameId != -1;
    }
}