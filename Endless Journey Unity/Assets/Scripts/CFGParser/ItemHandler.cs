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
        public IEnumerator<WaitForEndOfFrame> positionItemsIter, disableItemsIter, positionItemsIterPrev, disableItemsIterPrev;

        private void FixedUpdate()
        {
            #region optimizedImmutableSolution
            // Start each coroutine once
            if (positionItemsIter != null && positionItemsIter != positionItemsIterPrev)
            {
                // If previous isn't null it was started, so stop it
                if (positionItemsIterPrev != null)
                {
                    StopCoroutine(positionItemsIterPrev);
                }

                StartCoroutine(positionItemsIter);

                // Save enumerator pointer so that the next update if the enumerator hasn't been replaced, don't start again
                positionItemsIterPrev = positionItemsIter;
            }

            if (disableItemsIter != null && disableItemsIter != disableItemsIterPrev)
            {
                // If previous isn't null it was started, so stop it
                if (disableItemsIterPrev != null)
                {
                    StopCoroutine(disableItemsIterPrev);
                }

                StartCoroutine(disableItemsIter);

                // Save enumerator pointer so that the next update if the enumerator hasn't been replaced, don't start again
                disableItemsIterPrev = disableItemsIter;
            }
            #endregion

            //if (positionItemsIter != null)
            //    StartCoroutine(positionItemsIter);

            //if (disableItemsIter != null)
            //    StartCoroutine(disableItemsIter);
        }
    }
}
