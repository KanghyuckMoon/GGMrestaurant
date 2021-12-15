using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using HtmlAgilityPack;
using System;
using System.IO;
using DG.Tweening;

#region Json
[System.Serializable]
public class Row
{
    public string ATPT_OFCDC_SC_CODE;
    public string ATPT_OFCDC_SC_NM;
    public string SD_SCHUL_CODE;
    public string SCHUL_NM;
    public string MMEAL_SC_CODE;
    public string MMEAL_SC_NM;
    public string MLSV_YMD;
    public string MLSV_FGR;
    public string DDISH_NM;
    public string ORPLC_INFO;
    public string CAL_INFO;
    public string NTR_INFO;
    public string MLSV_FROM_YMD;
    public string MLSV_TO_YMD;
}

[Serializable]
public class RESULT
{
    public string CODE;
    public string MESSAGE;
}

[Serializable]
public class Head
{
    public int list_total_count;
    public RESULT RESULT;
}



[Serializable]
public class MealServiceDietInfo
{
    public List<Head> head;
    public List<Row> row;
}

[Serializable]
public class Root
{
    public List<MealServiceDietInfo> mealServiceDietInfo;
}
#endregion

public class RandomNickName : MonoBehaviour
{
    string jsonResult;
    private bool isLoading;

    //인트로
    [SerializeField]
    private Image introbackground;
    [SerializeField]
    private Text introText;
    //로딩
    [SerializeField]
    private RectTransform fadeImage;
    #region 메뉴판

    //중앙
    public Root data;
    public Menu menuNow;
    public Menu menuNext;
    public Menu menuPrevious;
    public Menu menuNNext;
    public Menu menuPPrevious;
    public ScrollPanel Panel;
    public Vector2[] originVectors = new Vector2[5];
    public RectTransform[] menuRects = new RectTransform[5];


    #endregion

    #region 날짜
    public int year;
    public int month;
    public int day;
    public int maxday;
    public Text yearText; 
    public Text monthText;
    public System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
    #endregion

    private void Start()
    {
        originVectors[0] = menuPPrevious.GetComponent<RectTransform>().anchoredPosition;
        originVectors[1] = menuPrevious.GetComponent<RectTransform>().anchoredPosition;
        originVectors[2] = menuNow.GetComponent<RectTransform>().anchoredPosition;
        originVectors[3] = menuNext.GetComponent<RectTransform>().anchoredPosition;
        originVectors[4] = menuNNext.GetComponent<RectTransform>().anchoredPosition;

        year = int.Parse(DateTime.Now.ToString("yyyy"));
        month = int.Parse(DateTime.Now.ToString("MM"));
        day = int.Parse(DateTime.Now.ToString("dd"));

        maxday = DateTime.DaysInMonth(year, month);

        introText.gameObject.SetActive(true);
        introbackground.gameObject.SetActive(true);

        introText.DOFade(1, 3).OnComplete(() =>
        {
            introbackground.DOFade(0, 1);
            introText.DOFade(0, 1).OnComplete(() =>
             {
                 introText.gameObject.SetActive(false);
                 introbackground.gameObject.SetActive(false);
             });
        });

        DateTextSetting();
        GetItem();
    }

    private void DateTextSetting()
    {
        yearText.text = year.ToString();
        monthText.text = month.ToString();
    }

    public void GetItem()
    {
        if (isLoading) return;
        StartCoroutine(GetItemco());
    }

