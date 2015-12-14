using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataServices.SimpleAzureIdentityDataService.Models
{
    /// <summary>
    /// Represents the status of submission to the platform for a change to the data.
    /// </summary>
    public class SubmissionStatus
    {
        /// <summary>
        /// Status code of the submission, 0 is a successful submission, non-zero is some error
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Detailed message of the status of the submission
        /// </summary>
        public String Message { get; set; }
    }
}