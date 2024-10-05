using Types;
using UnityEngine;

namespace Core
{
    [System.Serializable]
    public class RoutePointPair
    {
        [field: SerializeField] public Transform FurtherPoint { get; private set; }
        [field: SerializeField] public Transform CloserPoint { get; private set; }
        [field: SerializeField] public RoomTypes RoomType { get; private set; }
        public bool IsReserved { get; set; }
    }
}