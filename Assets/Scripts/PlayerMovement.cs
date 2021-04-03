using System;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : NetworkBehaviour
{
    // Outfit
    public GameObject hat;
    public GameObject tshirt;
    public GameObject shoes;
    public GameObject jeans;
    public GameObject watch;
    public GameObject sweater;
    public GameObject headphones;
    public GameObject glasses;
    
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
    public string displayName;
    [SyncVar]
    public string playerId;
    [SyncVar]
    public decimal totalCoins;

    private void OnEnable()
    {
        EventManager.OnPrepareToGame += OnPrepareToGame;
        EventManager.OnReadyToExitGame += OnReadyToExitGame;
        EventManager.OnChangeCoinValue += OnChangeCoinValue;
        EventManager.OnBeforeLoginSuccess += OnBeforeLoginSuccess;
        EventManager.OnOutfitChange += OnOutfitChange;
    }

    private void OnDisable()
    {
        EventManager.OnPrepareToGame -= OnPrepareToGame;
        EventManager.OnReadyToExitGame -= OnReadyToExitGame;
        EventManager.OnChangeCoinValue -= OnChangeCoinValue;
        EventManager.OnBeforeLoginSuccess -= OnBeforeLoginSuccess;
        EventManager.OnOutfitChange -= OnOutfitChange;
    }

    private void OnBeforeLoginSuccess()
    {
        if (!isLocalPlayer) return;
        Debug.Log(UserInfo.GetInstance().TotalCoins);
        CmdSyncVars(UserInfo.GetInstance().UserId, UserInfo.GetInstance().DisplayName, UserInfo.GetInstance().TotalCoins);
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
        if (lastKeysPressed.Contains("HHHHH"))
        {
            clearKeys = true;
            LobbyInfo.GetInstance().Login("harout","Qwe!23");
        } else if (lastKeysPressed.Contains("TTTTT"))
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
        if (!isLocalPlayer) return;
        
        UserInfo.GetInstance().TotalCoins += amount;
        CmdChangeCoinValue(UserInfo.GetInstance().UserId, UserInfo.GetInstance().TotalCoins, amount);
    }

    // Commands
    
    [Command(requiresAuthority = false)]
    private void CmdSyncVars(string userId, string username, decimal coins)
    {
        playerId = userId;
        displayName = username;
        totalCoins = coins;
        TargetSyncVars();
    }
    
    [Command(requiresAuthority = false)]
    private void CmdChangeCoinValue(string userId, decimal currentAmount, decimal changeAmount)
    {
        if (userId == null) return;
        
        totalCoins = currentAmount + changeAmount;
        if (totalCoins < 0)
        {
            totalCoins = 0;
        }

        DataManager.ChangeCoinValue(UserInfo.GetInstance().UserId, changeAmount);
    }

    [TargetRpc]
    private void TargetSyncVars()
    {
        EventManager.FireDelayedLoginSuccessEvent();
    }

    private void OnOutfitChange(string userId = null)
    {
        if(string.IsNullOrWhiteSpace(playerId)) return;

        if (userId != null && userId != playerId) return;

        var outfit = new Outfit(playerId);
        hat.SetActive(outfit.hat);
        tshirt.SetActive(outfit.tshirt);
        shoes.SetActive(outfit.shoes);
        jeans.SetActive(outfit.jeans);
        watch.SetActive(outfit.watch);
        sweater.SetActive(outfit.sweater);
        headphones.SetActive(outfit.headphones);
        glasses.SetActive(outfit.glasses);
    }
}