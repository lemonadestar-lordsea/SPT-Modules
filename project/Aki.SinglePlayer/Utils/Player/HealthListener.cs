/* HealthListener.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Ginja
 * Merijn Hendriks
 */


using System;
using System.Collections.Generic;
using UnityEngine;
using Aki.Loader;
using Aki.SinglePlayer.Utils;
using IHealthController = GInterface171;
using DamageInfo = GStruct240;
using IEffect = GInterface130;
using Aki.Common.Utils.App;

namespace Aki.SinglePlayer.Utils.Player
{
    class HealthListener
    {
        private static object _lock = new object();
        private static HealthListener _instance = null;
        private IHealthController _healthController;
        private IDisposable _disposable = null;
        private readonly SimpleTimer _simpleTimer;

        public PlayerHealth CurrentHealth { get; }

        public static HealthListener Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new HealthListener();
                        }
                    }
                }

                return _instance;
            }
        }

        // ctor
        private HealthListener()
        {
            if (CurrentHealth == null)
            {
                CurrentHealth = new PlayerHealth();
            }
            
            _simpleTimer = Target.HookObject.GetOrAddComponent<SimpleTimer>();
        }

        /// <summary>
        /// Initialize HealthListener.
        /// This method is executed on loading profile in menu (on load game, on raid finish, on error...),
        /// and on raid start
        /// </summary>
        /// <param name="healthController">player health controller</param>
        /// <param name="inRaid">true - when executed from raid</param>
        public void Init(IHealthController healthController, bool inRaid)
        {
            // cleanup
            if (_disposable != null)
            {
                _disposable.Dispose();
            }

            // init dependencies
            _healthController = healthController;
            _simpleTimer.isSyncHealthEnabled = !inRaid;
            CurrentHealth.IsAlive = true;

            // init current health
            SetCurrentHealth(_healthController, CurrentHealth.Health, EBodyPart.Head);
            SetCurrentHealth(_healthController, CurrentHealth.Health, EBodyPart.Chest);
            SetCurrentHealth(_healthController, CurrentHealth.Health, EBodyPart.Stomach);
            SetCurrentHealth(_healthController, CurrentHealth.Health, EBodyPart.LeftArm);
            SetCurrentHealth(_healthController, CurrentHealth.Health, EBodyPart.RightArm);
            SetCurrentHealth(_healthController, CurrentHealth.Health, EBodyPart.LeftLeg);
            SetCurrentHealth(_healthController, CurrentHealth.Health, EBodyPart.RightLeg);

            CurrentHealth.Energy = _healthController.Energy.Current;
            CurrentHealth.Hydration = _healthController.Hydration.Current;
            CurrentHealth.Temperature = _healthController.Temperature.Current;

            // subscribe to events
            _healthController.DiedEvent += OnDiedEvent;
            _healthController.HealthChangedEvent += OnHealthChangedEvent;
            _healthController.EffectAddedEvent += OnEffectAddedEvent;
            _healthController.EffectRemovedEvent += OnEffectRemovedEvent;
            _healthController.HydrationChangedEvent += OnHydrationChangedEvent;
            _healthController.EnergyChangedEvent += OnEnergyChangedEvent;
            _healthController.TemperatureChangedEvent += OnTemperatureChangedEvent;

            // don't forget to unsubscribe
            _disposable = new Disposable(() =>
            {
                _healthController.DiedEvent -= OnDiedEvent;
                _healthController.HealthChangedEvent -= OnHealthChangedEvent;
                _healthController.EffectAddedEvent -= OnEffectAddedEvent;
                _healthController.EffectRemovedEvent -= OnEffectRemovedEvent;
                _healthController.HydrationChangedEvent -= OnHydrationChangedEvent;
                _healthController.EnergyChangedEvent -= OnEnergyChangedEvent;
            });
        }

        private void SetCurrentHealth(IHealthController healthController, IReadOnlyDictionary<EBodyPart, BodyPartHealth> dictionary, EBodyPart bodyPart)
        {
            var bodyPartHealth = healthController.GetBodyPartHealth(bodyPart);
            dictionary[bodyPart].Initialize(bodyPartHealth.Current, bodyPartHealth.Maximum);

            // set effects
            if (healthController.IsBodyPartBroken(bodyPart))
            {
                dictionary[bodyPart].AddEffect(BodyPartEffect.Fracture);
            }
            else
            {
                dictionary[bodyPart].RemoveEffect(BodyPartEffect.Fracture);
            }
        }

        private void OnDiedEvent(EFT.EDamageType obj)
        {
            CurrentHealth.IsAlive = false;
        }

        public void OnHealthChangedEvent(EBodyPart bodyPart, float diff, DamageInfo effect)
        {
            CurrentHealth.Health[bodyPart].ChangeHealth(diff);
            _simpleTimer.isHealthSynchronized = false;
        }

        public void OnEffectAddedEvent(IEffect effect)
        {
            if (effect == null)
            {
                return;
            }

            string effectType = effect.GetType().Name;

            if (effectType != "Fracture")
            {
                return;
            }

            CurrentHealth.Health[effect.BodyPart].AddEffect(BodyPartEffect.Fracture);
            _simpleTimer.isHealthSynchronized = false;
        }

        public void OnEffectRemovedEvent(IEffect effect)
        {
            if (effect == null)
            {
                return;
            }

            string effectType = effect.GetType().Name;

            if (effectType != "Fracture")
            {
                return;
            }

            CurrentHealth.Health[effect.BodyPart].RemoveEffect(BodyPartEffect.Fracture);
            _simpleTimer.isHealthSynchronized = false;
        }


        public void OnHydrationChangedEvent(float diff)
        {
            CurrentHealth.Hydration += diff;
            _simpleTimer.isHealthSynchronized = false;
        }

        public void OnEnergyChangedEvent(float diff)
        {
            CurrentHealth.Energy += diff;
            _simpleTimer.isHealthSynchronized = false;
        }

        public void OnTemperatureChangedEvent(float diff)
        {
            CurrentHealth.Temperature += diff;
            _simpleTimer.isHealthSynchronized = false;
        }

        class Disposable : IDisposable
        {
            private readonly Action _onDispose;

            public Disposable(Action onDispose)
            {
                _onDispose = onDispose ?? throw new ArgumentNullException(nameof(onDispose));
            }

            public void Dispose()
            {
                _onDispose();
            }
        }

        class SimpleTimer : MonoBehaviour
        {
            // tick each 5 seconds
            float sleepTime = 5f;
            float timer = 0f;

            public bool isSyncHealthEnabled = false;
            public bool isHealthSynchronized = false;

            void Update()
            {
                timer += Time.deltaTime;

                if (timer > sleepTime)
                {
                    timer -= sleepTime;

                    if (isSyncHealthEnabled && !isHealthSynchronized)
                    {
                        RequestHandler.SynchroniseHealth(Instance.CurrentHealth.ToJson());
                        isHealthSynchronized = true;
                    }
                }
            }
        }
    }
}
