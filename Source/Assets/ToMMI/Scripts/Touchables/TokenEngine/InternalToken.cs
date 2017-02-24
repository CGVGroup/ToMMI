/*
 * @author Francesco Strada
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Touchables.MultiTouchManager;
using Touchables.TokenEngine.TokenTypes;
using Touchables.Utils;
using UnityEngine;

namespace Touchables.TokenEngine
{
    internal sealed class InternalToken : IToken
    {
        #region Fields

        //Inherited properties from IToken interface
        private int _id;
        private int? _class;
        private string _hashId;
        private Vector2 _position;
        private Vector2 _deltaPosition;
        private float _angle;
        private float _deltaAngle;

        //Token properties which are need for internal operations
        //HashSet<int> _markersIds = new HashSet<int>();
        Dictionary<int, TokenMarker> markers = new Dictionary<int, TokenMarker>();
        Dictionary<MarkerType, TokenMarker> typeMarkers = new Dictionary<MarkerType, TokenMarker>();

        private TokenMarker[] meanSquaredTokenReferenceSystem;
        private Vector2 meanSquaredTokenPosition;
        private float meanSquaredTokenAngle;
        private Vector2 deltaMeanSquarePosition;
        private float deltaMeanSquareAngle;
        private Vector3 meanSquareXAxis;
        private int[] dataGridIndexes;

        private Vector3 XAxisVector;
        private Vector3 YAxisVector;

        #endregion

        #region Properties

        /// <inheritdoc />
        public int Id { get { return _id; } }

        /// <inheritdoc />
        public int? Class { get { return _class; } }

        /// <summary>
        /// Unique hash inherited from cluster. <seealso cref="Cluster.Hash"/>
        /// </summary>
        public string HashId { get { return _hashId; } }

        /// <inheritdoc />
        public Vector2 Position { get { return _position; } }

        /// <inheritdoc />
        public Vector2 DeltaPosition { get { return _deltaPosition; } }

        /// <inheritdoc />
        public float Angle { get { return _angle; } }

        /// <inheritdoc />
        public float DeltaAngle { get { return _deltaAngle; } }

        public Vector2 MeanSquarePosition { get { return meanSquaredTokenPosition; } }
        public float MeanSquareAngle { get { return meanSquaredTokenAngle; } }
        public Vector2 MeanSquareDeltaPosition { get { return deltaMeanSquarePosition; } }
        public float MeanSquareDeltaAngle { get { return deltaMeanSquareAngle; } }

        #endregion

        #region Constructors

        public InternalToken(string hashID, Dictionary<int, TokenMarker> markers)
        {
            this._hashId = hashID;
            this._id = 0;
            this.markers = markers;
            this.typeMarkers = markers.Values.ToDictionary(m => m.Type, m => m);
            this.XAxisVector = typeMarkers[MarkerType.XAxis].Position - typeMarkers[MarkerType.Origin].Position;
            this.YAxisVector = typeMarkers[MarkerType.YAxis].Position - typeMarkers[MarkerType.Origin].Position;

            //For the moment position will be the origin marker

            _position = typeMarkers[MarkerType.Origin].Position;

            _deltaPosition = Vector2.zero;
            _deltaAngle = 0.0f;

            UpdateAngle();
            UpdatePosition();

        }

        #endregion

        #region Public Methods

        public void Update(Cluster cluster)
        {
            int markerId;
            Vector2 newPostion;
            foreach (KeyValuePair<int, TouchInput> entry in cluster.Points)
            {
                markerId = entry.Key;
                newPostion = entry.Value.Position;

                this.markers[markerId].UpdatePosition(newPostion);
            }

            UpdateMarkersTypeList();
            UpdateTokenAxis();
            UpdateAngle();
            UpdatePosition();

            TokenMarker[] meanSquaredTokenReferenceSystemOptimized = TokenUtils.MeanSquareOrthogonalReferenceSystemOptimized(typeMarkers[MarkerType.Origin],
                                                                                                                             typeMarkers[MarkerType.XAxis],
                                                                                                                             typeMarkers[MarkerType.YAxis],
                                                                                                                             TokenManager.CurrentTokenType.DistanceOriginAxisMarkersPX);
            SetMeanSquareReferenceSystem(meanSquaredTokenReferenceSystemOptimized);
        }

        public InternalToken UpdateAngle()
        {
            float newAngle = CalculateAngle(XAxisVector);
            this._deltaAngle = newAngle - this._angle;
            this._angle = newAngle;

            return this;
        }

        public InternalToken UpdateMeanSquareAngle()
        {
            float newAngle = CalculateAngle(meanSquareXAxis);
            this.deltaMeanSquareAngle = newAngle - this.meanSquaredTokenAngle;
            this.meanSquaredTokenAngle = newAngle;

            return this;
        }

        public InternalToken UpdatePosition()
        {
            //The center position of the Token
            //Current Token Angle + PI/4 
            float posAngle = this._angle * Mathf.Deg2Rad + (Mathf.PI / 4f);
            Vector2 newPosition = new Vector2(typeMarkers[MarkerType.Origin].Position.x + (TokenManager.CurrentTokenType.DistanceOriginCenterPX * Mathf.Cos(posAngle)),
                                              typeMarkers[MarkerType.Origin].Position.y + (TokenManager.CurrentTokenType.DistanceOriginCenterPX * Mathf.Sin(posAngle)));

            this._deltaPosition = newPosition - this._position;
            this._position = newPosition;

            return this;
        }

        public InternalToken UpdatePositionMeanSquare()
        {
            //The center position of the Token
            //Current Token Angle + PI/4 
            float posAngle = this.meanSquaredTokenAngle * Mathf.Deg2Rad + (Mathf.PI / 4f);
            Vector2 newPosition = new Vector2(this.meanSquaredTokenReferenceSystem[0].Position.x + (TokenManager.CurrentTokenType.DistanceOriginCenterPX * Mathf.Cos(posAngle)),
                                              this.meanSquaredTokenReferenceSystem[0].Position.y + (TokenManager.CurrentTokenType.DistanceOriginCenterPX * Mathf.Sin(posAngle)));

            this.deltaMeanSquarePosition = newPosition - this._position;
            this.meanSquaredTokenPosition = newPosition;

            return this;
        }

        public bool ComputeTokenClass(ClassComputeReferenceSystem refSystem, ClassComputeDimension dimension)
        {
            float tokenAngleRad;
            Vector2 dataOriginalPosition;
            Vector2 originPosition;

            if (refSystem == ClassComputeReferenceSystem.Regular)
            {
                tokenAngleRad = this._angle * Mathf.Deg2Rad;
                originPosition = typeMarkers[MarkerType.Origin].Position;
            }
            else
            {
                tokenAngleRad = this.meanSquaredTokenAngle * Mathf.Deg2Rad;
                originPosition = this.meanSquaredTokenReferenceSystem[0].Position;
            }

            dataOriginalPosition = typeMarkers[MarkerType.Data].Position;

            Vector2 dataCoordinates = TokenUtils.ComputeRotoTranslation(dataOriginalPosition, originPosition, tokenAngleRad);

            dataGridIndexes = CalculateDataGridIndexes(dataCoordinates, dimension);

            this._class = TokenManager.CurrentTokenType.GetTokenClass(dataGridIndexes[0], dataGridIndexes[1]);

            if (this._class != null)
                return true;
            else
                return false;
        }

        public void SetMeanSquareReferenceSystem(TokenMarker[] referenceSystem)
        {
            this.meanSquaredTokenReferenceSystem = referenceSystem;
            Vector3[] meanSquaredAxis = CalculateAxis(this.meanSquaredTokenReferenceSystem[1].Position,
                                                      this.meanSquaredTokenReferenceSystem[2].Position,
                                                      this.meanSquaredTokenReferenceSystem[0].Position);

            meanSquareXAxis = meanSquaredAxis[0];
            this.meanSquaredTokenAngle = CalculateAngle(meanSquareXAxis);
            UpdatePositionMeanSquare();

        }

        public void SetTokenId(int id)
        {
            this._id = id;
        }

        #endregion

        #region Private Methods

        private void UpdateMarkersTypeList()
        {
            this.typeMarkers.Clear();
            this.typeMarkers = markers.Values.ToDictionary(m => m.Type, m => m);
        }

        private void UpdateTokenAxis()
        {
            Vector3[] axis = CalculateAxis(typeMarkers[MarkerType.XAxis].Position, typeMarkers[MarkerType.YAxis].Position, typeMarkers[MarkerType.Origin].Position);
            this.XAxisVector = axis[0];
            this.YAxisVector = axis[1];
        }

        private int[] CalculateDataGridIndexes(Vector2 dataCoordinates, ClassComputeDimension dimension)
        {
            int[] result = new int[2];
            float xCoord;
            float yCoord;

            if (dimension == ClassComputeDimension.Pixels)
            {
                xCoord = (dataCoordinates.x - TokenManager.CurrentTokenType.DataMarkerOriginPositionPX) / TokenManager.CurrentTokenType.DataGridMarkersStepPX;
                yCoord = (dataCoordinates.y - TokenManager.CurrentTokenType.DataMarkerOriginPositionPX) / TokenManager.CurrentTokenType.DataGridMarkersStepPX;
            }
            else
            {
                Vector2 dataCoordinatesCM = new Vector2(TokensEngine.PixelsToCm(dataCoordinates.x), TokensEngine.PixelsToCm(dataCoordinates.y));
                xCoord = (dataCoordinatesCM.x - TokenManager.CurrentTokenType.DataMarkerOriginPositionCM) / TokenAttributes.TOKEN_DATA_MARKERS_STEP;
                yCoord = (dataCoordinatesCM.y - TokenManager.CurrentTokenType.DataMarkerOriginPositionCM) / TokenAttributes.TOKEN_DATA_MARKERS_STEP;
            }

            result[0] = (int)Math.Round(xCoord, 0, MidpointRounding.AwayFromZero);
            result[1] = (int)Math.Round(yCoord, 0, MidpointRounding.AwayFromZero);

            return result;

        }

        private Vector3[] CalculateAxis(Vector2 xAxisMarker, Vector2 yAxisMarker, Vector2 originMarker)
        {
            Vector3[] axis = new Vector3[2];
            axis[0] = xAxisMarker - originMarker;
            axis[1] = yAxisMarker - originMarker;

            return axis;
        }

        private float CalculateAngle(Vector3 xAxis)
        {
            float newAngle;
            float direction = Vector3.Cross(Vector3.right, xAxis).normalized.z;
            float tmpAngle = Vector3.Angle(Vector3.right, xAxis);

            if (direction > 0)
                newAngle = tmpAngle;
            else
            {
                newAngle = 180 + (180 - tmpAngle);
            }

            return newAngle;
        }

        #endregion
    }

    internal enum ClassComputeReferenceSystem
    {
        MeanSqure,
        Regular

    }

    internal enum ClassComputeDimension
    {
        Pixels,
        Centimeters
    }
}
