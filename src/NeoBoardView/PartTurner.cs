using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Math;

namespace NeoBoardView
{
    internal class PartTurner
    {
        private const double ParallelRowThreshold = 0.04;

        //private void FindPinLine(SceneObjects.Part src, out int pinCount, out double angle)
        //{
        //    var firstPin = src.FirstPin;
        //    var endPin = src.FirstPin+src.PinCount;
        //    int[] strides = {1, 4};
        //    pinCount = 0;
        //    angle = 0;
        //}

        // XXX: make variant of CheckPinRow that would find row length

        // order-independent version
        public bool CheckPinRowOI(SceneObjects.Part part, int first, int count, int stride = 1)
        {
            const double distThreshold = 1;
            var iFirst = part.FirstPin+first;
            var firstPin = Root.Scene.Pins[iFirst].Location;
            var lastPin = Root.Scene.Pins[iFirst+(count-1)*stride].Location;
            var maxDst = 0.0;
            for (int i = iFirst+stride; i<iFirst+(count-1)*stride; i+=stride)
            {
                var pin = Root.Scene.Pins[i].Location;
                var newDst = pin.DistanceToLine(firstPin, lastPin);
                if (newDst>maxDst)
                    maxDst = newDst;
            }
            if (maxDst>distThreshold)
                return false;
            return true;
        }

        public bool CheckPinRow(SceneObjects.Part part, int first, int count, int stride = 1)
        {
            const double distThreshold = 1; // 5 mil?
            //const double stepThreshold = 0.025; // 1.43 deg
            //const double finalThreshold = 0.025; // 1.43 deg
            //var dir = Vector2.Origin;
            //var error = 0.0;
            //for (int i = part.FirstPin+first+1; i<part.FirstPin+first+count; i++)
            //{
            //    var newDir = Root.Scene.Pins[i].Location-Root.Scene.Pins[i-1].Location;
            //    var newAng = newDir.RelativeAngle(dir);
            //    error += newAng;
            //}
            var iFirst = part.FirstPin+first;
            var firstPin = Root.Scene.Pins[iFirst].Location;
            var lastPin = Root.Scene.Pins[iFirst+(count-1)*stride].Location;
            var maxDst = 0.0;
            for (int i = iFirst+stride; i<iFirst+(count-1)*stride; i+=stride)
            {
                var pin = Root.Scene.Pins[i].Location;
                var newDst = pin.DistanceToSegment(firstPin, lastPin);
                if (newDst>maxDst)
                    maxDst = newDst;
            }
            if (maxDst>distThreshold)
                return false;
            return true;
        }
        
        private bool CheckDualRow(SceneObjects.Part src, ref double angle,
            int r1a, int r1b, int r2a, int r2b)
        {
            var d1 = GetPinPairDir(src, r1a, r1b);
            var d2 = GetPinPairDir(src, r2a, r2b);
            if (d1==Vector2.Origin || d2==Vector2.Origin)
                return false;
            if (d1.CrossingAngle(d2)<ParallelRowThreshold)
            {
                angle = d1.OriginAngle();
                return true;
            }
            return false;
        }

        private bool CheckTripleRow(SceneObjects.Part src, ref double angle,
            int r1a, int r1b, int r2a, int r2b, int r3a, int r3b)
        {
            var d1 = GetPinPairDir(src, r1a, r1b);
            var d2 = GetPinPairDir(src, r2a, r2b);
            var d3 = GetPinPairDir(src, r3a, r3b);
            if (d1==Vector2.Origin || d2==Vector2.Origin || d3==Vector2.Origin)
                return false;
            if (d1.CrossingAngle(d2)<ParallelRowThreshold && d2.CrossingAngle(d3)<ParallelRowThreshold)
            {
                angle = d2.OriginAngle();
                return true;
            }
            return false;
        }

        // tests if angle abc is a right angle
        private bool IsOrthogonal(Vector2 a, Vector2 b, Vector2 c, double threshold)
        {
            var ab = (b-a).Normal;
            var bc = (c-b).Normal;
            return Math.Abs(ab.Dot(bc))<threshold;
        }

