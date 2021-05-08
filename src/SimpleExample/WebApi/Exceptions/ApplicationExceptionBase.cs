using System;

namespace WebApi.Exceptions {
    public abstract class ApplicationExceptionBase : Exception {
        protected ApplicationExceptionBase(string message) : base(message) { }
    }
}