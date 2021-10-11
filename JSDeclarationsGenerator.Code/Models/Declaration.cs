using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSDeclarationsGenerator.Code.Models
{
    internal class Declaration
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool IsChild { get; set; }
        public int NestedDepth { get; set; }

        public List<Declaration> SubDeclarations = new List<Declaration>();

        public bool HasChildren => (SubDeclarations.Count > 0);

        public Declaration()
        {

        }

        public Declaration(string name, bool ischild, int nesteddepth)
        {
            Name = name;
            IsChild = ischild;
            NestedDepth = nesteddepth;
        }

        public Declaration(string name, string value, bool ischild, int nesteddepth)
        {
            Name = name;
            Value = value;
            IsChild = ischild;
            NestedDepth = nesteddepth;
        }

        public void Add(string address, string id)
        {
            if (address.Contains("__"))
            {
                // check sub declarations
                string[] addresses = address.Split("__");
                string first_address_node = addresses[0];
                string next_address = address.Substring(first_address_node.Length + 2);
                bool child_added = false;

                // check if there is a Sub Declaration with the same path
                for (int i = 0; i < SubDeclarations.Count; i++) 
                {
                    string SD_Name = SubDeclarations[i].Name;

                    if (SD_Name == first_address_node)
                    {
                        SubDeclarations[i].Add(next_address, id);
                        child_added = true;
                        break;
                    }
                        
                }

                // if child_added is false then
                // add a child and call .Add
                if (!child_added)
                {
                    Declaration child = new Declaration(first_address_node, "", true, NestedDepth + 1);
                    SubDeclarations.Add(child);
                    child.Add(next_address, id);
                }


            }
            else 
            {
                // add a child with name and value
                Declaration Child = new Declaration(address, id, true, NestedDepth + 1);
                SubDeclarations.Add(Child);
            }
        }

        public override string ToString()
        {
            if (IsChild)
            {
                // USE: "Name: ...,"
                if (HasChildren)
                {
                    // Name: { ... },
                    StringBuilder sb = new StringBuilder();

                    //  Name: {
                    sb.AppendLine($"{ new string('\t', NestedDepth) }{ Name }: {{");

                    // Call Children ToString Method
                    int LastChildIndex = (SubDeclarations.Count - 1);
                    for (int i = 0; i < LastChildIndex; i++)
                    {
                        //  Possible Output of ToString
                        //      Name: {...},
                        //  OR
                        //      Name: "#Value",
                        sb.AppendLine(SubDeclarations[i].ToString());
                    }

                    //  Append Last Child With Commas Trimmed off
                    sb.AppendLine(SubDeclarations[LastChildIndex].ToString().TrimEnd(','));

                    //  },
                    sb.AppendLine($"{ new string('\t', NestedDepth) }}},");

                    return sb.ToString();
                }
                else
                {
                    // Name: "#Value",
                    return $"{ new string('\t', NestedDepth) }{ Name }: \"#{ Value }\",";
                }
            }
            else
            {
                if (HasChildren)
                {
                    // var Name = { ... };
                    StringBuilder sb = new StringBuilder();

                    // var Name = {
                    sb.AppendLine($"{ new string('\t', NestedDepth) }var { Name } = {{");

                    // Call Children ToString Method
                    int LastChildIndex = (SubDeclarations.Count - 1);
                    for (int i = 0; i < LastChildIndex; i++)
                    {
                        //  Possible Output of ToString
                        //      Name: {...},
                        //  OR
                        //      Name: "#Value",
                        sb.AppendLine(SubDeclarations[i].ToString());
                    }

                    //  Append Last Child With Commas Trimmed off
                    sb.AppendLine(SubDeclarations[LastChildIndex].ToString().TrimEnd('\n', '\r', ','));

                    // };
                    sb.AppendLine($"{ new string('\t', NestedDepth) }}};");

                    return sb.ToString();
                }
                else
                {
                    // var Name = "#Value";
                    return $"{ new string('\t', NestedDepth) }var { Name } = \"#{ Value }\";";
                }

            }

        }

    }
}
