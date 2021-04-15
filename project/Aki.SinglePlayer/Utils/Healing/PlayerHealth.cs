using System.Collections.Generic;

namespace Aki.SinglePlayer.Utils.Healing
{
    public enum BodyPartEffect
    {
        Fracture,
        LightBleeding,
        HeavyBleeding
    }

    public class BodyPartHealth
    {
        private Dictionary<BodyPartEffect, float> _effects = new Dictionary<BodyPartEffect, float>();
        public float Maximum { get; private set; }
        public float Current { get; private set; }

        public IReadOnlyDictionary<BodyPartEffect, float> Effects => _effects;

        public void Initialize(float current, float maximum)
        {
            Maximum = maximum;
            Current = current;
        }

        public void ChangeHealth(float diff)
        {
            Current += diff;
        }

        public void AddEffect(BodyPartEffect bodyPartEffect, float time = -1)
        {
            _effects[bodyPartEffect] = time;
        }

        public void RemoveEffect(BodyPartEffect bodyPartEffect)
        {
            if (_effects.ContainsKey(bodyPartEffect))
            {
                _effects.Remove(bodyPartEffect);
            }
        }
    }

    public class PlayerHealth
    {
        private readonly Dictionary<EBodyPart, BodyPartHealth> _health = new Dictionary<EBodyPart, BodyPartHealth>() {
            { EBodyPart.Head, new BodyPartHealth() },
            { EBodyPart.Chest, new BodyPartHealth() },
            { EBodyPart.Stomach, new BodyPartHealth() },
            { EBodyPart.LeftArm, new BodyPartHealth() },
            { EBodyPart.RightArm, new BodyPartHealth() },
            { EBodyPart.LeftLeg, new BodyPartHealth() },
            { EBodyPart.RightLeg, new BodyPartHealth() }
        };

        public bool IsAlive { get; set; } = true;

        public IReadOnlyDictionary<EBodyPart, BodyPartHealth> Health => _health;
        
        public float Hydration { get; set; }

        public float Energy { get; set; }

        public float Temperature { get; set; }
    }
}
