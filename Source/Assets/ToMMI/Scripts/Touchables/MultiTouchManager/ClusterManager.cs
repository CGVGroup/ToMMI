/*
 * @author Francesco Strada
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Touchables.TokenEngine;
using UnityEngine;
//using UnityEngine.Profiling;

namespace Touchables.MultiTouchManager
{
    internal sealed class ClusterManager
    {
        #region Fields

        private static readonly ClusterManager _instance = new ClusterManager();
        private Dictionary<String, Cluster> clusters = new Dictionary<String, Cluster>();
        private float _clusterDistThreshold;

        private List<Cluster> ClustersToIdentify = new List<Cluster>();
        private List<Cluster> IdentifiedClustersMoved = new List<Cluster>();
        private Dictionary<string, Cluster> IdentifiedClustersCancelled = new Dictionary<string, Cluster>();

        readonly object ClusterUpdateLock = new object();

        #endregion

        #region Constructor

        private ClusterManager() { }

        #endregion

        #region Events

        public event EventHandler<ClustersUpdateEventArgs> ClustersUpdateEvent;

        #endregion

        #region Public Properties

        /// <summary>
        /// Cluster Manager instance
        /// </summary>
        public static ClusterManager Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// List of all registered Clusters
        /// </summary>
        public List<Cluster> Clusters
        {
            get
            {
                return clusters.Values.ToList();
            }
        }

        /// <summary>
        /// Distance threshold from cluster centroid
        /// </summary>
        public float ClusterDistThreshold
        {
            get
            {
                return _clusterDistThreshold;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Initializes Cluster Manager with subscription to specific events
        /// </summary>
        /// <returns></returns>
        public void Initialize()
        {
            InputServer.Instance.InputUpdated += OnInputsUpdateEventHandler;
            TokenManager.Instance.TokenIdentifiedEvent += OnTokenIdentified;
            
            //TokenManager.Instance.TokenCancelledEvent += OnTokenCancelled;
        }

        /// <summary>
        /// Disables Cluster Manager unsubscribing it from specific events
        /// </summary>
        public void Disable()
        {
            InputServer.Instance.InputUpdated -= OnInputsUpdateEventHandler;
            TokenManager.Instance.TokenIdentifiedEvent -= OnTokenIdentified;

            //TokenManager.Instance.TokenCancelledEvent -= OnTokenCancelled;
        }

        public void Update()
        {
            // first of all, remove finger touches previously identified as ended
            // (to comply with unity standards of handling touches)
            InputManager.UpdateFingersCancelled();

            InputServer.Instance.Update();
        }

        public void SetClusterDistThreshold(float threshold)
        {
            this._clusterDistThreshold = threshold;
        }

        #endregion

        #region Private Methods
        
        /// <summary>
        /// Finds in which cluster is the specified TouchInput
        /// </summary>
        /// <param name="touchPointId">Specified TouchInput id</param>
        /// <returns>If cluster is found returns its HashId otherwise returns null</returns>
        private String GetClusterIdFromTouchPoint(int touchPointId)
        {
            foreach (KeyValuePair<String, Cluster> entry in clusters)
            {
                if (entry.Value.PointsIds.Contains(touchPointId))
                    return entry.Key;
            }
            return null;
        }

        /// <summary>
        /// Cluster Update hierarchical algorithm 
        /// </summary>
        private void UpdateClusters()
        {
            float minDist = this._clusterDistThreshold;
            float dist;
            bool minFound = false;
            List<Cluster> clustersList = this.clusters.Values.ToList();

            int mergeCluster1Index = 0;
            int mergeCluster2Index = 0;
            Cluster r1 = new Cluster();
            Cluster r2 = new Cluster();

            if (clusters.Count > 1)
            {
                while (minDist <= this._clusterDistThreshold)
                {
                    for (int i = 0; i < clustersList.Count; i++)
                    {
                        for (int j = i + 1; j < clustersList.Count; j++)
                        {
                            Cluster c1 = clustersList.ElementAt(i);
                            Cluster c2 = clustersList.ElementAt(j);

                            dist = Vector2.Distance(c1.Centroid, c2.Centroid);

                            if (dist < minDist)
                            {
                                minDist = dist;
                                mergeCluster1Index = i;
                                mergeCluster2Index = j;
                                r1 = clustersList[i];
                                r2 = clustersList[j];
                                minFound = true;
                            }
                        }
                    }
                    if (minFound)
                    {
                        minFound = false;
                        Cluster mergedCluster = MergeClusters(clustersList.ElementAt(mergeCluster1Index), clustersList.ElementAt(mergeCluster2Index));
                        clustersList.Remove(r1);
                        clustersList.Remove(r2);
                        clustersList.Add(mergedCluster);
                    }
                    else
                        minDist *= 2;
                }

                clusters = clustersList.ToDictionary(v => v.Hash, v => v);
            }
        }

        /// <summary>
        /// Merges 2 clusters together
        /// </summary>
        /// <param name="c1">Cluster 1</param>
        /// <param name="c2">Cluster 2</param>
        /// <returns>Merged cluster</returns>
        private Cluster MergeClusters(Cluster c1, Cluster c2)
        {
            if (c1.State == ClusterState.Updated || c1.State == ClusterState.Identidied)
            {
                c1.SetCancelledClusterHash(c1.Hash);
                c1.SetCancelledPointIds(c1.PointsIds);
                c1.SetState(ClusterState.Cancelled);
                IdentifiedClustersCancelled.Add(c1.Hash, c1);
            }
            if (c2.State == ClusterState.Updated || c2.State == ClusterState.Identidied)
            {
                c2.SetCancelledClusterHash(c2.Hash);
                c2.SetCancelledPointIds(c2.PointsIds);
                c2.SetState(ClusterState.Cancelled);
                IdentifiedClustersCancelled.Add(c2.Hash, c2);

            }

            List<TouchInput> allPoints = c1.Points.Values.ToList();
            allPoints.AddRange(c2.Points.Values.ToList());

            return new Cluster(allPoints);
        }

        /// <summary>
        /// Checks if there have been updated clusters
        /// </summary>
        private void CheckClustersUpdated()
        {
            foreach (KeyValuePair<String, Cluster> entry in clusters)
            {
                Cluster cluster = entry.Value;
                switch (cluster.State)
                {
                    case ClusterState.Unidentified:
                        {
                            //Cluster has reached for points and needs to be sent to identifier for check
                            ClustersToIdentify.Add(cluster);
                            break;
                        }
                    case ClusterState.Updated:
                        {
                            //Identified cluster has moved
                            IdentifiedClustersMoved.Add(cluster);
                            break;
                        }
                }
            }
        }

        private void ResetClustersBuffers()
        {
            ClustersToIdentify.Clear();
            IdentifiedClustersMoved.Clear();
            IdentifiedClustersCancelled.Clear();

        }

        #endregion

        #region Event Handlers

        private void OnInputsUpdateEventHandler(object sender, InputUpdateEventArgs e)
        {
            Profiler.BeginSample("---ClusterManager : Clusters Update Total");
            String clusterHash;

            ResetClustersBuffers();

            Profiler.BeginSample("------ClusterManager : Cancelled Buffer");
            foreach (int touchId in InternalTouches.CancelledTouchBuffer)
            {
                clusterHash = GetClusterIdFromTouchPoint(touchId);
                if (clusterHash != null)
                {
                    //Is a cluster with more than one point
                    if (clusters[clusterHash].PointsIds.Count > 1)
                    {
                        Cluster updatedCluster = clusters[clusterHash].RemovePoint(touchId);
                        //Update Current state Clusters
                        clusters.Remove(clusterHash);
                        clusters.Add(updatedCluster.Hash, updatedCluster);

                        //If State is Cancelled update CancelledCluster Buffer
                        if (updatedCluster.State == ClusterState.Cancelled)
                        {
                            IdentifiedClustersCancelled.Remove(updatedCluster.CancelledClusterHash);
                            IdentifiedClustersCancelled.Add(updatedCluster.CancelledClusterHash, updatedCluster);
                        }
                    }
                    //Is a cluster with only one point
                    else
                    {
                        //Update CancelledClusterBuffer
                        Cluster cluster = clusters[clusterHash].RemovePoint(touchId);
                        if (cluster.State == ClusterState.Cancelled)
                        {
                            IdentifiedClustersCancelled.Remove(cluster.CancelledClusterHash);
                            IdentifiedClustersCancelled.Add(cluster.CancelledClusterHash, cluster);
                        }

                        //Remove cluster from current Clusters
                        clusters.Remove(clusterHash);
                    }


                }
                //Remove touch from fingers touch list

            }
            Profiler.EndSample();

            Profiler.BeginSample("------ClusterManager : Moved Buffer");
            foreach (int touchId in InternalTouches.MovedTouchBuffer)
            {
                clusterHash = GetClusterIdFromTouchPoint(touchId);
                if (clusterHash != null)
                {
                    Cluster[] updatedCluster = clusters[clusterHash].UpdatePoint(touchId);

                    clusters.Remove(clusterHash);
                    if (updatedCluster[0].Points.Count != 0)
                    {
                        clusters.Add(updatedCluster[0].Hash, updatedCluster[0]);
                        if (updatedCluster[0].State == ClusterState.Cancelled)
                        {
                            IdentifiedClustersCancelled.Remove(updatedCluster[0].CancelledClusterHash);
                            IdentifiedClustersCancelled.Add(updatedCluster[0].CancelledClusterHash, updatedCluster[0]);
                        }
                    }

                    //Its the case where a previous cluster gets separeted into two
                    if (updatedCluster[1] != null)
                        clusters.Add(updatedCluster[1].Hash, updatedCluster[1]);

                }
                else
                {
                    Cluster c = new Cluster(InternalTouches.List[touchId]);
                    clusters.Add(c.Hash, c);
                }
            }
            Profiler.EndSample();


            foreach (int touchId in InternalTouches.BaganTouhBuffer)
            {
                Cluster cl = new Cluster(InternalTouches.List[touchId]);
                clusters.Add(cl.Hash, cl);

            }

            if (InternalTouches.CancelledTouchBuffer.Count > 0 || InternalTouches.MovedTouchBuffer.Count > 0 || InternalTouches.BaganTouhBuffer.Count > 0)
            {
                Profiler.BeginSample("------ClusterManager : Clusters Update");
                UpdateClusters();
                Profiler.EndSample();

                CheckClustersUpdated();

                if (ClustersToIdentify.Count > 0 || IdentifiedClustersMoved.Count > 0 || IdentifiedClustersCancelled.Count > 0)
                    RaiseClustersUpdateEvent(new ClustersUpdateEventArgs(ClustersToIdentify, IdentifiedClustersMoved, IdentifiedClustersCancelled.Values.ToList()));
                    
                
                //Get points which are touches and not markers
                InputManager.SetFingersCancelled(InternalTouches.CancelledTouchBuffer.ToArray());

                foreach (Cluster c in clusters.Values)
                {
                    if (c.State == ClusterState.Invalid || c.State == ClusterState.Cancelled)
                    {
                        //This cluster is only made of finger touch points
                        foreach (TouchInput touch in c.Points.Values)
                        {
                            InputManager.AddFingerTouch(touch);
                        }

                    }

                }
            }

            Profiler.EndSample();
        }

        private void OnTokenIdentified(object sender, InternalTokenIdentifiedEventArgs e)
        {
            if (e.Success)
            {
                clusters[e.TokenHashId].SetState(ClusterState.Identidied);
                InputManager.SetFingersCancelled(clusters[e.TokenHashId].PointsIds.ToArray());
            }
            else
            {
                clusters[e.TokenHashId].SetState(ClusterState.Invalid);
            }
        }

        private void OnTokenCancelled(object sender, InternalTokenCancelledEventArgs e)
        {
            //clusters[e.TokenHashId].SetState(ClusterState.Invalid);

            //Add these to FingerTouches
            //Cluster c = clusters[e.TokenHashId];
            //foreach (KeyValuePair<int, TouchInput> record in c.Points)
            //{
            //    InputManager.AddFingerTouch(record.Value);
            //}
        }
        #endregion

        #region Event Launchers

       //This manages all cluster events simultaneously
        private void RaiseClustersUpdateEvent(ClustersUpdateEventArgs e)
        {
            EventHandler<ClustersUpdateEventArgs> handler;

            lock (ClusterUpdateLock)
            {
                handler = ClustersUpdateEvent;
            }

            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion
    }

    //TODO eventual customized event args for cancelled cluster in which put also the index of the touch point cancelled

    internal class ClustersUpdateEventArgs : EventArgs
    {
        private List<Cluster> _clustersToIdentify = new List<Cluster>();
        private List<Cluster> _clustersIdentifiedMoved = new List<Cluster>();
        private List<Cluster> _clustersIdentifiedCancelled = new List<Cluster>();

        public List<Cluster> ClustersToIdentify { get { return _clustersToIdentify; } }
        public List<Cluster> ClustersIdentifiedMoved { get { return _clustersIdentifiedMoved; } }
        public List<Cluster> ClustersIdentifiedCancelled { get { return _clustersIdentifiedCancelled; } }

        internal ClustersUpdateEventArgs(List<Cluster> cToIdentify, List<Cluster> cIdentifiedMoved, List<Cluster> cIdentifiedCancelled)
        {
            _clustersToIdentify = cToIdentify;
            _clustersIdentifiedMoved = cIdentifiedMoved;
            _clustersIdentifiedCancelled = cIdentifiedCancelled;
        }
    }
}
