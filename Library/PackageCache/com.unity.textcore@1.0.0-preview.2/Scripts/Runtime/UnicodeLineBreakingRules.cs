﻿using System;
using System.Collections.Generic;


namespace UnityEngine.TextCore.Text
{
    [Serializable]
    public class UnicodeLineBreakingRules
    {
        private static UnicodeLineBreakingRules s_Instance = new UnicodeLineBreakingRules();

        /// <summary>
        /// Text file that contains the Unicode line breaking rules defined here https://www.unicode.org/reports/tr14/tr14-22.html
        /// </summary>
        public UnityEngine.TextAsset lineBreakingRules
        {
            get => m_UnicodeLineBreakingRules;
        }
        [SerializeField]
        #pragma warning disable 0649
        private UnityEngine.TextAsset m_UnicodeLineBreakingRules;

        /// <summary>
        /// Text file that contains the leading characters
        /// </summary>
        public UnityEngine.TextAsset leadingCharacters
        {
            get => m_LeadingCharacters;
        }
        [SerializeField]
        #pragma warning disable 0649
        private UnityEngine.TextAsset m_LeadingCharacters;

        /// <summary>
        /// Text file that contains the following characters
        /// </summary>
        public UnityEngine.TextAsset followingCharacters
        {
            get => m_FollowingCharacters;
        }
        [SerializeField]
        #pragma warning disable 0649
        private UnityEngine.TextAsset m_FollowingCharacters;

        /// <summary>
        ///
        /// </summary>
        public HashSet<uint> leadingCharactersLookup
        {
            get => s_LeadingCharactersLookup;
            internal set => s_LeadingCharactersLookup = value;
        }

        /// <summary>
        ///
        /// </summary>
        public HashSet<uint> followingCharactersLookup
        {
            get => s_FollowingCharactersLookup;
            internal set => s_FollowingCharactersLookup = value;
        }

        /// <summary>
        /// Determines if Modern or Traditional line breaking rules should be used for Korean text.
        /// </summary>
        public bool useModernHangulLineBreakingRules
        {
            get => m_UseModernHangulLineBreakingRules;
            set => m_UseModernHangulLineBreakingRules = value;
        }
        [SerializeField]
        private bool m_UseModernHangulLineBreakingRules;

        // =============================================
        // Private backing fields for public properties.
        // =============================================

        private static HashSet<uint> s_LeadingCharactersLookup;
        private static HashSet<uint> s_FollowingCharactersLookup;

        /// <summary>
        ///
        /// </summary>
        public static void LoadLineBreakingRules()
        {
            if (s_LeadingCharactersLookup == null)
            {
                UnityEngine.TextAsset leadingRules = Resources.Load<UnityEngine.TextAsset>("LineBreaking Leading Characters");
                s_LeadingCharactersLookup = leadingRules != null ? GetCharacters(leadingRules) : new HashSet<uint>();

                UnityEngine.TextAsset followingRules = Resources.Load<UnityEngine.TextAsset>("LineBreaking Following Characters");
                s_FollowingCharactersLookup = followingRules != null ? GetCharacters(followingRules) : new HashSet<uint>();
            }
        }

        public static void LoadLineBreakingRules(UnityEngine.TextAsset leadingRules, UnityEngine.TextAsset followingRules)
        {
            if (s_LeadingCharactersLookup == null)
            {
                if (leadingRules == null)
                    leadingRules = Resources.Load<UnityEngine.TextAsset>("LineBreaking Leading Characters");

                s_LeadingCharactersLookup = leadingRules != null ? GetCharacters(leadingRules) : new HashSet<uint>();

                if (followingRules == null)
                    followingRules = Resources.Load<UnityEngine.TextAsset>("LineBreaking Following Characters");

                s_FollowingCharactersLookup = followingRules != null ? GetCharacters(followingRules) : new HashSet<uint>();
            }
        }

        private static HashSet<uint> GetCharacters(UnityEngine.TextAsset file)
        {
            HashSet<uint> ruleSet = new HashSet<uint>();
            string text = file.text;

            for (int i = 0; i < text.Length; i++)
                ruleSet.Add(text[i]);

            return ruleSet;
        }
    }
}
