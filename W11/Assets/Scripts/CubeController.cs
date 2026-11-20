using UnityEngine;

/// <summary>
/// Animasi prosedural (logic-driven, ditulis dalam kode) untuk kubus.
/// - Rotasi  : tombol di layar, tombol panah / WASD, atau drag mouse pada kubus.
/// - Translate & Scale : tombol di layar mengubah Transform lewat kode.
/// Semua perubahan per frame memakai Time.deltaTime agar tidak bergantung frame rate.
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

    private void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        startScale = transform.localScale;
    }

    private void Update()
    {
        // Tombol rotasi ditahan -> putar setiap frame.
        if (rotateLeftHeld) RotateBy(-rotationSpeed * Time.deltaTime, 0f);
        if (rotateRightHeld) RotateBy(rotationSpeed * Time.deltaTime, 0f);

        // Keyboard & mouse hanya tersedia bila Input Manager lama aktif.
#if ENABLE_LEGACY_INPUT_MANAGER
        HandleKeyboardRotation();
        HandleMouseDrag();
#endif
    }

    // ------------------------------------------------------------------ rotasi

    /// <summary>Memutar kubus. yaw = sumbu Y, pitch = sumbu X (dalam derajat).</summary>
    public void RotateBy(float yaw, float pitch)
    {
        transform.Rotate(Vector3.up, yaw, Space.World);
        transform.Rotate(Vector3.right, pitch, Space.Self);
    }

#if ENABLE_LEGACY_INPUT_MANAGER
    private void HandleKeyboardRotation()
    {
        float yaw = 0f;
        float pitch = 0f;

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) yaw -= 1f;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) yaw += 1f;
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) pitch -= 1f;
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) pitch += 1f;

        if (yaw != 0f || pitch != 0f)
        {
            RotateBy(yaw * rotationSpeed * Time.deltaTime,
                     pitch * rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleMouseDrag()
    {
        // Mulai drag hanya jika klik mengenai kubus (butuh Collider, Cube default punya BoxCollider).
        if (Input.GetMouseButtonDown(0) && !IsPointerOverPanel())
        {
            draggingCube = RaycastHitsCube();
        }

        if (Input.GetMouseButtonUp(0))
        {
            draggingCube = false;
        }

        if (draggingCube && !IsPointerOverPanel())
        {
            RotateBy(Input.GetAxis("Mouse X") * dragSensitivity,
                     Input.GetAxis("Mouse Y") * dragSensitivity);
        }
    }

    private bool RaycastHitsCube()
    {
        if (Camera.main == null) return false;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform;
    }

    private bool IsPointerOverPanel()
    {
        // Input.mousePosition berawal dari kiri bawah, GUI dari kiri atas.
        Vector2 guiMouse = new Vector2(Input.mousePosition.x,
                                       Screen.height - Input.mousePosition.y);
        return PanelRect.Contains(guiMouse);
    }
#endif

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

        GUI.Label(new Rect(PanelRect.x, PanelRect.yMax + 4f, 460f, 40f),
                  "Arrow keys / WASD or drag the cube to rotate.");
    }
}
