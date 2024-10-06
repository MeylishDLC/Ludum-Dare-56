using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Camera;
using Cysharp.Threading.Tasks;
using Gnomes;
using Items;
using Sound;
using Types;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

namespace Core
{
    public class GnomeSpawner : MonoBehaviour
    {

        [Header("MAIN")] 
        [SerializeField] private float percentageAmountToSpawnTwoAtOnce;
        [SerializeField] private Transform gnomeContainer;
        [SerializeField] private RoutePointPair[] routes;
        [SerializeField] private Gnome[] gnomePrefabs;

        [Header("Timers")] 
        [SerializeField] private float maxTimeBetweenSpawn;
        [SerializeField] private float minTimeBetweenSpawn;

        [Header("Unit Stuff")] 
        [SerializeField] private Tomato[] tomatoes;
        [SerializeField] private SoundButton[] soundButtons; 
        
        private CancellationTokenSource _cancelSpawningCycle = new();

        private Flashlight _flashlight;
        private Screamer _screamer;
        private CameraMovement _cameraMovement;
        private SoundManager _soundManager;

        [Inject]
        public void Initialize(Screamer screamer, Flashlight flashlight, CameraMovement cameraMovement, 
            SoundManager soundManager)
        {
            _screamer = screamer;
            _flashlight = flashlight;
            _cameraMovement = cameraMovement;
            _soundManager = soundManager;
        }
        private void Start()
        {
            StartSpawningSequence(_cancelSpawningCycle.Token).Forget();
        }

        public void StopSpawning()
        {
            _cancelSpawningCycle.Cancel();
            _cancelSpawningCycle.Dispose();
            Destroy(gameObject);
        }
        private async UniTask StartSpawningSequence(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var randomTime = Random.Range(minTimeBetweenSpawn, maxTimeBetweenSpawn);
                await UniTask.Delay(TimeSpan.FromSeconds(randomTime), cancellationToken: token);
                
                var randomValue = Random.Range(0f, 100f);
            
                if (randomValue <= percentageAmountToSpawnTwoAtOnce)
                {
                    CheckSpawnGnome();
                    CheckSpawnGnome();
                }
                else
                {
                    CheckSpawnGnome();
                }
            }
        }
        private void CheckSpawnGnome()
        {
            var freeRoute = GetFreeRoute();
            if (freeRoute == null) return;

            var roomType = freeRoute.RoomType;
            PlaySound(roomType);
            SpawnInRoom(roomType, freeRoute);
        }
        private void SpawnInRoom(RoomTypes roomType, RoutePointPair freeRoute)
        {
            if (TryFindGnomeByType(GetGnomeTypesForRoom(roomType), out var gnome))
            {
                if (gnome is Tomatozilla tomatozilla)
                {
                    SpawnTomatozilla(tomatozilla, freeRoute, roomType);
                }
                if (gnome is RadioBass radioBass)
                {
                    SpawnRadioBass(radioBass, freeRoute);
                }
                if (gnome is Spoonkin spoonkin)
                {
                    SpawnSpoonkin(spoonkin, freeRoute);
                }
                if (gnome is Radiozilla radiozilla)
                {
                    SpawnRadiozilla(radiozilla, freeRoute, roomType);
                }
            }
        }

        private void SpawnSpoonkin(Spoonkin gnome, RoutePointPair freeRoute)
        {
            var spawnedGnome = Instantiate(gnome, freeRoute.FurtherPoint.position, Quaternion.identity);
            spawnedGnome.gameObject.transform.SetParent(gnomeContainer);

            spawnedGnome.Initialize(freeRoute, _screamer, _flashlight, _cameraMovement, _soundManager);
            freeRoute.IsReserved = true;
        }

