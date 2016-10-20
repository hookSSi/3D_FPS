using UnityEngine;

public class Utill 
{

    // 자식 오브젝트의 레이어들도 모두 바꿔주기 위해 재귀함수를 사용한다.
    public static void SetLayerRecursively(GameObject p_object, int p_newLayer)
    {
        if (p_object == null)
            return;

        p_object.layer = p_newLayer;

        foreach(Transform child in p_object.transform)
        {
            if (child == null)
                continue;

            SetLayerRecursively(child.gameObject, p_newLayer);
        }
    }
}
