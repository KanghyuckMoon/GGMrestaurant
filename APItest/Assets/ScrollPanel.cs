using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ScrollPanel : MonoBehaviour
{
    [SerializeField]
    private RandomNickName randomNickName;
    private bool isDrag;
    private Camera main;
    private Vector2 originPos;
    private Vector2 touchPos;
    private Vector2 mousePos;
    [SerializeField]
    private RectTransform rect;
    private bool isMove;

    [SerializeField]
    private Image nextimage;
    [SerializeField]
    private Image previousimage;

    [SerializeField]
    private GameObject nextpanel;
    [SerializeField]
    private GameObject previouspanel;

    private void Start()
    {
        main = Camera.main;
        originPos = rect.anchoredPosition;
        nextimage.GetComponent<RectTransform>().DOScale(2, 1).SetLoops(-1, LoopType.Restart);
        nextimage.DOFade(0, 1).OnComplete(() => nextimage.color = new Color(nextimage.color.r, nextimage.color.g, nextimage.color.b, 1)).SetLoops(-1, LoopType.Restart);
        previousimage.GetComponent<RectTransform>().DOScale(2, 1).SetLoops(-1, LoopType.Restart);
        previousimage.DOFade(0, 1).OnComplete(() => previousimage.color = new Color(previousimage.color.r, previousimage.color.g, previousimage.color.b, 1)).SetLoops(-1, LoopType.Restart);
    }

    public void OnMouseDown()
    {
        touchPos = main.ScreenToWorldPoint(Input.mousePosition);
    }

    public void OnMouseDrag()
    {
        if (isMove) return;
        Vector3 mouse = Input.mousePosition;
        Ray mouseRay = main.ScreenPointToRay(mouse);

        float middlePoint = (transform.position - main.transform.position).magnitude * 0.5f;
        Vector3 middledir = mouseRay.direction * middlePoint;
        transform.LookAt(new Vector3((mouseRay.origin.x + middledir.x) * -1, (mouseRay.origin.y + middledir.y), (mouseRay.origin.z - middledir.z) * -1));

        mousePos = main.ScreenToWorldPoint(Input.mousePosition);

        if ((originPos.x + originPos.x * 0.5f) < mousePos.x)
        {
            previouspanel.gameObject.SetActive(true);
            nextpanel.gameObject.SetActive(false);
        }
        else if (-(originPos.x + originPos.x * 0.5f) > mousePos.x)
        {
            nextpanel.gameObject.SetActive(true);
            previouspanel.gameObject.SetActive(false);
        }
        else
        {
            previouspanel.gameObject.SetActive(false);
            nextpanel.gameObject.SetActive(false);
        }
    }

    public void OnMouseUp()
    {
        previouspanel.gameObject.SetActive(false);
        nextpanel.gameObject.SetActive(false);
        if ((originPos.x + originPos.x * 0.5f) < mousePos.x)
        {
            //오른쪽으로 스와이프
            isMove = true;
            randomNickName.ChangeDay(-1);
            rect.anchoredPosition = originPos;
            rect.DOLocalRotate(new Vector3(0, 0, 0), 0.4f);
        }
        else if (-(originPos.x + originPos.x * 0.5f) > mousePos.x)
        {
            //왼쪽으로 스와이프
            isMove = true;
            randomNickName.ChangeDay(1);
            rect.anchoredPosition = originPos;
            rect.DOLocalRotate(new Vector3(0, 0, 0), 0.4f);
        }
        else
        {
            rect.DOAnchorPosX(0, 0.5f);
            rect.DOScale(1, 0.5f);
            rect.DOLocalRotate(new Vector3(0, 0, 0), 0.4f);
        }
    }

    public void IsMoveFalse()
    {
        isMove = false;
    }
}