        private void SpawnTomatozilla(Tomatozilla tomatozilla, RoutePointPair freeRoute, RoomTypes roomType)
        {
            var spawnedGnome = Instantiate(tomatozilla, freeRoute.FurtherPoint.position, Quaternion.identity);
            spawnedGnome.gameObject.transform.SetParent(gnomeContainer);

            if (roomType == RoomTypes.HospitalRoomRight)
            {
                spawnedGnome.Initialize(freeRoute, _screamer, _flashlight, _cameraMovement, _soundManager, tomatoes[0]);
            }
            if (roomType == RoomTypes.HospitalRoomLeft)
            {
                spawnedGnome.Initialize(freeRoute, _screamer, _flashlight, _cameraMovement, _soundManager, tomatoes[1]);
            }
            
            freeRoute.IsReserved = true;
        }
        private void SpawnRadioBass(RadioBass radioBass, RoutePointPair freeRoute)
        {
            var spawnedGnome = Instantiate(radioBass, freeRoute.FurtherPoint.position, Quaternion.identity);
            spawnedGnome.gameObject.transform.SetParent(gnomeContainer);
            
            spawnedGnome.Initialize(freeRoute, _screamer, _flashlight, _cameraMovement, _soundManager, soundButtons);
            freeRoute.IsReserved = true;
        }
        private void SpawnRadiozilla(Radiozilla radiozilla, RoutePointPair freeRoute, RoomTypes roomType)
        {
            var spawnedGnome = Instantiate(radiozilla, freeRoute.FurtherPoint.position, Quaternion.identity);
            spawnedGnome.gameObject.transform.SetParent(gnomeContainer);

            if (roomType == RoomTypes.HospitalRoomRight)
            {
                spawnedGnome.Initialize(freeRoute, _screamer, _flashlight, _cameraMovement, _soundManager, tomatoes[0], soundButtons);
            }
            if (roomType == RoomTypes.HospitalRoomLeft)
            {
                spawnedGnome.Initialize(freeRoute, _screamer, _flashlight, _cameraMovement, _soundManager, tomatoes[1], soundButtons);
            }
            
            freeRoute.IsReserved = true;
        }
        private bool TryFindGnomeByType(GnomeTypes[] types, out Gnome appealingGnome)
        {
            appealingGnome = null;
            var gnomes = gnomePrefabs.Where(gnome => types.Contains(gnome.GnomeType)).ToList();

            if (!gnomes.Any())
            {
                return false;
            }

            appealingGnome = gnomes[Random.Range(0, gnomes.Count)];
            return true;
        }
        private GnomeTypes[] GetGnomeTypesForRoom(RoomTypes roomType)
        {
            return roomType switch
            {
                RoomTypes.HospitalRoomLeft => new[]
                    {GnomeTypes.RadioBassLeft, GnomeTypes.TomatozillaLeft, GnomeTypes.RadiozillaLeft},
                
                RoomTypes.HospitalRoomRight => new[]
                    {GnomeTypes.RadioBassRight, GnomeTypes.TomatozillaRight, GnomeTypes.RadiozillaRight},
                
                RoomTypes.CorridorCeiling => new[] {GnomeTypes.RadioBass},
                
                RoomTypes.CorridorFloor => new[] {GnomeTypes.Spoonkin},
                
                _ => Array.Empty<GnomeTypes>()
            };
        }
        private RoutePointPair GetFreeRoute()
        {
            var freeRoutes = routes.Where(route => !route.IsReserved).ToList();
            return freeRoutes.Any() ? freeRoutes[Random.Range(0, freeRoutes.Count)] : null;
        }

        private void PlaySound(RoomTypes roomType)
        {
            if (roomType == RoomTypes.HospitalRoomLeft)
            {
                _soundManager.PlayOneShot(_soundManager.FMODEvents.OpeningDoorLeft);
            }
            if (roomType == RoomTypes.HospitalRoomRight)
            {
                _soundManager.PlayOneShot(_soundManager.FMODEvents.OpeningDoorRight);
            }
            if (roomType == RoomTypes.CorridorFloor)
            {
                _soundManager.PlayOneShot(_soundManager.FMODEvents.Footsteps);
            }
        }
    }
}