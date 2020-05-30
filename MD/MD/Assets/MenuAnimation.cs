using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAnimation : MonoBehaviour
{
    bool animGoing;
    public void Press()
    {
        if (animGoing) return;
        animGoing = true;
        StartCoroutine(Animation());
    }
    bool open = false;
    public RectTransform menu;
    public IEnumerator Animation()
    {
        if (open)
        {
            for (float i = 0; i <= 1; i += 0.05f)
            {
                menu.sizeDelta = new Vector2(625f, 850 - (i * 700));
                yield return new WaitForFixedUpdate();
            }
            menu.sizeDelta = new Vector2(625f, 150);
            open = false;
            animGoing = false;
        }
        else
        {
            for (float i = 0; i <= 1; i += 0.05f)
            {
                menu.sizeDelta = new Vector2(625f,150 +(i*700));
                yield return new WaitForFixedUpdate();
            }
            menu.sizeDelta = new Vector2(625f, 850);
            open = true;
            animGoing = false;
        }
    }
    public GameObject loading;
    private void FixedUpdate()
    {
        if (loading.activeSelf)
        {
            loading.transform.Rotate(0,0,5);
        }
    }
}
