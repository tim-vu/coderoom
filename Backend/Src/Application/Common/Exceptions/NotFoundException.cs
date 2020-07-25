using System;

namespace Application.Common.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string @object) : base($"Not found: {@object}")
        {
            
        }
    }
}