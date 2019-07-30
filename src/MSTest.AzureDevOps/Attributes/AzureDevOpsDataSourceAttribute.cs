using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTest.AzureDevOps.Extensions;
using MSTest.AzureDevOps.Models;
using MSTest.AzureDevOps.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace MSTest.AzureDevOps.Attributes
{
    /// <summary>
    /// Azure DevOps data source for MSTest V2.
    /// </summary>
    public class AzureDevOpsDataSourceAttribute : Attribute, ITestDataSource
    {
        private const string TESTCASE = "Test Case";

        #region Properties

        /// <summary>
        /// Work item id for get data.
        /// </summary>
        private int _workItemId { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="workItem">Work item id for get data</param>
        public AzureDevOpsDataSourceAttribute(int workItem)
        {
            _workItemId = workItem;
        }

        #endregion

        #region ITestDataSource Implementation

        /// <summary>
        /// Gets the test data from custom test data source.
        /// </summary>
        /// <param name="methodInfo">The method info of test method.</param>
        /// <returns>Test data for calling test method.</returns>
        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        {
            // Get work item's information by id
            var workItemAsync = WorkItemService.Instance.GetById(_workItemId);
            workItemAsync.Wait();

            if (workItemAsync.Result.WorkItemType != TESTCASE)
                throw new System.NullReferenceException("The following Work Item is not a Test Case: " + _workItemId.ToString());

            // Check if service parameters data was retrieved
            if ((string.IsNullOrWhiteSpace(workItemAsync.Result.TestDataSource)) &&
                    (string.IsNullOrWhiteSpace(workItemAsync.Result.TestParameters)))
                throw new System.NullReferenceException("Unable to find parameter data in Test Case " + _workItemId.ToString());

            var arrayTestCaseData = new List<object[]>();

            // Check the response type
            // An XML response indicates parameters are saved in the Test Case
            // A JSON response indicates a Shared Parameter is referenced in the Test Case
            if (Extension.IsXML(workItemAsync.Result.TestDataSource))
            {
                // convert parameters data (from work item) to dataset
                var testDataSourceStream = new StringReader(workItemAsync.Result.TestDataSource);
                var dataSetTestDataSource = new DataSet();

                dataSetTestDataSource.ReadXml(testDataSourceStream);

                // Iterate each row from parameters
                foreach (System.Data.DataRow row in dataSetTestDataSource.Tables[0].Rows)
                {
                    // Create test case object
                    var testCaseData = new TestCaseData()
                    {
                        Id = _workItemId,
                        Title = workItemAsync.Result.Title
                    };

                    foreach (DataColumn column in dataSetTestDataSource.Tables[0].Columns)
                        testCaseData.Parameters.Add(column.ColumnName, row[column.ColumnName].ToString());

                    arrayTestCaseData.Add(new[] { testCaseData });
                }
            }
            else if (Extension.IsJson(workItemAsync.Result.TestDataSource))
            {
                // Retrieve information about the Shared Parameter
                var deserializedTempDataSource = JsonConvert.DeserializeObject<SharedParameterFormat>(workItemAsync.Result.TestDataSource);

                if (deserializedTempDataSource.ParameterMap.Count == 0)
                    throw new System.NullReferenceException("Unable to find parameter data in Test Case " + _workItemId.ToString());

                int sharedParamID = deserializedTempDataSource.SharedParameterDataSetIds[0];

                // Create a list of the columns utilized from the Shared Parameter
                IEnumerable<string> columnList = deserializedTempDataSource.ParameterMap.Select(x => x.SharedParameterName).ToList();

                // Get work item's information by id
                var sharedParamAsync = WorkItemService.Instance.GetById(sharedParamID);
                workItemAsync.Wait();

                // Check if test parameter data was retrieved
                if (string.IsNullOrWhiteSpace(sharedParamAsync.Result.TestParameters)) 
                    return null;

                // Read and deserialize the Shared Parameter xml response
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(SharedParameterData));

                XmlReader xmlReader = XmlReader.Create(new StringReader(sharedParamAsync.Result.TestParameters));

                SharedParameterData result = (SharedParameterData)xmlSerializer.Deserialize(xmlReader);

                // Iterate through each data row in the Shared Parameter response
                foreach (Models.DataRow dataRow in result.ParamData.DataRow)
                {
                    // Create test case object
                    var testCaseData = new TestCaseData()
                    {
                        Id = _workItemId,
                        Title = workItemAsync.Result.Title
                    };

                    // Look through each column in the list
                    foreach (string column in columnList)
                    {
                        // Search for the Key that matches the column name
                        var myVal = dataRow.Kvp.First(x => x.Key  == column);

                        // Add the column name and the value to the Test Case Data object
                        testCaseData.Parameters.Add(column, myVal.Value);
                    }
                    arrayTestCaseData.Add(new[] { testCaseData });
                }
            }
            // Return test data array
            return arrayTestCaseData.ToArray();
        }



        /// <summary>
        /// Gets the display name corresponding to test data row for displaying in TestResults.
        /// </summary>
        /// <param name="methodInfo">The method info of test method.</param>
        /// <param name="data">The test data which is passed to test method.</param>
        /// <returns>The System.String.</returns>
        public string GetDisplayName(MethodInfo methodInfo, object[] data)
        {
            if (data != null)
                return string.Format(CultureInfo.CurrentCulture, "{0} ({1})", methodInfo.Name, string.Join(",", data));

            return null;
        }

        #endregion


    }
}
