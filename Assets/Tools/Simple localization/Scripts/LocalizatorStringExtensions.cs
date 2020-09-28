namespace SimpleLocalization.Helpers
{
    public static class LocalizatorStringExtensions
    {
        #region Text formatting methods

        public static string SetCaseType(this string translatedText, CaseType caseType)
        {
            switch (caseType)
            {
                case CaseType.Default:
                    return translatedText;
                case CaseType.Uppercase:
                    return translatedText.ToUpper();
                case CaseType.Capitalize:
                    return translatedText.ToCapitalize();
                case CaseType.Lowercase:
                    return translatedText.ToLower();
                default:
                    return translatedText;
            }
        }

        public static string ToCapitalize(this string translatedText)
        {
            translatedText.ToLower();
            char.ToUpper(translatedText[0]);
            return translatedText;
        }

        public static string NewlineReplacer(this string translatedText)
        {
            return translatedText.Replace("<br>", "\n");
        }

        #endregion
    }
}