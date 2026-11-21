using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// Animasi prosedural (logic-driven, ditulis dalam kode) untuk kubus.
/// - Rotasi  : tombol di layar, tombol panah / WASD, atau drag mouse pada kubus.
/// - Translate & Scale : tombol di layar mengubah Transform lewat kode.
/// Semua perubahan per frame memakai Time.deltaTime agar tidak bergantung frame rate.
///
/// Input mendukung Input Manager lama maupun Input System baru.
/// </summary>
public class CubeController : MonoBehaviour
{
    [Header("Rotasi")]
    [SerializeField] private float rotationSpeed = 90f;   // derajat per detik
    [SerializeField] private float dragSensitivity = 4f;  // derajat per pixel mouse

    [Header("Translate")]
    [SerializeField] private float translateStep = 0.5f;  // jarak per klik
    [SerializeField] private float translateRange = 2.5f; // batas kiri/kanan dari posisi awal

    [Header("Scale")]
    [SerializeField] private float scaleStep = 0.25f;
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 3f;

    // Area panel tombol (koordinat GUI: 0,0 = kiri atas)
    private static readonly Rect PanelRect = new Rect(10f, 10f, 240f, 250f);

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector3 startScale;

    private float translateDirection = 1f;
    private bool draggingCube;

    // Status tombol rotasi yang sedang ditahan (dipakai di Update)
    private bool rotateLeftHeld;
    private bool rotateRightHeld;

    // Animator (animasi keyframe) juga menulis localScale; perlu dimatikan saat scale manual.
    private Animator cubeAnimator;

    private void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        startScale = transform.localScale;
        cubeAnimator = GetComponent<Animator>();

        // Membantu memastikan backend input mana yang dipakai saat kompilasi.
#if ENABLE_LEGACY_INPUT_MANAGER
        Debug.Log("CubeController: input = Input Manager lama.", this);
#elif ENABLE_INPUT_SYSTEM
        Debug.Log("CubeController: input = Input System (baru).", this);
#else
        Debug.LogWarning("CubeController: tidak ada input backend aktif. " +
                         "Set Player Settings > Active Input Handling ke 'Both'.", this);
#endif
    }

    private void Update()
    {
        // Tombol rotasi ditahan -> putar setiap frame.
        if (rotateLeftHeld) RotateBy(-rotationSpeed * Time.deltaTime, 0f);
        if (rotateRightHeld) RotateBy(rotationSpeed * Time.deltaTime, 0f);

        HandleKeyboardRotation();
        HandleMouseDrag();
    }

    // ------------------------------------------------------------------ rotasi

    /// <summary>Memutar kubus. yaw = sumbu Y, pitch = sumbu X (dalam derajat).</summary>
    public void RotateBy(float yaw, float pitch)
    {
        transform.Rotate(Vector3.up, yaw, Space.World);
        transform.Rotate(Vector3.right, pitch, Space.Self);
    }

    private void HandleKeyboardRotation()
    {
        float yaw = 0f;
        float pitch = 0f;

#if ENABLE_LEGACY_INPUT_MANAGER
        // Input Manager lama.
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) yaw -= 1f;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) yaw += 1f;
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) pitch -= 1f;
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) pitch += 1f;
#elif ENABLE_INPUT_SYSTEM
        // Input System baru (dipakai bila Active Input Handling = Input System Package).
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.leftArrowKey.isPressed || keyboard.aKey.isPressed) yaw -= 1f;
            if (keyboard.rightArrowKey.isPressed || keyboard.dKey.isPressed) yaw += 1f;
            if (keyboard.upArrowKey.isPressed || keyboard.wKey.isPressed) pitch -= 1f;
            if (keyboard.downArrowKey.isPressed || keyboard.sKey.isPressed) pitch += 1f;
        }
