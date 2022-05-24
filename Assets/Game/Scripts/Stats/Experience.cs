using UnityEngine;
using TheOrb.Saving;
using System;

namespace TheOrb.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float expPoints = 0;
        public float ExpPoints { get { return expPoints; } }

        public event Action onExpGained;

        public void GainExp(float exp)
        {
            expPoints += exp;
            onExpGained();
        }

        public object CaptureState()
        {
            return expPoints;
        }

        public void RestoreState(object state)
        {
            expPoints = (float)state;
        }
    }
}
