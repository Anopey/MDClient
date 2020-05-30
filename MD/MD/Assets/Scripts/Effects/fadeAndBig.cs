using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fadeAndBig : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.localPosition = new Vector3(0,0,0);
        StartCoroutine(FadeAndGrow());
    }
    public CanvasGroup alp; 
    public IEnumerator FadeAndGrow()
    {
        for (float i = 1; i >= 0; i -= 0.05f)
        {
            float a = Mathf.Abs(i -1f) / 2;
            transform.localScale = new Vector3(1+a,1 + a, 1);
            alp.alpha = i;
            yield return new WaitForFixedUpdate();
        }
        Destroy(gameObject);
    }
    public Vector3 turnAmount;
    private void FixedUpdate()
    {
        transform.Rotate(turnAmount, Space.Self);
    }
}
