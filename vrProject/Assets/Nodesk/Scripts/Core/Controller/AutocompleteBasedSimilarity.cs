using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class AutocompleteBasedSimilarity : MonoBehaviour
{
	public string[] suggestions;
	public int[] costs;

    private List<string> corpus = new List<string>();
   
	public TextAsset google20k;
	
    void Start()
    {
        corpus = GetList(google20k.text);
		suggestions = new string[10];
		costs = new int[10];
    }
    
    private List<string> GetList(string s)
    {
	    string[] tokens = s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
	    return tokens.ToList();
    }

    public List<string> DoAutocomplete(string aString)
    {
        if (string.IsNullOrEmpty(aString)) return new List<string>();

        var firstUpper = char.IsUpper(aString[0]);

        aString = aString.ToLower();
        
		for (int i = 0; i < costs.Length; i++)
		{
			costs[i] = int.MaxValue;
		}

		for (int i = 0; i < corpus.Count; i++)
        {
			string word = corpus[i];
            int cost = (int)(JaroWinklerDistance.distance(aString, word) * 1000.0 );
            for (int c = 0; c < costs.Length; c++)
			{
				if (cost < costs[c])
				{
					for (int c2 = costs.Length-1; c2 > c; c2--)
					{
						costs[c2] = costs[c2-1];
						suggestions[c2] = suggestions[c2-1];
					}
					costs[c] = cost;
					suggestions[c] = word;
					break;
				}
			}
        }

		if (firstUpper)
		{
			for(int i = 0 ; i < suggestions.Length; i++)
			{
				suggestions[i] = char.ToUpper(suggestions[i][0])  + suggestions[i].Substring(1);
			}
		}
		
		
		return suggestions.ToList();
    }
    
    public static class JaroWinklerDistance
    {
        /* The Winkler modification will not be applied unless the 
         * percent match was at or above the mWeightThreshold percent 
         * without the modification. 
         * Winkler's paper used a default value of 0.7
         */
        private static readonly double mWeightThreshold = 0.863;

        /* Size of the prefix to be concidered by the Winkler modification. 
         * Winkler's paper used a default value of 4
         */
        private static readonly int mNumChars = 4;


        /// <summary>
        /// Returns the Jaro-Winkler distance between the specified  
        /// strings. The distance is symmetric and will fall in the 
        /// range 0 (perfect match) to 1 (no match). 
        /// </summary>
        /// <param name="aString1">First String</param>
        /// <param name="aString2">Second String</param>
        /// <returns></returns>
        public static double distance(string aString1, string aString2) {
            return 1.0 - proximity(aString1,aString2);
        }


        /// <summary>
        /// Returns the Jaro-Winkler distance between the specified  
        /// strings. The distance is symmetric and will fall in the 
        /// range 0 (no match) to 1 (perfect match). 
        /// </summary>
        /// <param name="aString1">First String</param>
        /// <param name="aString2">Second String</param>
        /// <returns></returns>
        public static double proximity(string aString1, string aString2)
        {
            int lLen1 = aString1.Length;
            int lLen2 = aString2.Length;
            if (lLen1 == 0)
                return lLen2 == 0 ? 1.0 : 0.0;

            int  lSearchRange = Math.Max(0,Math.Max(lLen1,lLen2)/2 - 1);

            // default initialized to false
            bool[] lMatched1 = new bool[lLen1];
            bool[] lMatched2 = new bool[lLen2];

            int lNumCommon = 0;
            for (int i = 0; i < lLen1; ++i) {
                int lStart = Math.Max(0,i-lSearchRange);
                int lEnd = Math.Min(i+lSearchRange+1,lLen2);
                for (int j = lStart; j < lEnd; ++j) {
                    if (lMatched2[j]) continue;
                    if (aString1[i] != aString2[j])
                        continue;
                    lMatched1[i] = true;
                    lMatched2[j] = true;
                    ++lNumCommon;
                    break;
                }
            }
            if (lNumCommon == 0) return 0.0;

            int lNumHalfTransposed = 0;
            int k = 0;
            for (int i = 0; i < lLen1; ++i) {
                if (!lMatched1[i]) continue;
                while (!lMatched2[k]) ++k;
                if (aString1[i] != aString2[k])
                    ++lNumHalfTransposed;
                ++k;
            }
            int lNumTransposed = lNumHalfTransposed/2;

            double lNumCommonD = lNumCommon;
            double lWeight = (lNumCommonD/lLen1
                             + lNumCommonD/lLen2
                             + (lNumCommon - lNumTransposed)/lNumCommonD)/3.0;

            if (lWeight <= mWeightThreshold) return lWeight;
            int lMax = Math.Min(mNumChars,Math.Min(aString1.Length,aString2.Length));
            int lPos = 0;
            while (lPos < lMax && aString1[lPos] == aString2[lPos])
                ++lPos;
            if (lPos == 0) return lWeight;
            return lWeight + 0.1 * lPos * (1.0 - lWeight);

        }
	}
}