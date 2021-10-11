using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JSDeclarationsGenerator.Code.Models
{

    public class JSGenerator
    {
        public bool ASPX_Mode { get; set; }
        public bool Include_Hard_Coded_Options { get; set; }

        private string[] ASPX_Tags_To_Skip;
        private string[] ASPX_Tags_With_Options;

        private List<Element> elements;

        private void Intialize() 
        {
            ASPX_Mode = false;
            Include_Hard_Coded_Options = false;

            ASPX_Tags_To_Skip = new string[]
            {
                "asp:sqldatasource",
                "asp:content",
            };

            ASPX_Tags_With_Options = new string[]
            {
                "asp:checkboxlist",
                "asp:radiobuttonlist",
            };

            elements = new List<Element>();
        }

        public JSGenerator()
        {
            Intialize();
        }

        /// <summary>
        /// Scans a html file and stores elements into elements list
        /// </summary>
        /// <param name="filename">Path of the file</param>
        public void LoadFile(string filename)
        {
            string file_text = "";
            string file_ext = Path.GetExtension(filename);
            HtmlDocument document = new HtmlDocument();
            HtmlNodeCollection elements_with_ids = null;

            // ReadAllText
            try
            {
                file_text = File.ReadAllText(filename);
            }
            catch (Exception)
            {

                throw;
            }

            // Check if the file is an ASPX Page
            switch (file_ext)
            {
                case ".aspx":
                    ASPX_Mode = true;

                    // Remove <% %> tags
                    string UnSanitizedText = Regex.Replace(file_text, Constants.ASPX_Code_Block, "");

                    // Replace ':' in asp:tags with '.'
                    var matches = Regex.Matches(UnSanitizedText, Constants.ASPX_Tag);
                    foreach (Match m in matches)
                    {
                        string value = m.Value;
                        string replaced = m.Value.Replace(':', '.');

                        UnSanitizedText = UnSanitizedText.Replace(value, replaced);
                    }

                    file_text = UnSanitizedText;
                    break;
                
                case ".html":
                case ".cshtml":
                    ASPX_Mode = false;
                    break;
                
                default:
                    Console.WriteLine($"Unknown File Extension: \"{file_ext}\", \"{filename}\".");
                    return;
            }

            // Load the html into Html Agility Pack
            document.LoadHtml(file_text);

            elements_with_ids = document.DocumentNode.SelectNodes(Constants.ID_Selector);

            if (elements_with_ids != null)
            {

                switch (ASPX_Mode)
                {
                    case true:
                        Load_ASPX_Elements(elements_with_ids);
                        break;
                    case false:
                        Load_HTML_Elements(elements_with_ids);
                        break;
                }

            }

        }

        private void Load_ASPX_Elements(HtmlNodeCollection nodes)
        {
            if (nodes != null)
            {
                foreach (HtmlNode node in nodes)
                {
                    string tag = node.Name.Replace('.', ':');
                    string id = node.Id;

                    bool SkipTag =
                    (
                        from t in ASPX_Tags_To_Skip
                        where
                            t == tag
                        select true
                    ).FirstOrDefault();

                    bool TagHasOptions =
                    (
                        from t in ASPX_Tags_With_Options
                        where
                            t == tag
                        select true
                    ).FirstOrDefault();

                    // Skip Tag
                    if (SkipTag)
                        continue;

                    // check for ClientIDMode=Static if asp tag
                    if (tag.Contains(":"))
                    {
                        var attr = node.Attributes["clientidmode"];

                        if (attr == null || attr.Value.ToLower() != "static")
                        {
                            continue;
                        }
                    }

                    // add the element
                    Element thisElement = new Element(tag, id);
                    if (IsValidAddress(id))
                    { 
                        elements.Add(thisElement);
                    }
                    

                    if (TagHasOptions)
                    {
                        HtmlNodeCollection childnodes = node.ChildNodes;
                        int index = 0;

                        foreach (HtmlNode child in childnodes)
                        {
                            switch (child.Name)
                            {
                                case "asp.listitem":

                                    string option_value = child.Attributes["value"]?.Value;
                                    string option_text = child.Attributes["text"]?.Value ?? child.InnerText;

                                    string option_label = option_value ?? option_text ?? "";

                                    if (option_label != "")
                                    {
                                        string option_tag = "asp:listitem";
                                        string option_address = $"{thisElement.ID}__Options__{option_label}";
                                        string option_id = $"{thisElement.ID}_{index}";

                                        if (IsValidAddress(option_address))
                                        { 
                                            elements.Add(new Element(option_tag, option_address, option_id));
                                        }
                                    }
                                    index++;
                                    break;
                                default:
                                    break;
                            }

                        }

                        if (index > 0)
                            thisElement.Address = thisElement.ID + "__Table";
                    }


                }
            }
        }

        private void Load_HTML_Elements(HtmlNodeCollection nodes)
        {
            if (nodes != null)
            {
                foreach (HtmlNode node in nodes)
                {
                    if (IsValidAddress(node.Id))
                    { 
                        elements.Add(new Element(node.Name, node.Id));
                    }
                }
            }
        }

        public string GenerateDeclarations()
        {
            string DeclarationsString = "";

            // Sort Elements List
            elements = 
            (
                from e in elements 
                orderby e.ID 
                select new Element 
                { 
                    Tag = e.Tag, 
                    ID = e.ID, 
                    Address = (e.Address != null && e.Address.Trim() != "") ? e.Address : e.ID
                }
            ).ToList();

            Declaration root = new Declaration();
            root.Name = "IDs";
            root.IsChild = false;
            root.NestedDepth = 1;

            foreach (Element e in elements)
            {
                root.Add(e.Address, e.ID);
            }

            DeclarationsString = root.ToString().Replace("\r\n\r\n", "\r\n");

            return DeclarationsString;
        }

        public bool IsValidAddress(string address)
        {
            if (address.Contains("__"))
            {
                string[] addresses = address.Split("__");
                foreach (string s in addresses)
                {
                    if (!Regex.IsMatch(s, Constants.Valid_Address))
                    {
                        return false;
                    }
                }
            }
            else 
            {
                return Regex.IsMatch(address, Constants.Valid_Address);
            }

            return true;
        }

    }
}
