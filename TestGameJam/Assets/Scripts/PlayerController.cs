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
    public float m_punchCooldownTime = 0.2f;
    public float m_punchDistance = 1f;
    public float m_startPunchSpeed = 0.1f;
    public float m_punchSpeedOverTimeMultiplier = 2f;
    public float m_slowdownWhenPunch = 0.2f;
    public GameObject[] m_fists;

    [HideInInspector]
    public bool m_bPunching = false;

    private Rigidbody m_rigidbody;
    private Vector3[] m_fistStartPos;
    private float m_fCurrentPunchCooldown = 0f;
    private float m_fCurrentPunchSpeed;
    private bool m_bUseRightFist = true;

	// Use this for initialization
	void Start()
    {
        m_fCurrentPunchSpeed = m_startPunchSpeed;
        m_rigidbody = GetComponent<Rigidbody>();
        m_fistStartPos = new Vector3[2];
        for (int i = 0; i < 2; i++)
        {
            m_fistStartPos[i] = m_fists[i].transform.localPosition;
        }
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
        if (gamePadState.Buttons.A == ButtonState.Pressed
            || gamePadState.Triggers.Left > 0f)
            if (Physics.Raycast(transform.position, Vector3.down, 1f))
                v3Velocity.y = m_jumpVelocity;
        // set new velocity
        m_rigidbody.velocity = v3Velocity;

        // look towards right stick input
        if (Mathf.Abs(gamePadState.ThumbSticks.Right.X) > m_thumbstickDeadzone || Mathf.Abs(gamePadState.ThumbSticks.Right.Y) > m_thumbstickDeadzone)
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(gamePadState.ThumbSticks.Right.X, 0, gamePadState.ThumbSticks.Right.Y)), m_rotateSpeed);

        // start punch
        if (gamePadState.Triggers.Right > 0f
            && m_fCurrentPunchCooldown <= 0f
            && !m_bPunching)
        {
            m_bPunching = true;
            Vector3 v3NewVelocity = m_rigidbody.velocity;
            v3NewVelocity.x *= m_slowdownWhenPunch;
            v3NewVelocity.z *= m_slowdownWhenPunch;
            m_rigidbody.velocity = v3NewVelocity;
        }

        // stop punch if function returns false
        if (m_bPunching
            && !Punch(m_bUseRightFist))
        {
            m_bPunching = false; // stop punching
            m_fCurrentPunchCooldown = m_punchCooldownTime; // set cooldown
            m_fCurrentPunchSpeed = m_startPunchSpeed;
            m_bUseRightFist = !m_bUseRightFist; // alternate fists
        }

        // decrement cooldown of punch
        if (!m_bPunching
            && m_fCurrentPunchCooldown > 0f)
        {
            m_fCurrentPunchCooldown -= Time.deltaTime;
        }
    }

    // returns false when punch has ended
    // otherwise returns true
    private bool Punch(bool bUseRightFist)
    {
        // punch with right fist
        if (bUseRightFist)
        {
            // move fist
            Vector3 v3FistPos = m_fists[1].transform.localPosition;
            Vector3 v3PunchEndPos = m_fistStartPos[1] + new Vector3(0, 0, m_punchDistance);
            v3FistPos = Vector3.Lerp(v3FistPos, v3PunchEndPos, m_fCurrentPunchSpeed);
            m_fCurrentPunchSpeed += Time.deltaTime * m_punchSpeedOverTimeMultiplier;
            m_fists[1].transform.localPosition = v3FistPos;
            if (Vector3.Distance(v3FistPos, v3PunchEndPos) < 0.1f)
            {
                // reset position
                m_fists[1].transform.localPosition = m_fistStartPos[1];
                return false;
            }
        }
        // punch with left fist
        else
        {
            // move fist
            Vector3 v3FistPos = m_fists[0].transform.localPosition;
            Vector3 v3PunchEndPos = m_fistStartPos[0] + new Vector3(0, 0, m_punchDistance);
            v3FistPos = Vector3.Lerp(v3FistPos, v3PunchEndPos, m_fCurrentPunchSpeed);
            m_fCurrentPunchSpeed += Time.deltaTime * m_punchSpeedOverTimeMultiplier;
            m_fists[0].transform.localPosition = v3FistPos;
            if (Vector3.Distance(v3FistPos, v3PunchEndPos) < 0.1f)
            {
                // reset position
                m_fists[0].transform.localPosition = m_fistStartPos[0];
                return false;
            }
        }
        return true;
    }
}