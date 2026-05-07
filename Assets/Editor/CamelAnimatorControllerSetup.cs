using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

/// <summary>
/// Editor utility to create the CamelAnimatorController with all states and transitions.
///
/// Run: Tools → Camel Runner → Setup Camel Animator Controller
///
/// Creates a state machine with:
///   - Idle (default)
///   - Running (triggered by IsRunning bool)
///   - Jump (triggered by Jump trigger)
///   - Slide (triggered by Slide trigger)
///   - Hit (triggered by Hit trigger)
/// </summary>
public class CamelAnimatorControllerSetup
{
    private const string ControllerPath = "Assets/Animations/CamelAnimatorController.controller";

    [MenuItem("Tools/Camel Runner/Setup Camel Animator Controller")]
    public static void SetupCamelAnimatorController()
    {
        Debug.Log("[CamelAnimatorControllerSetup] Setting up Camel Animator Controller...");

        // Ensure folder exists
        if (!AssetDatabase.IsValidFolder("Assets/Animations"))
            AssetDatabase.CreateFolder("Assets", "Animations");

        // Load or create the controller
        var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
        if (controller == null)
        {
            // Create new controller
            controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
            Debug.Log($"[CamelAnimatorControllerSetup] Created new controller at {ControllerPath}");
        }
        else
        {
            Debug.Log($"[CamelAnimatorControllerSetup] Loaded existing controller at {ControllerPath}");
        }

        // Ensure parameters exist
        EnsureParameter(controller, "IsRunning", AnimatorControllerParameterType.Bool);
        EnsureParameter(controller, "Jump", AnimatorControllerParameterType.Trigger);
        EnsureParameter(controller, "Slide", AnimatorControllerParameterType.Trigger);
        EnsureParameter(controller, "Hit", AnimatorControllerParameterType.Trigger);

        // Get or create the root state machine (layer 0)
        var rootStateMachine = controller.layers[0].stateMachine;
        rootStateMachine.name = "Base Layer";

        // Clear existing states to start fresh
        ClearStates(rootStateMachine);

        // ── Create states ───────────────────────────────────────────────────────

        // Idle state (default)
        var idleState = rootStateMachine.AddState("Idle");
        rootStateMachine.defaultState = idleState;

        // Running state
        var runningState = rootStateMachine.AddState("Running");

        // Jump state
        var jumpState = rootStateMachine.AddState("Jump");

        // Slide state
        var slideState = rootStateMachine.AddState("Slide");

        // Hit state
        var hitState = rootStateMachine.AddState("Hit");

        // ── Create transitions ──────────────────────────────────────────────────

        // Idle ↔ Running (via IsRunning bool)
        var idleToRunning = rootStateMachine.AddAnyStateTransition(runningState);
        idleToRunning.AddCondition(AnimatorConditionMode.If, 1f, "IsRunning");
        idleToRunning.hasExitTime = false;
        idleToRunning.duration = 0.1f;

        var runningToIdle = rootStateMachine.AddAnyStateTransition(idleState);
        runningToIdle.AddCondition(AnimatorConditionMode.IfNot, 0f, "IsRunning");
        runningToIdle.hasExitTime = false;
        runningToIdle.duration = 0.1f;

        // Jump trigger from any state
        var anyToJump = rootStateMachine.AddAnyStateTransition(jumpState);
        anyToJump.AddCondition(AnimatorConditionMode.If, 1f, "Jump");
        anyToJump.hasExitTime = false;
        anyToJump.duration = 0f;

        // Jump → back to previous state (auto-exit after 0.5s)
        var jumpToIdle = jumpState.AddTransition(idleState);
        jumpToIdle.hasExitTime = true;
        jumpToIdle.exitTime = 0.5f;
        jumpToIdle.duration = 0.1f;

        var jumpToRunning = jumpState.AddTransition(runningState);
        jumpToRunning.AddCondition(AnimatorConditionMode.If, 1f, "IsRunning");
        jumpToRunning.hasExitTime = true;
        jumpToRunning.exitTime = 0.5f;
        jumpToRunning.duration = 0.1f;

        // Slide trigger from any state
        var anyToSlide = rootStateMachine.AddAnyStateTransition(slideState);
        anyToSlide.AddCondition(AnimatorConditionMode.If, 1f, "Slide");
        anyToSlide.hasExitTime = false;
        anyToSlide.duration = 0f;

        // Slide → back to previous state (auto-exit after 0.6s)
        var slideToIdle = slideState.AddTransition(idleState);
        slideToIdle.hasExitTime = true;
        slideToIdle.exitTime = 0.6f;
        slideToIdle.duration = 0.15f;

        var slideToRunning = slideState.AddTransition(runningState);
        slideToRunning.AddCondition(AnimatorConditionMode.If, 1f, "IsRunning");
        slideToRunning.hasExitTime = true;
        slideToRunning.exitTime = 0.6f;
        slideToRunning.duration = 0.15f;

        // Hit trigger from any state
        var anyToHit = rootStateMachine.AddAnyStateTransition(hitState);
        anyToHit.AddCondition(AnimatorConditionMode.If, 1f, "Hit");
        anyToHit.hasExitTime = false;
        anyToHit.duration = 0f;

        // Hit → back to previous state (auto-exit after 0.3s)
        var hitToIdle = hitState.AddTransition(idleState);
        hitToIdle.hasExitTime = true;
        hitToIdle.exitTime = 0.3f;
        hitToIdle.duration = 0.1f;

        var hitToRunning = hitState.AddTransition(runningState);
        hitToRunning.AddCondition(AnimatorConditionMode.If, 1f, "IsRunning");
        hitToRunning.hasExitTime = true;
        hitToRunning.exitTime = 0.3f;
        hitToRunning.duration = 0.1f;

        // Save controller
        EditorUtility.SetDirty(controller);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("[CamelAnimatorControllerSetup] ✓ Controller setup complete with 5 states and transitions");
    }

