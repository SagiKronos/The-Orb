using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheOrb.Core.Environment
{
    public class Wall : MonoBehaviour
    {
        [SerializeField] MeshRenderer topPart;
        [SerializeField] float rangeOfHiding = 5f;
        Transform player;

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void Update()
        {
            topPart.gameObject.SetActive(transform.rotation.y % 180 == 0 || transform.position.x > player.position.x || Mathf.Abs(transform.position.z - player.position.z) > rangeOfHiding);
        }
    }
}