    public void InitData()
    {
        data = JsonUtility.FromJson<Root>(jsonResult);
        string nowdate = string.Format("{0:D4}{1:D2}{2:D2}", year, month, day);
        string nextdate = DateTime.ParseExact(nowdate,"yyyyMMdd",null).AddDays(1).ToString("yyyyMMdd", ci);
        string nnextdate = DateTime.ParseExact(nowdate,"yyyyMMdd",null).AddDays(2).ToString("yyyyMMdd", ci);
        string previousdate = DateTime.ParseExact(nowdate, "yyyyMMdd", null).AddDays(-1).ToString("yyyyMMdd", ci);
        string ppreviousdate = DateTime.ParseExact(nowdate, "yyyyMMdd", null).AddDays(-2).ToString("yyyyMMdd", ci);
        //Debug.Log(nowdate);
        menuNow.SettingMenu(false);
        menuNext.SettingMenu(false);
        menuNNext.SettingMenu(false);
        menuPrevious.SettingMenu(false);
        menuPPrevious.SettingMenu(false);
        if ((data?.mealServiceDietInfo?.Count ?? 0) == 0)
        {
            MenuObjChange(menuNow, "None", nowdate, 0);
            MenuObjChange(menuNext, "None", nextdate, 0);
            MenuObjChange(menuNNext, "None", nnextdate, 0);
            MenuObjChange(menuPrevious, "None", previousdate, 0);
            MenuObjChange(menuPPrevious, "None", ppreviousdate, 0);
        }
        else
        {
            for (int i = 0; i <= data.mealServiceDietInfo[1].row.Count; i++)
            {
                if(i == data.mealServiceDietInfo[1].row.Count)
                {
                    MenuObjChange(menuNow, "None", nowdate, 0);
                    MenuObjChange(menuNext, "None", nextdate, 0);
                    MenuObjChange(menuNNext, "None", nnextdate, 0);
                    MenuObjChange(menuPrevious, "None", previousdate, 0);
                    MenuObjChange(menuPPrevious, "None", ppreviousdate, 0);
                }
                else
                {
                    if (nowdate == data.mealServiceDietInfo[1].row[i].MLSV_YMD)
                    {
                        MenuObjChange(menuNow, data.mealServiceDietInfo[1].row[i].DDISH_NM.ToString(), nowdate);
                    }
                    if(day >= maxday)
                    {
                        MenuObjChange(menuNext, "Next Month", nextdate);
                    }
                    else if (nextdate == data.mealServiceDietInfo[1].row[i].MLSV_YMD)
                    {
                        MenuObjChange(menuNext, data.mealServiceDietInfo[1].row[i].DDISH_NM.ToString(), nextdate);
                    }
                    if (day <= 1)
                    {
                        MenuObjChange(menuPrevious, "Previous Month", previousdate);
                    }
                    else if (previousdate == data.mealServiceDietInfo[1].row[i].MLSV_YMD)
                    {
                        MenuObjChange(menuPrevious, data.mealServiceDietInfo[1].row[i].DDISH_NM.ToString(), previousdate);
                    }

                    if (day+1 >= maxday)
                    {
                        MenuObjChange(menuNNext, "Next Month", nnextdate);
                    }
                    else if (nnextdate == data.mealServiceDietInfo[1].row[i].MLSV_YMD)
                    {
                        MenuObjChange(menuNNext, data.mealServiceDietInfo[1].row[i].DDISH_NM.ToString(), nnextdate);
                    }

                    if (day-1 <= 1)
                    {
                        MenuObjChange(menuPPrevious, "Previous Month", ppreviousdate);
                    }
                    else if (ppreviousdate == data.mealServiceDietInfo[1].row[i].MLSV_YMD)
                    {
                        MenuObjChange(menuPPrevious, data.mealServiceDietInfo[1].row[i].DDISH_NM.ToString(), ppreviousdate);
                    }
                }
            }
        }
        isLoading = false;
    }

    public void ChangeMonth(int value)
    {
        if (isLoading) return;
        month += value;
        MonthCalculation();
        maxday = DateTime.DaysInMonth(year, month);
        DateTextSetting();
        GetItem();
    }
    private void MonthCalculation()
    {
        if (month < 1)
        {
            month = 12;
            year--;
        }
        else if (month > 12)
        {
            month = 1;
            year++;
        }
    }