    private static void EnsureParameter(AnimatorController controller, string name, AnimatorControllerParameterType type)
    {
        // Check if parameter already exists
        foreach (var param in controller.parameters)
        {
            if (param.name == name)
                return;
        }

        // Add if missing
        controller.AddParameter(name, type);
    }

    private static void ClearStates(AnimatorStateMachine stateMachine)
    {
        // Remove all states except the root
        var states = stateMachine.states;
        foreach (var state in states)
        {
            stateMachine.RemoveState(state.state);
        }

        // Transitions are removed automatically when states are removed
    }

    [MenuItem("Tools/Camel Runner/Validate Camel Animator Controller")]
    public static void ValidateCamelAnimatorController()
    {
        var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
        if (controller == null)
        {
            Debug.LogError($"[CamelAnimatorControllerSetup] ✗ Controller not found at {ControllerPath}");
            return;
        }

        int stateCount = controller.layers[0].stateMachine.states.Length;
        int paramCount = controller.parameters.Length;

        string report = $"[CamelAnimatorControllerSetup] Validation — {ControllerPath}\n" +
                       $"  States: {stateCount} (expected 5)\n" +
                       $"  Parameters: {paramCount} (expected 4)\n";

        bool hasIsRunning = System.Array.Exists(controller.parameters, p => p.name == "IsRunning");
        bool hasJump = System.Array.Exists(controller.parameters, p => p.name == "Jump");
        bool hasSlide = System.Array.Exists(controller.parameters, p => p.name == "Slide");
        bool hasHit = System.Array.Exists(controller.parameters, p => p.name == "Hit");

        report += $"  IsRunning param: {(hasIsRunning ? "✓" : "✗")}\n" +
                 $"  Jump param: {(hasJump ? "✓" : "✗")}\n" +
                 $"  Slide param: {(hasSlide ? "✓" : "✗")}\n" +
                 $"  Hit param: {(hasHit ? "✓" : "✗")}";

        bool allOk = stateCount == 5 && paramCount == 4 && hasIsRunning && hasJump && hasSlide && hasHit;

        if (allOk) Debug.Log(report);
        else       Debug.LogWarning(report);
    }
}
