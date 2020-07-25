using System;
using Application.Common.Interfaces;

namespace Infrastructure.MachineDateTime
{
    public class MachineDateTime : IDateTime
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}