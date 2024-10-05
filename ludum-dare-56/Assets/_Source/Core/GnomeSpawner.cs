using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gnomes;
using Types;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Core
{
    public class GnomeSpawner: MonoBehaviour
    {
        [SerializeField] private Image screamerImage;
        [SerializeField] private RoutePointPair[] routes;
        [SerializeField] private Gnome[] gnomePrefabs;

        [Header("Timers")] 
        [SerializeField] private float timeBetweenSpawn;
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
                //Debug.Log("Tried to spawn");
            }
        }
        private void CheckSpawnGnome()
        {
            var freeRoute = GetFreeRoute();
            if (freeRoute == null)
            {
                return;
            }
            
            var roomType = freeRoute.RoomType;
            if (roomType == RoomTypes.HospitalRoomLeft)
            {
                if (TryFindGnomeByType(new [] 
                        {GnomeTypes.RadioBassLeft, GnomeTypes.TomatozillaLeft, GnomeTypes.TomatozillaRadioBassPairLeft}, out var gnome))
                {
                    SpawnGnome(gnome, freeRoute);
                }
            }
            if (roomType == RoomTypes.HospitalRoomRight)
            {
                if (TryFindGnomeByType(new [] 
                        {GnomeTypes.RadioBassRight, GnomeTypes.TomatozillaRight, GnomeTypes.TomatozillaRadioBassPairRight}, out var gnome))
                {
                    SpawnGnome(gnome, freeRoute);
                }
            }
            
            if (roomType == RoomTypes.CorridorCeiling)
            {
                if (TryFindGnomeByType(GnomeTypes.RadioBass, out var gnome))
                {
                    SpawnGnome(gnome, freeRoute);
                }
            }
            if (roomType == RoomTypes.CorridorFloor)
            {
                if (TryFindGnomeByType(GnomeTypes.Spoonkin, out var gnome))
                {
                    SpawnGnome(gnome, freeRoute);
                }
            }
        }

        private void SpawnGnome(Gnome gnome, RoutePointPair freeRoute)
        {
            var spawnedGnome = Instantiate(gnome, freeRoute.FurtherPoint.position, Quaternion.identity);
            spawnedGnome.Initialize(freeRoute, screamerImage);
            freeRoute.IsReserved = true;
        }
        private bool TryFindGnomeByType(GnomeTypes[] types, out Gnome appealingGnome)
        {
            appealingGnome = null;
            var gnomes = new List<Gnome>();
            foreach (var gnome in gnomePrefabs)
            {
                foreach (var type in types)
                {
                    if (gnome.GnomeType == type && !gnomes.Contains(gnome))
                    {
                        gnomes.Add(gnome);
                    }
                }
            }

            if (!gnomes.Any())
            {
                return false;
            }

            var randomGnome = gnomes[Random.Range(0, gnomes.Count - 1)];
            appealingGnome = randomGnome;
            return true;
        }
        private bool TryFindGnomeByType(GnomeTypes type, out Gnome appealingGnome)
        {
            appealingGnome = null;
            var gnomes = new List<Gnome>();
            foreach (var gnome in gnomePrefabs)
            {
                if (gnome.GnomeType == type && !gnomes.Contains(gnome))
                {
                    gnomes.Add(gnome);
                }
            }

            if (!gnomes.Any())
            {
                return false;
            }

            var randomGnome = gnomes[Random.Range(0, gnomes.Count - 1)];
            appealingGnome = randomGnome;
            return true;
        }

        private RoutePointPair GetFreeRoute()
        {
            var freeRoutes = new List<RoutePointPair>();
            foreach (var route in routes)
            {
                if (!route.IsReserved)
                {
                    freeRoutes.Add(route);
                }
            }

            if (!freeRoutes.Any())
            {
                return null;
            }
            
            var randomRoute = freeRoutes[Random.Range(0, routes.Length - 1)];
            return randomRoute;
        }
    }
}