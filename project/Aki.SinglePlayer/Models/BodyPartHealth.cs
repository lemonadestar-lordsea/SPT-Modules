﻿using System.Collections.Generic;

namespace Aki.SinglePlayer.Models
{
    public class BodyPartHealth
    {
        private Dictionary<EBodyPartEffect, float> _effects = new Dictionary<EBodyPartEffect, float>();
        public float Maximum { get; private set; }
        public float Current { get; private set; }

        public IReadOnlyDictionary<EBodyPartEffect, float> Effects => _effects;

        public void Initialize(float current, float maximum)
        {
            Maximum = maximum;
            Current = current;
        }

        public void ChangeHealth(float diff)
        {
            Current += diff;
        }

        public void AddEffect(EBodyPartEffect bodyPartEffect, float time = -1)
        {
            _effects[bodyPartEffect] = time;
        }

        public void RemoveEffect(EBodyPartEffect bodyPartEffect)
        {
            if (_effects.ContainsKey(bodyPartEffect))
            {
                _effects.Remove(bodyPartEffect);
            }
        }
    }
}
