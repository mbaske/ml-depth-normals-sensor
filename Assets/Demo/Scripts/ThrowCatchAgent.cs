using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class ThrowCatchAgent : Agent
{
    [SerializeField]
    private int m_EpisodeLength = 100;

    private Ball m_Ball;
    private Goal m_Goal;
    private Platform m_Platform;
    private StatsRecorder m_Stats;
    private int m_DecisionCount;
    private int m_FrameCountdown;

    public override void Initialize()
    {
        m_Stats = Academy.Instance.StatsRecorder;
        m_Platform = transform.parent.GetComponentInChildren<Platform>();
        m_Goal = transform.parent.GetComponentInChildren<Goal>();
        m_Goal.Initialize();

        m_Ball = GetComponentInChildren<Ball>();
        m_Ball.Initialize();
        m_Ball.DropEvent += OnDropBall;
        m_Ball.HitGoalEvent += OnHitGoal;
        m_Ball.HitPlatformEvent += OnHitPlatform;

        ResetAndRandomize();
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        base.OnActionReceived(actions);

        m_Ball.Throw(actions.ContinuousActions[0]);
        m_Goal.SetPosition(actions.ContinuousActions[1]);
    }

    private void ResetAndRandomize()
    {
        transform.localPosition = new Vector3(0,
            Random.Range(5f, 15f), Random.Range(-30f, -40f));

        m_Ball.ResetPosition();
        m_Platform.Randomize();
        m_Goal.SetPosition(0); // Disables goal
        m_FrameCountdown = 2; // Wait a moment
    }

    private void Update()
    {
        if (--m_FrameCountdown == 0)
        {
            if (++m_DecisionCount % m_EpisodeLength == 0)
            {
                EndEpisode();
            }

            RequestDecision();
        }
    }

    private void OnDropBall()
    {
        if (!m_Goal.IsEnabled())
        {
            m_Stats.Add("Platform", 0);
        }
        m_Stats.Add("Goal", 0);
        ResetAndRandomize();
    }

    private void OnHitPlatform()
    {
        AddReward(0.5f);
        m_Stats.Add("Platform", 1);
        m_Goal.SetEnabled(true);
    }

    private void OnHitGoal()
    {
        AddReward(0.5f);
        m_Stats.Add("Goal", 1);
        m_Goal.Highlight();
        ResetAndRandomize();
    }

    private void OnApplicationQuit()
    {
        m_Ball.DropEvent -= OnDropBall;
        m_Ball.HitGoalEvent -= OnHitGoal;
        m_Ball.HitPlatformEvent -= OnHitPlatform;
    }
}
