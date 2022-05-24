using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace TheOrb.UI.Skills
{
    public class TypelessUIClickHandler : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] public UnityEvent rightClicked;
        [SerializeField] public UnityEvent leftClicked;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
                rightClicked.Invoke();
            if (eventData.button == PointerEventData.InputButton.Left)
                leftClicked.Invoke();
        }
    }
}
