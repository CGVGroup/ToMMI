/*
 * @author Francesco Strada
 */

using System;
using UnityEngine;
using UnityEngine.Profiling;

namespace Touchables.MultiTouchManager
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class InputServer
    {
        #region Properties

        private static readonly InputServer _instance = new InputServer();

        readonly object UpdateInputsLock = new object();

        #endregion

        #region Events

        /// <summary>
        /// Inputs updated from previous frame
        /// </summary>
        public event EventHandler<InputUpdateEventArgs> InputUpdated;

        #endregion

        #region Constructor

        private InputServer() { }

        #endregion

        #region Properties

        /// <summary>
        /// Instance of the InputServer 
        /// </summary>
        public static InputServer Instance
        {
            get
            {
                return _instance;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Update method must be called every frame in order to dispatch Unity's Input.touches()
        /// </summary>
        public void Update()
        {
            InternalTouches.ResetBuffers();

            //PROFILER
            Profiler.BeginSample("---MultiTouch Manager: Input Buffers Update");
            foreach (Touch t in Input.touches)
            {
                InternalTouches.UpdateTouchInput(t);
            }
            Profiler.EndSample();
            //END PROFILER

            if (InternalTouches.Updated())
            {
                // There has been at least one updated TouchInput from previous frame
                RaiseInputsUpdatedEvent(new InputUpdateEventArgs("Successfull Update"));
            }
        }

        #endregion

        #region Event Launcher

        private void RaiseInputsUpdatedEvent(InputUpdateEventArgs e)
        {
            EventHandler<InputUpdateEventArgs> handler;

            lock (UpdateInputsLock)
            {
                handler = InputUpdated;
            }
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion 
    }

    public class InputUpdateEventArgs : EventArgs
    {
        public String EventMsg { get; private set; }

        public InputUpdateEventArgs(String msg)
        {
            EventMsg = msg;
        }
    }
}
