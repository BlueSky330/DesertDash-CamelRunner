using UnityEngine;

/// <summary>
/// Lightweight visual behaviour: bobs and rotates collectibles in-place.
/// No allocations per frame. Attached by PlaceholderAssetBuilder.
/// </summary>
public class CollectibleBob : MonoBehaviour
{
    [SerializeField] private float bobHeight = 0.15f;
    [SerializeField] private float bobSpeed  = 2f;
    [SerializeField] private float rotSpeed  = 90f; // degrees per second

    private Vector3 startPos;

    void OnEnable()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float y = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.localPosition = new Vector3(startPos.x, y, startPos.z);
        transform.Rotate(Vector3.up, rotSpeed * Time.deltaTime, Space.World);
    }
}
