using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Parser
{
    public class Error: Exception
    {
        public string Name;
        public int Index;
        public int LineNumber;
        public int Column;
        public string Description;
        public Error(string message): base(message) {
        }
    }
    public class ErrorHandler
    {
        public readonly List<Error> Errors;
        public bool Tolerant;
        public ErrorHandler()
        {
            this.Errors = new List<Error>();
            this.Tolerant = false;
        }
        void RecordError(Error error)
        {
            this.Errors.Add(error);
        }
        void Tolerate(Error error)
        {
            if (this.Tolerant)
            {
                this.RecordError(error);
            }
            else
            {
                throw error;
            }
        }
        Error ConstructError(string msg, double column)
        {
            var error = new Error(msg);
            return error;
        }
        Error CreateError(int index, int line, int col, string description)
        {
            var msg = "Line " + line + ": " + description;
            var error = this.ConstructError(msg, col);
            error.Index = index;
            error.LineNumber = line;
            error.Description = description;
            return error;
        }
        public void ThrowError(int index, int line, int col, string description)
        {
            throw this.CreateError(index, line, col, description);
        }
        public void TolerateError(int index, int line, int col, string description)
        {
            var error = this.CreateError(index, line, col, description);
            if (this.Tolerant)
            {
                this.RecordError(error);
            }
            else
            {
                throw error;
            }
        }
    }
}