/**
* Copyright (c) 2020 Nodesk Inc. All rights reserved.
*/

using System.Collections.Generic;

namespace Nodesk.Scripts.Core.Converter
{
    public static class NodeskKeyToCharacter
    {
     
        //OS固有動作をするキー(元のキーコードとの論理和を取る)
        private const int ExtKeyCode = 0x100;
        
        private static readonly Dictionary<string, string> ArrowKeyMap = new Dictionary<string, string>
        {
            {"↑", "ArrowUp"},
            {"→", "ArrowRight"},
            {"↓", "ArrowDown"},
            {"←", "ArrowLeft"},
        };
        
        private static readonly Dictionary<string, string> LowerAlphabetKeyMap = new Dictionary<string, string>
        {
            {"a","a"},
            {"b","b"},
            {"c","c"},
            {"d","d"},
            {"e","e"},
            {"f","f"},
            {"g","g"},
            {"h","h"},
            {"i","i"},
            {"j","j"},
            {"k","k"},
            {"l","l"},
            {"m","m"},
            {"n","n"},
            {"o","o"},
            {"p","p"},
            {"q","q"},
            {"r","r"},
            {"s","s"},
            {"t","t"},
            {"u","u"},
            {"v","v"},
            {"w","w"},
            {"x","x"},
            {"y","y"},
            {"z","z"},
        };
        
        
        private static readonly Dictionary<string, string> UpperAlphabetKeyMap = new Dictionary<string, string>
        {
            {"A","A"},
            {"B","B"},
            {"C","C"},
            {"D","D"},
            {"E","E"},
            {"F","F"},
            {"G","G"},
            {"H","H"},
            {"I","I"},
            {"J","J"},
            {"K","K"},
            {"L","L"},
            {"M","M"},
            {"N","N"},
            {"O","O"},
            {"P","P"},
            {"Q","Q"},
            {"R","R"},
            {"S","S"},
            {"T","T"},
            {"U","U"},
            {"V","V"},
            {"W","W"},
            {"X","X"},
            {"Y","Y"},
            {"Z","Z"},
        };
        
        
        private static readonly Dictionary<string, string> SignKeyMap = new Dictionary<string, string>
        {
            {"0","0" },
            {"1","1" },
            {"2","2" },
            {"3","3" },
            {"4","4" },
            {"5","5" },
            {"6","6" },
            {"7","7" },
            {"8","8" },
            {"9","9" },
            {"+","+" },
            {"-","-" },
            {"*","*"  },
            {"/","/" },
            {"=","=" },
            {"#","#" },
            {"|","|" },
            {"$","$" },
            {"'","'" },
            {"\"","\""},
            {"%","%" },
            {"<","<" },
            {">",">" },
            {"^","^" },
            {":",":" },
            {"(","(" },
            {")",")" },
            {"!","!" },
            {"?","?" },
            {".","." },
            {",","," },
            {";",";" },
            {"~","~" },
            {"_","_" },
            {"@","@" },
            {"&","&" },
            {"\\","\\"},
            {"{","{"},
            {"}","}"},
            {"[","["},
            {"]","]"},
        };
        
        private static readonly Dictionary<string, string> ControlKeyMap = new Dictionary<string, string>
        {
            {"Space", " "},
            {"Enter", "\n"},
            {"Tab", "\t"},
            {"BkSp", "BackSpace"},
            {"Submit", "Submit"},
        };
        
        
        public static bool TryConvertKey(string str, out string code)
        {
            
            if (ControlKeyMap.TryGetValue(str, out code))
            {
                return true;
            }

            
            if (ArrowKeyMap.TryGetValue(str, out code))
            {
                return true;
            }

            if (LowerAlphabetKeyMap.TryGetValue(str, out code))
            {
                return true;
            }
            
            if (UpperAlphabetKeyMap.TryGetValue(str, out code))
            {
                return true;
            }
            
            if (SignKeyMap.TryGetValue(str, out code))
            {
                return true;
            }

            

            return false;
        }
    }
}