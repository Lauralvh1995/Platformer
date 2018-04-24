using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(BoxCollider2D))]
public abstract class RaycastController : MonoBehaviour {
    public LayerMask colliderMask;

    protected float skinWidth = 0.15f;

    public int horizontalRays = 4;
    public int verticalRays = 4;

    protected float horizontalRaySpacing;
    protected float verticalRaySpacing;

    BoxCollider2D collider2D;
    protected RaycastOrigins origins;

    public virtual void Start()
    {
        collider2D = GetComponent<BoxCollider2D>();
        CalculateSpacing();
    }
    protected void UpdateRaycastOrigins()
    {
        Bounds bounds = collider2D.bounds;
        bounds.Expand(skinWidth * -2);

        origins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        origins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        origins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        origins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    protected void CalculateSpacing()
    {
        Bounds bounds = collider2D.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRays = Mathf.Clamp(horizontalRays, 2, int.MaxValue);
        verticalRays = Mathf.Clamp(verticalRays, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRays - 1);
        verticalRaySpacing = bounds.size.x / (verticalRays - 1);
    }

    protected struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
}
