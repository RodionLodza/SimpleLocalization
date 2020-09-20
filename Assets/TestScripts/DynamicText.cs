using SimpleLocalization;
using UnityEngine.UI;
using UnityEngine;

public class DynamicText : MonoBehaviour
{
    [SerializeField] private Text someText = null;

    private void Start()
    {
        someText.text = Localizator.Translate("coin");
    }
}