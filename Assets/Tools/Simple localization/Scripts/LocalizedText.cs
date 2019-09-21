using UnityEngine.UI;
using UnityEngine;

namespace Tools.Localizator
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Text))]
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField] private string translationKey = string.Empty;
        [SerializeField] private CaseType caseType = CaseType.Normal;

        private Text textComponent;

        private void Awake()
        {
            Localizator.OnLanguageChanged += SetTranslatedText;
        }

        private void OnEnable()
        {
            SetTranslatedText();
        }

        private void OnValidate()
        {
            SetTranslatedText();
        }

        private void OnDestroy()
        {
            Localizator.OnLanguageChanged -= SetTranslatedText;
        }

        private void SetTranslatedText()
        {
            if (textComponent is null)
                textComponent = GetComponent<Text>();

            if (textComponent is null) return;

            string localizedText = Localizator.Translate(translationKey, caseType);
            if (string.IsNullOrEmpty(localizedText))
                localizedText = "Key not found!";
            textComponent.text = localizedText;
        }
    }
}