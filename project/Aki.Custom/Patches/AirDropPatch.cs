using Aki.Reflection.Patching;
using EFT;
using System.Reflection;
using System;
using UnityEngine;
using Comfort.Common;
using System.Collections.Generic;
using System.Linq;

namespace Aki.Custom.Patches
{
    public class AirDropPatch : ModulePatch
    {
        private static GameObject gameObject = null;
        private static GameWorld gameWorld = null;

        protected override MethodBase GetTargetMethod()
        {
            return typeof(GameWorld).GetMethod("OnGameStarted", BindingFlags.Public | BindingFlags.Instance);
        }

        [PatchPostfix]
        public static void PatchPostFix()
        {
            if (gameObject == null)
            {
                gameObject = new GameObject
                {
                    name = "Aki.AirDrop"
                };
            }

            if (gameWorld != null)
            {

                AirDrop component = gameObject.GetComponent<AirDrop>();

                if (component == null)
                {
                    gameObject.AddComponent<AirDrop>();
                    UnityEngine.Object.DontDestroyOnLoad(gameObject);
                }
                else
                {
                    gameObject.GetComponent<AirDrop>().OnDestroy();
                    gameObject.AddComponent<AirDrop>();
                    UnityEngine.Object.DontDestroyOnLoad(gameObject);
                }
            }
        }
    }

    public class AirDrop : MonoBehaviour
    {
        public SynchronizableObject plane;
        public SynchronizableObject box;
        public bool planeEnabled;
        public bool boxEnabled;
        public int amountDropped;
        public int dropChance;
        public List<AirdropPoint> airdropPoints;
        public AirdropPoint randomAirdropPoint;
        public int boxObjId;
        public Vector3 boxPosition;
        public Vector3 planeStartPosition;
        public Vector3 planeStartRotation;
        public int planeObjId;
        public float planePositivePosition;
        public float planeNegativePosition;
        public float defaultDropHeight;
        public float timer;
        public float timeToDrop;
        public bool doNotRun;
        public GameWorld gameWorld;

        public AirDrop()
        {
            planeEnabled = false;
            boxEnabled = false;
            dropChance = 20;
            boxObjId = 10;
            planePositivePosition = 3000f;
            planeNegativePosition = -3000f;
            defaultDropHeight = 400f;
            doNotRun = false;
            timeToDrop = RandomChanceGen(60, 900);
            planeObjId = RandomChanceGen(1, 4);
            plane = LocationScene.GetAll<SynchronizableObject>().First(x => x.name.Contains("IL76MD-90"));
            box = LocationScene.GetAll<SynchronizableObject>().First(x => x.name.Contains("scontainer_airdrop_box_04"));
            airdropPoints = LocationScene.GetAll<AirdropPoint>().ToList();
            randomAirdropPoint = airdropPoints.OrderBy(_ => Guid.NewGuid()).FirstOrDefault();
            gameWorld = Singleton<GameWorld>.Instance;
        }

        public void FixedUpdate()
        {
            if (gameWorld == null)
            {
                return;
            }

            timer += 0.02f;

            if (timer >= timeToDrop && !planeEnabled && amountDropped != 1 && !doNotRun)
            {
                ScriptStart();
            }

            if (timer >= timeToDrop && planeEnabled && !doNotRun)
            {
                plane.transform.Translate(Vector3.forward, Space.Self); // transform foward on local rotation using Space.Self, vector3.forward moves forward 1f per run

                switch (planeObjId)
                {
                    case 1:
                        if (plane.transform.position.z >= planePositivePosition && planeEnabled) // spawn 1 moves along Z axis and goes in the positive direction, 
                        {
                            DisablePlane();
                        }

                        if (plane.transform.position.z >= randomAirdropPoint.transform.position.z && !boxEnabled)
                        {
                            InitDrop();
                        }
                        break;
                    case 2:
                        if (plane.transform.position.x >= planePositivePosition && planeEnabled) // spawn 2 moves along x axis and goes in the positive direction
                        {
                            DisablePlane();
                        }

                        if (plane.transform.position.x >= randomAirdropPoint.transform.position.x && !boxEnabled)
                        {
                            InitDrop();
                        }
                        break;
                    case 3:
                        if (plane.transform.position.z <= planeNegativePosition && planeEnabled) // spawn 3 moves along z axis and goes in the negative direction
                        {
                            DisablePlane();
                        }

                        if (plane.transform.position.z <= randomAirdropPoint.transform.position.z && !boxEnabled)
                        {
                            InitDrop();
                        }
                        break;
                    case 4:
                        if (plane.transform.position.x <= planeNegativePosition && planeEnabled) // spawn 2 moves along x axis and goes in the negative direction
                        {
                            DisablePlane();
                        }

                        if (plane.transform.position.x <= randomAirdropPoint.transform.position.x && !boxEnabled)
                        {
                            InitDrop();
                        }
                        break;
                }
            }
        }

