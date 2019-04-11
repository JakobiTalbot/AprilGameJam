using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class PlayerController : MonoBehaviour
{
    public int m_playerNumber;
    public float m_movementSpeed = 10;
    public float m_rotateSpeed = 0.5f;

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
        v3Velocity.x += gamePadState.ThumbSticks.Left.X * m_movementSpeed * Time.deltaTime;
        v3Velocity.z += gamePadState.ThumbSticks.Left.Y * m_movementSpeed * Time.deltaTime;
        m_rigidbody.velocity = v3Velocity;

        // look towards right stick input
        if (Mathf.Abs(gamePadState.ThumbSticks.Right.X) > 0.15f || Mathf.Abs(gamePadState.ThumbSticks.Right.Y) > 0.15f)
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(gamePadState.ThumbSticks.Right.X, 0, gamePadState.ThumbSticks.Right.Y)), m_rotateSpeed);
    }
}
