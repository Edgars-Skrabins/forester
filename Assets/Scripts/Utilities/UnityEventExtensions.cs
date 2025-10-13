using UnityEngine;
using UnityEngine.Events;
public static class UnityEventExtensions
{
    public static void AddListenerOnce(this UnityEvent unityEvent, UnityAction call)
    {
        UnityAction wrapper = null;
        wrapper = () =>
        {
            call();
            unityEvent.RemoveListener(wrapper);
        };
        unityEvent.AddListener(wrapper);
    }
}
