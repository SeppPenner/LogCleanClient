using System;
using System.Collections.Generic;

namespace LogCleanClient
{
    [Serializable]
    public class Config
    {
        public List<LogModel> LogModels { get; set; }
    }
}