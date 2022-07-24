using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Panel : MonoBehaviour
{
    public const float MOVE_SPEED = 0.3f;
    public const float MOVE_DOWN_SPEED = 0.2f;
    public const float DESTROY_SPEED = 0.3f;

    public PanelTypes CurrentType;
    
   
    public IEnumerator PlayDestroyEffect()
    {
        gameObject.transform.DOScale(Vector3.one * 0.3f, DESTROY_SPEED);
        yield return new WaitForSeconds(DESTROY_SPEED);
        gameObject.transform.localScale = Vector3.one;
        gameObject.SetActive(false);
    }
    

}

public enum PanelTypes
{
    panel1,
    panel2,
    panel3,
    panel4,
    panel5
}
