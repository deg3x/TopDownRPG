using System;
using System.Collections;

public static class Utilities
{
    public static IEnumerator InvokeAfterFrames(int framesToWait, Action action)
    {
        for (int i = 0; i < framesToWait; i++)
        {
            yield return null;
        }

        action?.Invoke();
    }
}
