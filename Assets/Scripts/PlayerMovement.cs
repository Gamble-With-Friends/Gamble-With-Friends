using System;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerMovement : NetworkBehaviour
{
    // Public vars
    public CharacterController controller;
    public Transform groundCheck;
    public LayerMask groundMask;
    public GameObject playerCamera;
    public bool isMovementDisabled;

    // Private Const
    private const KeyCode USE_KEY = KeyCode.Mouse0;
    private const KeyCode EXIT_GAME_KEY = KeyCode.Escape;
    private const float MAX_DISTANCE = 20f;
    private const float SPEED = 12f;
    private const float GROUND_DISTANCE = 0.4f;
    private const float GRAVITY = -18.81f;
    private const float JUMP_HEIGHT = 3f;

    // Private vars
    private Vector3 velocity;
    private bool isGrounded;
    public TextMesh displayNameTextMesh;
    private int currentGameId = -1;

    // Sync vars
    [SyncVar]
    private string displayName;
    [SyncVar]
    private string playerId;
    [SyncVar]
    private decimal totalCoins;

    private void OnEnable()
    {
        EventManager.OnPrepareToGame += OnPrepareToGame;
        EventManager.OnReadyToExitGame += OnReadyToExitGame;
        EventManager.OnPlayerLogin += OnPlayerLogin;
        EventManager.OnChangeCoinValue += OnChangeCoinValue;
    }

    private void OnDisable()
    {
        EventManager.OnPrepareToGame -= OnPrepareToGame;
        EventManager.OnReadyToExitGame -= OnReadyToExitGame;
        EventManager.OnPlayerLogin -= OnPlayerLogin;
        EventManager.OnChangeCoinValue -= OnChangeCoinValue;
    }

    private void OnPlayerLogin(PlayerModelScript player)
    {
        if (!isLocalPlayer) return;
        CmdSetDisplayName(player.UserName);
        CmdSetUserId(player.PlayerId);
        CmdSetCoins(player.Coins);
    }

    private void OnPrepareToGame(int instanceId)
    {
        if (!isLocalPlayer && IsInGame()) return;

        isMovementDisabled = true;
        currentGameId = instanceId;
        Cursor.lockState = CursorLockMode.Confined;
        playerCamera.SetActive(false);
        EventManager.FireReadyToGameEvent(instanceId);
    }

    private void OnReadyToExitGame(int instanceId)
    {
        if (!isLocalPlayer && !IsInGame()) return;

        isMovementDisabled = false;
        currentGameId = -1;
        Cursor.lockState = CursorLockMode.Locked;
        playerCamera.SetActive(true);
    }
    
    private void Start()
    {
        if (!isLocalPlayer) return;
        displayNameTextMesh.text = "Guest";
    }

    private void Update()
    {
        // Change display name for all players
        displayNameTextMesh.text = displayName;
        
        if (!isLocalPlayer) return;

        SetSyncVars();
        HandleExitGame();
        HandleCamera();
        if (!playerCamera.activeSelf) return;
        HandleRaycasting();
        HandlePlayerMovement();
    }

    private void SetSyncVars()
    {
        UserInfo.GetInstance().UserId = playerId;
        UserInfo.GetInstance().TotalCoins = totalCoins;
        UserInfo.GetInstance().DisplayName = displayName;
    }
    
    private void HandleExitGame()
    {
        if (!isLocalPlayer) return;

        if (IsInGame() && Input.GetKeyUp(EXIT_GAME_KEY))
        {
            EventManager.FirePrepareToExitGameEvent(currentGameId);
        }
    }

    private void HandleCamera()
    {
        if (!isLocalPlayer) return;

        if (!IsInGame() && !playerCamera.activeSelf)
        {
            playerCamera.SetActive(true);
        }
    }

    private void HandlePlayerMovement()
    {
        if (isMovementDisabled) return;
        
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

        var x = Input.GetAxis("Horizontal");
        var z = Input.GetAxis("Vertical");

        var move = transform.right * x + transform.forward * z;
        controller.Move(move * (SPEED * Time.deltaTime));

        velocity.y += GRAVITY * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleRaycasting()
    {
        if (!Input.GetKeyUp(USE_KEY)) return;
        
        Vector2 mouseScreenPosition = Input.mousePosition;

        var ray = playerCamera.GetComponent<Camera>().ScreenPointToRay(mouseScreenPosition);
        Debug.DrawRay(ray.origin, ray.direction * MAX_DISTANCE, Color.green);
        var result = Physics.Raycast(ray, out RaycastHit raycastHit, MAX_DISTANCE);

        if (!result) return;
        EventManager.FireClickEvent(raycastHit.transform.GetInstanceID());
    }

    private bool IsInGame()
    {
        return currentGameId != -1;
    }

    private void OnChangeCoinValue(decimal amount)
    {
        if (isLocalPlayer)
        {
            CmdChangeCoinValue(amount);
        }
    }

    // Commands
    
    [Command(requiresAuthority = false)]
    private void CmdSetDisplayName(string playerDisplayName)
    {
        displayName = playerDisplayName;
    }
    
    [Command(requiresAuthority = false)]
    private void CmdSetUserId(string id)
    {
        playerId = id;
    }
    
    [Command(requiresAuthority = false)]
    private void CmdSetCoins(decimal total)
    {
        totalCoins = total;
    }
    
    [Command(requiresAuthority = false)]
    private void CmdChangeCoinValue(decimal amount)
    {
        var userId = UserInfo.GetInstance().UserId;

        if (userId == null) return;
        
        totalCoins += amount;
        if (totalCoins < 0)
        {
            totalCoins = 0;
        }

        DataManager.ChangeCoinValue(UserInfo.GetInstance().UserId, amount);
    }
}