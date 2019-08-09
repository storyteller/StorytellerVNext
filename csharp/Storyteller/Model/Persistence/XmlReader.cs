﻿using System;
using System.Xml;
using Baseline;
using Storyteller.Util;

namespace Storyteller.Model.Persistence
{
    public class XmlReader : XmlConstants
    {
        public static Specification ReadFromFile(string path)
        {
            var document = new XmlDocument().FromFile(path);
            var specification = ReadFromXml(document);

            if (specification.NeedsToBeRenumbered())
            {
                Console.WriteLine("Renumbering id's in {0} because duplicates were detected", path);
                specification.ApplyRenumbering();
            }

            specification.Filename = path;

            return specification;

        }

        public static Specification ReadFromXml(XmlDocument document)
        {
            var spec = ReadHeaderInformation(document);

            ReadBody(document, spec);

            return spec;
        }

        public static void ReadBody(XmlDocument document, Specification spec)
        {
            document.DocumentElement.ForEachElement(element =>
            {
                if (element.Name == Comment)
                {
                    spec.Children.Add(ReadComment(element));
                }
                else
                {
                    spec.Children.Add(ReadSection(element));
                }
            });

            if (spec.NeedsToBeRenumbered())
            {
                spec.ApplyRenumbering();
            }
        }

        public static Specification ReadHeaderInformation(XmlDocument document)
        {
            var spec = new Specification();
            var top = document.DocumentElement;
            spec.name = top.GetAttribute("name");

            var lifecycle = top.GetAttribute(LifecycleAtt);
            spec.Lifecycle = lifecycle.AsLifecycle();

            spec.id = top.ReadId();
            var maxRetries = top.GetAttribute(MaxRetries);
            spec.MaxRetries = maxRetries.IsEmpty() ? 0 : int.Parse(maxRetries);

            try
            {
                var lastUpdatedString = top.GetAttribute(LastUpdated);
                var lastUpdated = !lastUpdatedString.IsEmpty() ? DateTime.Parse(top.GetAttribute(LastUpdated)) : DateTime.Now;
                spec.LastUpdated = lastUpdated;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error trying to read the last updated date\n {e}");
            }


            spec.name = top.GetAttribute(Name);

            var tags = top.GetAttribute(TagsAtt);
            if (tags.IsNotEmpty())
            {
                spec.Tags = tags.ToDelimitedArray();
            }
            return spec;
        }

        public static Section ReadSection(XmlElement element)
        {
            var section = new Section(XmlConvert.DecodeName(element.Name))
            {
                id = element.GetAttribute(Id)
            };

            if (element.HasAttribute(ActiveCells))
            {
                var text = element.GetAttribute(ActiveCells);
                if (!text.IsEmpty())
                {
                    text.Split(',').Each(str =>
                    {
                        var parts = str.Split('=');
                        section.ActiveCells.Add(parts[0], bool.Parse(parts[1]));
                    });
                }
            }

            element.ForEachElement(child =>
            {
                if (child.Name == Comment)
                {
                    section.Children.Add(ReadComment(child));
                }
                else
                {
                    section.Children.Add(ReadStep(child));                    
                }
            });


            return section;
        }

        public static Step ReadStep(XmlElement child)
        {
            var step = new Step(XmlConvert.DecodeName(child.Name)) {id = child.ReadId()};

            foreach (XmlAttribute att in child.Attributes)
            {
                if (att.Name == "isStep") continue; // Old detritus

                step.Values[XmlConvert.DecodeName(att.Name)] = att.Value;
            }

            child.ForEachElement(collection =>
            {
                var section = ReadSection(collection);
                step.Collections[collection.Name] = section;
            });

            return step;
        }

        private static Comment ReadComment(XmlElement element)
        {
            return new Comment{id = element.ReadId(), Text = element.InnerText};
        }
    }
}
