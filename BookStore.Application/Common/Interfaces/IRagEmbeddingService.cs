using System.Threading.Tasks;

namespace FixHub.Application.Common.Interfaces
{
    public interface IRagEmbeddingService
    {
        /// <summary>
        /// Generates a vector embedding for the given text.
        /// </summary>
        /// <param name="text">The input text (e.g., search query or article content).</param>
        /// <returns>A float array representing the vector embedding.</returns>
        Task<float[]> GenerateEmbeddingAsync(string text);
    }
}