        private bool IsRectangle(Vector2 a, Vector2 b, Vector2 c, Vector2 d, double threshold)
        {
            return IsOrthogonal(a, b, c, threshold) &&
                IsOrthogonal(b, c, d, threshold) &&
                IsOrthogonal(c, d, a, threshold);
        }

        // If the order is not known in advance, we need a slightly more complicated check:
        private bool CheckPinRect(SceneObjects.Part src, double threshold)
        {
            var a = Root.Scene.Pins[src.FirstPin+0].Location;
            var b = Root.Scene.Pins[src.FirstPin+1].Location;
            var c = Root.Scene.Pins[src.FirstPin+2].Location;
            var d = Root.Scene.Pins[src.FirstPin+3].Location;
            return IsRectangle(a, b, c, d, threshold) ||
                IsRectangle(b, c, a, d, threshold) ||
                IsRectangle(c, a, b, d, threshold);
        }

        private bool DetectR(SceneObjects.Part src, ref double aspect, ref double growR, out double angle)
        {
            angle = 0;
            switch (src.PinCount)
            {
            case 2:
                angle = CalculatePinPairAngle(src, 0, 1);
                return true;
            case 4:
                aspect = -0.6; // force aspect
                if (CheckPinRowOI(src, 0, 4))
                {
                    var d34 = GetPinPairDir(src, 2, 3).Length;
                    var d13 = GetPinPairDir(src, 0, 2).Length;
                    if (d34<d13)
                    {
                        var f = d13/d34;
                        if (f<3.5 && !(1.3<f && f<1.7))
                            aspect = -1/0.6;
                    }
                    angle = CalculatePinPairAngle(src, 0, 3);
                    return true;
                }
                // check this shape:
                /*
                    |      * |
                    | *      |
                    | *      |
                    |      * |
                */
                if (CheckPinRect(src, 0.02))
                {
                    angle = CalculatePinPairAngle(src, 0, 1);
                    return true;
                }
                angle = 0;
                return true;
            }
            return false;
        }

        private bool DetectL(SceneObjects.Part src, ref double aspect, ref double growR, out double angle)
        {
            if (src.PinCount<2)
            {
                angle = 0;
                return false;
            }
            const double maxLongCoilSize = 123;// 104;// 73; // mil?
            const double maxDiagCoilSize = 175;
            const double bigCoilRotFactor = 9;
            const double cornerDx = 0.5/1.41421356237; //0.5/Math.Sqrt(2);
            angle = 0;
            var dir = GetPinPairDir(src, 0, 1);
            var diam = dir.Length;
            var bigCoil = diam>maxLongCoilSize;
            if (bigCoil)
                aspect = 1;
            switch (src.PinCount)
            {
            case 2:
                if (bigCoil)
                {
                    var diagCoil = diam<=maxDiagCoilSize;
                    // XXX: either 0 or 45 degrees
                    var center = dir/2;
                    var dx = Math.Abs(center.X)/diam;
                    var dDiagX = Math.Abs(cornerDx-dx);
                    // same small distance from coil X-axis
                    if (0<dx && dx<0.1 && dx<diam/bigCoilRotFactor)
                    {
                        // isn't done if dx1 or dx2 is zero
                        angle = Math.PI/4;
                        return true;
                    }
                    else if (0.1<dx && dx<0.3 || (!diagCoil && (0.05<dDiagX && dDiagX<0.1 || dDiagX<0.01)))
                    {
                        angle = CalculatePinPairAngle(src, 0, 1);
                        return true;
                    }
                    var dy = Math.Abs(center.Y)/diam;
                    var dDiagY = Math.Abs(cornerDx-dy);
                    // same small distance from coil Y-axis
                    if (0<dy && dy<0.1 && dy<diam/bigCoilRotFactor)
                    {
                        angle = Math.PI/4;
                        return true;
                    }
                    else if (0.1<dy && dy<0.3 || (!diagCoil && (0.05<dDiagY && dDiagY<0.1 || dDiagY<0.01)))
                    {
                        angle = CalculatePinPairAngle(src, 0, 1);
                        return true;
                    }
                    angle = 0;
                    return true;
                }
                angle = CalculatePinPairAngle(src, 0, 1);
                return true;
            case 3:
            case 4:
                angle = CalculatePinPairAngle(src, 0, 1);
                return true;
            }
            return false;
        }

