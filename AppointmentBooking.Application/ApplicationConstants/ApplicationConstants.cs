using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Application.ApplicationConstants
{
    public class ApplicationConstants
    {

    }

    public class CommonMessages
    {
        public const string CreateOperationSuccess = "Reocrd created successfully.";
        public const string UpdateOperationSuccess = "Record updated successfully.";
        public const string DeleteOperationSuccess = "Record deleted successfully.";

        public const string CreateOperationFailed = "Failed to create the record.";
        public const string UpdateOperationFailed = "Failed to update the record.";
        public const string DeleteOperationFailed = "Failed to delete the record.";

        public const string RecordNotFound = "The specified record was not found.";
        public const string SystemError = "An error occurred while processing the request. Please try again later.";

        public const string NotFound = "The requested resource was not found.";
        public const string BadRequest = "The request was invalid or cannot be processed.";
        public const string InternalServerError = "An unexpected error occurred on the server.";
        public const string Unauthorized = "You are not authorized to access this resource.";
        public const string Forbidden = "You do not have permission to access this resource.";
       
    }
}
