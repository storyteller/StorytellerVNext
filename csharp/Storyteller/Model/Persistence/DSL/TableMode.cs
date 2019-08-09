﻿using System.Linq;
using Baseline;
using Storyteller.Model.Persistence.Markdown;

namespace Storyteller.Model.Persistence.DSL
{
    public class TableMode : GrammarModeBase
    {
        private readonly Table _table;

        public TableMode(Table table)
        {
            _table = table;
        }

        public override IReaderMode Read(int indention, string text)
        {
            if (!text.IsTableLine()) return null;

            var values = text.ToTableValues();

            if (_table.cells == null)
            {
                _table.cells = values
                    .Skip(1)
                    .Select(x => new Cell(CellHandling.Basic(), x, typeof(string)))
                    .ToArray();

                return this;
            }

            var target = values.First();
            var rest = values.Skip(1).Take(_table.cells.Length).ToList();

            if (!rest.Any()) return null;

            rest.Each((value, i) =>
            {
                var cell = _table.cells[i];
                applyValue(target, cell, value);
            });

            return this;
        }
    }
}
