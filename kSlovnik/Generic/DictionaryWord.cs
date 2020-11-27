using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kSlovnik.Generic
{
    public class DictionaryWord
    {
        /// <summary>
        /// Представки.
        /// </summary>
        public List<string> Prefixes { get; set; } = new List<string>();

        /// <summary>
        /// Корен.
        /// </summary>
        public string Root { get; set; } = string.Empty;

        /// <summary>
        /// Наставки.
        /// </summary>
        public List<string> Suffixes { get; set; } = new List<string>();

        /// <summary>
        /// Окончание
        /// </summary>
        public string Ending { get; set; } = string.Empty;

        /// <summary>
        /// Определителен член.
        /// </summary>
        public string DefiniteArticle { get; set; } = string.Empty;

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

        public bool? IsApproved { get; set; }
    }
}
