using UnityEngine;

/// <summary>
/// Memicu animasi keyframe (Animation Clip + Animator Controller) lewat parameter.
/// Tombol "Pulse" (atau tombol Space) mengirim Trigger bernama "Pulse" ke Animator.
/// Ini melengkapi CubeController: gerakan interaktif tetap prosedural,
/// sedangkan animasi keyframe dipakai untuk gerakan yang sudah terstruktur.
/// </summary>
[RequireComponent(typeof(Animator))]
public class CubeAnimationTrigger : MonoBehaviour
{
    [SerializeField] private string triggerName = "Pulse";
    [SerializeField] private KeyCode triggerKey = KeyCode.Space;
    [SerializeField] private bool showOnScreenButton = true;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Keyboard hanya tersedia bila Input Manager lama aktif.
#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKeyDown(triggerKey)) TriggerPulse();
#endif
    }

    /// <summary>Memulai animasi Pulse lewat parameter Trigger.</summary>
    public void TriggerPulse()
    {
        if (animator == null || animator.runtimeAnimatorController == null)
        {
            Debug.LogWarning("CubeAnimationTrigger: Animator / Controller belum di-setup. " +
                             "Jalankan menu Tools > Grafika Komputer > Setup Cube Animation.");
            return;
        }

        // CubeController mematikan Animator saat Scale manual.
        // Nyalakan lagi supaya animasi keyframe bisa diputar.
        if (!animator.enabled) animator.enabled = true;

        animator.SetTrigger(triggerName);
    }

    private void OnGUI()
    {
        if (!showOnScreenButton) return;

        const float panelWidth = 240f;
        Rect panel = new Rect(Screen.width - panelWidth - 10f, 10f, panelWidth, 96f);

        GUI.Box(panel, "Keyframe Animation");

        string buttonLabel = "Play Pulse (" + triggerKey + ")";
        if (GUI.Button(new Rect(panel.x + 12f, panel.y + 28f, panel.width - 24f, 30f), buttonLabel))
        {
            TriggerPulse();
        }

        bool ready = animator != null && animator.runtimeAnimatorController != null;
        GUI.Label(new Rect(panel.x + 12f, panel.y + 62f, panel.width - 24f, 24f),
                  ready ? "Trigger: " + triggerName : "Animator belum di-setup");
    }
}
