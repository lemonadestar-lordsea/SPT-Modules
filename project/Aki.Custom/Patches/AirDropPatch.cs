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
        private static GameWorld gameWorld = null;
        private static int points = 0;

        protected override MethodBase GetTargetMethod()
        {
            return typeof(GameWorld).GetMethod("OnGameStarted", BindingFlags.Public | BindingFlags.Instance);
        }

        [PatchPostfix]
        public static void PatchPostFix()
        {
            gameWorld = Singleton<GameWorld>.Instance;
            points = LocationScene.GetAll<AirdropPoint>().Count();

            if (gameWorld != null && points != 0)
            {
                gameWorld.GetOrAddComponent<AirDrop>();
            }
        }
    }

    public class AirDrop : MonoBehaviour
    {
        private SynchronizableObject plane;
        private SynchronizableObject box;
        private bool planeEnabled;
        private bool boxEnabled;
        private int amountDropped;
        private int dropChance;
        private List<AirdropPoint> airdropPoints;
        private AirdropPoint randomAirdropPoint;
        private int boxObjId;
        private Vector3 boxPosition;
        private Vector3 planeStartPosition;
        private Vector3 planeStartRotation;
        private int planeObjId;
        private float planePositivePosition;
        private float planeNegativePosition;
        private float defaultDropHeight;
        private float timer;
        private float timeToDrop;
        private bool doNotRun;
        private GameWorld gameWorld;

        public void Awake(SynchronizableObject planes) // https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html - this method is another form of Ctor
        {
            planeEnabled = false;
            boxEnabled = false;
            dropChance = 100; // 20
            boxObjId = 10;
            planePositivePosition = 3000f;
            planeNegativePosition = -3000f;
            defaultDropHeight = 400f;
            doNotRun = false;
            timeToDrop = RandomChanceGen(60, 70); // 60, 900
            planeObjId = RandomChanceGen(1, 4);
            plane = LocationScene.GetAll<SynchronizableObject>().First(x => x.name.Contains("IL76MD-90"));
            box = LocationScene.GetAll<SynchronizableObject>().First(x => x.name.Contains("scontainer_airdrop_box_04"));
            airdropPoints = LocationScene.GetAll<AirdropPoint>().ToList();
            randomAirdropPoint = airdropPoints.OrderBy(_ => Guid.NewGuid()).FirstOrDefault();
            gameWorld = Singleton<GameWorld>.Instance;
        }

        public void FixedUpdate() // https://docs.unity3d.com/ScriptReference/MonoBehaviour.FixedUpdate.html
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
                plane.transform.Translate(Vector3.forward, Space.Self);

                switch (planeObjId)
                {
                    case 1:
                        if (plane.transform.position.z >= planePositivePosition && planeEnabled)
                        {
                            DisablePlane();
                        }

                        if (plane.transform.position.z >= randomAirdropPoint.transform.position.z && !boxEnabled)
                        {
                            InitDrop();
                        }
                        break;
                    case 2:
                        if (plane.transform.position.x >= planePositivePosition && planeEnabled)
                        {
                            DisablePlane();
                        }

                        if (plane.transform.position.x >= randomAirdropPoint.transform.position.x && !boxEnabled)
                        {
                            InitDrop();
                        }
                        break;
                    case 3:
                        if (plane.transform.position.z <= planeNegativePosition && planeEnabled)
                        {
                            DisablePlane();
                        }
                        if (plane.transform.position.z <= randomAirdropPoint.transform.position.z && !boxEnabled)
                        {
                            InitDrop();
                        }
                        break;
                    case 4:
                        if (plane.transform.position.x <= planeNegativePosition && planeEnabled)
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
            return RandomChanceGen(1, 99) <= dropChance;
        }

        public void DoNotRun() // currently not doing anything, could be used later for multiple drops
        {
            doNotRun = true;
        }

        public int RandomChanceGen(int minValue, int maxValue)
        {
            System.Random chance = new System.Random();
            return chance.Next(minValue, maxValue);
        }

        public void ScriptStart()
        {
            if (!ShouldAirdropOccur())
            {
                DoNotRun();
                return;
            }

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

        public void InitPlane()
        {
            planeEnabled = true;
            plane.TakeFromPool();
            plane.Init(planeObjId, planeStartPosition, planeStartRotation);
            plane.transform.LookAt(boxPosition);
            plane.ManualUpdate(0);

            var sound = plane.GetComponentInChildren<AudioSource>();
            sound.volume = 1f;
            sound.dopplerLevel = 0;
            sound.Play();
        }

        public void InitDrop()
        {
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
    }
}
