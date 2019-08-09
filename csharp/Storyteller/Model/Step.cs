﻿using System;
using System.Collections.Generic;
using System.Linq;
using Baseline;
using Newtonsoft.Json;
using Storyteller.Model.Persistence.Markdown;

namespace Storyteller.Model
{
#if NET46
    [Serializable]
#endif
    public class Step : Node, INodeHolder
    {
        [JsonIgnore] public readonly LightweightCache<string, Section> Collections =
            new LightweightCache<string, Section>(key => new Section(key));


        [JsonProperty("key")] public readonly string Key;

        [JsonProperty("cells")] public readonly IDictionary<string, string> Values = new Dictionary<string, string>();

        public Step(string key)
        {
            Key = key;
        }

        public Section[] collections
        {
            get { return Collections.GetAll(); }
            set
            {
                value.Each(pair => Collections[pair.Key] = pair);
            }
        }

        public Step With(string key, string value)
        {
            Values.Add(key, value);
            return this;
        }

        public static IDictionary<string, string> ParseValues(string text)
        {
            var dict = new Dictionary<string, string>();
            if (text.IsEmpty()) return dict;

            string[] data = text.Split(',');
            foreach (string property in data)
            {
                string[] parts = property.Split(':');
                dict.Add(parts[0].Trim(), parts[1].Trim());
            }

            return dict;
        }

        public void AssertValuesMatch(Step other)
        {
            if (other.Values.Count != Values.Count) throwValuesDoNotMatch(other);

            string[] otherKeys = other.Values.Keys.OrderBy(x => x).ToArray();
            string[] keys = Values.Keys.OrderBy(x => x).ToArray();
            if (!otherKeys.SequenceEqual(keys)) throwValuesDoNotMatch(other);

            other.Values.Keys.Each(key => { if (other.Values[key] != Values[key]) throwValuesDoNotMatch(other); });
        }

        private void throwValuesDoNotMatch(Step other)
        {
            throw new Exception("Step values do not match. \n  1st --> {0}\n  2nd --> {1}".ToFormat(ToValueString(),
                other.ToValueString()));
        }

        public string ToValueString()
        {
            string values = Values.ToArray()
                .Select(pair => "{0}={1}".ToFormat(pair.Key, pair.Value))
                .Join(", ");

            return "Step '{0}' with values {1}".ToFormat(Key, values);
        }

        public Section AddCollection(string key)
        {
            var section = new Section(key);
            Collections[key] = section;

            return section;
        }

        [JsonIgnore]
        public IList<Node> Children => Collections.OfType<Node>().ToList();

        [JsonIgnore]
        public string[] StagedValues { get; set; }


        public override string ToString()
        {
            return ToValueString();
        }

        public static Step Parse(string text)
        {
            var line = text.Trim().Substring(2).Trim();
            var tokens = line.Tokenize().ToArray();

            var key = tokens[0];
            Step step = null;
            if (key.Contains("#"))
            {
                var parts = key.ToDelimitedArray('#');
                step = new Step(parts[0])
                {
                    id = parts[1]
                };
            }
            else
            {
                step = new Step(key);
            }

            var valueIndex = line.IndexOf(key) + key.Length;
            var valueText = line.Substring(valueIndex);

            var values = valueText.ToDelimitedArray();
            if (!values.Any() || valueText.IsEmpty()) return step;

            if (values.All(x => x.Contains("=")))
            {
                foreach (var value in values)
                {
                    var parts = value.TrimEnd(',').ToDelimitedArray('=');
                    step.Values.Add(parts[0], parts[1]);
                }
            }
            else
            {
                step.StagedValues = values;
            }

            return step;
        }

        public void ProcessCells(Cell[] cells, IStepValidator stepValidator)
        {
            if (cells == null || cells.Length == 0) return;

            if (StagedValues != null)
            {
                for (int i = 0; i < StagedValues.Length; i++)
                {
                    if (cells.Length <= i) break;

                    Values[cells[i].Key] = StagedValues[i];
                }
            }

            foreach (var cell in cells)
            {
                if (Values.ContainsKey(cell.Key)) continue;

                if (cell.DefaultValue.IsNotEmpty())
                {
                    Values[cell.Key] = cell.DefaultValue;
                }
                else
                {
                    stepValidator.AddError($"Missing value for '{cell.Key}'");
                }
            }
        }
    }
}