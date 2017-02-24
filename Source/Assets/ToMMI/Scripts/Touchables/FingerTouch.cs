/*
 * @author Francesco Strada
 */

using Touchables.MultiTouchManager;
using UnityEngine;

namespace Touchables
{
    public class FingerTouch : ITouchInput
    {
        private int _id;
        private Vector2 _position;
        private TouchState _state;
        public int Id
        {
            get
            {
                return _id;
            }
        }

        public Vector2 Position
        {
            get
            {
                return _position;
            }
        }

        public TouchState State
        {
            get
            {
                return _state;
            }
        }

        internal FingerTouch(TouchInput t)
        {
            this._id = t.Id;
            this._position = t.Position;
            this._state = t.State;
        }

        internal void Update(TouchInput t)
        {
            this._id = t.Id;
            this._position = t.Position;
            this._state = t.State;
        }

        internal void SetState(TouchState newState)
        {
            this._state = newState;
        }
    }
}
