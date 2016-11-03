namespace DataHelpers.Contracts
{
    /// <summary>
    /// Contract defines functionality for any object that validates imported data before it is
    /// copied to a resource.
    /// </summary>
    public interface IValidator
    {
        /// <summary>
        /// Validate a row and its contents.
        /// </summary>
        /// <param name="row">the row to validate</param>
        void Validate(Row row);
    }
}
