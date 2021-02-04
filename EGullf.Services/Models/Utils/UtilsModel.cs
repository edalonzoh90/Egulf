using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGullf.Services.Models.Utils
{
    public class UtilsModel
    {
    }

    public class ErrorResult
    {
        public bool? IsError { get; set; }

        public string Message { get; set; }

        public int? Line { get; set; }

        public string Subject { get; set; }
    }

    public class RequestResult<T> 
    {
        public Status Status { get; set; }

        public string Message { get; set; }

        public T Data { get; set; }
    }

    public enum Status
    {
        Success,
        Error,
        Warning,
        Info
    }

    



}
