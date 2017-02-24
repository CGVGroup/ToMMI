/*
 * @author Francesco Strada
 */

using UnityEngine;
using Touchables.MultiTouchManager;

namespace Touchables.TokenEngine
{
    internal sealed class TokenMarker : ITouchInput
    {
        #region Fields
        //TODO ADD x,y position with respect to Token

        #endregion
        
        #region Properties

        /// <inheritdoc />
        public int Id { get; private set; }
        
        /// <inheritdoc />
        public Vector2 Position { get; private set; }
        
        /// <inheritdoc />
        public TouchState State { get; private set; }

        /// <summary>
        /// Reppresents marker type within Token
        /// </summary>
        public MarkerType Type { get; private set; }

        #endregion
        
        #region Constructors

        public TokenMarker(int id, Vector2 postion, TouchState state, MarkerType type)
        {
            this.Id = id;
            this.Position = postion;
            this.State = state;
            this.Type = type;
        }

        #endregion
        
        #region Public methods
        
        /// <summary>
        ///Updates Marker Current Position 
        /// </summary>
        /// <param name="newPostion">Marker's new position</param>
        public void UpdatePosition(Vector2 newPostion)
        {
            this.Position = newPostion;
        }

        #endregion
    }

    /// <summary>
    /// Different types a Token's marker can be
    /// </summary>
    public enum MarkerType
    {
        /// <summary>
        /// Marker representing the Origin within the Token domain
        /// </summary>
        Origin,

        /// <summary>
        /// Marker representing the X axis within the Token domain
        /// </summary>
        XAxis,

        /// <summary>
        /// Marker representing the Y axis within the Token domain
        /// </summary>
        YAxis,

        /// <summary>
        /// Marker representing the token's unique id, information encoded in its (x,y) coordinates within the token domanin
        /// </summary>
        Data,

        /// <summary>
        /// Marker type not identified 
        /// </summary>
        Unknown
    }
}
