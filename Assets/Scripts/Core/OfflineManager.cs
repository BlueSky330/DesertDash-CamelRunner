using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Detects offline/online state and maintains a queue of pending events
/// that failed while offline. Events are drained automatically when connectivity
/// is restored.
/// </summary>
public class OfflineManager : MonoBehaviour
{
    public static OfflineManager Instance { get; private set; }

    public bool isOnline { get; private set; }

    public delegate void OnOnlineStatusChanged(bool online);
    public static event OnOnlineStatusChanged onOnlineStatusChanged;

    // ── Pending event queue ───────────────────────────────────────────────────

    public enum PendingEventType
    {
        AnalyticsEvent,
        LeaderboardSubmit,
        CloudSave,
        AdImpression
    }

    [Serializable]
    public class PendingEvent
    {
        public PendingEventType type;
        public string payload;   // JSON-serialised event data
        public float enqueuedAt; // Time.realtimeSinceStartup when queued
    }

    private readonly Queue<PendingEvent> _pendingQueue = new Queue<PendingEvent>();

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        CheckInternetConnectivity();
        StartCoroutine(ConnectivityCheckRoutine());
    }

    // ── Connectivity detection ────────────────────────────────────────────────

    private void CheckInternetConnectivity()
    {
        bool wasOnline = isOnline;
        isOnline = Application.internetReachability != NetworkReachability.NotReachable;

        if (isOnline != wasOnline)
        {
            onOnlineStatusChanged?.Invoke(isOnline);
            Debug.Log($"[OfflineManager] Status changed → {(isOnline ? "ONLINE" : "OFFLINE")}");

            if (isOnline)
                StartCoroutine(DrainPendingQueue());
        }
    }

    private IEnumerator ConnectivityCheckRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            CheckInternetConnectivity();
        }
    }

    public bool CanAccessOnlineFeatures() => isOnline;

    // ── Event queue ───────────────────────────────────────────────────────────

    /// <summary>
    /// Enqueue an event that requires network. If online, dispatches immediately;
    /// otherwise queues for when connectivity returns.
    /// </summary>
    public void SubmitEvent(PendingEventType type, string payload)
    {
        if (isOnline)
        {
            DispatchEvent(type, payload);
        }
        else
        {
            _pendingQueue.Enqueue(new PendingEvent
            {
                type = type,
                payload = payload,
                enqueuedAt = Time.realtimeSinceStartup
            });
            Debug.Log($"[OfflineManager] Queued {type} event. Queue depth: {_pendingQueue.Count}");
        }
    }

    /// <summary>Drain all queued events one frame apart to avoid spikes.</summary>
    private IEnumerator DrainPendingQueue()
    {
        Debug.Log($"[OfflineManager] Back online — draining {_pendingQueue.Count} queued events.");
        while (_pendingQueue.Count > 0)
        {
            var evt = _pendingQueue.Dequeue();
            DispatchEvent(evt.type, evt.payload);
            yield return null; // spread across frames
        }
    }

    /// <summary>
    /// Dispatch a single event to the appropriate backend.
    /// Replace stub bodies with real SDK calls (Firebase Analytics, Leaderboards, etc.).
    /// </summary>
    private void DispatchEvent(PendingEventType type, string payload)
    {
        switch (type)
        {
            case PendingEventType.AnalyticsEvent:
                // FirebaseAnalytics.LogEvent(payload);
                Debug.Log($"[OfflineManager] Dispatched analytics: {payload}");
                break;
            case PendingEventType.LeaderboardSubmit:
                // LeaderboardService.Submit(payload);
                Debug.Log($"[OfflineManager] Dispatched leaderboard: {payload}");
                break;
            case PendingEventType.CloudSave:
                // CloudSaveService.Save(payload);
                Debug.Log($"[OfflineManager] Dispatched cloud save: {payload}");
                break;
            case PendingEventType.AdImpression:
                // AdAnalytics.RecordImpression(payload);
                Debug.Log($"[OfflineManager] Dispatched ad impression: {payload}");
                break;
        }
    }

    public int PendingEventCount => _pendingQueue.Count;
}
