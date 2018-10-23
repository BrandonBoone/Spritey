namespace Spritey.ImageProcessing
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// List extension methods
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Calls Dispose() on all objects in the list.
        /// </summary>
        /// <typeparam name="T">A type that implements <see cref="IDisposable"/></typeparam>
        /// <param name="list">The list.</param>
        public static void DisposeAll<T>(this List<T> list)
            where T : IDisposable
        {
            list.ForEach(obj => obj.Dispose());
        }
    }
}
