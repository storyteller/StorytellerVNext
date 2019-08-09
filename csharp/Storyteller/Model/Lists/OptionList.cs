﻿using System.Collections.Generic;
using System.Linq;
using Baseline;

namespace Storyteller.Model.Lists
{
    public class OptionList
    {
        private readonly IList<Option> _options = new List<Option>();

        public readonly string Name;

        public OptionList(string name)
        {
            Name = name;
        }

        public void AddValues(params string[] values)
        {
            _options.AddRange(Option.For(values));
        }

        public void AddOption(string display, string value)
        {
            _options.Add(new Option{display = display, value = value});
        }

        public void AddOptions(IEnumerable<Option> options)
        {
            _options.AddRange(options);
        }

        public Option[] Options => _options.ToArray();

    }

    
}