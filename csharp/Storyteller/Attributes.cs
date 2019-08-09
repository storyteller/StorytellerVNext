using System;
using Storyteller.Model;

namespace Storyteller
{
    public abstract class ModifyCellAttribute : Attribute
    {
        public abstract void Modify(Cell cell);
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class FormatAsAttribute : Attribute
    {
        public FormatAsAttribute(string format)
        {
            Format = format;
        }

        public string Format { get; }
    }

    [AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Method, Inherited = false)]
    public class AliasAsAttribute : ModifyCellAttribute
    {
        public AliasAsAttribute(string alias)
        {
            Alias = alias;
        }

        public override void Modify(Cell cell)
        {
            cell.Key = Alias;
        }

        public string Alias { get; }
    }

    [AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Parameter | AttributeTargets.Property,
        AllowMultiple = false, Inherited = false)]
    public class HeaderAttribute : ModifyCellAttribute
    {
        private readonly string _header;

        public HeaderAttribute(string header)
        {
            _header = header;
        }

        public override void Modify(Cell cell)
        {
            cell.header = _header;
        }
    }

    [AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Parameter | AttributeTargets.Property,
        AllowMultiple = false, Inherited = false)
    ]
    public class DefaultAttribute : ModifyCellAttribute
    {
        private readonly string _value;

        public DefaultAttribute(string value)
        {
            _value = value;
        }

        public string Value => _value == "GUID" ? Guid.NewGuid().ToString() : _value;

        public override void Modify(Cell cell)
        {
            cell.DefaultValue = _value;
        }


    }

    [AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Parameter | AttributeTargets.Property,
        AllowMultiple = false, Inherited = false)
    ]
    public class EditorAttribute : ModifyCellAttribute
    {
        public EditorAttribute(string editor)
        {
            Editor = editor;
        }

        public string Editor { get; }

        public override void Modify(Cell cell)
        {
            cell.editor = Editor;
        }


    }

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue)]
    public class SelectionValuesAttribute : ModifyCellAttribute
    {
        private readonly string[] _values;

        public SelectionValuesAttribute(Type enumType)
            : this(Enum.GetNames(enumType))
        {
        }

        public SelectionValuesAttribute(params string[] values)
        {
            if (values.Length == 0)
            {
                throw new ArgumentOutOfRangeException(
                    "At least one value needs to be supplied in the constructor function");
            }
            _values = values;
        }

        public override void Modify(Cell cell)
        {
            cell.editor = "select";
            cell.options = Option.For(_values);
        }
    }

    public class SelectionListAttribute : ModifyCellAttribute
    {
        private readonly string _listName;

        public SelectionListAttribute(string listName)
        {
            _listName = listName;
        }

        public override void Modify(Cell cell)
        {
            cell.editor = "select";
            cell.OptionListName = _listName;
        }
    }
}