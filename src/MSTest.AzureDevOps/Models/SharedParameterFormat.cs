using Newtonsoft.Json;
using System.Collections.Generic;

/// <summary>
/// Data model to represent a JSON response received 
/// from a Test Case that contains a Shared Parameter
/// </summary>
public class SharedParameterFormat
{
    [JsonProperty("parameterMap")]
    public List<ParameterMap> ParameterMap { get; set; }
    [JsonProperty("sharedParameterDataSetIds")]
    public List<int> SharedParameterDataSetIds { get; set; }
    [JsonProperty("rowMappingType")]
    public int RowMappingType { get; set; }
}

public class ParameterMap
{
    [JsonProperty("localParamName")]
    public string LocalParamName { get; set; }
    [JsonProperty("sharedParameterName")]
    public string SharedParameterName { get; set; }
    [JsonProperty("sharedParameterDataSetId")]
    public int SharedParameterDataSetId { get; set; }
}
