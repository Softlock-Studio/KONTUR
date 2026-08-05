using UnityEngine.AI;

namespace Game.AI
{
    // Shared by Babooshka and Employee movement states so both give up a destination instead of
    // sitting forever waiting to "arrive" at a point the NavMesh can't actually reach.
    public static class NavMeshAgentPathExtensions
    {
        public static bool HasUnreachableDestination(this NavMeshAgent agent)
        {
            return !agent.pathPending && agent.pathStatus != NavMeshPathStatus.PathComplete;
        }
    }
}
