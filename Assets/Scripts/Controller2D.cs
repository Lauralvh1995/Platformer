using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour {

    public LayerMask colliderMask;

    private float skinWidth = 0.15f;

    public int horizontalRays = 4;
    public int verticalRays = 4;

    float horizontalRaySpacing;
    float verticalRaySpacing;

    BoxCollider2D collider2D;
    RaycastOrigins origins;

    public CollisionInfo info;
    private void Start()
    {
        collider2D = GetComponent<BoxCollider2D>();
        CalculateSpacing();
    }

    public void Move(Vector3 velocity)
    {
        UpdateRaycastOrigins();
        info.Reset();
        if (velocity.x != 0)
        {
            HorizontalCollisions(ref velocity);
        }
        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }

        transform.Translate(velocity);
    }

    void VerticalCollisions(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLenght = Mathf.Abs(velocity.y) + skinWidth;
        for (int i = 0; i < verticalRays; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? origins.bottomLeft : origins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);

            RaycastHit2D hitInfo = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLenght, colliderMask);
            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLenght, Color.red);

            if(hitInfo)
            {
                velocity.y = (hitInfo.distance - skinWidth) * directionY;
                rayLenght = hitInfo.distance;

                info.below = directionY == -1;
                info.above = directionY == 1;
            }
        }
    }
    void HorizontalCollisions(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLenght = Mathf.Abs(velocity.x) + skinWidth;
        for (int i = 0; i < horizontalRays; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? origins.bottomLeft : origins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);

            RaycastHit2D hitInfo = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLenght, colliderMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLenght, Color.red);
            if (hitInfo)
            {
                velocity.x = (hitInfo.distance - skinWidth) * directionX;
                rayLenght = hitInfo.distance;

                info.left = directionX == -1;
                info.right = directionX == 1;
            }
        }
    }

    void UpdateRaycastOrigins()
    {
        Bounds bounds = collider2D.bounds;
        bounds.Expand(skinWidth * -2);

        origins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        origins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        origins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        origins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    void CalculateSpacing()
    {
        Bounds bounds = collider2D.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRays = Mathf.Clamp(horizontalRays, 2, int.MaxValue);
        verticalRays = Mathf.Clamp(verticalRays, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRays - 1);
        verticalRaySpacing = bounds.size.x / (verticalRays - 1);
    }

    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public void Reset()
        {
            above = below = false;
            left = right = false;
        }
    }
}
