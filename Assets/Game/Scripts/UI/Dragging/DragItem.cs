using TheOrb.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TheOrb.UI.Dragging
{
    public class DragItem<T> : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler where T : class
    {
        Vector3 startPosition;
        Transform originalParent;
        IDragSource<T> source;

        Canvas parentCanvas;

        private void Awake()
        {
            parentCanvas = GetComponentInParent<Canvas>();
            source = GetComponentInParent<IDragSource<T>>();
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.position;
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            UIInteractionBlockers.AddToBag(this);
            startPosition = transform.position;
            originalParent = transform.parent;

            GetComponent<CanvasGroup>().blocksRaycasts = false; // To enable drop
            transform.SetParent(parentCanvas.transform, true);    
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            transform.position = startPosition;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            transform.SetParent(originalParent, true);
            transform.SetAsFirstSibling();

            IDragDestination<T> container;

            if (!EventSystem.current.IsPointerOverGameObject())
            {
                container = parentCanvas.GetComponent<IDragDestination<T>>();
            }
            else
            {
                container = GetContainer(eventData);
            }

            if (container != null)
            {
                DropIntoContainer(container);
            }
            UIInteractionBlockers.RemoveBlocker(this);
        }

        private IDragDestination<T> GetContainer(PointerEventData eventData)
        {
            if (eventData.pointerEnter)
            {
                return eventData.pointerEnter.GetComponentInParent<IDragDestination<T>>();
            }

            return null;
        }

        private void DropIntoContainer(IDragDestination<T> dest)
        {
            if (ReferenceEquals(dest, source)) return;

            var destContainer = dest as IDragContainer<T>;
            var sourceContainer = source as IDragContainer<T>;
            
            if (destContainer == null || sourceContainer == null || destContainer.GetItem() == null || destContainer.GetItem() == sourceContainer.GetItem())
            {
                TryTransfer(dest);
            }
            else
            {
                TrySwap(sourceContainer, destContainer);
            }
        }

        private void TryTransfer(IDragDestination<T> dest)
        {
            var item = source.GetItem();
            var itemAmount = source.GetAmount();
            var acceptedAmount = dest.MaxAcceptable(item);
            var transferAmount = Mathf.Min(itemAmount, acceptedAmount);

            if (transferAmount > 0)
            {
                source.RemoveItems(transferAmount);
                dest.AddItems(item,transferAmount);
            }
        }

        private void TrySwap(IDragContainer<T> source, IDragContainer<T> dest)
        {
            var sourceOriginalItem = source.GetItem();
            var sourceOriginalAmount = source.GetAmount();
            var destOriginalItem = dest.GetItem();
            var destOriginalAmount = dest.GetAmount();

            source.RemoveItems(sourceOriginalAmount);
            dest.RemoveItems(destOriginalAmount);

            var sourceTakeBackAmount = CalculateTakeBack(sourceOriginalItem, sourceOriginalAmount, source, dest);
            var destTakeBackAmount = CalculateTakeBack(destOriginalItem, destOriginalAmount, dest, source);

            if (sourceTakeBackAmount > 0)
            {
                source.AddItems(sourceOriginalItem, sourceTakeBackAmount);
                sourceOriginalAmount -= sourceTakeBackAmount;
            }

            if (destTakeBackAmount > 0)
            {
                dest.AddItems(destOriginalItem, destTakeBackAmount);
                destOriginalAmount -= destTakeBackAmount;
            }

            if (source.MaxAcceptable(destOriginalItem) < destOriginalAmount ||
                dest.MaxAcceptable(sourceOriginalItem) < sourceOriginalAmount)
            {
                dest.AddItems(destOriginalItem, destOriginalAmount);
                source.AddItems(sourceOriginalItem, sourceOriginalAmount);
                return;
            }

            if (destOriginalAmount > 0)
            {
                source.AddItems(destOriginalItem, destOriginalAmount);
            }
            if (sourceOriginalAmount > 0)
            {
                dest.AddItems(sourceOriginalItem, sourceOriginalAmount);
            }
        }

        private int CalculateTakeBack(T removedItem, int removedNumber, IDragContainer<T> removeSource, IDragContainer<T> dest)
        {
            var takeBackNumber = 0;
            var destMaxAcceptable = dest.MaxAcceptable(removedItem);

            if (destMaxAcceptable < removedNumber)
            {
                takeBackNumber = removedNumber - destMaxAcceptable;

                var sourceTakeBackAcceptable = removeSource.MaxAcceptable(removedItem);

                // Abort and reset
                if (sourceTakeBackAcceptable < takeBackNumber)
                {
                    return 0;
                }
            }
            return takeBackNumber;
        }
    }
}