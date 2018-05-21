using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Controller2D : RaycastController {
    float maxSlopeAngle = 60f;
    float maxDescendAngle = 60f;

    public CollisionInfo info;
    public override void Start()
    {
        base.Start();
    }

    public void Move(Vector3 velocity, bool standingOnPlatform = false)
    {
        UpdateRaycastOrigins();
        info.Reset();
        info.velocityOLD = velocity;

        if(velocity.y < 0)
        {
            DescendSlope(ref velocity);
        }
        if (velocity.x != 0)
        {
            HorizontalCollisions(ref velocity);
        }
        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }

        transform.Translate(velocity);
        if(standingOnPlatform)
        {
            info.below = true;
        }
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
                if(info.climbingSlope)
                {
                    velocity.x = velocity.y / Mathf.Tan(info.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }
                info.below = directionY == -1;
                info.above = directionY == 1;
            }
        }
        if(info.climbingSlope)
        {
            float directionX = Mathf.Sign(velocity.x);
            rayLenght = Mathf.Abs(velocity.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? origins.bottomLeft : origins.bottomRight) + Vector2.up * velocity.y;
            RaycastHit2D hitInfo = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLenght, colliderMask);

            if (hitInfo)
            {
                float slopeAngle = Vector2.Angle(hitInfo.normal, Vector2.up);
                if(slopeAngle != info.slopeAngle)
                {
                    velocity.x = (hitInfo.distance - skinWidth) * directionX;
                    info.slopeAngle = slopeAngle;
                }
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
                if(hitInfo.distance == 0)
                {
                    continue;
                }
                float slopeAngle = Vector2.Angle(hitInfo.normal, Vector2.up);
                if(i==0 & slopeAngle <= maxSlopeAngle)
                {
                    float distanceToSlope = 0;
                    if(slopeAngle != info.slopeAngleOLD)
                    {
                        distanceToSlope = hitInfo.distance - skinWidth;
                        velocity.x -= distanceToSlope * directionX;
                    }
                    ClimbSlope(ref velocity, slopeAngle);
                    velocity.x += distanceToSlope * directionX;
                }
                if (!info.climbingSlope || slopeAngle > maxSlopeAngle)
                {
                    velocity.x = (hitInfo.distance - skinWidth) * directionX;
                    rayLenght = hitInfo.distance;
                    if(info.climbingSlope)
                    {
                        velocity.y = Mathf.Tan(info.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                    }
                    info.left = directionX == -1;
                    info.right = directionX == 1;
                }
            }
        }
    }
    void ClimbSlope(ref Vector3 velocity, float slopeAngle)
    {
        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
        if (velocity.y <= climbVelocityY)
        {
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
            info.below = true;
            info.climbingSlope = true;
        }
    }

    void DescendSlope(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        Vector2 rayOrigin = (directionX == -1) ? origins.bottomRight : origins.bottomLeft;
        RaycastHit2D hitInfo = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, colliderMask);
        if(hitInfo)
        {
            float slopeAngle = Vector2.Angle(hitInfo.normal, Vector2.up);
            if(slopeAngle != 0 && slopeAngle < maxDescendAngle)
            {
                if (info.descendingSlope)
                {
                    info.descendingSlope = false;
                    velocity = info.velocityOLD;
                }
                if(Mathf.Sign(hitInfo.normal.x) == directionX)
                {
                    if(hitInfo.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
                    {
                        float moveDistance = Mathf.Abs(velocity.x);
                        float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                        velocity.y -= descendVelocityY;

                        info.slopeAngle = slopeAngle;
                        info.descendingSlope = true;
                        info.below = true;
                    }
                }
            }
        }
    }
    

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;
        public bool climbingSlope;
        public bool descendingSlope;

        public Vector3 velocityOLD;

        public float slopeAngle, slopeAngleOLD;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            descendingSlope = false;

            slopeAngleOLD = slopeAngle;
            slopeAngle = 0;
        }
    }
}
