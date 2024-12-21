using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IdentityProvider.Application.Framework
{
    public class OperationResult<T>
    {
        public bool Success { get; private set; }
        public string OperationName { get; private set; }
        public string Message { get; private set; }
        public string ExMessage { get; private set; }
        public DateTime OperationDate { get; private set; }
        public HttpStatusCode Status { get; private set; }
        public T Object { get; private set; }
        public List<T> List { get; private set; }

        public OperationResult(string OperationName)
        {
            this.Success = false;
            this.OperationName = OperationName;
            this.OperationDate = DateTime.Now;
        }

        public OperationResult<T> Succeed(string Message)
        {
            this.Success = true;
            this.Message = Message;
            this.Status = HttpStatusCode.OK;
            return this;
        }

        public OperationResult<T> Succeed(string Message, HttpStatusCode Status)
        {
            this.Success = true;
            this.Message = Message;
            this.Status = Status;
            return this;
        }

        public OperationResult<T> Succeed(string Message, T Object)
        {
            this.Success = true;
            this.Message = Message;
            this.Status = HttpStatusCode.OK;
            this.Object = Object;
            return this;
        }
        public OperationResult<T> Succeed(string Message, HttpStatusCode Status, T Object)
        {
            this.Success = true;
            this.Message = Message;
            this.Status = Status;
            this.Object = Object;
            return this;
        }
        public OperationResult<T> Succeed(string Message, List<T> List)
        {
            this.Success = true;
            this.Message = Message;
            this.Status = HttpStatusCode.OK;
            this.List = List;
            return this;
        }
        public OperationResult<T> Succeed(string Message, HttpStatusCode Status, List<T> List)
        {
            this.Success = true;
            this.Message = Message;
            this.Status = Status;
            this.List = List;
            return this;
        }
        public OperationResult<T> Succeed(string Message, T Object, List<T> List)
        {
            this.Success = true;
            this.Message = Message;
            this.Status = HttpStatusCode.OK;
            this.Object = Object;
            this.List = List;
            return this;
        }
        public OperationResult<T> Succeed(string Message, HttpStatusCode Status, T Object, List<T> List)
        {
            this.Success = true;
            this.Message = Message;
            this.Status = Status;
            this.Object = Object;
            this.List = List;
            return this;
        }



        public OperationResult<T> Failed(string Message)
        {
            this.Success = false;
            this.Message = Message;
            this.Status = HttpStatusCode.BadRequest;
            return this;
        }
        public OperationResult<T> Failed(string Message, T Object)
        {
            this.Success = false;
            this.Message = Message;
            this.Status = HttpStatusCode.BadRequest;
            this.Object = Object;
            return this;
        }
        public OperationResult<T> Failed(string Message, string ExMessage)
        {
            this.Success = false;
            this.Message = Message;
            this.Status = HttpStatusCode.BadRequest;
            this.ExMessage = ExMessage;
            return this;
        }
        public OperationResult<T> Failed(string Message, string ExMessage, T Object)
        {
            this.Success = false;
            this.Message = Message;
            this.Status = HttpStatusCode.BadRequest;
            this.ExMessage = ExMessage;
            this.Object = Object;
            return this;
        }
        public OperationResult<T> Failed(string Message, HttpStatusCode Status)
        {
            this.Success = false;
            this.Message = Message;
            this.Status = Status;
            return this;
        }
        public OperationResult<T> Failed(string Message, HttpStatusCode Status, T Object)
        {
            this.Success = false;
            this.Message = Message;
            this.Status = Status;
            this.Object = Object;
            return this;
        }
        public OperationResult<T> Failed(string Message, string ExMessage, HttpStatusCode Status)
        {
            this.Success = false;
            this.Message = Message;
            this.Status = Status;
            this.ExMessage = ExMessage;
            return this;
        }
        public OperationResult<T> Failed(string Message, string ExMessage, HttpStatusCode Status, T Object)
        {
            this.Success = false;
            this.Message = Message;
            this.Status = Status;
            this.ExMessage = ExMessage;
            this.Object = Object;
            return this;
        }
    }
}
