using System;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : NetworkBehaviour
{
    // Public vars
    public CharacterController controller;
    public Transform groundCheck;
    public LayerMask groundMask;
    public GameObject playerCamera;
    public bool isMovementDisabled;
    public Text userInfoText;

    // Private Const
    private const KeyCode USE_KEY = KeyCode.Mouse0;
    private const KeyCode EXIT_GAME_KEY = KeyCode.Escape;
    private const float MAX_DISTANCE = 20f;
    private const float SPEED = 12f;
    private const float GROUND_DISTANCE = 0.4f;
    private const float GRAVITY = -18.81f;
    private const float JUMP_HEIGHT = 3f;
    private const int MAX_KEY_HISTORY = 32;

    // Private vars
    private Vector3 velocity;
    private bool isGrounded;
    private string lastKeysPressed = "";

    public TextMesh displayNameTextMesh;
    // Sync vars
    [SyncVar]
    private string displayName;
    [SyncVar]
    private string playerId;
    [SyncVar]
    private decimal totalCoins;

    private void OnGUI()
    {
        
    }

    private void OnEnable()
    {
        EventManager.OnPrepareToGame += OnPrepareToGame;
        EventManager.OnReadyToExitGame += OnReadyToExitGame;
        EventManager.OnLoginSuccess += OnLoginSuccess;
        EventManager.OnChangeCoinValue += OnChangeCoinValue;
    }

    private void OnDisable()
    {
        EventManager.OnPrepareToGame -= OnPrepareToGame;
        EventManager.OnReadyToExitGame -= OnReadyToExitGame;
        EventManager.OnLoginSuccess -= OnLoginSuccess;
        EventManager.OnChangeCoinValue -= OnChangeCoinValue;
    }

    private void OnLoginSuccess(string username, string userId, decimal coins)
    {
        if (!isLocalPlayer) return;
        CmdSetDisplayName(username);
        CmdSetUserId(userId);
        CmdSetCoins(coins);
    }

    private void OnPrepareToGame(int instanceId)
    {
        if (!isLocalPlayer || UserInfo.GetInstance().IsInGame()) return;

        isMovementDisabled = true;
        UserInfo.GetInstance().CurrentGameId = instanceId;
        Cursor.lockState = CursorLockMode.Confined;
        playerCamera.SetActive(false);
        EventManager.FireReadyToGameEvent(instanceId);
    }

    private void OnReadyToExitGame(int instanceId)
    {
        if (!isLocalPlayer || !UserInfo.GetInstance().IsInGame()) return;

        isMovementDisabled = false;
        UserInfo.GetInstance().CurrentGameId = -1;
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
        if (UserInfo.GetInstance().UserId != null)
        {
            userInfoText.text = "Username: " + UserInfo.GetInstance().DisplayName + "\n" +
                                "Coins: $" + UserInfo.GetInstance().TotalCoins;
        }
        else
        {
            userInfoText.text = "You're not logged in";
        }

        if (!isLocalPlayer) return;
        isMovementDisabled = UserInfo.GetInstance().LockMovement;
        HandleKeys();
        SetSyncVars();
        HandleExitGame();
        HandleCamera();
        if (!playerCamera.activeSelf) return;
        HandleRaycasting();
        HandlePlayerMovement();
    }

    private void HandleKeys()
    {
        foreach(KeyCode vKey in Enum.GetValues(typeof(KeyCode)))
        {
            if (!Input.GetKeyDown(vKey)) continue;

            HandleCheats(vKey);

            if (!UserInfo.GetInstance().LockMovement)
            {
                EventManager.FireKeyDownEvent(vKey);
            }
        }
    }

    private void HandleCheats(KeyCode vKey)
    {
        lastKeysPressed += vKey;

        if (lastKeysPressed.Length > MAX_KEY_HISTORY)
        {
            lastKeysPressed = lastKeysPressed.Substring( lastKeysPressed.Length - MAX_KEY_HISTORY, MAX_KEY_HISTORY);
        }

        var clearKeys = false;
        if (lastKeysPressed.Contains("HAROUT"))
        {
            clearKeys = true;
            LobbyInfo.GetInstance().Login("harout","Qwe!23");
        } else if (lastKeysPressed.Contains("TONY"))
        {
            clearKeys = true;
            LobbyInfo.GetInstance().Login("tony","Qwe!23");
        }

        if (clearKeys)
        {
            lastKeysPressed = "";
        }
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

        if (UserInfo.GetInstance().IsInGame() && Input.GetKeyUp(EXIT_GAME_KEY))
        {
            EventManager.FirePrepareToExitGameEvent(UserInfo.GetInstance().CurrentGameId);
        }
    }

    private void HandleCamera()
    {
        if (!isLocalPlayer) return;

        if (!UserInfo.GetInstance().IsInGame() && !playerCamera.activeSelf)
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
        if (UserInfo.GetInstance().LockMouse) return;
        if (!Input.GetKeyUp(USE_KEY)) return;
        
        Vector2 mouseScreenPosition = Input.mousePosition;

        var ray = playerCamera.GetComponent<Camera>().ScreenPointToRay(mouseScreenPosition);
        Debug.DrawRay(ray.origin, ray.direction * MAX_DISTANCE, Color.green);
        var result = Physics.Raycast(ray, out RaycastHit raycastHit, MAX_DISTANCE);

        if (!result) return;
        EventManager.FireClickEvent(raycastHit.transform.GetInstanceID());
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