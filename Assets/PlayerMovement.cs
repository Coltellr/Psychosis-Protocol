using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Réglages de vitesse")]
    public float walkSpeed = 3f;
    public float runSpeed = 5f;
    public float crouchSpeed = 2f;
    public float jumpPower = 7f;
    public float gravity = 10f;

    [Header("Reglages de vue")]
    public Camera playerCamera;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    [Header("Crouch")]
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;

    [Header("Mouvement lisse")]
    public float acceleration = 10f; // vitesse à laquelle le joueur atteint la vitesse voulue

    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero; // vitesse actuelle du joueur
    private float rotationX = 0;

    private Vector3 cameraStartPos; // position de la caméra pour le head bob
    private float bobTimer = 0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Verrouiller la souris au centre
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Stocker la position initiale de la caméra
        cameraStartPos = playerCamera.transform.localPosition;
    }

    void Update()
    {
        // 1️⃣ Lire les touches
        float inputZ = Input.GetAxis("Vertical");   // W/S ou flèches
        float inputX = Input.GetAxis("Horizontal"); // A/D ou flèches
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        bool isCrouching = Input.GetKey(KeyCode.R);

        // 2️⃣ Déterminer la vitesse cible
        float speed = isCrouching ? crouchSpeed : (isRunning ? runSpeed : walkSpeed);

        // 3️⃣ Calculer la direction cible
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        Vector3 targetDirection = (forward * inputZ + right * inputX) * speed;

        // 4️⃣ Lissage du mouvement (pour éviter l’effet “glisse”)
        float verticalSpeed = moveDirection.y; // garder la vitesse verticale
        moveDirection = Vector3.Lerp(moveDirection, targetDirection, acceleration * Time.deltaTime);
        moveDirection.y = verticalSpeed;

        // 5️⃣ Saut
        if (Input.GetButton("Jump") && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }

        // 6️⃣ Gravité
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // 7️⃣ Appliquer le mouvement
        characterController.Move(moveDirection * Time.deltaTime);

        // 8️⃣ Crouch
        characterController.height = isCrouching ? crouchHeight : defaultHeight;

        // 9️⃣ Rotation de la caméra
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.Rotate(0, Input.GetAxis("Mouse X") * lookSpeed, 0);

        // 10️⃣ Head bob simple
        HandleHeadBobbing(inputZ, inputX);
    }

    void HandleHeadBobbing(float x, float y)
    {
        if (Mathf.Abs(x) > 0.1f || Mathf.Abs(y) > 0.1f)
        {
            bobTimer += Time.deltaTime * 8f; // fréquence
            float bobOffset = Mathf.Sin(bobTimer) * 0.05f; // amplitude
            Vector3 newPos = cameraStartPos;
            newPos.y += bobOffset;
            playerCamera.transform.localPosition = newPos;
        }
        else
        {
            bobTimer = 0;
            playerCamera.transform.localPosition = Vector3.Lerp(
                playerCamera.transform.localPosition,
                cameraStartPos,
                Time.deltaTime * 5f);
        }
    }
}