        public bool ShouldAirdropOccur()
        {
            var player = gameWorld.RegisteredPlayers[0];
            if (player != null)
            {
                if (player.Location.Contains("factory") || player.Location.Contains("laboratory") || player.Location.Contains("hideout"))
                {
                    DoNotRun(); // will stay true till next map load
                    return false;
                }
            }

            return (RandomChanceGen(1, 99) <= dropChance);
        }

        public void DoNotRun()
        {
            doNotRun = false;
        }

        public int RandomChanceGen(int minValue, int maxValue)
        {
            System.Random chance = new System.Random();
            return chance.Next(minValue, maxValue);
        }


        public void ScriptStart()
        {
            if (ShouldAirdropOccur())
            {

                if (box != null)
                {
                    boxPosition = randomAirdropPoint.transform.position;
                    boxPosition.y = defaultDropHeight;
                }

                if (plane != null)
                {
                    PlanePositionGen();
                }
            }
        }

        public void InitPlane()
        {
            planeEnabled = true;
            plane.TakeFromPool();
            plane.Init(planeObjId, planeStartPosition, planeStartRotation);
            plane.transform.LookAt(boxPosition); // turns the plane to look at the box drop point
            plane.ManualUpdate(0);

            var sound = plane.GetComponentInChildren<AudioSource>();
            sound.volume = 1f;
            sound.dopplerLevel = 1;
            sound.Play();
        }

        public void InitDrop()
        {
            // this is basically a copy of the debugAirdrop script BSG has in the syncProcess
            object[] objToPass = new object[1];
            objToPass[0] = SynchronizableObjectType.AirDrop;
            gameWorld.SynchronizableObjectLogicProcessor.GetType().GetMethod("method_9", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(gameWorld.SynchronizableObjectLogicProcessor, objToPass);

            boxEnabled = true;
            box.SetLogic(new AirdropLogic2Class());
            box.ReturnToPool();
            box.TakeFromPool();
            box.Init(boxObjId, boxPosition, Vector3.zero);
        }

        public void PlanePositionGen()
        {
            switch (planeObjId)
            {
                case 1:
                    planeStartPosition = new Vector3(0, defaultDropHeight, planeNegativePosition);
                    planeStartRotation = new Vector3(0, 0, 0);
                    break;
                case 2:
                    planeStartPosition = new Vector3(planeNegativePosition, defaultDropHeight, 0);
                    planeStartRotation = new Vector3(0, 90, 0);
                    break;
                case 3:
                    planeStartPosition = new Vector3(0, defaultDropHeight, planePositivePosition);
                    planeStartRotation = new Vector3(0, 180, 0);
                    break;
                case 4:
                    planeStartPosition = new Vector3(planePositivePosition, defaultDropHeight, 0);
                    planeStartRotation = new Vector3(0, 270, 0);
                    break;
            }

            InitPlane();
        }

        public void DisablePlane()
        {
            planeEnabled = false;
            amountDropped = 1;
            plane.ReturnToPool();
        }

        public void OnDestroy()
        {
            Destroy(this);
        }
    }
}
