using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;

public class Menu : MonoBehaviour
{
    [SerializeField]
    private Text dateText;
    [SerializeField]
    private Text dayOfWeekText;
    [SerializeField]
    private Text menuText;
    private CultureInfo ci = new CultureInfo("en-US");
    private bool isSetting;
    [SerializeField]
    private Image menuimage;

    private void Start()
    {
        menuimage = GetComponent<Image>();
    }

    public void ChangeMenu(string menu, DateTime date)
    {
        if (isSetting) return;
        //요일 수정
        dayOfWeekText.text = date.ToString("ddd", ci);

        //날짜 수정
        dateText.text = date.ToString("MMM-d", ci);

        //메뉴 수정
        menuText.text = menu;

        //색깔수정
        Color color = Color.black;
        switch(int.Parse(date.ToString("0M", CultureInfo.CreateSpecificCulture("en-US"))))
        {
            case 1:
            default:
                ColorUtility.TryParseHtmlString("#FF526C", out color);
                break;
            case 2:
                ColorUtility.TryParseHtmlString("#6F3FE8", out color);
                break;
            case 3:
                ColorUtility.TryParseHtmlString("#2121FF", out color);
                break;
            case 4:
                ColorUtility.TryParseHtmlString("#FFC440", out color); 
                break;
            case 5:
                ColorUtility.TryParseHtmlString("#12CCFF", out color);
                break;
            case 6:
                ColorUtility.TryParseHtmlString("#1AEB9B", out color);
                break;
            case 7:
                ColorUtility.TryParseHtmlString("#FFD399", out color);
                break;
            case 8:
                ColorUtility.TryParseHtmlString("#FFA373", out color);
                break;
            case 9:
                ColorUtility.TryParseHtmlString("#FF6452", out color);
                break;
            case 10:
                ColorUtility.TryParseHtmlString("#5D21FF", out color);
                break;
            case 11:
                ColorUtility.TryParseHtmlString("#E86B8D", out color);
                break;
            case 12:
                ColorUtility.TryParseHtmlString("#656DEB", out color);
                break;
        }
        menuimage.color = color;
    }

    public void SettingMenu(bool boolean)
    {
        isSetting = boolean;
    }
}
