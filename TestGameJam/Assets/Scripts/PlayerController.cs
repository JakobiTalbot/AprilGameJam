using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
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
    public float m_punchDistance = 2f;
    public float m_startPunchSpeed = 0.1f;
    public float m_punchSpeedOverTimeMultiplier = 2f;
    public float m_slowdownWhenPunch = 0.2f;
    public float m_baseKnockbackForce = 500f;
    public float m_knockbackForce = 3000f;
    public float m_forceToDamageFactor = 0.01f;
    public List<GameObject> m_fists;

    //UI Stuff
    public Text m_playerOnePercent;
    public Text m_playerTwoPercent;
    public GameObject m_player1;
    public GameObject m_player2;

    [HideInInspector]
    public bool m_bPunching = false;
    [HideInInspector]
    public float m_fDamagePercent = 0f;

    private Rigidbody m_rigidbody;
    private Vector3[] m_fistStartPos;
    private float m_fCurrentPunchCooldown = 0f;
    private float m_fCurrentPunchSpeed;
    // 0 = left, 1 = right
    private int m_iFist = 0;

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
            // set punch to true
            m_bPunching = true;
            // slow velocity when punch
            Vector3 v3NewVelocity = m_rigidbody.velocity;
            v3NewVelocity.x *= m_slowdownWhenPunch;
            v3NewVelocity.z *= m_slowdownWhenPunch;
            m_rigidbody.velocity = v3NewVelocity;
        }

        // punch but stop punch if function returns false
        if (m_bPunching
            && !Punch(m_iFist))
        {
            m_bPunching = false; // stop punching
            m_fCurrentPunchCooldown = m_punchCooldownTime; // set cooldown
            m_fCurrentPunchSpeed = m_startPunchSpeed;
            // change fist index
            switch (m_iFist)
            {
                case 0:
                    m_iFist = 1;
                    break;
                case 1:
                    m_iFist = 0;
                    break;
            }
        }

        // decrement cooldown of punch if not punching
        if (!m_bPunching
            && m_fCurrentPunchCooldown > 0f)
        {
            m_fCurrentPunchCooldown -= Time.deltaTime;
        }
    }

    // returns false when punch has ended
    // otherwise returns true
    private bool Punch(int iFist)
    {
        // move fist
        Vector3 v3FistPos = m_fists[iFist].transform.localPosition;
        Vector3 v3PunchEndPos = new Vector3(0, m_fistStartPos[iFist].y, m_fistStartPos[iFist].z) + new Vector3(0, 0, m_punchDistance);
        v3FistPos = Vector3.Lerp(v3FistPos, v3PunchEndPos, m_fCurrentPunchSpeed);
        m_fCurrentPunchSpeed += Time.deltaTime * m_punchSpeedOverTimeMultiplier;
        m_fists[iFist].transform.localPosition = v3FistPos;

        if (Vector3.Distance(v3FistPos, v3PunchEndPos) < 0.1f)
        {
            // reset position
            m_fists[iFist].transform.localPosition = m_fistStartPos[iFist];
            return false;
        }
        return true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // if hit by punching enemy fist
        if (collision.gameObject.CompareTag("Fist")
            && collision.gameObject.GetComponentInParent<PlayerController>().m_bPunching
            && !m_fists.Contains(collision.gameObject))
        {
            // calculate force
            float fForce = m_baseKnockbackForce + (m_knockbackForce * (m_fDamagePercent / 100f));
            // apply force
<<<<<<< HEAD

            //Display Force on UI
            if (m_player1.GetComponentInParent<PlayerController>().m_bPunching == true)
            {
                m_playerOnePercent.text = fForce.ToString();
            }
            if (m_player2.GetComponentInParent<PlayerController>().m_bPunching == true)
            {
                m_playerTwoPercent.text = fForce.ToString();
            }

            m_rigidbody.AddExplosionForce(fForce, collision.transform.position, 10f);
=======
            m_rigidbody.AddExplosionForce(fForce, collision.contacts[0].point, 10f);
>>>>>>> master
            m_fDamagePercent += fForce * m_forceToDamageFactor; // add damage
        }
    }
}