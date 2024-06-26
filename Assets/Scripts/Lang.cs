using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public static class Lang
{
	public class LangItem
	{
		public List<string> text = new List<string>();
	};

	private static List<string> languages = new List<string>();

	private static Dictionary<string, Dictionary<string, LangItem>> data = new Dictionary<string, Dictionary<string, LangItem>>();

	private static int curLang = 0;

    private static bool isInit = false;

	public static void Init()
	{
		if(isInit)
			return;

        isInit = true;

		languages.Add("ru");
		languages.Add("en");
		curLang = 0;

		
		TextAsset aAsset = (TextAsset) Resources.Load("lang", typeof (TextAsset));
		if (aAsset == null)
		{
			Debug.LogError("error load lang.xml");
			return;
		}
		XmlDocument aDoc = new XmlDocument();
		aDoc.LoadXml(aAsset.text);
		XmlNodeList aRoot = aDoc.GetElementsByTagName("Lang");

		XmlNodeList aList = aRoot.Item(0).ChildNodes;
		foreach (XmlNode it in aList)
		{
			string aNameSection = it.Attributes["id"].Value;

			XmlNodeList aListChild = it.ChildNodes;

			Dictionary<string, LangItem> aDic = new Dictionary<string, LangItem>();
			foreach (XmlNode jt in aListChild)
			{

				LangItem aItem = new LangItem();
				string aIdItem = jt.Attributes["id"].Value;
				XmlNodeList aList1 = jt.ChildNodes;
				foreach (XmlNode it1 in aList1)
				{

					aItem.text.Add(it1.InnerText);
				}
				aDic.Add(aIdItem, aItem);
			}
			data.Add(aNameSection, aDic);
		}
	}

	public static string Get(string inSection, string inTag)
	{
		if (data.ContainsKey(inSection) && data[inSection].ContainsKey(inTag))
		{
			return data[inSection][inTag].text[curLang];
		}
		return "??" + inSection + "[" + inTag + "]";
	}

    public static List<string> GetAll(string inSection, string inTag)
    {
        if (data.ContainsKey(inSection) && data[inSection].ContainsKey(inTag))
        {
            return data[inSection][inTag].text;
        }
        return null;
    }

	public static bool IsTag(string inSection, string inTag)
    {
        if (data.ContainsKey(inSection) && data[inSection].ContainsKey(inTag))
        {
            return true;
        }
        return false;
	}

	public static void SetLang(int in_lang)
	{
		curLang = in_lang;
	}

	public static void NextLang()
	{
		curLang++;
		if (curLang == languages.Count)
		{
			curLang = 0;
		}
	}

	public static void PrevLang()
	{
		curLang--;
		if (curLang < 0)
		{
			curLang = languages.Count - 1;
		}
	}

	public static int GetCurLang()
	{
		return curLang;
	}
}
