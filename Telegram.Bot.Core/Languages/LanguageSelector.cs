using System.Collections.Generic;

namespace Telegram.Bot.Core.Languages
{
    /// <summary>
    /// Класс, представляющий методы для выбора языка пользователяы
    /// </summary>
    public class LanguageSelector : ILanguageSelector
    {
        /// <summary>
        /// Словарь кодов языка
        /// </summary>
        protected static readonly Dictionary<string, LanguageCode> languagesCodes = new Dictionary<string, LanguageCode>()
        {
            ["af"] = LanguageCode.Afrikaans,
            ["am"] = LanguageCode.Amharic,
            ["ar"] = LanguageCode.Arabic,
            ["arn"] = LanguageCode.Mapudungun,
            ["as"] = LanguageCode.Assamese,
            ["az"] = LanguageCode.Azeri,
            ["ba"] = LanguageCode.Bashkir,
            ["be"] = LanguageCode.Belarusian,
            ["bg"] = LanguageCode.Bulgarian,
            ["bn"] = LanguageCode.Bengali,
            ["bo"] = LanguageCode.Tibetan,
            ["br"] = LanguageCode.Breton,
            ["bs"] = LanguageCode.Bosnian,
            ["ca"] = LanguageCode.Catalan,
            ["co"] = LanguageCode.Corsican,
            ["cs"] = LanguageCode.Czech,
            ["cy"] = LanguageCode.Welsh,
            ["da"] = LanguageCode.Danish,
            ["de"] = LanguageCode.German,
            ["dsb"] = LanguageCode.LowerSorbian,
            ["dv"] = LanguageCode.Divehi,
            ["el"] = LanguageCode.Greek,
            ["en"] = LanguageCode.English,
            ["es"] = LanguageCode.Spanish,
            ["et"] = LanguageCode.Estonian,
            ["eu"] = LanguageCode.Basque,
            ["fa"] = LanguageCode.Persian,
            ["fi"] = LanguageCode.Finnish,
            ["fil"] = LanguageCode.Filipino,
            ["fo"] = LanguageCode.Faroese,
            ["fr"] = LanguageCode.French,
            ["fy"] = LanguageCode.Frisian,
            ["ga"] = LanguageCode.Irish,
            ["gd"] = LanguageCode.ScottishGaelic,
            ["gl"] = LanguageCode.Galician,
            ["gsw"] = LanguageCode.Alsatian,
            ["gu"] = LanguageCode.Gujarati,
            ["ha"] = LanguageCode.Hausa,
            ["he"] = LanguageCode.Hebrew,
            ["hi"] = LanguageCode.Hindi,
            ["hr"] = LanguageCode.Croatian,
            ["hsb"] = LanguageCode.UpperSorbian,
            ["hu"] = LanguageCode.Hungarian,
            ["hy"] = LanguageCode.Armenian,
            ["id"] = LanguageCode.Indonesian,
            ["ig"] = LanguageCode.Igbo,
            ["ii"] = LanguageCode.Yi,
            ["is"] = LanguageCode.Icelandic,
            ["it"] = LanguageCode.Italian,
            ["iu"] = LanguageCode.Inuktitut,
            ["ja"] = LanguageCode.Japanese,
            ["ka"] = LanguageCode.Georgian,
            ["kk"] = LanguageCode.Kazakh,
            ["kl"] = LanguageCode.Greenlandic,
            ["km"] = LanguageCode.Khmer,
            ["kn"] = LanguageCode.Kannada,
            ["ko"] = LanguageCode.Korean,
            ["kok"] = LanguageCode.Konkani,
            ["ky"] = LanguageCode.Kyrgyz,
            ["lb"] = LanguageCode.Luxembourgish,
            ["lo"] = LanguageCode.Lao,
            ["lt"] = LanguageCode.Lithuanian,
            ["lv"] = LanguageCode.Latvian,
            ["mi"] = LanguageCode.Maori,
            ["mk"] = LanguageCode.Macedonian,
            ["ml"] = LanguageCode.Malayalam,
            ["mn"] = LanguageCode.Mongolian,
            ["moh"] = LanguageCode.Mohawk,
            ["mr"] = LanguageCode.Marathi,
            ["ms"] = LanguageCode.Malay,
            ["mt"] = LanguageCode.Maltese,
            ["my"] = LanguageCode.Burmese,
            ["nb"] = LanguageCode.NorwegianBokmal,
            ["ne"] = LanguageCode.Nepali,
            ["nl"] = LanguageCode.Dutch,
            ["nn"] = LanguageCode.NorwegianNynorsk,
            ["no"] = LanguageCode.Norwegian,
            ["nso"] = LanguageCode.Sesotho,
            ["oc"] = LanguageCode.Occitan,
            ["or"] = LanguageCode.Oriya,
            ["pa"] = LanguageCode.Punjabi,
            ["pl"] = LanguageCode.Polish,
            ["prs"] = LanguageCode.Dari,
            ["ps"] = LanguageCode.Pashto,
            ["pt"] = LanguageCode.Portuguese,
            ["qut"] = LanguageCode.Kiche,
            ["quz"] = LanguageCode.Quechua,
            ["rm"] = LanguageCode.Romansh,
            ["ro"] = LanguageCode.Romanian,
            ["ru"] = LanguageCode.Russian,
            ["rw"] = LanguageCode.Kinyarwanda,
            ["sa"] = LanguageCode.Sanskrit,
            ["sah"] = LanguageCode.Yakut,
            ["se"] = LanguageCode.SamiNorthern,
            ["si"] = LanguageCode.Sinhala,
            ["sk"] = LanguageCode.Slovak,
            ["sl"] = LanguageCode.Slovenian,
            ["sma"] = LanguageCode.SamiSouthern,
            ["smj"] = LanguageCode.SamiLule,
            ["smn"] = LanguageCode.SamiInari,
            ["sms"] = LanguageCode.SamiSkolt,
            ["sq"] = LanguageCode.Albanian,
            ["sr"] = LanguageCode.Serbian,
            ["sv"] = LanguageCode.Swedish,
            ["sw"] = LanguageCode.Kiswahili,
            ["syr"] = LanguageCode.Syriac,
            ["ta"] = LanguageCode.Tamil,
            ["te"] = LanguageCode.Telugu,
            ["tg"] = LanguageCode.Tajik,
            ["th"] = LanguageCode.Thai,
            ["tk"] = LanguageCode.Turkmen,
            ["tn"] = LanguageCode.Setswana,
            ["tr"] = LanguageCode.Turkish,
            ["tt"] = LanguageCode.Tatar,
            ["tzm"] = LanguageCode.Tamazight,
            ["ug"] = LanguageCode.Uyghur,
            ["uk"] = LanguageCode.Ukrainian,
            ["ur"] = LanguageCode.Urdu,
            ["uz"] = LanguageCode.Uzbek,
            ["vi"] = LanguageCode.Vietnamese,
            ["wo"] = LanguageCode.Wolof,
            ["xh"] = LanguageCode.IsiXhosa,
            ["yo"] = LanguageCode.Yoruba,
            ["zh"] = LanguageCode.Chinese,
            ["zu"] = LanguageCode.IsiZulu,
        };

        /// <summary>
        /// Парсит .IETF Language Code в <see cref="LanguageCode"/>
        /// </summary>
        /// <param name="languageCode">Код в формате <code>en</code> или <code>en-US</code></param>
        /// <returns><see cref="LanguageCode"/>, соответствующий <paramref name="languageCode"/>. В случае, если совпадение не найдено, возвращает <see cref="LanguageCode.Undefined"/></returns>
        public LanguageCode GetLanguageCode(string languageCode)
        {
            if (string.IsNullOrWhiteSpace(languageCode))
                return LanguageCode.Undefined;

            if (languageCode.Contains("-"))
                languageCode = languageCode.Substring(0, languageCode.IndexOf("-"));

            bool codeGetted = languagesCodes.TryGetValue(languageCode, out var code);

            if (!codeGetted)
                return LanguageCode.Undefined;

            return code;
        } 
    }
}