        private bool DetectC(SceneObjects.Part src, ref double aspect, ref double growR, out double angle)
        {
            if (src.PinCount<2)
            {
                angle = 0;
                return false;
            }
            angle = CalculatePinPairAngle(src, 0, 1);
            if (src.PinCount==2)
            {
                var d = GetPinPairDir(src, 0, 1).Length;
                if (163<=d && d<=164 ||
                    202<=d && d<=203 ||
                    137<=d && d<=138 ||
                    d==210)
                {
                    src.DrawAsCircle = true;
                }
            }
            return true;
        }
        // tantalum cap
        private bool DetectTC(SceneObjects.Part src, ref double aspect, ref double growR, out double angle)
        {
            if (src.PinCount<2)
            {
                angle = 0;
                return false;
            }
            angle = CalculatePinPairAngle(src, 0, 1);
            return true;
        }

        private bool DetectF(SceneObjects.Part src, ref double aspect, ref double growR, out double angle)
        {
            if (src.PinCount<2)
            {
                angle = 0;
                return false;
            }
            angle = CalculatePinPairAngle(src, 0, 1);
            return true;
        }

        private bool DetectU(SceneObjects.Part src, ref double aspect, ref double growR, out double angle)
        {
            if (src.PinCount<2)
            {
                angle = 0;
                return false;
            }
            angle = 0;
            // U-specific settings
            growR = 0;
            var defaultAspect = aspect;
            aspect = 0;
            switch (src.PinCount)
            {
            case 2:
                aspect = defaultAspect;
                angle = CalculatePinPairAngle(src, 0, 1);
                return true;
            case 3:
                angle = CalculatePinPairAngle(src, 0, 1);
                return true;
            case 4:
            case 5:
                if (CheckPinRow(src, 0, 3))
                {
                    angle = CalculatePinPairAngle(src, 0, 2);
                    return true;
                }
                if (CheckDualRow(src, ref angle, 0, 1, 2, 3))
                    return true;
                break;
            case 6: // 2(3)
            case 7: // 2(3) +gnd
                if (CheckPinRow(src, 0, 3) && CheckPinRow(src, 3, 3))
                {
                    angle = CalculatePinPairAngle(src, 0, 2);
                    return true;
                }
                break;
            case 8:
                if (CheckPinRow(src, 0, 4) && CheckPinRow(src, 4, 4))
                {
                    angle = CalculatePinPairAngle(src, 0, 3);
                    return true;
                }
                break;
            case 9:
                if (CheckDualRow(src, ref angle, 0, 2, 3, 5))
                    return true;
                goto case 8;
            case 20: // 2(2+8) (+gnd)
            case 21:
                if (CheckPinRow(src, 1, 8) && CheckPinRow(src, 11, 8)) // mux
                {
                    angle = CalculatePinPairAngle(src, 11, 18);
                    return true;
                }
                break;
            case 14:
            case 15:
                if (CheckPinRow(src, 1, 5) && CheckPinRow(src, 8, 5)) // mux
                {
                    angle = CalculatePinPairAngle(src, 8, 12);
                    return true;
                }
                break;
            case 16: // 2(4+4)
                {
                    #region
                    int good = 0;
                    int pa = -1, pb = -1;
                    if (CheckPinRow(src, 0, 4))
                    {
                        pa = 0;
                        pb = 3;
                        good++;
                    }
                    if (CheckPinRow(src, 4, 4))
                    {
                        pa = 4;
                        pb = 7;
                        good++;
                    }
                    if (CheckPinRow(src, 8, 4))
                    {
                        pa = 8;
                        pb = 13;
                        good++;
                    }
                    if (CheckPinRow(src, 12, 4))
                    {
                        pa = 12;
                        pb = 15;
                        good++;
                    }
                    if (good>2)
                    {
                        angle = CalculatePinPairAngle(src, pa, pb);
                        return true;
                    }
                    #endregion
                    goto case 17;
                }
            case 17: // 2(2+6) (+gnd)
                if (CheckPinRow(src, 1, 6) && CheckPinRow(src, 9, 6)) // mux
                {
                    angle = CalculatePinPairAngle(src, 9, 14);
                    return true;
                }
                break;
            }
            if (src.PinCount>=32)
            {
                if (CheckPinRow(src, 2, 4))
                {
                    angle = CalculatePinPairAngle(src, 2, 5);
                    return true;
                }
            }
            if (src.PinCount>=20)
            {
                if (CheckPinRow(src, 0, 5))
                {
                    angle = CalculatePinPairAngle(src, 0, 4);
                    return true;
                }
            }
            if (src.PinCount>10)
            {
                if (CheckPinRow(src, 0, 3))
                    angle = CalculatePinPairAngle(src, 0, 2);
                else if (CheckPinRow(src, 1, 3))
                    angle = CalculatePinPairAngle(src, 1, 3);
                else
                    angle = 0;
                return true;
            }
            angle = 0.0;
            return false;
        }

