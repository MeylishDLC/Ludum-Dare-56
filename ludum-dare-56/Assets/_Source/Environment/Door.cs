using System;
using Core;
using Gnomes;
using UnityEngine;

namespace Environment
{
    public class Door: MonoBehaviour
    {
        private enum Side
        {
            Left,
            Right
        }
        [SerializeField] private Side side;
        private void Start()
        {
            RadioBass.OnDespawnInDoors += EnableDoor;
            RadioBass.OnSpawnInDoors += DisableDoor;

            Radiozilla.OnDespawnInDoors += EnableDoor;
            Radiozilla.OnSpawnInDoors += DisableDoor;
            
            Tomatozilla.OnDespawnInDoors += EnableDoor;
            Tomatozilla.OnSpawnInDoors += DisableDoor;
        }
        private void OnDestroy()
        {
            RadioBass.OnDespawnInDoors -= EnableDoor;
            RadioBass.OnSpawnInDoors -= DisableDoor;
            
            Radiozilla.OnDespawnInDoors -= EnableDoor;
            Radiozilla.OnSpawnInDoors -= DisableDoor;

            Tomatozilla.OnDespawnInDoors -= EnableDoor;
            Tomatozilla.OnSpawnInDoors -= DisableDoor;
        }
        private void DisableDoor(Gnome gnome)
        {
            if ((gnome.GnomeType == GnomeTypes.RadioBassLeft 
                 || gnome.GnomeType == GnomeTypes.TomatozillaLeft 
                 || gnome.GnomeType == GnomeTypes.RadiozillaLeft)
                && side == Side.Left)
            {
                gameObject.SetActive(false);
            }

            if ((gnome.GnomeType == GnomeTypes.RadiozillaRight
                || gnome.GnomeType == GnomeTypes.TomatozillaRight
                || gnome.GnomeType == GnomeTypes.RadioBassRight)
                && side == Side.Right)
            {
                gameObject.SetActive(false);
            }
        }
        private void EnableDoor(Gnome gnome)
        {
            if ((gnome.GnomeType == GnomeTypes.RadioBassLeft 
                 || gnome.GnomeType == GnomeTypes.TomatozillaLeft 
                 || gnome.GnomeType == GnomeTypes.RadiozillaLeft)
                && side == Side.Left)
            {
                gameObject.SetActive(true);
            }

            if ((gnome.GnomeType == GnomeTypes.RadiozillaRight
                || gnome.GnomeType == GnomeTypes.TomatozillaRight
                || gnome.GnomeType == GnomeTypes.RadioBassRight)
                && side == Side.Right)
            {
                gameObject.SetActive(true);
            }
        }
    }
}