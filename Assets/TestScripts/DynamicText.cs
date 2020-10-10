using SimpleLocalization;
using UnityEngine.UI;
using UnityEngine;

public class DynamicText : MonoBehaviour
{
    [SerializeField] private Text someText = null;

    private void Start()
    {
        Localizator.Initialize();
    }

    public void SetText()
    {
        someText.text = Localizator.Translate("coins");
    }
}