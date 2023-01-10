using UnityEditor;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "Game/Constants")]
    public class Constants : ScriptableSingleton<Constants>
    {
        [field: Header("Development")]
        [field: SerializeField] public bool DebugLog { get; private set; } = true;
        [field: SerializeField] public bool DebugGizmos { get; private set; } = true;
        
        [field: Header("Unit Views")]
        [field: SerializeField] public float AgentsAngularSpeed { get; private set; } = 120;
        [field: SerializeField] public float DetectingGroundRayLength { get; private set; } = 2;
        [field: SerializeField] public LayerMask DetectingGroundLayerMask { get; private set; } = 1 | (1 << 4);
        [field: SerializeField] public float LegsIKRayLength { get; private set; } = 10;
        [field: SerializeField] public float LegsIKRayCastHeight { get; private set; } = 5;
        [field: SerializeField] public float AttackDistance { get; private set; } = 2.5f;
        [field: SerializeField] public float AnimationLerpSpeed { get; private set; } = 10f;
        [field: SerializeField] public float AnimationLayerTransitionSpeed { get; private set; } = 5f;
        
        [field: Header("Camera")]
        [field: SerializeField] public float RayCastHeight { get; private set; } = 1000;
        [field: SerializeField] public float RayLength { get; private set; } = 1500;
        [field: SerializeField] public float CameraCenterHeight { get; private set; } = 1f;
        
        [field: Header("Commands")]
        [field: SerializeField] public float MovingStopDistance { get; private set; } = 2f;
        [field: SerializeField] public float AttackMinDistanceKeeping { get; private set; } = 2f;
        [field: SerializeField] public float AttackMaxDistanceKeeping { get; private set; } = 2.5f;
        
    }
}
