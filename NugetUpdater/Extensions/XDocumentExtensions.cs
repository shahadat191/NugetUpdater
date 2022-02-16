using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NugetUpdater.Extensions
{
    public static class XDocumentExtensions
    {
        public static XElement ElementByTag(this XDocument document, string tagName)
        {
            return document.Elements().FirstOrDefault(x => x.Name.LocalName == tagName);
        }
        public static XElement ElementByTag(this XElement document, string tagName)
        {
            return document.Elements().FirstOrDefault(x => x.Name.LocalName == tagName);
        }
        public static IEnumerable<XElement> ElementsByTag(this XElement document, string tagName)
        {
            return document.Elements().Where(x => x.Name.LocalName == tagName);
        }
    }
}
