/*
 * @author Francesco Strada
 */

using UnityEngine;

namespace Touchables.MultiTouchManager
{
    /// <summary>
    /// Interface reppresenting general touch point on screen
    /// </summary>
    internal interface ITouchInput
    {
        /// <summary>
        /// TouchInput unique id
        /// </summary>
        int Id { get; }
        
        /// <summary>
        /// TouchPoint position in screen coordinates where (0,0) is in lower left
        /// </summary>
        Vector2 Position { get; }
        
        /// <summary>
        /// State of the TouchPoint
        /// </summary>
        TouchState State { get; }

        

    }

    /// <summary>
    /// Possible touch states
    /// </summary>
    public enum TouchState
    {
        /// <summary>
        /// TouchPoint started
        /// </summary>
        Began,

        /// <summary>
        /// TouchPoint has not changed since previous frame
        /// </summary>
        Stationary,

        /// <summary>
        /// TouchPoint updated his position since last frame
        /// </summary>
        Moved,

        /// <summary>
        /// TouchPoint was removed from screen
        /// </summary>
        Ended
    }
}
