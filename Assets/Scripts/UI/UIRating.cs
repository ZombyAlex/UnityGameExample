using SWFServer.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRating : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textPlace;
    [SerializeField] private TextMeshProUGUI textName;
    [SerializeField] private TextMeshProUGUI textVal;

    private UserRating userRating;

    void Update()
    {
        if (string.IsNullOrEmpty(textName.text))
        {
            textName.text = Data.Instance.GetUserName(userRating.Id);
        }
    }

    public void Init(UserRating userRating, int place)
    {
        this.userRating = userRating;
        textPlace.text = place.ToString();
        textName.text = Data.Instance.GetUserName(userRating.Id);
        textVal.text = userRating.val.ToString("F0");
    }
}
