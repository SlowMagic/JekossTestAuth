using System;

namespace JekossTest.Models
{
    public class BaseResponse<TModel> : BaseResponse
    {
        public TModel Model { get; set; }

        public BaseResponse(bool success, string message) : base(success, message)
        {
            
        }
        public BaseResponse(bool success, string message,TModel model)
        {
            Message = message;
            Model = model;
            Success = success;
        }
    }
    
    public class BaseResponse
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public int? ErrorCode { get; set; }
        public BaseResponse()
        {
        }
        
        public BaseResponse( bool success, string message)
        {
            Message = message;
            Success = success;
        }
    }
}