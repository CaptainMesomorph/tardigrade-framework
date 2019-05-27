namespace Tardigrade.Framework.Security.Privacy
{
    /// <summary>
    /// Service for encoding keys for public usage.
    /// </summary>
    /// <typeparam name="TEnc">Type of the encoded key.</typeparam>
    /// <typeparam name="TDec">Type of the decoded key.</typeparam>
    public interface IKeyEncoder<TEnc, TDec>
    {
        /// <summary>
        /// Encode a key value.
        /// </summary>
        /// <param name="key">Key value to encode.</param>
        /// <exception cref="System.ArgumentNullException">The key parameter is null.</exception>
        /// <exception cref="Exceptions.EncodingException">Error encoding key value.</exception>
        TEnc Encode(TDec key);

        /// <summary>
        /// Decode an encoded key value.
        /// </summary>
        /// <param name="encodedKey">Encoded key value to decode.</param>
        /// <exception cref="System.ArgumentNullException">The encodedKey parameter is null.</exception>
        /// <exception cref="Exceptions.EncodingException">Error decoding key value.</exception>
        TDec Decode(TEnc encodedKey);
    }
}