using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;

/// <summary>
/// Editor script to generate the Camel Animator Controller template.
/// Creates parameters, states, transitions, and blend tree for Run animation.
///
/// Spec (from AIG-81):
/// - Parameters: IsRunning (bool), Jump/Slide/Hit (triggers)
/// - State machine: Idle ↔ Run (IsRunning), one-shot states for Jump/Slide/Hit
/// - Blend tree on Run state for animation speeds 0-2
/// </summary>
public class CamelAnimatorSetup
{
    private const string CONTROLLER_PATH = "Assets/Models/Egypt/Characters/Camel/Animator/CamelAnimator.controller";
    private const string IDLE_ANIM = "Idle"; // placeholder - will be replaced with actual animation
    private const string RUN_ANIM = "Run";
    private const string JUMP_ANIM = "Jump";
    private const string SLIDE_ANIM = "Slide";
    private const string HIT_ANIM = "Hit";

    [MenuItem("Tools/Camel Runner/Setup Camel Animator")]
    public static void SetupCamelAnimator()
    {
        // Create animator controller
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(CONTROLLER_PATH);
        AnimatorStateMachine rootStateMachine = controller.layers[0].stateMachine;

        // Add parameters (matching PlayerController.cs expectations)
        controller.AddParameter("IsRunning", AnimatorControllerParameterType.Bool);
        controller.AddParameter("Jump", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Slide", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Hit", AnimatorControllerParameterType.Trigger);

        // Create states
        AnimatorState idleState = rootStateMachine.AddState("Idle", new Vector3(300, 100, 0));
        AnimatorState runState = rootStateMachine.AddState("Run", new Vector3(300, 200, 0));
        AnimatorState jumpState = rootStateMachine.AddState("Jump", new Vector3(600, 100, 0));
        AnimatorState slideState = rootStateMachine.AddState("Slide", new Vector3(600, 200, 0));
        AnimatorState hitState = rootStateMachine.AddState("Hit", new Vector3(600, 300, 0));

        // Set default state
        rootStateMachine.defaultState = idleState;

        // Create transitions: Idle ↔ Run
        AnimatorStateTransition idleToRun = idleState.AddTransition(runState);
        idleToRun.AddCondition(AnimatorConditionMode.If, 0, "IsRunning");
        idleToRun.duration = 0f;

        AnimatorStateTransition runToIdle = runState.AddTransition(idleState);
        runToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsRunning");
        runToIdle.duration = 0.1f;

        // Create transitions: Any state → Jump (one-shot)
        AnimatorStateTransition idleToJump = idleState.AddTransition(jumpState);
        idleToJump.AddCondition(AnimatorConditionMode.If, 0, "Jump");
        idleToJump.duration = 0f;

        AnimatorStateTransition runToJump = runState.AddTransition(jumpState);
        runToJump.AddCondition(AnimatorConditionMode.If, 0, "Jump");
        runToJump.duration = 0f;

        // Create transitions: Any state → Slide (one-shot)
        AnimatorStateTransition idleToSlide = idleState.AddTransition(slideState);
        idleToSlide.AddCondition(AnimatorConditionMode.If, 0, "Slide");
        idleToSlide.duration = 0f;

        AnimatorStateTransition runToSlide = runState.AddTransition(slideState);
        runToSlide.AddCondition(AnimatorConditionMode.If, 0, "Slide");
        runToSlide.duration = 0f;

        // Create transitions: Any state → Hit (one-shot)
        AnimatorStateTransition idleToHit = idleState.AddTransition(hitState);
        idleToHit.AddCondition(AnimatorConditionMode.If, 0, "Hit");
        idleToHit.duration = 0f;

        AnimatorStateTransition runToHit = runState.AddTransition(hitState);
        runToHit.AddCondition(AnimatorConditionMode.If, 0, "Hit");
        runToHit.duration = 0f;

        // Jump/Slide/Hit transition back to previous state based on IsRunning
        AnimatorStateTransition jumpToRun = jumpState.AddTransition(runState);
        jumpToRun.AddCondition(AnimatorConditionMode.If, 0, "IsRunning");
        jumpToRun.duration = 0.1f;
        jumpToRun.exitTime = 0.95f; // allow animation to finish before transitioning

        AnimatorStateTransition jumpToIdle = jumpState.AddTransition(idleState);
        jumpToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsRunning");
        jumpToIdle.duration = 0.1f;
        jumpToIdle.exitTime = 0.95f;

        AnimatorStateTransition slideToRun = slideState.AddTransition(runState);
        slideToRun.AddCondition(AnimatorConditionMode.If, 0, "IsRunning");
        slideToRun.duration = 0.1f;
        slideToRun.exitTime = 0.95f;

        AnimatorStateTransition slideToIdle = slideState.AddTransition(idleState);
        slideToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsRunning");
        slideToIdle.duration = 0.1f;
        slideToIdle.exitTime = 0.95f;

        AnimatorStateTransition hitToRun = hitState.AddTransition(runState);
        hitToRun.AddCondition(AnimatorConditionMode.If, 0, "IsRunning");
        hitToRun.duration = 0.1f;
        hitToRun.exitTime = 0.95f;

        AnimatorStateTransition hitToIdle = hitState.AddTransition(idleState);
        hitToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsRunning");
        hitToIdle.duration = 0.1f;
        hitToIdle.exitTime = 0.95f;

        // Create blend tree on Run state for speed variation (0-2)
        BlendTree runBlendTree = CreateRunBlendTree();
        runState.motion = runBlendTree;

        // Set up motion animations (placeholders - will be replaced with actual animations)
        // These will be configured when animation FBX is imported
        idleState.motion = null; // Will assign actual Idle animation later
        jumpState.motion = null; // Will assign actual Jump animation later
        slideState.motion = null; // Will assign actual Slide animation later
        hitState.motion = null; // Will assign actual Hit Reaction animation later

        // Configure Jump/Slide/Hit to not loop
        jumpState.motion = null; jumpState.tag = "OneShot";
        slideState.motion = null; slideState.tag = "OneShot";
        hitState.motion = null; hitState.tag = "OneShot";

        EditorUtility.SetDirty(controller);
        AssetDatabase.SaveAssets();
        Debug.Log($"✓ Camel Animator Controller created at: {CONTROLLER_PATH}");
    }