        private bool DetectQ(SceneObjects.Part src, ref double aspect, ref double growR, out double angle)
        {
            if (src.PinCount<2)
            {
                angle = 0;
                return false;
            }
            angle = 0;
            switch (src.PinCount)
            {
            case 3:
                //if (!CheckPinRow(src, 0, 3))
                //    aspect = -1/aspect;
                // XXX: 3-pin Q/D parts should have rotated standard aspect
                // (and size based on max dimension instead of min)
                {
                    var d12 = GetPinPairDir(src, 0, 1).Length;
                    var d23 = GetPinPairDir(src, 1, 2).Length;
                    var d13 = GetPinPairDir(src, 0, 2).Length;
                    if (d12<d23 && d12<d13)
                    {
                        angle = CalculatePinPairAngle(src, 0, 1);
                        return true;
                    }
                    if (d23<d12 && d23<d13)
                    {
                        angle = CalculatePinPairAngle(src, 1, 2);
                        return true;
                    }
                    if (d13<d23 && d13<d12)
                    {
                        angle = CalculatePinPairAngle(src, 0, 2);
                        return true;
                    }
                }
                return true;
            case 4:
                if (CheckPinRow(src, 0, 3)) // 3+1
                {
                    angle = CalculatePinPairAngle(src, 0, 2);
                    return true;
                }
                // XXX: 2+2
                break;
            case 5:
                if (CheckPinRow(src, 0, 3)) // 4+1, 3+2
                {
                    angle = CalculatePinPairAngle(src, 0, 2);
                    return true;
                }
                break;
            case 6:
                if (CheckDualRow(src, ref angle, 0, 1, 4, 5))
                    return true;
                if (CheckPinRow(src, 0, 3))
                {
                    angle = CalculatePinPairAngle(src, 0, 2);
                    return true;
                }
                break;
            case 7:
                angle = CalculatePinPairAngle(src, 2, 3);
                return true;
            case 8: // weird looking pin configuration (docked pentagons)
                if (CheckTripleRow(src, ref angle, 0, 2, 4, 6, 3, 7))
                    return true;
                angle = CalculatePinPairAngle(src, 2, 3);
                return true;
            case 9: // weird looking apple mosfet
                if (CheckPinRow(src, 4, 4))
                {
                    angle = CalculatePinPairAngle(src, 4, 7);
                    return true;
                }
                if (CheckPinRow(src, 0, 3))
                {
                    angle = CalculatePinPairAngle(src, 0, 2);
                    return true;
                }
                break;
            case 10: // diode assembly
                if (CheckDualRow(src, ref angle, 0, 1, 3, 4))
                    return true;
                break;
            }
            if (src.PinCount>10)
            {
                angle = CalculatePinPairAngle(src, 0, 1);
                return true;
            }
            angle = 0.0;
            return false;
        }

