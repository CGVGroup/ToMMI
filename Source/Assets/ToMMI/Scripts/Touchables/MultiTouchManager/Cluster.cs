/*
 * @author Francesco Strada
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Touchables.Utils;
using System.Text;

namespace Touchables.MultiTouchManager
{
    internal sealed class Cluster
    {
        #region Fields

        private Dictionary<int, TouchInput> _points = new Dictionary<int, TouchInput>();
        private HashSet<int> _pointsIds = new HashSet<int>();

        private String _hashId;
        private Vector2 _centroid;
        private ClusterState _state;

        private string _cancelledHash;
        private HashSet<int> _cancelledPointsIds = new HashSet<int>();

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a Cluster instance from a single TouchInput
        /// </summary>
        /// <param name="point"></param>
        public Cluster(TouchInput point)
        {
            this.AddPoint(point.Id);
            this._hashId = "#" + point.Id;
            this._centroid = point.Position;
            _state = ClusterState.Invalid;
        }

        /// <summary>
        /// Cretaes a Cluster instance from a list of List of InputTouch
        /// </summary>
        /// <param name="touchPoints"></param>
        public Cluster(List<TouchInput> touchPoints)
        {
            foreach (TouchInput t in touchPoints)
            {
                _pointsIds.Add(t.Id);
                _points.Add(t.Id, t);
            }

            UpdateCentroid();
            this._hashId = GetPointsHash(_pointsIds.ToArray<int>());

            if (_pointsIds.Count == 4)
                //It is a new cluster of 4 points and requires to be identified
                this._state = ClusterState.Unidentified;
            else
                this._state = ClusterState.Invalid;
        }

        public Cluster() { }

        #endregion

        #region Properties

        /// <summary>
        /// String Hash uniquely identifing the Cluster
        /// </summary>
        public String Hash
        {
            get
            {
                return _hashId;
            }
        }
        
        /// <summary>
        /// Cluster centroid
        /// </summary>
        public Vector2 Centroid
        {
            get
            {
                return _centroid;
            }
        }

        /// <summary>
        /// Previously identified cancelled cluster hash  
        /// </summary>
        public String CancelledClusterHash
        {
            get
            {
                return _cancelledHash;
            }
        }

        /// <summary>
        /// Points ids which were cancelled from an identified cluster
        /// </summary>
        public HashSet<int> CancelledPointIds
        {
            get
            {
                return _cancelledPointsIds;
            }
        }

        /// <summary>
        /// TouchInput points present in cluster
        /// </summary>
        public Dictionary<int, TouchInput> Points
        {
            get
            {
                return _points;
            }
        }

        /// <summary>
        /// TouchInput ids present in cluster
        /// </summary>
        public HashSet<int> PointsIds
        {
            get
            {
                return _pointsIds;
            }
        }

        /// <summary>
        /// Current cluster state
        /// </summary>
        public ClusterState State
        {
            get
            {
                return _state;
            }
        }

        #endregion

        #region Public Methods

        public Cluster AddPoint(int touchId)
        {
            TouchInput touch = new TouchInput(touchId, InternalTouches.List[touchId].Position, InternalTouches.List[touchId].State);
            _pointsIds.Add(touchId);
            _points.Add(touchId, touch);
            UpdateCentroid();
            this._hashId = GetPointsHash(_pointsIds.ToArray<int>());

            if (_pointsIds.Count == 4)
                this._state = ClusterState.Unidentified;

            else if (_pointsIds.Count > 4)
                this._state = ClusterState.Invalid;

            return this;
        }

        public Cluster[] UpdatePoint(int touchId)
        {
            Cluster newCluster;
            if (Vector2.Distance(this.Centroid, InternalTouches.List[touchId].Position) < ClusterManager.Instance.ClusterDistThreshold)
            {
                //Point still in clustrer
                _points[touchId] = new TouchInput(touchId, InternalTouches.List[touchId].Position, TouchState.Moved);
                newCluster = null;

                if (State == ClusterState.Identidied)
                    _state = ClusterState.Updated;
            }
            else
            {
                //Point has moved out of the cluster
                //Handle current Cluster

                //If it was just one point then we must cancel the cluster!!!!!!
                //       if (_pointsIds.Count != 1)
                //       {
                _pointsIds.Remove(touchId);
                _points.Remove(touchId);

                if (_state == ClusterState.Identidied || _state == ClusterState.Updated)
                {
                    _state = ClusterState.Cancelled;
                    _cancelledHash = this._hashId;
                    _cancelledPointsIds.Add(touchId);
                }

                else if (State == ClusterState.Cancelled)
                    _cancelledPointsIds.Add(touchId);

                else if (_pointsIds.Count == 4)
                    _state = ClusterState.Unidentified;

                else
                    _state = ClusterState.Invalid;

                //Update new Hash
                this._hashId = GetPointsHash(_pointsIds.ToArray<int>());
                //       }
                //        else


                newCluster = new Cluster(InternalTouches.List[touchId]);

            }

            UpdateCentroid();

            return new Cluster[] { this, newCluster };

        }

        public Cluster RemovePoint(int touchId)
        {
            _pointsIds.Remove(touchId);
            _points.Remove(touchId);
            UpdateCentroid();

            if (State == ClusterState.Identidied || State == ClusterState.Updated)
            {
                _state = ClusterState.Cancelled;
                _cancelledHash = this._hashId;
                _cancelledPointsIds.Add(touchId);
            }
            else if (State == ClusterState.Cancelled)
                _cancelledPointsIds.Add(touchId);

            else if (_pointsIds.Count == 4)
                _state = ClusterState.Unidentified;
            else
                _state = ClusterState.Invalid;

            this._hashId = GetPointsHash(_pointsIds.ToArray<int>());

            return this;
        }

        public void SetState(ClusterState newState)
        {
            this._state = newState;
        }

        public void SetCancelledClusterHash(string hash)
        {
            this._cancelledHash = hash;
        }

        public void SetCancelledPointIds(HashSet<int> cancelledIds)
        {
            this._cancelledPointsIds = cancelledIds;
        }

        #endregion

        #region Private Methods

        private void UpdateCentroid()
        {
            float xTmp = 0f;
            float yTmp = 0f;
            foreach (KeyValuePair<int, TouchInput> entry in _points)
            {
                xTmp += entry.Value.Position.x;
                yTmp += entry.Value.Position.y;
            }

            if (_points.Count != 0)
            {
                _centroid.x = xTmp / _points.Count;
                _centroid.y = yTmp / _points.Count;
            }

        }
        #endregion        

        /// <summary>
        /// Given an Array of ints, reppresenting touch points ids, it formats them in a string like #x#y... unique Hash
        /// </summary>
        /// <param name="pointIds">Array of touch point ids</param>
        /// <returns>Hash string</returns>
        private String GetPointsHash(int[] pointIds)
        {
            StringBuilder hashString = new StringBuilder();
            hashString.Remove(0, hashString.Length);
            for (int i = 0; i < pointIds.Length; i++)
            {
                hashString.Append("#");
                hashString.Append(pointIds[i]);
            }
            return hashString.ToString();
        }
    }

    /// <summary>
    /// All possible states a Cluster can be in
    /// </summary>
    public enum ClusterState
    {
        /// <summary>
        /// Cluster has reached 4 points and need to be Identified by TokenManager
        /// </summary>
        Unidentified,

        /// <summary>
        /// Cluster has succesfully been identified by TokenManager
        /// </summary>
        Identidied,

        /// <summary>
        /// Points in identified Cluster were updated
        /// </summary>
        Updated,

        /// <summary>
        /// One ore more points were removed from an Identified Cluster
        /// </summary>
        Cancelled,

        /// <summary>
        /// A cluster in an unspecific state, mostly composed of finger touch points
        /// </summary>
        Invalid
    }
}
