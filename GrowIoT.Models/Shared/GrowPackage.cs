using System;
using System.Collections.Generic;
using System.Text;
using GrowIoT.Models.Enums;

namespace GrowIoT.Models.Shared
{
    public class GrowPackage<T> : GrowPackage
    {
        public T Data { get; set; }

        public GrowPackage(IoTCommand command, T data) : base(command)
        {
            Data = data;
        }
    }

    public class GrowPackage
    {
        public DateTime CreateTime { get; set; }
        public IoTCommand Command { get; set; }

        public GrowPackage(IoTCommand command)
        {
            CreateTime = DateTime.Now;
            Command = command;
        }
    }

}
