using FMODUnity;
using UnityEngine;

namespace Sound
{
    [CreateAssetMenu]
    public class FMODEvents: ScriptableObject
    {
        [field: Header("Background Music")]
        [field: SerializeField] public EventReference GameMusic { get; private set; }
        
        [field: Header("SFX")]
        [field: SerializeField] public EventReference FlashlightSound { get; private set; }
        
    }
}