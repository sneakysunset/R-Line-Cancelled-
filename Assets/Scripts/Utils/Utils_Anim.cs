using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Utils_Anim
{
    public static IEnumerator AnimationLerp(Vector2 startPos, Vector2 endPos, Vector2 currentPos, AnimationCurve anim, float speed, Action<Vector2> callBack, Action onLerpEnd = null)
    {
        float j = 0;
        while (j < 1)
        {
            j += Time.deltaTime * (1 / speed);
            currentPos = Vector2.Lerp(startPos, endPos, anim.Evaluate(j));
            callBack.Invoke(currentPos);
            yield return new WaitForEndOfFrame();
        }

        callBack.Invoke(endPos);
        onLerpEnd?.Invoke();
        yield return null;
    }
}
