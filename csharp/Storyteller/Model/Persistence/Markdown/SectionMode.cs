using Baseline;

namespace Storyteller.Model.Persistence.Markdown
{
    public class SectionMode : IReaderMode
    {
        public Section Section { get; }

        public SectionMode(Section section)
        {
            Section = section;
        }

        public IReaderMode Read(int indention, string text)
        {
            if (text.StartsWith("|>"))
            {
                if (indention == Indention)
                {
                    var mode = new StepMode(text);
                    Section.Children.Add(mode.Step);

                    return mode;
                }

                return null;
            }

            // Just keep going
            if (text.IsEmpty()) return this;

            // Need to pop this mode in the reader
            if (text.IsSectionHeader()) return null;

            if (text.IsMetadata())
            {
                string cell;
                string value;

                text.ParseMetadata(out cell, out value);

                Section.ActiveCells.Add(cell, bool.Parse(value));

                return this;
            }

            if (indention != Indention) return null;

            if (text == MarkdownWriter.SectionEnd) return null;

            if (text.IsTableLine())
            {
                var table = new TableParser(Section)
                {
                    Indention = indention
                };

                table.Read(indention, text);

                return table;
            }

            // Assume it's a comment
            var comment = new CommentMode(Section);
            comment.Read(indention, text);

            return comment;
        }

        public int Indention { get; set; }
    }
}