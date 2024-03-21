using System;
using System.Collections;
using UnityEngine;

namespace POPBlocks.Server.Network
{
    public static class CoroutineUtils
    {
        public static IEnumerator WaitWhile(Func<bool> predicate, Action callback = null)
        {
            yield return new WaitWhile(predicate);
            callback?.Invoke();
        }
        
        public static IEnumerator WaitUntil(Func<bool> predicate, Action callback = null)
        {
            yield return new WaitUntil(predicate);
            callback?.Invoke();
        }
    }
}