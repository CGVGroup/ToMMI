/*
 * @author Francesco Strada
 */

using UnityEngine;

namespace Touchables.MultiTouchManager
{
    /// <summary>
    /// Class which internally represents a touch.
    /// </summary>
    internal sealed class TouchInput : ITouchInput
    {
        #region Properties

        /// <inheritdoc />
        public int Id { get; private set; }

        /// <inheritdoc />
        public TouchState State { get; private set; }

        /// <inheritdoc />
        public Vector2 Position { get; private set; }

        /// <summary>
        /// TouchPoint position in previous frame
        /// </summary>
        public Vector2 PreviousPosition { get; private set; }

        /// <summary>
        /// Id reppresenting cluster which TouchPoint is part of
        /// </summary>
        //TODO it should be changed to Cluster HASH and managed in ClusterManager in order to more easily retrive the cluster in which a point is part of
        public int? ClusterId { get; private set; }

        //TODO add TimeStamp variable
        
        #endregion

        public TouchInput(int id, Vector2 position, TouchState state)
        {
            this.Id = id;
            this.Position = position;
            this.State = state;

            //ClusterId is initially set to null and updated when first inseted 
            //into a new Cluster 
            this.ClusterId = null;
        }

        #region Public Methods

        /// <summary>
        /// Sets TouchInput State
        /// </summary>
        /// <param name="state">New TouchInput state</param>
        public void SetState(TouchState state)
        {
            this.State = state;
        }

        /// <summary>
        /// Update TouchInput Position
        /// </summary>
        /// <param name="position">New position</param>
        public void UpdatePosition(Vector2 position)
        {
            this.Position = position;
        }

        /// <summary>
        /// Checks if current TouchInput is in the same position with specified one  
        /// </summary>
        /// <param name="other">TouchInput to compare with current</param>
        /// <returns>true if other is in same position</returns>
        public bool Equals(TouchInput other)
        {
            if (this.Position.x == other.Position.x || this.Position.y == other.Position.y)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if current TouchInput is in the same position with specified within
        ///  a pixel distance threshold 
        /// </summary>
        /// <param name="other">TouchInput to compare with current</param>
        /// <param name="pxThreshold">TouchInput comparison threshold in pixels </param>
        /// <returns>true if comparison is successfull</returns>
        public bool Equals(TouchInput other, float pxThreshold)
        {
            float dist = Vector2.Distance(this.Position, other.Position);

            if (dist <= pxThreshold)
                return true;
            else
                return false;
        }

        #endregion
    }
}