#endif

        if (yaw != 0f || pitch != 0f)
        {
            RotateBy(yaw * rotationSpeed * Time.deltaTime,
                     pitch * rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleMouseDrag()
    {
        Vector2 pointer;
        Vector2 delta;
        bool pressed;
        bool held;

#if ENABLE_LEGACY_INPUT_MANAGER
        pointer = Input.mousePosition;
        pressed = Input.GetMouseButtonDown(0);
        held = Input.GetMouseButton(0);
        delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * dragSensitivity;
#elif ENABLE_INPUT_SYSTEM
        Mouse mouse = Mouse.current;
        pointer = mouse != null ? mouse.position.ReadValue() : Vector2.zero;
        pressed = mouse != null && mouse.leftButton.wasPressedThisFrame;
        held = mouse != null && mouse.leftButton.isPressed;

        // Delta Input System dalam pixel; faktor 0.1 menyamakan rasanya dengan Input Manager.
        const float pixelToAxis = 0.1f;
        delta = (mouse != null ? mouse.delta.ReadValue() : Vector2.zero) *
                (dragSensitivity * pixelToAxis);
#else
        pointer = Vector2.zero;
        delta = Vector2.zero;
        pressed = false;
        held = false;
#endif

        // Mulai drag hanya jika klik mengenai kubus (butuh Collider, Cube default punya BoxCollider).
        if (pressed && !IsPointerOverPanel(pointer))
        {
            draggingCube = RaycastHitsCube(pointer);
        }

        if (!held) draggingCube = false;

        if (draggingCube && !IsPointerOverPanel(pointer))
        {
            RotateBy(delta.x, delta.y);
        }
    }

    private bool RaycastHitsCube(Vector2 screenPosition)
    {
        if (Camera.main == null) return false;

        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        return Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform;
    }

    private bool IsPointerOverPanel(Vector2 screenPosition)
    {
        // Input.mousePosition berawal dari kiri bawah, GUI dari kiri atas.
        Vector2 guiMouse = new Vector2(screenPosition.x,
                                       Screen.height - screenPosition.y);
        return PanelRect.Contains(guiMouse);
    }

    // --------------------------------------------------------------- translate

    /// <summary>Menggeser kubus satu langkah ke kanan/kiri, memantul di batas.</summary>
    public void TranslateCube()
    {
        transform.position += Vector3.right * (translateStep * translateDirection);

        if (Mathf.Abs(transform.position.x - startPosition.x) >= translateRange)
        {
            translateDirection *= -1f;
        }
    }

    // ------------------------------------------------------------------- scale

    public void ScaleUp() => ScaleBy(scaleStep);
    public void ScaleDown() => ScaleBy(-scaleStep);

    private void ScaleBy(float amount)
    {
        // Animasi keyframe (Pulse) menulis localScale setiap frame saat aktif,
        // sehingga scale manual akan ditimpa. Matikan dulu Animator-nya;
        // CubeAnimationTrigger akan menyalakannya lagi saat Pulse dipicu.
        if (cubeAnimator != null && cubeAnimator.enabled)
        {
            cubeAnimator.enabled = false;
        }

        // Skala dibuat uniform (x = y = z).
        float next = Mathf.Clamp(transform.localScale.x + amount, minScale, maxScale);
        transform.localScale = Vector3.one * next;
    }

    // ------------------------------------------------------------------- reset

    public void ResetCube()
    {
        transform.SetPositionAndRotation(startPosition, startRotation);
        transform.localScale = startScale;
        translateDirection = 1f;
        draggingCube = false;
    }

    // --------------------------------------------------------------------- GUI

    private void OnGUI()
    {
        GUI.Box(PanelRect, "Cube Controls");

        float x = PanelRect.x + 12f;
        float y = PanelRect.y + 28f;
        float w = PanelRect.width - 24f;
        float h = 28f;
        float gap = 6f;

        // RepeatButton = tetap aktif selama ditahan. Statusnya dibaca di Update.
        rotateLeftHeld  = GUI.RepeatButton(new Rect(x, y, w, h), "< Rotate (hold)");
        y += h + gap;

        rotateRightHeld = GUI.RepeatButton(new Rect(x, y, w, h), "Rotate (hold) >");
        y += h + gap;

        if (GUI.Button(new Rect(x, y, w, h), "Translate")) TranslateCube();
        y += h + gap;

        if (GUI.Button(new Rect(x, y, w, h), "Scale Up")) ScaleUp();
        y += h + gap;

        if (GUI.Button(new Rect(x, y, w, h), "Scale Down")) ScaleDown();
        y += h + gap;

        if (GUI.Button(new Rect(x, y, w, h), "Reset")) ResetCube();

        GUI.Label(new Rect(PanelRect.x, PanelRect.yMax + 4f, 460f, 22f),
                  "Scale: " + transform.localScale.x.ToString("0.00"));

        GUI.Label(new Rect(PanelRect.x, PanelRect.yMax + 22f, 460f, 40f),
                  "Arrow keys / WASD or drag the cube to rotate.");
    }
}
