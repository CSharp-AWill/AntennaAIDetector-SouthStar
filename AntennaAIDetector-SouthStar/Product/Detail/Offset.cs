﻿using AntennaAIDetector_SouthStar.Algorithm;
using SimpleGroup.Core.Struct;
using Aqrose.Framework.Utility.DataStructure;

namespace AntennaAIDetector_SouthStar.Product.Detail
{
    public class Offset:IEvaluateAIDI
    {
        public bool IsAddToDetection { get; set; } = true;
        public bool IsResultOK
        {
            get
            {
                return 0 == Region.XldPointsNums.Count;
            }
        }

        //
        public MatrixD Matrix { get; set; } = null;

        public double StandardXFilter { get; set; } = 0.0;
        public double StandardYFilter { get; set; } = 0.0;
        public double UpFilter { get; set; } = 0.0;
        public double DownFilter { get; set; } = 0.0;
        public double LeftFilter { get; set; } = 0.0;
        public double RightFilter { get; set; } = 0.0;
        //
        public double CurrX { get; /*private*/ set; } = 0.0;
        public double CurrY { get; /*private*/ set; } = 0.0;
        //
        public ResultOfAIDI ResultOfAIDI { get; set; } = new ResultOfAIDI(null);
        public ShapeOf2D Region { get; set; } = new ShapeOf2D();

        public Offset()
        {
        }

        private void CorrectPos(PointShape org, out PointShape res)
        {
            if (null == org)
            {
                res = null;
            }
            if (null == Matrix)
            {
                Matrix = new MatrixD();
            }
            org.GetPoint(out var orgX, out var orgY);
            Affine.AffineTransPoint2D(Matrix, org, out res);

            return;
        }

        private bool IsInRange(PointShape point)
        {
            if (null == point)
            {
                return false;
            }

            point.GetPoint(out var x, out var y);
            if (StandardXFilter - LeftFilter >= x || StandardXFilter + RightFilter <= x || StandardYFilter - UpFilter >= y || StandardYFilter + DownFilter <= y)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #region IEvaluateAIDI
        public void CalculateRegion()
        {
            Region = new ShapeOf2D();
            if (null != ResultOfAIDI.ResultDetailOfAIDI && 0 != ResultOfAIDI.ResultDetailOfAIDI.Count)
            {
                foreach (var aidiResult in ResultOfAIDI.ResultDetailOfAIDI.GetRange(ResultOfAIDI.ResultDetailOfAIDI.Count - 1, 1))
                {
                    CorrectPos(new PointShape(aidiResult.CenterX, aidiResult.CenterY), out var point);
                    if (!IsInRange(point))
                    {
                        Region += aidiResult.Region;
                    }
                    CurrX = aidiResult.CenterX;
                    CurrY = aidiResult.CenterY;
                }
            }

            return;
        }
        #endregion
    }

}
