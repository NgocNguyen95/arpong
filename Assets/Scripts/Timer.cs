using UnityEngine;
using System;
using System.Collections;

public static class Timer
{    
    public static void StartCooldown(float cooldown, Action onCooldownComplete)
    {
        TimerRunner.Instance.StartCoroutine(CooldownCoroutine(cooldown, onCooldownComplete));
    }        

    private static IEnumerator CooldownCoroutine(float cooldown, Action onCooldownComplete)
    {
        yield return new WaitForSeconds(cooldown);

        if (onCooldownComplete != null)
        {
            onCooldownComplete.Invoke();
        }
    }

    private class TimerRunner : MonoBehaviour
    {
        private static TimerRunner instance;

        public static TimerRunner Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject container = new GameObject("TimerRunner");
                    instance = container.AddComponent<TimerRunner>();
                    DontDestroyOnLoad(container);
                }
                return instance;
            }
        }
    }
}