    public void ChangeDay(int value)
    {
        if (isLoading) return;
        day += value;
        if (value > 0)
        {
            if (day > maxday)
            {
                month++;
                MonthCalculation();
                maxday = DateTime.DaysInMonth(year, month);
                day = 1;
                MenuMoveAnimation(value, true);
                fadeImage.anchoredPosition = new Vector2(0, -1500);
                fadeImage.DOKill();
                fadeImage.DOAnchorPosY(1500, 1.2f);
            }
            else
            {
                MenuMoveAnimation(value, false);
            }
        }
        else
        {
            if (day < 1)
            {
                month--;
                MonthCalculation();
                maxday = DateTime.DaysInMonth(year, month);
                day = maxday;
                MenuMoveAnimation(value, true);
                fadeImage.anchoredPosition = new Vector2(0, -1500);
                fadeImage.DOKill();
                fadeImage.DOAnchorPosY(1500, 1.2f);
            }
            else
            {
                MenuMoveAnimation(value, false);
            }
        }
        DateTextSetting();
    }

    private void MenuMoveAnimation(int dir,bool getItem)
    {
        //-1은 오른쪽
        //1은 왼쪽

        if(dir == -1)
        {
            menuRects[0].GetComponent<Canvas>().sortingLayerName = "Middle";
            menuRects[1].GetComponent<Canvas>().sortingLayerName = "Front";
            menuRects[2].GetComponent<Canvas>().sortingLayerName = "Middle";
            menuRects[3].GetComponent<Canvas>().sortingLayerName = "Back";
            menuRects[4].GetComponent<Canvas>().sortingLayerName = "Bottom";

            menuRects[0].DOScale(0.8f, 0.3f);
            menuRects[1].DOScale(1f, 0.3f);
            menuRects[2].DOScale(0.8f, 0.3f);
            menuRects[3].DOScale(0.6f, 0.3f);
            menuRects[4].DOScale(0.6f, 0.3f);

            menuRects[0].DOAnchorPos(originVectors[1], 0.3f);
            menuRects[1].DOAnchorPos(originVectors[2], 0.3f);
            menuRects[2].DOAnchorPos(originVectors[3], 0.3f);
            menuRects[3].DOAnchorPos(originVectors[4], 0.3f);
            menuRects[4].DOAnchorPos(originVectors[0], 0.3f).OnComplete(() =>
            {
                menuRects[0].GetComponent<Canvas>().sortingLayerName = "Back";
                menuRects[1].GetComponent<Canvas>().sortingLayerName = "Middle";
                menuRects[2].GetComponent<Canvas>().sortingLayerName = "Front";
                menuRects[3].GetComponent<Canvas>().sortingLayerName = "Middle";
                menuRects[4].GetComponent<Canvas>().sortingLayerName = "Bottom";

                menuRects[0].anchoredPosition = originVectors[0];
                menuRects[1].anchoredPosition = originVectors[1];
                menuRects[2].anchoredPosition = originVectors[2];
                menuRects[3].anchoredPosition = originVectors[3];
                menuRects[4].anchoredPosition = originVectors[4];

                menuRects[0].localScale = new Vector3(0.6f,0.6f,0.6f);
                menuRects[1].localScale = new Vector3(0.8f,0.8f,0.8f);
                menuRects[2].localScale = new Vector3(1f,1f,1f);
                menuRects[3].localScale = new Vector3(0.8f,0.8f,0.8f);
                menuRects[4].localScale = new Vector3(0.6f,0.6f,0.6f);
                if (getItem)
                {
                    GetItem();
                }
                else
                {
                    InitData();
                }
                Panel.IsMoveFalse();
            });
        }
        else
        {
            menuRects[0].GetComponent<Canvas>().sortingLayerName = "Bottom";
            menuRects[1].GetComponent<Canvas>().sortingLayerName = "Back";
            menuRects[2].GetComponent<Canvas>().sortingLayerName = "Middle";
            menuRects[3].GetComponent<Canvas>().sortingLayerName = "Front";
            menuRects[4].GetComponent<Canvas>().sortingLayerName = "Middle";

            menuRects[0].DOScale(0.6f, 0.3f);
            menuRects[1].DOScale(0.6f, 0.3f);
            menuRects[2].DOScale(0.8f, 0.3f);
            menuRects[3].DOScale(1f, 0.3f);
            menuRects[4].DOScale(0.8f, 0.3f);

            menuRects[0].DOAnchorPos(originVectors[4], 0.3f);
            menuRects[1].DOAnchorPos(originVectors[0], 0.3f);
            menuRects[2].DOAnchorPos(originVectors[1], 0.3f);
            menuRects[3].DOAnchorPos(originVectors[2], 0.3f);
            menuRects[4].DOAnchorPos(originVectors[3], 0.3f).OnComplete(() =>
            {

                menuRects[0].GetComponent<Canvas>().sortingLayerName = "Back";
                menuRects[1].GetComponent<Canvas>().sortingLayerName = "Middle";
                menuRects[2].GetComponent<Canvas>().sortingLayerName = "Front";
                menuRects[3].GetComponent<Canvas>().sortingLayerName = "Middle";
                menuRects[4].GetComponent<Canvas>().sortingLayerName = "Back";

                menuRects[0].anchoredPosition = originVectors[0];
                menuRects[1].anchoredPosition = originVectors[1];
                menuRects[2].anchoredPosition = originVectors[2];
                menuRects[3].anchoredPosition = originVectors[3];
                menuRects[4].anchoredPosition = originVectors[4];

                menuRects[0].localScale = new Vector3(0.6f, 0.6f, 0.6f);
                menuRects[1].localScale = new Vector3(0.8f, 0.8f, 0.8f);
                menuRects[2].localScale = new Vector3(1f, 1f, 1f);
                menuRects[3].localScale = new Vector3(0.8f, 0.8f, 0.8f);
                menuRects[4].localScale = new Vector3(0.6f, 0.6f, 0.6f);
                if (getItem)
                {
                    GetItem();
                }
                else
                {
                    InitData();
                }
                Panel.IsMoveFalse();
            });
        }
    }

