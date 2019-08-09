﻿using System;
using System.IO;
using Baseline;
using Storyteller.Model;

namespace Storyteller
{
    public static class StringExtensions
    {
        public static string EscapeIllegalChars(this string initial)
        {
            var answer = initial;
            Path.GetInvalidFileNameChars().Each(c =>
            {
                answer = answer.Replace(c.ToString(), "");
            });

            return answer;
        }

        public static Lifecycle AsLifecycle(this string text)
        {
            if (text.IsEmpty()) return Lifecycle.Acceptance;

            return (Lifecycle) Enum.Parse(typeof (Lifecycle), text);
        }

        public static int LeadingSpaces(this string text)
        {
            var i = 0;

            foreach (char c in text)
            {
                if (c == ' ')
                {
                    i++;
                }
                else
                {
                    break;
                }
            }

            return i;
        }

        public static bool IsGuidString(this string text)
        {
            if (text.IsEmpty()) return false;

            Guid id;
            return Guid.TryParse(text, out id);
        }
    }
}