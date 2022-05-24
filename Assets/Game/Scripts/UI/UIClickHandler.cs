using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace TheOrb.UI
{
    public class UIClickHandler<T> : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] protected T data;
        [SerializeField] public UnityEvent<T> rightClicked;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
                rightClicked.Invoke(data);
        }
    }
}
