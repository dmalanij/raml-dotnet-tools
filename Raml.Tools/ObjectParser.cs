using System;
using System.Collections.Generic;
using System.Linq;
using Raml.Common;

namespace Raml.Tools
{
    public class ObjectParser
    {
        private readonly JsonSchemaParser jsonSchemaParser = new JsonSchemaParser();

        public ApiObject ParseObject(string key, string value, IDictionary<string, ApiObject> objects, IDictionary<string, string> warnings, IDictionary<string, ApiEnum> enums, IDictionary<string, ApiObject> otherObjects, IDictionary<string, ApiObject> schemaObjects)
        {
            var obj = ParseSchema(key, value, objects, warnings, enums, otherObjects, schemaObjects);
            if (obj == null)
                return null;

            obj.Name = NetNamingMapper.GetObjectName(key);

            if (schemaObjects.Values.Any(o => o.Name == obj.Name) || objects.Values.Any(o => o.Name == obj.Name) ||
                otherObjects.Values.Any(o => o.Name == obj.Name))
            {
                if(UniquenessHelper.HasSameProperties(obj, objects, key, otherObjects, schemaObjects))
                    return null;

                obj.Name = UniquenessHelper.GetUniqueName(objects, obj.Name, otherObjects, schemaObjects);
            }

            return obj;
        }

        private ApiObject ParseSchema(string key, string schema, IDictionary<string, ApiObject> objects, IDictionary<string, string> warnings, IDictionary<string, ApiEnum> enums, IDictionary<string, ApiObject> otherObjects, IDictionary<string, ApiObject> schemaObjects)
        {
            if (schema == null)
                return null;

            // is a reference, should then be defined elsewhere
            if (schema.Contains("<<") && schema.Contains(">>"))
                return null;

            if (schema.Trim().StartsWith("<"))
                return ParseXmlSchema(key, schema, objects);

            if (!schema.Contains("{"))
                return null;

            return jsonSchemaParser.Parse(key, schema, objects, warnings, enums, otherObjects, schemaObjects);
        }



        // TODO
        private ApiObject ParseXmlSchema(string key, string schema, IDictionary<string, ApiObject> objects)
        {
            throw new System.NotImplementedException("XML Schema Parsing not implemented - " + key);
        }

    }
}