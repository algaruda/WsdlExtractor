using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Web.Services.Description;
using System.Xml.Linq;

namespace WsdlExtractor
{
    /// <summary>
    /// Core class for getting web service types descriptions
    /// </summary>
    public static class Core
    {
        /// <summary>
        /// Get data types from webservice
        /// </summary>
        /// <param name="url">WSDL to discover</param>
        /// <returns>List of schema xsds and request/response dictionary</returns>
        public static SchemaRequest GetDataTypes(string url)
        {
            url = url.Trim();

            if (!url.ToLower().EndsWith("?wsdl"))
            {
                url += "?wsdl";
            }

            var xsd = GetXsdLocationsAndTypes(url);

            if (xsd.Types.Count > 0)
            {
                var schemas = GetAllTypes(xsd.XsdUrls, xsd.Types);

                return schemas;
            }
            else
            {
                WebClient client = new WebClient();
                var content = client.DownloadString(url);
                if (content.Contains("Request\""))
                {

                }
            }

            return xsd;
        }

        /// <summary>
        /// Get specific type info from list of ulrs
        /// </summary>
        public static SchemaRequest GetAllTypes(List<string> urls, Dictionary<string, List<TypeDescription>> types)
        {
            var result = new SchemaRequest() { XsdUrls = urls };

            foreach (var url in urls)
            {
                var description = GetTypesDescription(url, types);

                if (description != null && description.Keys.Count > 0)
                {
                    result.Types.MergeDictionaries(description);
                }
            }

            return result;
        }

        /// <summary>
        /// Get specific type information from this url
        /// </summary>
        public static Dictionary<string, List<TypeDescription>> GetTypesDescription(string url, Dictionary<string, List<TypeDescription>> types)
        {
            var result = types;
            var root = XElement.Load(url);

            foreach (var datatype in types.Keys)
            {
                var tolo = root.Elements().Where(o => o.FirstAttribute != null && o.FirstAttribute.Value == datatype && o.HasElements).FirstOrDefault();
                //var complexType = tolo.NextNode;
                var sequence = tolo?.Elements().Where(o => o.Name.LocalName == "sequence")?.FirstOrDefault();
                if (sequence == null)
                {
                    if (tolo != null)
                    {
                        var complexType = tolo.FirstNode as XElement;
                        var inner = complexType.FirstNode as XElement;
                        sequence = inner;
                    }
                    else
                    {
                        continue;
                    }

                }


                foreach (var type in sequence.Elements())
                {
                    var iter = type;
                    if (!type.HasAttributes && type.HasElements)
                    {
                        iter = type.FirstNode as XElement;
                    }

                    var schema = new TypeDescription();
                    schema.Name = iter.Attribute("name")?.Value;
                    var occurs = iter.Attribute("minOccurs")?.Value;
                    int.TryParse(occurs, out int intOccurs);
                    schema.MinOccurs = intOccurs;
                    var nullable = iter.Attribute("nillable")?.Value;
                    schema.Nullable = nullable == "true";


                    var yolo = iter.Attribute("type");


                    var fullType = iter.Attribute("type")?.Value?.Split(':');
                    schema.TypeLib = fullType?.FirstOrDefault();
                    schema.Type = fullType?.LastOrDefault();


                    if (schema.Name != null)
                        result[datatype].Add(schema);
                }
            }

            return result;
        }

        /// <summary>
        /// Get URLs of XSD file with type description
        /// </summary>
        public static SchemaRequest GetXsdLocationsAndTypes(string url)
        {
            var result = new SchemaRequest();
            XElement root = XElement.Load(url);

            try
            {
                var types = root.Elements().FirstOrDefault(o => o.Name.LocalName == "types");

                var schemas = types.Elements().Where(o => o.Name.LocalName == "schema").ToList();
                var imports = schemas.Elements().Where(o => o.Name.LocalName == "import").ToList();

                foreach (var imp in imports)
                {
                    var schemaLocation = imp.Attribute("schemaLocation").Value;
                    result.XsdUrls.Add(schemaLocation);
                }
            }
            catch (Exception ex)
            {
                // sigh, princess is in another castle
            }

            var messages = root.Elements().Where(o => o.Name.LocalName == "message").ToList();

            foreach (var item in messages)
            {
                var innerNode = item.FirstNode as XElement;
                var typeName = innerNode.Attribute("element");
                var nameSplit = typeName.Value.Split(':');

                if (!result.Types.ContainsKey(nameSplit.LastOrDefault()))
                    result.Types.Add(nameSplit.LastOrDefault(), new List<TypeDescription>());
            }

            // Get imported XSD's
            var import = root.Elements().Where(o => o.Name.LocalName == "import").ToList();

            if (import != null)
            {
                foreach (var imp in import)
                {
                    var importLocation = imp.Attribute("location")?.Value;
                    if (importLocation != null)
                    {
                        result.MergeSchemas(GetXsdLocationsAndTypes(importLocation));
                    }
                }
            }

            return result;
        }
    }
}