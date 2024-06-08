using UnityEngine;

namespace VerticalLevel;

public class AircraftHeight : MonoBehaviour
{
    public Aircraft m_Aircraft;
    public float height;
    public float targetHeight;
    public bool onTheGround;
    public float heightChangeSpeed = 300f; // per second scaled time

    private void Start()
    {
        if (m_Aircraft.direction == Aircraft.Direction.Outbound)
        {
            height = 0f;
            targetHeight = 1000f;
            onTheGround = true;
        }

        if (m_Aircraft.direction == Aircraft.Direction.Inbound)
        {
            height = 5000f;
            targetHeight = 5000f;
            onTheGround = false;
        }
    }

    private void Update()
    {
        if (m_Aircraft == null)
        {
            Destroy(gameObject);
            return;
        }

        TakeoffTouchdownProcess();
        HeightChase();

        if (m_Aircraft == Aircraft.CurrentCommandingAircraft)
        {
            bool command = false;

            if (Input.GetKeyDown(KeyCode.W))
            {
                targetHeight += 1000f;
                // max height 10000
                if (targetHeight > 10000f)
                {
                    targetHeight = 10000f;
                }
                command = true;
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                targetHeight -= 1000f;
                // min height 1000
                if (targetHeight < 1000f)
                {
                    targetHeight = 1000f;
                }
                command = true;
            }

            if (command)
            {
                FastMemberHelper<Aircraft, PlaceableWaypoint>.Set("HARWCurWP", m_Aircraft, null);
                if (m_Aircraft.NextWayPoint?.GetComponent<WaypointAutoHover>() != null)
                {
                    FastMemberHelper<Aircraft, Transform>.Set(nameof(Aircraft.NextWayPoint), m_Aircraft, null);
                    FastMemberHelper<Aircraft, Vector3>.Set(nameof(Aircraft.NextWayPointPos), m_Aircraft, Vector3.zero);
                }
            }
        }
    }

    private void TakeoffTouchdownProcess()
    {
        if (onTheGround && m_Aircraft.direction == Aircraft.Direction.Outbound &&
            m_Aircraft.state is Aircraft.State.Flying or Aircraft.State.HeadingAfterReachingWaypoint)
        {
            onTheGround = false;
            height = 1000f;
        }

        if (!onTheGround && m_Aircraft.direction == Aircraft.Direction.Inbound &&
            m_Aircraft.state == Aircraft.State.TouchedDown)
        {
            onTheGround = true;
            height = 0f;
            targetHeight = 0f;
        }
    }

    private void HeightChase()
    {
        if (onTheGround)
        {
            return;
        }

        if (height < targetHeight)
        {
            height += heightChangeSpeed * Time.deltaTime;
            if (height > targetHeight)
            {
                height = targetHeight;
            }
        }

        if (height > targetHeight)
        {
            height -= heightChangeSpeed * Time.deltaTime;
            if (height < targetHeight)
            {
                height = targetHeight;
            }
        }
    }

    public static bool CheckTCASHeightCondition(Aircraft ac1, Aircraft ac2)
    {
        if (ac1.GetComponent<AircraftHeight>() is not { } ah1 ||
            ac2.GetComponent<AircraftHeight>() is not { } ah2)
        {
            return true;
        }

        if ((int)(Mathf.Abs(ah1.height - ah1.targetHeight) / 100f) >= (int)(Mathf.Abs(ah2.height - ah1.targetHeight) / 100f) ||
            (int)(Mathf.Abs(ah2.height - ah2.targetHeight) / 100f) >= (int)(Mathf.Abs(ah1.height - ah2.targetHeight) / 100f))
            return true;
        return false;
    }
}