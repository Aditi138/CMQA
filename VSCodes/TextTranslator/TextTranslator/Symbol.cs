using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextTranslator
{
    public class Symbol
    {
        public String symbol;
        public CharTypeEnum key;

        public Symbol(String symbol, CharTypeEnum key)
        {
            this.symbol = symbol;
            this.key = key;

        }
    }
}
