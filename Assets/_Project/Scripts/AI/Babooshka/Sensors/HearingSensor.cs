using UnityEngine;

namespace Game.AI.Babooshka
{
    public sealed class HearingSensor : MonoBehaviour
    {
        private BabooshkaBlackboard blackboard;
        private BabooshkaConfig config;

        public void Bind(BabooshkaBlackboard board, BabooshkaConfig cfg)
        {
            blackboard = board;
            config = cfg;
        }

        public void NotifySound(Vector3 worldPosition)
        {
            float distance = Vector3.Distance(transform.position, worldPosition);
            if (distance > config.HearingRadius) return;

            blackboard.LastHeardSound = worldPosition;
            blackboard.LastHeardTime = Time.time;
        }

        private void OnDrawGizmos()
        {
            if (config == null) return;

            Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.5f);
            Gizmos.DrawWireSphere(transform.position, config.HearingRadius);

            if (blackboard == null) return;

            const float markerLifetime = 2f;
            float sinceHeard = Time.time - blackboard.LastHeardTime;
            if (sinceHeard < 0f || sinceHeard > markerLifetime) return;

            float t = 1f - sinceHeard / markerLifetime;
            Gizmos.color = new Color(1f, 0.85f, 0.1f, t);
            Gizmos.DrawSphere(blackboard.LastHeardSound, 0.1f + 0.3f * t);
        }
    }
}
