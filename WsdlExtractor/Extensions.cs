using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WsdlExtractor
{
    /// <summary>
    /// Some extensions to work with data
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Merge schemas from different XSD's
        /// </summary>
        public static void MergeSchemas(this SchemaRequest dest, SchemaRequest source)
        {
            foreach (var url in source.XsdUrls)
            {
                if (!dest.XsdUrls.Contains(url))
                    dest.XsdUrls.Add(url);
            }

            dest.Types.MergeDictionaries(source.Types);
        }

        /// <summary>
        /// Merge dictionaries together
        /// </summary>
        public static void MergeDictionaries(this Dictionary<string, List<TypeDescription>> dest, Dictionary<string, List<TypeDescription>> source)
        {
            foreach (var typeKey in source.Keys)
            {
                if (!dest.Keys.Contains(typeKey))
                {
                    dest.Add(typeKey, source[typeKey]);
                }
                else
                {
                    var list = dest[typeKey].ToList();
                    list.AddIfNotExists(source[typeKey]);
                    dest[typeKey] = list;
                }
            }
        }

        /// <summary>
        /// Add all Source values to Destination if they're not already there
        /// </summary>
        public static void AddIfNotExists(this List<TypeDescription> dest, List<TypeDescription> source)
        {
            foreach (var item in source)
            {
                if (!dest.Contains(item))
                    dest.Add(item);
            }
        }
    }
}
