using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;

namespace Assets.Scripts.CFGParser
{
    public class ItemHandler : MonoBehaviour
    {
        private IEnumerator<WaitForEndOfFrame> positionItemsIter, disableItemsIter;
        private Queue<IEnumerator<WaitForEndOfFrame>> positionItemsIterQueue, disableItemsIterQueue;

        private void Awake()
        {
            positionItemsIterQueue = new Queue<IEnumerator<WaitForEndOfFrame>>();
            disableItemsIterQueue = new Queue<IEnumerator<WaitForEndOfFrame>>();
        }

        public void AddPositionIter(IEnumerator<WaitForEndOfFrame> enumerator)
        {
            positionItemsIterQueue.Enqueue(enumerator);
        }

        public void AddDisableIter(IEnumerator<WaitForEndOfFrame> enumerator)
        {
            disableItemsIterQueue.Enqueue(enumerator);
        }

        private void Update()
        {
            // Start each coroutine once
            if (positionItemsIterQueue.Count > 0 && (positionItemsIter == null || positionItemsIter.Current == null))
            {
                // Was running, so stop
                if (positionItemsIter != null)
                {
                    StopCoroutine(positionItemsIter);
                }

                // Get next and start
                positionItemsIter = positionItemsIterQueue.Dequeue();
                StartCoroutine(positionItemsIter);
            }

            if (disableItemsIterQueue.Count > 0 && (disableItemsIter == null || disableItemsIter.Current == null))
            {
                // If previous isn't null it was started, so stop it
                if (disableItemsIter != null)
                {
                    StopCoroutine(disableItemsIter);
                }

                // Get next and start
                disableItemsIter = disableItemsIterQueue.Dequeue();
                StartCoroutine(disableItemsIter);
            }
        }

        internal bool IsDisableIterComplete()
        {
            return disableItemsIterQueue.Count == 0 && disableItemsIter != null && !disableItemsIter.MoveNext();
        }
    }
}
