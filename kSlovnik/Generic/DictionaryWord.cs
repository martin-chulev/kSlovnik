using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace kSlovnik.Generic
{
    public class DictionaryWord
    {
        /// <summary>
        /// Представки.
        /// </summary>
        [JsonPropertyName("Pfx")]
        public List<string> Prefixes { get; set; } = new List<string>();

        /// <summary>
        /// Корен.
        /// </summary>
        [JsonPropertyName("Rt")]
        public string Root { get; set; } = string.Empty;

        /// <summary>
        /// Наставки.
        /// </summary>
        [JsonPropertyName("Sfx")]
        public List<string> Suffixes { get; set; } = new List<string>();

        /// <summary>
        /// Окончание.
        /// </summary>
        [JsonPropertyName("End")]
        public string Ending { get; set; } = string.Empty;

        /// <summary>
        /// Определителен член.
        /// </summary>
        [JsonPropertyName("DefArt")]
        public string DefiniteArticle { get; set; } = string.Empty;

        [JsonIgnore]
        public string FullWord
        {
            get
            {
                var wordBuilder = new StringBuilder();

                foreach (var prefix in Prefixes)
                {
                    wordBuilder.Append(prefix);
                }
                wordBuilder.Append(Root);
                foreach (var suffix in Suffixes)
                {
                    wordBuilder.Append(suffix);
                }
                wordBuilder.Append(Ending);
                wordBuilder.Append(DefiniteArticle);

                return wordBuilder.ToString();
            }
        }

        [JsonIgnore]
        public string FullWordWithMarks
        {
            get
            {
                var wordBuilder = new StringBuilder();

                foreach (var prefix in Prefixes)
                {
                    wordBuilder.Append($"({prefix}-)");
                }

                wordBuilder.Append($"[{Root}]");

                foreach (var suffix in Suffixes)
                {
                    wordBuilder.Append($"(-{suffix})");
                }
                if(string.IsNullOrEmpty(Ending) == false) wordBuilder.Append($"-<{Ending}>");
                if (string.IsNullOrEmpty(DefiniteArticle) == false) wordBuilder.Append($"{{{DefiniteArticle}}}");

                return wordBuilder.ToString();
            }
        }

        [JsonPropertyName("Appr")]
        public bool? IsApproved { get; set; }
    }
}
