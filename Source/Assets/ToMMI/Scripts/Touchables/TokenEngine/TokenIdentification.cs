/*
 * @author Francesco Strada
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Touchables.MultiTouchManager;
using Touchables.Utils;
using UnityEngine;
//using UnityEngine.Profiling;

namespace Touchables.TokenEngine
{
    internal sealed class TokenIdentification
    {
        #region Fields

        private static readonly TokenIdentification _instance = new TokenIdentification();
        private float orthogonalAngleThreshold = 0.3f;
        
        #endregion

        #region Public Properties

        public static TokenIdentification Instance
        {
            get
            {
                return _instance;
            }
        }

        #endregion

        #region Public Methods

        public InternalToken IdentifyCluster(Cluster cluster)
        {
            
            InternalToken token = null;
            bool originFound, axisDistinguished;
            //Cluster identification Algorithm
            Dictionary<int, TouchInput> clusterPoints = cluster.Points;
            TokenMarker[] meanSquaredTokenReferenceSystem;

            /*Ordered indexes of markers 0 => Origin 
            *                            1 => X axis 
            *                            2 => Y axis
            *                            3 => Data
            */
            int[] orderedIndexes = new int[4];

            //Find two furthest apart poits which reppresent the x xis and the y axis markers
            Profiler.BeginSample("------Token Identification : Find Two Furthest Point");
            int[] furthestPointIndexes = FindTwoFurthestPoints(clusterPoints.Values.ToArray());
            Profiler.EndSample();

            //Gets indexes of two remaing points which reppresent origin and data marker
            int[] originAndDataIndexes = clusterPoints.Where(p => p.Key != furthestPointIndexes[0] && p.Key != furthestPointIndexes[1])
                                                      .Select(z => z.Key).ToArray();

            Profiler.BeginSample("------Token Identification : Find Origin");
            originFound = FindOriginIndex(furthestPointIndexes, originAndDataIndexes, clusterPoints, ref orderedIndexes);
            Profiler.EndSample();

            if (originFound)
            {
                //Origin Marker Identified and stored in orderedIndexes[0]
                Profiler.BeginSample("------Token Identification : Distinguish Axis");
                axisDistinguished = DistinguishAxisVectors(furthestPointIndexes, clusterPoints, ref orderedIndexes);
                Profiler.EndSample();

                if (axisDistinguished)
                {
                    //Axis markers have been correctly identified
                    //Remaing point is Data Marker
                    orderedIndexes[3] = clusterPoints.Where(p => p.Key != orderedIndexes[0] && p.Key != orderedIndexes[1] && p.Key != orderedIndexes[2])
                                                     .Select(z => z.Key).Single();

                    //Compute Mean Square Problem for reference System

                    Dictionary<int, TokenMarker> markers = TokenUtils.ConvertTouchInputToMarkers(orderedIndexes, clusterPoints);

                    Profiler.BeginSample("------Token Identification: MeanSquare Calcs");
                    meanSquaredTokenReferenceSystem = TokenUtils.MeanSquareOrthogonalReferenceSystem(markers[orderedIndexes[0]],
                                                                                                     markers[orderedIndexes[1]],
                                                                                                     markers[orderedIndexes[2]],
                                                                                                     TokenManager.CurrentTokenType.DistanceOriginAxisMarkersPX);
                    Profiler.EndSample();
                    Profiler.BeginSample("------Token Identification: MeanSquare Calcs Maxima");
                    TokenMarker[] meanSquaredTokenReferenceSystemOptimized = TokenUtils.MeanSquareOrthogonalReferenceSystemOptimized(markers[orderedIndexes[0]],
                                                                                                                       markers[orderedIndexes[1]],
                                                                                                                       markers[orderedIndexes[2]],
                                                                                                                       TokenManager.CurrentTokenType.DistanceOriginAxisMarkersPX);

                    Profiler.EndSample();
                    //Create Token
                    token = new InternalToken(cluster.Hash, markers);
                    token.SetMeanSquareReferenceSystem(meanSquaredTokenReferenceSystem);
                }
                
            }
            else
            {
                //No orthogonal vectors found thus no origin, failed identification
                //For the moment return null, in case consider doing second iteration on second maximum
                
                return token;
            }

            
            return token;

        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Given an array of TouchInputs finds the two furthes apart
        /// </summary>
        /// <param name="points">Array of TouchInputs</param>
        /// <returns>The two furthest TouchInputs indexes</returns>
        private int[] FindTwoFurthestPoints(TouchInput[] points)
        {
            int[] result = new int[2];
            float maxDist = 0.0f;
            float currentDist = 0.0f;

            for (int i = 0; i < points.Length; i++)
            {
                for (int j = i + 1; j < points.Length; j++)
                {
                    currentDist = Vector2.Distance(points[i].Position, points[j].Position);

                    if (currentDist > maxDist)
                    {
                        //Found new max
                        maxDist = currentDist;
                        result[0] = points[i].Id;
                        result[1] = points[j].Id;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Among Token markers find Origin type one
        /// </summary>
        /// <param name="axisIndexes">Indexes of axis markers</param>
        /// <param name="originAndDataIndexes">Indexes of data and origin marker</param>
        /// <param name="clusterPoints">All points</param>
        /// <param name="orderedIndexes"></param>
        /// <returns></returns>
        private bool FindOriginIndex(int[] axisIndexes, int[] originAndDataIndexes, Dictionary<int, TouchInput> clusterPoints, ref int[] orderedIndexes)
        {
            int originIndex;

            Vector2 a0 = clusterPoints[axisIndexes[0]].Position - clusterPoints[originAndDataIndexes[0]].Position;
            Vector2 b0 = clusterPoints[axisIndexes[1]].Position - clusterPoints[originAndDataIndexes[0]].Position;

            float angle0 = Vector2.Dot(a0.normalized, b0.normalized);
            if (angle0 >= -orthogonalAngleThreshold && angle0 <= orthogonalAngleThreshold)
            {
                //Orthogonal Vectors found
                originIndex = originAndDataIndexes[0];
                orderedIndexes[0] = originIndex;
                return true;
            }

            Vector2 a1 = clusterPoints[axisIndexes[0]].Position - clusterPoints[originAndDataIndexes[1]].Position;
            Vector2 b1 = clusterPoints[axisIndexes[1]].Position - clusterPoints[originAndDataIndexes[1]].Position;

            float angle1 = Vector2.Dot(a1.normalized, b1.normalized);
            if (angle1 >= -orthogonalAngleThreshold && angle1 <= orthogonalAngleThreshold)
            {
                originIndex = originAndDataIndexes[1];
                orderedIndexes[0] = originIndex;
                return true;
            }
            else
            {
                //No orthogonal vectors found
                return false;
            }
        }

        /// <summary>
        /// Among the 2 possible axis markers identify which reppresents X and which Y
        /// </summary>
        /// <param name="axisMarkersIndexes"></param>
        /// <param name="clusterPoints"></param>
        /// <param name="orderedIndexes"></param>
        /// <returns></returns>
        private bool DistinguishAxisVectors(int[] axisMarkersIndexes, Dictionary<int, TouchInput> clusterPoints, ref int[] orderedIndexes)
        {
            int xAxisMarkerIndex, yAxisMarkerIndex;

            Vector2 a = clusterPoints[axisMarkersIndexes[0]].Position - clusterPoints[orderedIndexes[0]].Position;
            Vector2 b = clusterPoints[axisMarkersIndexes[1]].Position - clusterPoints[orderedIndexes[0]].Position;

            Vector3 a3 = new Vector3(a.x, a.y, 0.0f);
            Vector3 b3 = new Vector3(b.x, b.y, 0.0f);

            Vector3 direction = Vector3.Normalize(Vector3.Cross(a3, b3));

            if (direction.z > 0.0f)
            {
                //Vector a reppresents x axis
                xAxisMarkerIndex = axisMarkersIndexes[0];
                yAxisMarkerIndex = axisMarkersIndexes[1];
                orderedIndexes[1] = xAxisMarkerIndex;
                orderedIndexes[2] = yAxisMarkerIndex;
                return true;
            }
            else if (direction.z < 0.0f)
            {
                xAxisMarkerIndex = axisMarkersIndexes[1];
                yAxisMarkerIndex = axisMarkersIndexes[0];
                orderedIndexes[1] = xAxisMarkerIndex;
                orderedIndexes[2] = yAxisMarkerIndex;
                return true;
            }
            else
                return false;
        }

        private void SetOrthogonalAngleThreshold(float value)
        {
            this.orthogonalAngleThreshold = value;
        }
        #endregion

    }
}
