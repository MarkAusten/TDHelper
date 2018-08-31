namespace TDHelper
{
    internal class NetLogModel
    {
        /// <summary>
        /// Gets or set the net log filename.
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// Gets or set s value indicating whether the file contains one or more systems.
        /// </summary>
        public bool HasSystems { get; set; }

        /// <summary>
        /// Gets or set the timestamp contained in the log header.
        /// </summary>
        public string HeaderTimestamp { get; set; }
    }
}