using Core;
using Items;
using UnityEngine.UI;

namespace Gnomes
{
    public class Spoonkin: Gnome
    {
        private Flashlight _flashlight;

        public void Initialize(RoutePointPair routePointPair, Image screamerUIImage, Flashlight flashlight)
        {
            _flashlight = flashlight;
            base.Initialize(routePointPair, screamerUIImage);
        }
        
        private float _remainingTimeToHold;
    }
}