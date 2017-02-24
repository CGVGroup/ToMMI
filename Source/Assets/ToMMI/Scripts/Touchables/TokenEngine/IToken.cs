/*
 * @author Francesco Strada
 */

using UnityEngine;

namespace Touchables.TokenEngine
{
    /// <summary>
    /// Represents principal Token properties to be used both internally and within the application
    /// </summary>
    internal interface IToken
    {
        /// <summary>
        /// Token Id within the application
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Token Class 
        /// </summary>
        int? Class { get; }

        /// <summary>
        /// Token Position in screen coordinates with respect to its center
        /// </summary>
        Vector2 Position { get; }

        /// <summary>
        /// Token position variation since last update
        /// </summary>
        Vector2 DeltaPosition { get; }

        /// <summary>
        /// Token angle, is angle between token x axis and screen x axis
        /// </summary>
        float Angle { get; }

        /// <summary>
        /// Token angle variation since last frame
        /// </summary>
        float DeltaAngle { get; }

    }
}
