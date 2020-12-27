// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Config.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The configuration class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogCleanClient
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The configuration class.
    /// </summary>
    [Serializable]
    public class Config
    {
        /// <summary>
        /// Gets or sets the log models.
        /// </summary>
        public List<LogModel> LogModels { get; set; }
    }
}