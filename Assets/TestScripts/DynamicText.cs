using Tools.Localizator;
using UnityEngine.UI;
using UnityEngine;

public class DynamicText : MonoBehaviour
{
    [SerializeField] private Text someText;

    private void Start()
    {
        someText.text = Localizator.Translate("coin");
    }
}




