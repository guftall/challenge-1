using System;

namespace HiliTechChallenge.Core
{
    public class BadRequestException  : Exception
    {
        public BadRequestException(string message) : base(message)
        {
        }
    }
}