using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace TheOrb.Core
{
    public class AnimationEventListner : MonoBehaviour
    {
        [SerializeField] UnityEvent onHit;
        [SerializeField] UnityEvent launchSkill;

        private void Hit()
        {
            if (onHit != null)
                onHit.Invoke();
        }

        private void LaunchSkill()
        {
            if (launchSkill != null)
                launchSkill.Invoke();
        }
    }
}
