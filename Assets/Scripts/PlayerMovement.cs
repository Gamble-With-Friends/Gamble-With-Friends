using System;
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
    public Text displayNameText;
    public Text totalCoinsText;

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
        
        var value = "Three|Diamonds";
        var strChucks = value.Split('|');
        var rank = (CardRank) Enum.Parse(typeof(CardRank), strChucks[0]);
        var suit = (CardSuit) Enum.Parse(typeof(CardSuit), strChucks[1]);
        var card = new Card(rank,suit);
        var matt = (Material)Resources.Load("CardMaterials/Black_PlayingCards_" + card.GetImageName() + "_00", typeof(Material));
        
        
        if (!isLocalPlayer) return;
        displayNameTextMesh.text = "Guest";
        // TODO: Comment out before commiting
        var player = DataManager.LoginUser("tony", "Qwe!23");
        EventManager.FirePlayerLoginEvent(player);
    }

    private void Update()
    {
        // Change display name for all players
        displayNameTextMesh.text = displayName;
        
        if (!isLocalPlayer) return;

        displayNameText.text = "Username: " + UserInfo.GetInstance().DisplayName;
        totalCoinsText.text = "Coins: $" + UserInfo.GetInstance().TotalCoins;

        isMovementDisabled = UserInfo.GetInstance().LockMovement;

        SetSyncVars();
        HandleExitGame();
        HandleCamera();
        if (!playerCamera.activeSelf) return;
        HandleRaycasting();
        HandlePlayerMovement();

        if (Input.GetKeyDown(KeyCode.I))
        {
            EventManager.FireKeyDownEvent(KeyCode.I);
        } 
        else if (Input.GetKeyDown(KeyCode.F))
        {
            EventManager.FireKeyDownEvent(KeyCode.F);
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