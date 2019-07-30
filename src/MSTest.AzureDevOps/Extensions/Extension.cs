namespace MSTest.AzureDevOps.Extensions
{
    public static class Extension
    {
        // Basic verification to see if a response is JSON
        public static bool IsJson(string jsonData)
        {
            jsonData = jsonData.Trim();
            return ((jsonData.StartsWith("{") && jsonData.EndsWith("}")) || //For object
                (jsonData.StartsWith("[") && jsonData.EndsWith("]"))); //For array
        }

        //Basic verification to see if a response in XML
        public static bool IsXML(string xmlData)
        {
            xmlData = xmlData.Trim();
            return ((xmlData.StartsWith("<") && xmlData.EndsWith(">")));
        }
    }
}
