using System.Collections.Generic;
using System.Net;
using System.Xml;
using System.Xml.Serialization;

namespace MSTest.AzureDevOps.Models
{
    /// <summary>
    /// Data model to represent an XML response received
    /// from the API call to gather the Shared Parameter data
    /// </summary>
    [XmlRoot("parameterSet")]
    public class SharedParameterData
    {
        [XmlElement("paramNames")]
        public ParamNames ParamNames { get; set; }
        [XmlElement("paramData")]
        public ParamData ParamData { get; set; }
    }
    public class ParamNames
    {
        [XmlElement("param")]
        public List<string> Param { get; set; }
    }
    public class ParamData
    {
        [XmlAttribute("lastId")]
        public string LastId { get; set; }
        [XmlElement("dataRow")]
        public List<DataRow> DataRow { get; set; }
    }
    public class DataRow
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
        [XmlElement("kvp")]
        public List<Kvp> Kvp { get; set; }
        
    }
    public class Kvp
    {
        private string keyField;
        private string valueField;

        [XmlAttribute("key")]
        public string Key {
            get
            {
                return WebUtility.HtmlDecode(keyField);
            }
            set
            {
                keyField = value;
            }
        }
        [XmlAttribute("value")]
        public string Value {
            get
            {
                return WebUtility.HtmlDecode(valueField);
            }
            set
            {
                valueField = value;
            }
        }
    }
}
