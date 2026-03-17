using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ClinicManagement.Helper
{
    public static class ToastHelper
    {
        public static void Success(ITempDataDictionary tempData, string message)
        {
            tempData["Success"] = message;
        }

        public static void Error(ITempDataDictionary tempData, string message)
        {
            tempData["Error"] = message;
        }

        public static void Info(ITempDataDictionary tempData, string message)
        {
            tempData["Info"] = message;
        }

        public static void Warning(ITempDataDictionary tempData, string message)
        {
            tempData["Warning"] = message;
        }
    }
}
