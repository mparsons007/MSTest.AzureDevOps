﻿namespace MSTest.AzureDevOps.Services.Models.WorkItems
{
    /// <summary>
    /// Service response model.
    /// </summary>
    internal class GetByIdResponseModel
    {
        #region Properties

        public int Id { get; set; }
        public string WorkItemType { get; set; }
        public string Title { get; set; }
        public string TestDataSource { get; set; }
        public string TestParameters { get; set; }
        #endregion
    }
}
