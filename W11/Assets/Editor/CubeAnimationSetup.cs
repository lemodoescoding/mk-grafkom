using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

/// <summary>
/// Editor tool yang mengotomatiskan langkah-langkah pada materi PDF:
///   - Buat Animation Clip baru (dengan keyframe + easing)
///   - Buat Animator Controller (state Idle & Pulse + transisi)
///   - Tambahkan komponen Animator ke objek terpilih
///   - Tambahkan CubeAnimationTrigger untuk memicu animasi lewat parameter/skrip
///
/// Cara pakai: pilih Cube di Hierarchy, lalu
/// Tools > Grafika Komputer > Setup Cube Animation.
/// </summary>
public static class CubeAnimationSetup
{
    private const string AnimationFolder = "Assets/Animations";
    private const string ClipPath = AnimationFolder + "/CubePulse.anim";
    private const string ControllerPath = AnimationFolder + "/CubeAnimator.controller";
    private const string TriggerParameter = "Pulse";

    [MenuItem("Tools/Grafika Komputer/Setup Cube Animation")]
    public static void Setup()
    {
        if (EditorApplication.isPlaying)
        {
            EditorUtility.DisplayDialog(
                "Setup Cube Animation",
                "Keluar dari Play mode terlebih dahulu, lalu jalankan menu ini lagi.",
                "OK");
            return;
        }

        GameObject target = Selection.activeGameObject != null
            ? Selection.activeGameObject
            : GameObject.Find("Cube");

        if (target == null)
        {
            EditorUtility.DisplayDialog(
                "Setup Cube Animation",
                "Pilih Cube di Hierarchy (atau buat GameObject bernama 'Cube') terlebih dahulu.",
                "OK");
            return;
        }

        EnsureFolder();

        // Hapus aset lama agar menu bisa dijalankan ulang tanpa duplikasi.
        DeleteAssetIfExists(ClipPath);
        DeleteAssetIfExists(ControllerPath);

        // 1) Animation Clip baru: keyframe skala 1 -> 1.25 -> 1 dengan easing.
        AnimationClip pulseClip = CreatePulseClip();
        AssetDatabase.CreateAsset(pulseClip, ClipPath);

        // 2) Animator Controller: state Idle & Pulse, transisi, parameter Trigger.
        AnimatorController controller = CreateController(pulseClip);

        // 3) Komponen Animator pada objek, lengkap dengan controller.
        Animator animator = target.GetComponent<Animator>();
        if (animator == null) animator = Undo.AddComponent<Animator>(target);

        Undo.RecordObject(animator, "Assign Animator Controller");
        animator.runtimeAnimatorController = controller;

        // 4) Skrip pemicu animasi lewat parameter.
        if (target.GetComponent<CubeAnimationTrigger>() == null)
            Undo.AddComponent<CubeAnimationTrigger>(target);

        EditorUtility.SetDirty(animator);
        EditorUtility.SetDirty(target);
        AssetDatabase.SaveAssets();

        Selection.activeObject = controller;
        Debug.Log("Setup Cube Animation selesai untuk '" + target.name + "'. Tekan Play lalu klik " +
                  "'Play Pulse' atau tekan Space.", target);
    }

    private static void EnsureFolder()
    {
        if (!AssetDatabase.IsValidFolder(AnimationFolder))
            AssetDatabase.CreateFolder("Assets", "Animations");
    }

    private static void DeleteAssetIfExists(string path)
    {
        if (AssetDatabase.LoadAssetAtPath<Object>(path) != null)
            AssetDatabase.DeleteAsset(path);
    }

    private static AnimationClip CreatePulseClip()
    {
        AnimationClip clip = new AnimationClip
        {
            name = "CubePulse",
            frameRate = 60f
        };

        // Easing: kurva halus dari 1.0 ke 1.25 (0.5 detik), lalu kembali ke 1.0 (1 detik).
        AnimationCurve scale = AnimationCurve.EaseInOut(0f, 1f, 0.5f, 1.25f);
        scale.AddKey(1f, 1f);

        // Samakan tangent semua keyframe supaya transisinya mulus (Ease In/Out).
        for (int i = 0; i < scale.length; i++)
        {
            AnimationUtility.SetKeyLeftTangentMode(scale, i, AnimationUtility.TangentMode.ClampedAuto);
            AnimationUtility.SetKeyRightTangentMode(scale, i, AnimationUtility.TangentMode.ClampedAuto);
        }

        // Animasi skala uniform pada objek yang memiliki Animator (path kosong).
        clip.SetCurve("", typeof(Transform), "localScale.x", scale);
        clip.SetCurve("", typeof(Transform), "localScale.y", scale);
        clip.SetCurve("", typeof(Transform), "localScale.z", scale);

        AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = false; // animasi sekali jalan, lalu kembali ke Idle
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        return clip;
    }

    private static AnimatorController CreateController(AnimationClip pulseClip)
    {
        AnimatorController controller =
            AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);

        // Parameter yang dipicu dari skrip: animator.SetTrigger("Pulse").
        controller.AddParameter(TriggerParameter, AnimatorControllerParameterType.Trigger);

        AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;

        AnimatorState idle = stateMachine.AddState("Idle");
        AnimatorState pulse = stateMachine.AddState("Pulse");
        pulse.motion = pulseClip;

        stateMachine.defaultState = idle;

        // Transisi Idle -> Pulse ketika parameter "Pulse" dipicu.
        AnimatorStateTransition idleToPulse = idle.AddTransition(pulse);
        idleToPulse.hasExitTime = false;
        idleToPulse.duration = 0.1f;
        idleToPulse.AddCondition(AnimatorConditionMode.If, 0f, TriggerParameter);

        // Transisi Pulse -> Idle setelah clip selesai.
        AnimatorStateTransition pulseToIdle = pulse.AddTransition(idle);
        pulseToIdle.hasExitTime = true;
        pulseToIdle.exitTime = 1f;
        pulseToIdle.duration = 0.1f;

        // Pastikan state & transisi tersimpan sebagai sub-asset di dalam controller.
        EnsureSubAsset(idle, controller);
        EnsureSubAsset(pulse, controller);
        EnsureSubAsset(idleToPulse, controller);
        EnsureSubAsset(pulseToIdle, controller);

        EditorUtility.SetDirty(controller);
        return controller;
    }

    private static void EnsureSubAsset(Object asset, Object parent)
    {
        if (asset != null && !AssetDatabase.Contains(asset) && AssetDatabase.Contains(parent))
            AssetDatabase.AddObjectToAsset(asset, parent);
    }
}
