using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheOrb.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Health health;
        [SerializeField] GameObject bar;
        [SerializeField] Canvas rootCanvas;

        void Update()
        {
            var healthFraction = health.GetFraction();
            var shouldBeVisible = !Mathf.Approximately(1f, healthFraction) && !Mathf.Approximately(0f, healthFraction);
            rootCanvas.enabled = shouldBeVisible;

            bar.transform.localScale = new Vector3(healthFraction, 1, 1);
        }
    }
}
