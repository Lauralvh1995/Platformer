using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {

    public float jumpHeight = 4;
    public float timeToJumpApex = 0.4f;
    public float accelerationTimeAirborne = 0.2f;
    public float accelerationTimeGrounded = 0.1f;

    float moveSpeed = 6;
    float jumpVelocity;
    float gravity;

    float smoothX;
    Vector3 velocity;

    Controller2D controller;
    private void Start()
    {
        controller = GetComponent<Controller2D>();
        gravity = -(2 * jumpHeight) / (timeToJumpApex * timeToJumpApex);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        Debug.Log("Gravity: " + gravity + "| Velocity: " + jumpVelocity);
    }
    private void Update()
    {
        if(controller.info.above || controller.info.below)
        {
            velocity.y = 0;
        }
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if(Input.GetKeyDown(KeyCode.Space)&&controller.info.below)
        {
            velocity.y = jumpVelocity;
        }
        float target = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, target, ref smoothX,(controller.info.below)?accelerationTimeGrounded:accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
