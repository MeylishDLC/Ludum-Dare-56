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
        [field: SerializeField] public EventReference Footsteps { get; private set; }
        [field: SerializeField] public EventReference KnockingLeft { get; private set; }
        [field: SerializeField] public EventReference KnockingRight { get; private set; }
        [field: SerializeField] public EventReference OpeningDoorLeft { get; private set; }
        [field: SerializeField] public EventReference OpeningDoorRight { get; private set; }
        [field: SerializeField] public EventReference Newspaper { get; private set; }
        
        [field: Header("Items")]
        [field: SerializeField] public EventReference FlashlightSound { get; private set; }
        [field: SerializeField] public EventReference RightButton { get; private set; }
        [field: SerializeField] public EventReference LeftButton { get; private set; }
        [field: SerializeField] public EventReference LeftTomato { get; private set; }
        [field: SerializeField] public EventReference RightTomato { get; private set; }
        
        [field: Header("RadioBass")]
        [field: SerializeField] public EventReference RadioBassScreamer { get; private set; }
        [field: SerializeField] public EventReference LaughHall { get; private set; }
        [field: SerializeField] public EventReference LaughLeft { get; private set; }
        [field: SerializeField] public EventReference LaughRight { get; private set; }
        [field: SerializeField] public EventReference FarGlitch { get; private set; }
        [field: SerializeField] public EventReference CloseGlitch { get; private set; }
        
        [field: Header("Tomatozilla")]
        [field: SerializeField] public EventReference TomatozillaScreamer { get; private set; }
        [field: SerializeField] public EventReference AppearRight { get; private set; }
        [field: SerializeField] public EventReference AppearLeft { get; private set; }
        [field: SerializeField] public EventReference EatingRight { get; private set; }
        [field: SerializeField] public EventReference EatingLeft { get; private set; }
        
        [field: Header("Spoonkin")]
        [field: SerializeField] public EventReference SpoonkinScreamer { get; private set; }
        [field: SerializeField] public EventReference Shocked { get; private set; }
        
        [field: Header("Radiozilla")]
        [field: SerializeField] public EventReference PairAppearLeft { get; private set; }
        [field: SerializeField] public EventReference PairAppearRight { get; private set; }
    }
}