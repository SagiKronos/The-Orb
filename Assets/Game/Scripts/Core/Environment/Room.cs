using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TheOrb.Core.Environment
{
    public class Room : MonoBehaviour
    {
        [SerializeField] Light[] lights;
        [SerializeField] GameObject[] gameObjects;


        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
                TurnLights(true);
        }


        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
                TurnLights(false);
        }

        private void TurnLights(bool isOn)
        {
            foreach (var light in lights)
            {
                light.enabled = isOn;
            }

            foreach (var gameObj in gameObjects)
            {
                gameObj.SetActive(isOn);
            }
        }

    }
}
