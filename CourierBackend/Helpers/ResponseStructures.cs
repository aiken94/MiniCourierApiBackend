namespace CourierBackend.Helpers
{
    using System.Collections.Generic;

    public static class ResponseStructures
    {
        public static object ErrorResponse(IDictionary<string, string[]> errors)
        {
            return new
            {
                message = "Validation failed",
                errors = errors,
                status = "error"
            };
        }

        public static object SuccessResponse(object data)
        {
            return new
            {
                data = data,
                status = "success"
            };
        }

        public static object EmptyOkResponse(string message)
        {
            return new
            {
                message = message,
                status = "success"
            };
        }

        public static object _404Response(string message)
        {
            return new
            {
                message = message,
                status = "error"
            };
        }
    }
}