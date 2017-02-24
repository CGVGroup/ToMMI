/*
 * @author Francesco Strada
 */

using System.Collections.Generic;
using UnityEngine;

namespace Touchables.MultiTouchManager
{
    internal static class InternalTouches
    {
        #region Constants

        private const int MAX_CONTEMPORARY_TOUCHES = 11;
        
        #endregion
        
        #region Fields

        private static HashSet<int> _touchIds = new HashSet<int>();
        private static Dictionary<int, TouchInput> _touches = new Dictionary<int, TouchInput>(MAX_CONTEMPORARY_TOUCHES);

        private static HashSet<int> _beganBuffer = new HashSet<int>();
        private static HashSet<int> _movedBuffer = new HashSet<int>();
        private static HashSet<int> _cancelledBuffer = new HashSet<int>();

        private static bool updateBeganTouches = false;
        private static bool updateMovedTouches = false;
        private static bool updateEndedTouches = false;
        private static bool updateCanceledTouches = false;
        #endregion
        
        //TODO make sure lists are never null in case they can be empty
        #region Public Properties

        public static Dictionary<int, TouchInput> List
        {
            get
            {
                return _touches;
            }
        }

        public static HashSet<int> BaganTouhBuffer
        {
            get
            {
                return _beganBuffer;
            }
        }

        public static HashSet<int> MovedTouchBuffer
        {
            get
            {
                return _movedBuffer;
            }
        }

        public static HashSet<int> CancelledTouchBuffer
        {
            get
            {
                return _cancelledBuffer;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Return true if there have been updates in TouchInputs
        /// </summary>
        /// <returns></returns>
        public static bool Updated()
        {
            return updateBeganTouches || updateMovedTouches || updateEndedTouches || updateCanceledTouches;
        }

        /// <summary>
        /// Clears all InternalTouches buffers and sets internal updated boolean to false
        /// </summary>
        public static void ResetBuffers()
        {
            _beganBuffer.Clear();
            _movedBuffer.Clear();
            _cancelledBuffer.Clear();

            updateBeganTouches = false;
            updateMovedTouches = false;
            updateEndedTouches = false;
            updateCanceledTouches = false;

        }

        /// <summary>
        /// Checks how the specified Unity internal <see cref="Touch"/> has changed since last frame or if it is a new incoming one
        /// </summary>
        /// <param name="incomingTouch">Unity interal <see cref="Touch"/>></param>
        public static void UpdateTouchInput(Touch incomingTouch)
        {
            switch (incomingTouch.phase)
            {
                case TouchPhase.Began:
                    {
                        if (!_touchIds.Contains(incomingTouch.fingerId))
                        {
                            //Touch Began
                            _touchIds.Add(incomingTouch.fingerId);
                            AddTouchToList(incomingTouch, TouchState.Began);

                            //Add ID to BeganBuffer
                            _beganBuffer.Add(incomingTouch.fingerId);
                            updateBeganTouches = true;
                            break;
                        }
                        else
                        {
                            //We missed this Touch ID end or cancelled state
                            AddTouchToList(incomingTouch, TouchState.Began);

                            _cancelledBuffer.Add(incomingTouch.fingerId);
                            _beganBuffer.Add(incomingTouch.fingerId);
                            updateBeganTouches = true;
                            break;
                        }
                    }
                case TouchPhase.Moved:
                    {
                        if (!_touchIds.Contains(incomingTouch.fingerId))
                        {
                            //We missed this touch Began state
                            _touchIds.Add(incomingTouch.fingerId);
                            AddTouchToList(incomingTouch, TouchState.Moved);

                            _beganBuffer.Add(incomingTouch.fingerId);
                            updateBeganTouches = true;
                            break;
                        }
                        else
                        {
                            //An update has occurred of TouchInput
                            AddTouchToList(incomingTouch, TouchState.Moved);

                            _movedBuffer.Add(incomingTouch.fingerId);
                            updateMovedTouches = true;
                            break;
                        }
                    }
                case TouchPhase.Ended:
                    {
                        if (!_touchIds.Contains(incomingTouch.fingerId))
                        {
                            //We completly missed TouchInput begin-end transitions
                            break;
                        }
                        else
                        {
                            _touchIds.Remove(incomingTouch.fingerId);
                            _touches.Remove(incomingTouch.fingerId);

                            _cancelledBuffer.Add(incomingTouch.fingerId);
                            updateEndedTouches = true;
                            break;
                        }
                    }
                case TouchPhase.Canceled:
                    {
                        if (!_touchIds.Contains(incomingTouch.fingerId))
                        {
                            //We completly missed TouchInput begin-end transitions
                            break;
                        }
                        else
                        {
                            _touchIds.Remove(incomingTouch.fingerId);
                            _touches.Remove(incomingTouch.fingerId);

                            _cancelledBuffer.Add(incomingTouch.fingerId);
                            updateCanceledTouches = true;
                            break;
                        }
                    }
                case TouchPhase.Stationary:
                    {
                        if (!_touchIds.Contains(incomingTouch.fingerId))
                        {
                            //We missed TouchInput Began Phase
                            _touchIds.Add(incomingTouch.fingerId);
                            AddTouchToList(incomingTouch, TouchState.Stationary);

                            //Add ID to BeganBuffer
                            _beganBuffer.Add(incomingTouch.fingerId);
                            updateBeganTouches = true;
                            break;
                        }
                        else
                        {
                            AddTouchToList(incomingTouch, TouchState.Stationary);
                            break;
                        }
                    }
            }
        }
        #endregion

        #region Private Methods

        private static void AddTouchToList(Touch t, TouchState state)
        {
            _touches[t.fingerId] = new TouchInput(t.fingerId, t.position, state);
        }

        #endregion
    }
}
