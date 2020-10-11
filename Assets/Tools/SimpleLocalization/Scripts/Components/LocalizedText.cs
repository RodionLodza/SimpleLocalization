using UnityEngine.UI;
using UnityEngine;

namespace SimpleLocalization
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Text))]
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField] private Text textComponent = null;
        [SerializeField] private string translationKey = string.Empty;
        [SerializeField] private CaseType caseType = CaseType.Default;

        private bool textIsSet = false;

        private void Awake()
        {
            Localizator.OnLanguageChanged += SetTranslatedText;
        }

        private void OnEnable()
        {
            if (!textIsSet)
            {
                textIsSet = true;
                SetTranslatedText();
            }
        }

        private void OnValidate()
        {
            SetTranslatedText();
            CashTextComponent();
        }

        private void OnDestroy()
        {
            Localizator.OnLanguageChanged -= SetTranslatedText;
        }

        private void SetTranslatedText()
        {
            CashTextComponent();

            string localizedText = Localizator.Translate(translationKey, caseType);
            if (string.IsNullOrEmpty(localizedText))
            {
                localizedText = $"Key '{translationKey}' not found!";
            }
            textComponent.text = localizedText;
        }

        private void CashTextComponent()
        {
            if (textComponent is null)
            {
                textComponent = GetComponent<Text>();
            }
        }
    }
}