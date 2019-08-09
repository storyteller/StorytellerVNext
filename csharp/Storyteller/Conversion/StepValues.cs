﻿using System;
using System.Collections.Generic;
using System.Linq;
using Baseline;
using Storyteller.Model;
using Storyteller.Results;

namespace Storyteller.Conversion
{
    public class StepValues : Node
    {
        private readonly IList<Delayed> _delayeds = new List<Delayed>();

        public StepValues(string id)
        {
            this.id = id;
        }

        public void DoDelayedConversions(IExecutionContext context)
        {
            _delayeds.Each(x =>
            {
                try
                {
                    var converted = x.Converter.Convert(x.Raw, context);
                    if (converted == null)
                    {
                        LogError(x.Key, "The converter was not able to create a value. Check the formatting.");
                    }
                    else
                    {
                        Store(x.Key, converted);
                    }
                }
                catch (Exception ex)
                {
                    LogError(x.Key, ex);
                }
            });
        }

        public IDictionary<string, object> RawData { get; } = new Dictionary<string, object>();

        /// <summary>
        /// For set verification purposes
        /// </summary>
        public bool IsMatched { get; set; }

        /// <summary>
        /// For set verification purposes
        /// </summary>
        public int Order { get; set; }

        public void RegisterDelayedConversion(string key, string raw, IRuntimeConverter converter)
        {
            _delayeds.Add(new Delayed
            {
                Key = key, Raw = raw, Converter = converter
            });
        }

        public void Store(string key, object value)
        {
            RawData.Add(key, value);
        }

        public object Get(string key)
        {
            return RawData[key];
        }

        public IList<CellResult> Errors = new List<CellResult>();

        public StepResult ToConversionErrorResult()
        {
            return new StepResult(id, ResultStatus.ok)
            {
                cells = Errors.ToArray()
            };
        }

        public void LogError(string key, Exception ex)
        {
            Errors.Add(CellResult.Error(key, ex));
        }

        public void LogError(string key, string message)
        {
            Errors.Add(CellResult.Error(key, message));
        }

        public class Delayed
        {
            public string Key;
            public string Raw;
            public IRuntimeConverter Converter;
        }

        public bool Has(string key)
        {
            return RawData.ContainsKey(key);
        }

        public bool HasErrors()
        {
            return Errors.Any();
        }

        public IEnumerable<Delayed> DelayedConversions => _delayeds;
    }

    public static class StepValuesExtensions
    {
        public static void ApplyOrdering(this IEnumerable<StepValues> list)
        {
            var i = 0;
            list.Each(x => x.Order = ++i);
        }
    }
}