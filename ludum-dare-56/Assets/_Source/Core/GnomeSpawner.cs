using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gnomes;
using Items;
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
        [SerializeField] private RoutePointPair[] routes;
        [SerializeField] private Gnome[] gnomePrefabs;

        [Header("Timers")] [SerializeField] private float timeBetweenSpawn;

        [Header("Unit Stuff")] 
        [SerializeField] private Tomato[] tomatoes;
        [SerializeField] private SoundButton[] soundButtons; 
        
        private Flashlight _flashlight;
        private Screamer _screamer;

        [Inject]
        public void Initialize(Screamer screamer, Flashlight flashlight)
        {
            _screamer = screamer;
            _flashlight = flashlight;
        }
        private void Start()
        {
            StartSpawningSequence(CancellationToken.None).Forget();
        }
        private async UniTask StartSpawningSequence(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(timeBetweenSpawn), cancellationToken: token);
                CheckSpawnGnome();
            }
        }
        private void CheckSpawnGnome()
        {
            var freeRoute = GetFreeRoute();
            if (freeRoute == null) return;

            var roomType = freeRoute.RoomType;
            SpawnInRoom(roomType, freeRoute);
        }
        private void SpawnInRoom(RoomTypes roomType, RoutePointPair freeRoute)
        {
            if (TryFindGnomeByType(GetGnomeTypesForRoom(roomType), out var gnome))
            {
                if (gnome is Tomatozilla tomatozilla)
                {
                    SpawnTomatozilla(tomatozilla, freeRoute);
                }
                if (gnome is RadioBass radioBass)
                {
                    SpawnRadioBass(radioBass, freeRoute);
                }
                if (gnome is Spoonkin spoonkin)
                {
                    SpawnSpoonkin(spoonkin, freeRoute);
                }
            }
        }

        private void SpawnSpoonkin(Spoonkin gnome, RoutePointPair freeRoute)
        {
            var spawnedGnome = Instantiate(gnome, freeRoute.FurtherPoint.position, Quaternion.identity);
            spawnedGnome.Initialize(freeRoute, _screamer, _flashlight);
            freeRoute.IsReserved = true;
        }

        private void SpawnTomatozilla(Tomatozilla tomatozilla, RoutePointPair freeRoute)
        {
            var spawnedGnome = Instantiate(tomatozilla, freeRoute.FurtherPoint.position, Quaternion.identity);
            spawnedGnome.Initialize(freeRoute, _screamer, _flashlight, tomatoes);
            freeRoute.IsReserved = true;
        }

        private void SpawnRadioBass(RadioBass radioBass, RoutePointPair freeRoute)
        {
            var spawnedGnome = Instantiate(radioBass, freeRoute.FurtherPoint.position, Quaternion.identity);
            spawnedGnome.Initialize(freeRoute, _screamer, _flashlight, soundButtons);
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
                    {GnomeTypes.RadioBassLeft, GnomeTypes.TomatozillaLeft, GnomeTypes.TomatozillaRadioBassPairLeft},
                
                RoomTypes.HospitalRoomRight => new[]
                    {GnomeTypes.RadioBassRight, GnomeTypes.TomatozillaRight, GnomeTypes.TomatozillaRadioBassPairRight},
                
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
    }
}