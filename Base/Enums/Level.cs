namespace OptimaValue
{
    public enum Level
    {
        /// <summary>
        /// The logger will never output anything
        /// </summary>
        Nothing = 1,

        /// <summary>
        /// Log only critical errors and warnings and success, but no general information
        /// </summary>
        Critical = 2,

        /// <summary>
        /// Logs all informative message, ignoring any debug and verbose messages
        /// </summary>
        Informative = 3,

        /// <summary>
        /// Logs everything
        /// </summary>
        Debug = 4,



    }
}