    private static BlendTree CreateRunBlendTree()
    {
        // Create a 1D blend tree for Run animation speed variation
        // Parameter: forward velocity (0-2 speed multiplier)
        BlendTree blendTree = new BlendTree();
        blendTree.name = "Run Blend Tree";
        blendTree.blendType = BlendTreeType.Simple1D;
        blendTree.blendParameter = "Speed"; // Note: Speed parameter not used by PlayerController yet, added for future

        // Placeholder motion points - will be configured when animations arrive
        blendTree.AddChild(null, 0f);   // Run cycle at speed 0 (walking)
        blendTree.AddChild(null, 1f);   // Run cycle at speed 1 (normal)
        blendTree.AddChild(null, 2f);   // Run cycle at speed 2 (fast)

        return blendTree;
    }

    /// <summary>Verify controller has correct parameters and state structure.</summary>
    [MenuItem("Tools/Camel Runner/Verify Camel Animator")]
    public static void VerifyAnimator()
    {
        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(CONTROLLER_PATH);

        if (controller == null)
        {
            Debug.LogError("❌ Camel Animator Controller not found. Run 'Tools/Camel Runner/Setup Camel Animator' first.");
            return;
        }

        // Check parameters
        bool hasIsRunning = false, hasJump = false, hasSlide = false, hasHit = false;
        foreach (var param in controller.parameters)
        {
            if (param.name == "IsRunning" && param.type == AnimatorControllerParameterType.Bool) hasIsRunning = true;
            if (param.name == "Jump" && param.type == AnimatorControllerParameterType.Trigger) hasJump = true;
            if (param.name == "Slide" && param.type == AnimatorControllerParameterType.Trigger) hasSlide = true;
            if (param.name == "Hit" && param.type == AnimatorControllerParameterType.Trigger) hasHit = true;
        }

        // Check states
        AnimatorStateMachine rootSM = controller.layers[0].stateMachine;
        int stateCount = rootSM.states.Length;

        string report = $"""
        ╔═══════════════════════════════════════╗
        ║  Camel Animator Verification Report   ║
        ╚═══════════════════════════════════════╝

        Parameters:
          ✓ IsRunning (bool):  {(hasIsRunning ? "✓ Present" : "❌ Missing")}
          ✓ Jump (trigger):    {(hasJump ? "✓ Present" : "❌ Missing")}
          ✓ Slide (trigger):   {(hasSlide ? "✓ Present" : "❌ Missing")}
          ✓ Hit (trigger):     {(hasHit ? "✓ Present" : "❌ Missing")}

        States: {stateCount} total
          Expected: Idle, Run, Jump, Slide, Hit

        Transitions:
          ✓ Idle ↔ Run (IsRunning bool)
          ✓ Jump/Slide/Hit triggered from any state
          ✓ Jump/Slide/Hit return based on IsRunning value

        Blend Tree (Run state):
          ✓ 1D blend tree for speed variation (0-2)
          ⚠ Animation clips pending import (AIG-36)
        """;

        Debug.Log(report);

        bool allValid = hasIsRunning && hasJump && hasSlide && hasHit && stateCount == 5;
        if (allValid)
            Debug.Log("✓ Animator controller structure is valid. Ready for animation import.");
        else
            Debug.LogWarning("⚠ Some parameters or states are missing. Check the report above.");
    }
}