        private bool DetectJ(SceneObjects.Part src, ref double aspect, ref double growR, out double angle)
        {
            angle = 0;
            // J-specific
            aspect = 0;
            if (src.PinCount>=24)
            {
                if (CheckPinRow(src, 1, 3, 3)) // pci/ddr
                {
                    angle = CalculatePinPairAngle(src, 1, 7);
                    return true;
                }
            }
            if (src.PinCount>=6)
            {
                if (CheckPinRow(src, 0, 3, 2)) // dual row
                {
                    if (src.PinCount>=24 && CheckPinRow(src, 0, 6, 2)) // try get more accurate angle
                        angle = CalculatePinPairAngle(src, 0, 10);
                    else
                        angle = CalculatePinPairAngle(src, 0, 4);
                    return true;
                }
            }
            if (src.PinCount>=5)
            {
                if (CheckPinRow(src, 1, 3, 1)) // single row
                {
                    angle = CalculatePinPairAngle(src, 1, 3);
                    return true;
                }
                if (CheckPinRow(src, 0, 3, 1)) // single row
                {
                    if (CheckDualRow(src, ref angle, 0, 2, 3, 4))
                        return true;
                }
            }
            if (src.PinCount==4)
            {
                if (CheckPinRow(src, 0, 3, 1)) // single row
                {
                    angle = CalculatePinPairAngle(src, 1, 3);
                    return true;
                }
                // 2+2gnd
                if (CheckDualRow(src, ref angle, 0, 1, 2, 3))
                    return true;
            }
            if (src.PinCount==3)
            {
                angle = CalculatePinPairAngle(src, 0, 2);
                return true;
            }
            if (src.PinCount==2)
            {
                angle = CalculatePinPairAngle(src, 0, 1);
                return true;
            }
            angle = 0;
            return false;
        }

        private bool DetectXW(SceneObjects.Part src, ref double aspect, ref double growR, out double angle)
        {
            angle = 0;
            // XW-specific
            aspect = 0;
            growR = 0.5;
            switch (src.PinCount)
            {
            case 2:
                angle = CalculatePinPairAngle(src, 0, 1);
                return true;
            }
            return false;
        }

        private bool DetectY(SceneObjects.Part src, ref double aspect, ref double growR, out double angle)
        {
            if (src.PinCount<2)
            {
                angle = 0;
                return false;
            }
            if (src.PinCount>2) // long packages have >2 pins
                aspect = 0;
            angle = CalculatePinPairAngle(src, 0, 1);
            return true;
        }

        public Vector2 GetPinPairDir(SceneObjects.Part src, int iPin1, int iPin2)
        {
            var pin1 = Root.Scene.Pins[src.FirstPin+iPin1].Location;
            var pin2 = Root.Scene.Pins[src.FirstPin+iPin2].Location;
            return Vector2.Max(pin1, pin2)-Vector2.Min(pin1, pin2);
        }

        public double CalculatePinPairAngle(SceneObjects.Part src, int iPin1, int iPin2)
        {
            var dir = GetPinPairDir(src, iPin1, iPin2);
            if (dir.SqrLength<0.001)
                return double.NaN;
            return dir.OriginAngle();
        }

        private bool NameCharIsDigit(SceneObjects.Part src, int i)
        { return src.Name.Length>=i+1 && char.IsNumber(src.Name[i]); }

        private bool NameCharIsLetter(SceneObjects.Part src, int i, char chk)
        { return src.Name.Length>=i+1 && src.Name[i]==chk; }

