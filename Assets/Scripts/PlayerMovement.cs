using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : NetworkBehaviour
{
    public bool disableMovement = false;
    public string playerId;

    public CharacterController controller;
    public float speed = 12f;
    public float gravity = -9.81f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public float jumpHeight = 3f;
    public GameObject playerCamera;
    public MeshRenderer playerBodyMeshRenderer;

    [SyncVar]
    public Color playerColor;

    Vector3 velocity;
    bool isGrounded;

    void Update()
    {
        if (isLocalPlayer)
        {
            playerCamera.SetActive(true);

            if (Input.GetKey(KeyCode.X))
            {
                SceneManager.LoadScene("WarGame");
            }

            if (!disableMovement)
            {
                HandleMovement();
            }            

            playerBodyMeshRenderer.material.color = playerColor;
        }
    }

    private void HandleMovement()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded)
        {
            if (Input.GetButton("Jump"))
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            if (velocity.y < 0)
            {
                velocity.y = -2f;
            }
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
