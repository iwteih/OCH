using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OCHUtil
{
    public class MessageFormatter
    {
        private static Regex FORMATSENDTIMESTAMP = new Regex(@"<SPAN id=imsendtimestamp style=""FONT-SIZE: [0-9]+pt; WHITE-SPACE: nowrap; FLOAT: right; COLOR: #[0-9]+; PADDING-TOP: [0-9]+px\"">((20|21|22|23|[0-1]?\d):[0-5]?\d)</SPAN>");

        private static string NormalizeTimeStamp(Match m)
        {
            if (m.Groups.Count > 1)
            {
                string placed = string.Format("{0} {1}", DateTime.Now.ToString("yyyy-MM-dd"), m.Groups[1].Value);
                placed = m.Value.Replace(m.Groups[1].Value, placed);
                return placed;
            }

            return m.Value;
        }

        public static string FormatSendTimeStamp(string input)
        {
            MatchEvaluator evaluator = new MatchEvaluator(NormalizeTimeStamp);
            return FORMATSENDTIMESTAMP.Replace(input, evaluator);
        }
    }
}
