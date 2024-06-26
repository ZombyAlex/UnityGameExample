using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LangUpdate : MonoBehaviour {

	private int curLang = -1;
	void Start () 
	{
		curLang = Lang.GetCurLang();
		InitTexts();
	}
	
	void Update () 
	{
		if (curLang != Lang.GetCurLang())
			InitTexts();
	}

	private void InitTexts()
	{
		//Debug.Log("InitTexts " + name);
        curLang = Lang.GetCurLang();
        TextMeshProUGUI[] tx = GetComponentsInChildren<TextMeshProUGUI>(true);
		foreach (var t in tx)
		{
			if (t.name[0] != 'T')
				t.text = Lang.Get("UI", t.name);
		}
	}
}
