using CSharConverter.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CSharConverter.Converter
{
    public class TypeScriptConverter : IConverter
    {
        private readonly List<string> classes;
        public TypeScriptConverter()
        {
            classes = new List<string>();
        }
        public string Convert(string obj)
        {
            if (string.IsNullOrEmpty(obj))
            {
                return string.Empty;
            }
            var classes = obj.Split("public class",StringSplitOptions.RemoveEmptyEntries);
            foreach (var c in classes)
            {
                var className = GetClassName(c);

                Build(className, GetProperties(c.Replace(className, "")));
            }

            return ToString();
        }
        private void Build(string className, string properties)
        {
            classes.Add("export interface " + className + "{ \n" + properties + " \n }");
        }
        private string GetClassName(string classDefinition)
        {
            if (string.IsNullOrEmpty(classDefinition)) return string.Empty;
  
            return classDefinition.Trim().Split(' ')[0];
        }
        private string GetPropertyName(string propertyDefinition, DataType dataType)
        {
            if (string.IsNullOrEmpty(propertyDefinition)) return string.Empty;

            propertyDefinition = propertyDefinition.Replace("{ get; set; }", "").Replace("?","");

            Regex regReplace = new Regex(dataType.ToString().ToLower());
            string propName = regReplace.Replace(propertyDefinition, "", 1).Trim();

            if (dataType == DataType.List)
            {
                propName = propName.Split(" ")[1].Trim();
                return string.Format("{0}{1}: {2}[]", propName.Substring(0, 1).ToLower(), propName.Substring(1), propName.Split(" ")[0].Trim());
            }
              
            return string.Format("{0}{1}", propName.Substring(0, 1).ToLower(), propName.Substring(1));
        }
        private string GetProperties(string classDefinition)
        {
            List<string> props = new List<string>();
            if (string.IsNullOrEmpty(classDefinition)) return string.Empty;

            var properties = classDefinition.Trim().Split("public");
            var propName = string.Empty;
            var addNullable = string.Empty;
            foreach (var property in properties)
            {
                addNullable = string.Empty;
                propName = string.Empty;
                if (property.Contains(DataType.String.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    if (property.Contains("?"))
                        addNullable = "?";

                    props.Add($"{GetPropertyName(property, DataType.String)}{addNullable} :string;");
                }
                else if (property.Contains(DataType.Long.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    var dataType = DataType.Long;

                    if (property.Contains(DataType.Int.ToString(), StringComparison.InvariantCultureIgnoreCase))
                        dataType = DataType.Int;

                    if (property.Contains("?"))
                        addNullable = "?";

                    props.Add($"{GetPropertyName(property, dataType)}{addNullable} :number;");
                }
                else if (property.Contains(DataType.List.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {

                    if (property.Contains("?"))
                        addNullable = "?";

                    props.Add($"{GetPropertyName(property, DataType.List)}{addNullable};");
                }
            }

            return string.Join(Environment.NewLine, props.ToArray());
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, classes.ToArray()); 
        }
    }
}