    private IEnumerator GetItemco()
    {
        isLoading = true;
        string url = null;
        if (month >= 10)
        {
            url = "https://open.neis.go.kr/hub/mealServiceDietInfo?KEY=c0822b23311a4674846f9b301dc0a3c0&ATPT_OFCDC_SC_CODE=J10&SD_SCHUL_CODE=7531377&Type=json&MLSV_YMD=" + year + month;

        }
        else
        {
            url = "https://open.neis.go.kr/hub/mealServiceDietInfo?KEY=c0822b23311a4674846f9b301dc0a3c0&ATPT_OFCDC_SC_CODE=J10&SD_SCHUL_CODE=7531377&Type=json&MLSV_YMD=" + year + "0" + month;
        }

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.Send();
            if (www.isNetworkError || www.isHttpError) //불러오기 실패 시
            {
                Debug.Log(www.error);
            }
            else
            {
                if (www.isDone)
                {
                    jsonResult = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                    InitData();
                }
            }
        }
    }
    private void MenuObjChange(Menu menuobj, string menu, string date)
    {
        string[] arr = menu.Replace("<br/>", "\n").Split('\n');
        string m = null;
        for (int i = 0; i < arr.Length; i++)
        {
            m += arr[i].Split('★')[0] + '\n';
        }
        menuobj.ChangeMenu(m, DateTime.ParseExact(date, "yyyyMMdd", null));
        menuobj.SettingMenu(true);
    }

    private void MenuObjChange(Menu menuobj, string menu, string date, int type)
    {
        menuobj.ChangeMenu(menu, DateTime.ParseExact(date, "yyyyMMdd", null));
        menuobj.SettingMenu(true);
    }
}
