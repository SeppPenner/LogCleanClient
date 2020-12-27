// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogModel.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The log model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogCleanClient
{
    /// <summary>
    /// The log model.
    /// </summary>
    public class LogModel
    {
        /// <summary>
        /// Gets or sets the file filter.
        /// </summary>
        public string FileFilter { get; set; }

        /// <summary>
        /// Gets or sets log folder.
        /// </summary>
        public string LogFolder { get; set; }

        /// <summary>
        /// Gets or sets the file amount.
        /// </summary>
        public int FileAmount { get; set; }
    }
}