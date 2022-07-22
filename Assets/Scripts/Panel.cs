using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Panel : MonoBehaviour
{
    public const float MOVE_SPEED = 0.7f;

    public PanelTypes CurrentType;
    public bool inARow;

    //private GameObject currentGameObject;
    private RectTransform currentRect;

    public Vector2 vecInPanels;
    

    private void OnEnable()
    {
        //currentGameObject = this.gameObject;
        currentRect = GetComponent<RectTransform>();
        currentRect.localScale = Vector3.one;
        inARow = true;
    }

    private void OnDisable()
    {
        inARow = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPosition(Vector2 pos)
    {
        currentRect.anchoredPosition3D = new Vector3(pos.x, pos.y, 0);
        
    }

    public Vector2 GetPosition()
    {
        return new Vector2(currentRect.anchoredPosition3D.x, currentRect.anchoredPosition3D.y);
    }


    public IEnumerator DetonateMatch()
    {
        print("detonating " + currentRect.anchoredPosition3D + ": type " + (int)CurrentType);
        currentRect.DOScale(Vector3.zero, MOVE_SPEED);
        yield return new WaitForSeconds(MOVE_SPEED);        
        this.gameObject.SetActive(false);

    }

    public void MoveUp(int koef)
    {
        currentRect.DOLocalMove(new Vector3(currentRect.anchoredPosition3D.x, currentRect.anchoredPosition3D.y + GameControl.DEF_LENGHT * koef, currentRect.anchoredPosition3D.z), MOVE_SPEED);
    }

    public void MoveDown(int koef)
    {
        //print("from " + currentRect.anchoredPosition3D + "to " + new Vector3(currentRect.anchoredPosition3D.x, currentRect.anchoredPosition3D.y - GameControl.DEF_LENGHT * koef, currentRect.anchoredPosition3D.z));
        currentRect.DOLocalMove(new Vector3(currentRect.anchoredPosition3D.x, currentRect.anchoredPosition3D.y - GameControl.DEF_LENGHT * koef, currentRect.anchoredPosition3D.z), MOVE_SPEED);
    }

    public void MoveLeft(int koef)
    {
        currentRect.DOLocalMove(new Vector3(currentRect.anchoredPosition3D.x - GameControl.DEF_LENGHT * koef, currentRect.anchoredPosition3D.y , currentRect.anchoredPosition3D.z), MOVE_SPEED);
    }

    public void MoveRight(int koef)
    {
        currentRect.DOLocalMove(new Vector3(currentRect.anchoredPosition3D.x + GameControl.DEF_LENGHT * koef, currentRect.anchoredPosition3D.y, currentRect.anchoredPosition3D.z), MOVE_SPEED);
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