        public double CalculatePartShape(SceneObjects.Part src,
            ref double aspect, ref double growR, out bool match)
        {
            var angle = 0.0;
            int i1 = 0;
            int i2 = 1;
            //growR = 1.0;
            //aspect = 1.0/2;
            switch (src.Name[0])
            {
            case 'C':
                if (src.Name.Length>=3 && src.Name[1]=='N' && char.IsNumber(src.Name[2])) // connector
                    goto case 'J';
                match = DetectC(src, ref aspect, ref growR, out angle);
                return angle;
            case 'F':
                if (NameCharIsDigit(src, 1))
                {
                    match = DetectF(src, ref aspect, ref growR, out angle);
                    return 0;
                }
                if (NameCharIsLetter(src, 1, 'L') && NameCharIsDigit(src, 2))
                {
                    match = DetectL(src, ref aspect, ref growR, out angle);
                    return angle;
                }
                break;
            case 'R':
                match = DetectR(src, ref aspect, ref growR, out angle);
                return angle;
            case 'L':
                if (!NameCharIsDigit(src, 1))
                    //goto case 'H';
                    goto case 'D';
            //case 'H':
                match = DetectL(src, ref aspect, ref growR, out angle);
                return angle;
            case 'D':
                if (src.Name.Length>=3 && src.Name[1]=='M' && char.IsNumber(src.Name[2]))
                    goto case 'J';
                goto case 'Q';
            case 'Q':
                match = DetectQ(src, ref aspect, ref growR, out angle);
                return angle;
            case 'T':
                if (src.Name.Length>=3 && src.Name[1]=='C' && char.IsNumber(src.Name[2]))
                {
                    match = DetectTC(src, ref aspect, ref growR, out angle);
                    return angle;
                }
                break;
            case 'U': // inner box (-radius or -0)
                match = DetectU(src, ref aspect, ref growR, out angle);
                return angle;
            case 'J':
                match = DetectJ(src, ref aspect, ref growR, out angle);
                return angle;
            case 'X':
                if (src.Name[1]=='W')
                {
                    match = DetectXW(src, ref aspect, ref growR, out angle);
                    return angle;
                }
                if (char.IsNumber(src.Name[1]))
                    goto case 'Y';
                break;
            case 'Y':
                match = DetectY(src, ref aspect, ref growR, out angle);
                return angle;
            }
            aspect = 0;
            var result = 0.0;
            if (src.PinCount==2)
                result = CalculatePinPairAngle(src, i1, i2);
            match = !double.IsNaN(result);
            return result; // XXX: return 0
        }

        public double CalculatePartMinShape(SceneObjects.Part src,
            ref double aspect, ref double growR, out bool match)
        {
            match = true;
            if (src.PinCount<=1)
                return 0;
            if (src.PinCount==2)
            {
                var pin1 = Root.Scene.Pins[src.FirstPin+0].Location;
                var pin2 = Root.Scene.Pins[src.FirstPin+1].Location;
                var dir = Vector2.Max(pin1, pin2)-Vector2.Min(pin1, pin2);
                if (dir.SqrLength<0.001)
                    return 0;
                return dir.OriginAngle();
            }
            var verts = new List<Vector2>(src.PinCount);
            for (int i = src.FirstPin; i<src.FirstPin+src.PinCount; i++)
            {
                var nv = Root.Scene.Pins[i].Location;
                if (verts.Count==0)
                {
                    verts.Add(nv);
                    continue;
                }
                if (verts[verts.Count-1]!=nv)
                    verts.Add(nv);
            }
            var obb = MinimalOBB.Calculate(verts);
            return obb.Turn;
        }

        public bool Check90degAlignment(double originAngle)
        {
            var pi2x = originAngle/(Math.PI/2);
            var error = pi2x-Math.Round(pi2x);
            return Math.Abs(error)<=0.001;
        }

        // snap must be positive
        public bool AdjustSmallAngle(double angle, double snap, double tolerance, out double result)
        {
            const double pi4 = Math.PI/4;
            var offset = 0.0;
            if (-pi4<angle && angle<=pi4)
                offset = 0; // right
            else if (pi4<angle && angle<=3*pi4)
                offset = 2*pi4; // top
            else if (-3*pi4<=angle && angle<-pi4)
                offset = -2*pi4; // bottom
            else
                offset = Math.PI*Math.Sign(angle); // left            
            result = angle-offset;
            var diff = snap-Math.Abs(result);
            if (Math.Abs(diff)<tolerance)
            {
                result = offset + Math.Sign(result)*snap;
                return true;
            }
            return false;
        }

        public double AdjustPartAngle(double angle, bool forceAlign = false)
        {
            const double pi4 = Math.PI/4;
            var pi4x = angle/pi4;
            var r = Math.Round(pi4x);
            if (Math.Abs(pi4x-r)<=0.08) // ~9 degrees side to side
                return r*pi4;
            double smallAngle;
            const double deg32 = 0.558505; // apple lcd connectors
            const double deg30 = 0.523599; // apple lcd connectors
            const double deg25 = 0.436332; // apple lcd connectors
            const double deg20 = 0.349066; // apple lcd connectors
            //if (AdjustSmallAngle(angle, deg25, 0.08, out smallAngle))
            //    return smallAngle;
            if (forceAlign)
                return 0;
            return angle;
        }
    }
}
