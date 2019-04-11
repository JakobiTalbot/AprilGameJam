using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class PlayerController : MonoBehaviour
{
    public int m_playerNumber = 1;
    public float m_movementSpeed = 10f;
    public float m_rotateSpeed = 0.5f;
    public float m_jumpVelocity = 20f;
    public float m_thumbstickDeadzone = 0.15f;

    private Rigidbody m_rigidbody;

	// Use this for initialization
	void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update()
    {
        // get controller input
        GamePadState gamePadState = GamePad.GetState((PlayerIndex)m_playerNumber - 1);

        // movement input
        Vector3 v3Velocity = m_rigidbody.velocity;
        if (Mathf.Abs(gamePadState.ThumbSticks.Left.X) > m_thumbstickDeadzone || Mathf.Abs(gamePadState.ThumbSticks.Left.Y) > m_thumbstickDeadzone)
        {
            v3Velocity.x += gamePadState.ThumbSticks.Left.X * m_movementSpeed * Time.deltaTime;
            v3Velocity.z += gamePadState.ThumbSticks.Left.Y * m_movementSpeed * Time.deltaTime;
        }
        // jump
        if (gamePadState.Buttons.A == ButtonState.Pressed && Physics.Raycast(transform.position, Vector3.down, 0.5f))
            v3Velocity.y = m_jumpVelocity;
        // set new velocity
        m_rigidbody.velocity = v3Velocity;

        // look towards right stick input
        if (Mathf.Abs(gamePadState.ThumbSticks.Right.X) > m_thumbstickDeadzone || Mathf.Abs(gamePadState.ThumbSticks.Right.Y) > m_thumbstickDeadzone)
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(gamePadState.ThumbSticks.Right.X, 0, gamePadState.ThumbSticks.Right.Y)), m_rotateSpeed);
    }
}