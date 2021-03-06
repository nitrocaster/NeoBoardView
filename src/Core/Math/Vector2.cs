using System;
using System.Drawing;

namespace Core.Math
{
    using SysMath = System.Math;

    /// <summary>
    /// Vector of doubles with two components (x, y)
    /// </summary>
    /// <author>Richard Potter BSc(Hons), Pavel Kovalenko</author>
    /// <created>Jun-04</created>
    /// <modified>Nov-12</modified>
    /// <version>1.21</version>
    /// <Acknowledgement>This code is adapted from Exocortex - Ben Houston </Acknowledgement>
    [Serializable]
    public struct Vector2 : 
        IComparable, 
        IComparable<Vector2>, 
        IEquatable<Vector2>, 
        IFormattable
    {
        #region Struct variables

        /// <summary>
        /// The X component of the vector
        /// </summary>
        public double X;

        /// <summary>
        /// The Y component of the vector
        /// </summary>
        public double Y;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for the Vector2 class accepting two doubles
        /// </summary>
        /// <param name="x">The new x value for the Vector2</param>
        /// <param name="y">The new y value for the Vector2</param>
        public Vector2(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Constructor for the Vector2 class from an array
        /// </summary>
        /// <param name="xy">Array representing the new values for the Vector2</param>
        public Vector2(double[] xy) :
            this(xy[0], xy[1])
        {
            if (xy.Length!=2)
                throw new ArgumentException(MsgTwoComponents);
        }

        /// <summary>
        /// Constructor for the Vector2 class from another Vector2 object
        /// </summary>
        /// <param name="src">Vector2 representing the new values for the Vector2</param>
        public Vector2(Vector2 src) :
            this(src.X, src.Y)
        {}

        #endregion

        #region Accessors & Mutators

        public void Set(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Property for the length (aka. magnitude or absolute value) of the Vector2
        /// </summary>
        public double Length
        {
            get { return SysMath.Sqrt(SqrLength); }
            set
            {
                if (value<0)
                    throw new ArgumentOutOfRangeException("value", value, MsgNegativeMagnitude);
                if (this==Origin)
                    throw new ArgumentException(MsgOraginVectorMagnitude, "this");
                this = this*(value/Length);
            }
        }

        /// <summary>
        /// Get squared length
        /// </summary>
        /// <returns>The sum of the Vectors X^2, Y^2 and Z^2 components</returns>
        public double SqrLength
        { get { return X*X+Y*Y; } }

        /// <summary>
        /// Returns the Vector2 as an array
        /// </summary>
        /// <exception cref="System.ArgumentException">
        /// Thrown if the array argument does not contain exactly three components 
        /// </exception>
        public double[] ToArray()
        { return new[] {X, Y}; }

        /// <summary>
        /// An index accessor 
        /// Mapping index [0] -> X and [1] -> Y.
        /// </summary>
        /// <param name="index">The array index referring to a component within the vector (i.e. x, y, z)</param>
        /// <exception cref="System.ArgumentException">
        /// Thrown if the array argument does not contain exactly three components 
        /// </exception>
        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return X;
                    case 1: return Y;
                    default: throw new ArgumentException(MsgTwoComponents, "index");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    default:
                        throw new ArgumentException(MsgTwoComponents, "index");
                }
            }
        }

        #endregion

        #region Operators

        /// <summary>
        /// Addition of two Vectors
        /// </summary>
        /// <param name="v1">Vector2 to be added to </param>
        /// <param name="v2">Vector2 to be added</param>
        /// <returns>Vector2 representing the sum of two Vectors</returns>
        public static Vector2 operator+(Vector2 v1, Vector2 v2)
        { return new Vector2(v1.X+v2.X, v1.Y+v2.Y); }

        /// <summary>
        /// Addition of a Vector2 and a scalar value
        /// </summary>
        /// <param name="v1">Vector2 to be added to</param>
        /// <param name="s2">Scalar value to be added to each vector component</param>
        /// <returns>Vector2 representing the sum of v1 and another Vector2
        /// with all components set to s2</returns>
        public static Vector2 operator+(Vector2 v1, double s2)
        { return new Vector2(v1.X+s2, v1.Y+s2); }
        
        /// <summary>
        /// Subtraction of two Vectors
        /// </summary>
        /// <param name="v1">Vector2 to be subtracted from </param>
        /// <param name="v2">Vector2 to be subtracted</param>
        /// <returns>Vector2 representing the difference of two Vectors</returns>
        public static Vector2 operator-(Vector2 v1, Vector2 v2)
        { return new Vector2(v1.X-v2.X, v1.Y-v2.Y); }

        /// <summary>
        /// Subtraction of a Vector2 and a scalar value
        /// </summary>
        /// <param name="v1">Vector2 to be substracted from</param>
        /// <param name="s2">Scalar value to be substracted from each vector component</param>
        /// <returns>Vector2 representing the difference of v1 vector and another Vector2
        /// with all components set to s2</returns>
        public static Vector2 operator-(Vector2 v1, double s2)
        { return new Vector2(v1.X-s2, v1.Y-s2); }

        /// <summary>
        /// Product of a Vector2 and a scalar value
        /// </summary>
        /// <param name="v1">Vector2 to be multiplied </param>
        /// <param name="s2">Scalar value to be multiplied by </param>
        /// <returns>Vector2 representing the product of the vector and scalar</returns>
        public static Vector2 operator*(Vector2 v1, double s2)
        { return new Vector2(v1.X*s2, v1.Y*s2); }

        /// <summary>
        /// Product of a scalar value and a Vector2
        /// </summary>
        /// <param name="s1">Scalar value to be multiplied </param>
        /// <param name="v2">Vector2 to be multiplied by </param>
        /// <returns>Vector2 representing the product of the scalar and Vector2</returns>
        public static Vector2 operator*(double s1, Vector2 v2)
        { return v2*s1; }

        /// <summary>
        /// Division of a Vector2 and a scalar value
        /// </summary>
        /// <param name="v1">Vector2 to be divided </param>
        /// <param name="s2">Scalar value to be divided by </param>
        /// <returns>Vector2 representing the division of the vector and scalar</returns>
        public static Vector2 operator/(Vector2 v1, double s2)
        { return new Vector2(v1.X/s2, v1.Y/s2); }

        /// <summary>
        /// Negation of a Vector2
        /// Invert the direction of the Vector2
        /// Make Vector2 negative (-vector)
        /// </summary>
        /// <param name="v1">Vector2 to be negated  </param>
        /// <returns>Negated vector</returns>
        public static Vector2 operator-(Vector2 v1)
        { return new Vector2(-v1.X, -v1.Y); }

        /// <summary>
        /// Reinforcement of a Vector2
        /// Make Vector2 positive (+vector)
        /// </summary>
        /// <param name="v1">Vector2 to be reinforced </param>
        /// <returns>Reinforced vector</returns>
        public static Vector2 operator+(Vector2 v1)
        { return new Vector2(+v1.X, +v1.Y); }

        /// <summary>
        /// Compare the magnitude of two Vectors (less than)
        /// </summary>
        /// <param name="v1">Vector2 to be compared </param>
        /// <param name="v2">Vector2 to be compared with</param>
        /// <returns>True if v1 less than v2</returns>
        public static bool operator<(Vector2 v1, Vector2 v2)
        { return v1.SqrLength<v2.SqrLength; }

        /// <summary>
        /// Compare the magnitude of two Vectors (greater than)
        /// </summary>
        /// <param name="v1">Vector2 to be compared </param>
        /// <param name="v2">Vector2 to be compared with</param>
        /// <returns>True if v1 greater than v2</returns>
        public static bool operator>(Vector2 v1, Vector2 v2)
        { return v1.SqrLength>v2.SqrLength; }

        /// <summary>
        /// Compare the magnitude of two Vectors (less than or equal to)
        /// </summary>
        /// <param name="v1">Vector2 to be compared </param>
        /// <param name="v2">Vector2 to be compared with</param>
        /// <returns>True if v1 less than or equal to v2</returns>
        public static bool operator<=(Vector2 v1, Vector2 v2)
        { return v1.SqrLength<=v2.SqrLength; }

        /// <summary>
        /// Compare the magnitude of two Vectors (greater than or equal to)
        /// </summary>
        /// <param name="v1">Vector2 to be compared </param>
        /// <param name="v2">Vector2 to be compared with</param>
        /// <returns>True if v1 greater than or equal to v2</returns>
        public static bool operator>=(Vector2 v1, Vector2 v2)
        { return v1.SqrLength>=v2.SqrLength; }

        /// <summary>
        /// Compare two Vectors for equality.
        /// Are two Vectors equal.
        /// </summary>
        /// <param name="v1">Vector2 to be compared for equality </param>
        /// <param name="v2">Vector2 to be compared to </param>
        /// <returns>Boolean decision (truth for equality)</returns>
        public static bool operator==(Vector2 v1, Vector2 v2)
        { return SysMath.Abs(v1.X-v2.X)<=EqualityTolerence && SysMath.Abs(v1.Y-v2.Y)<=EqualityTolerence; }

        /// <summary>
        /// Negative comparator of two Vectors.
        /// Are two Vectors different.
        /// </summary>
        /// <param name="v1">Vector2 to be compared for in-equality </param>
        /// <param name="v2">Vector2 to be compared to </param>
        /// <returns>Boolean decision (truth for in-equality)</returns>
        public static bool operator!=(Vector2 v1, Vector2 v2)
        { return !(v1==v2); }

        // ******************************************************

        public static implicit operator PointF(Vector2 v1)
        { return new PointF((float)v1.X, (float)v1.Y); }

        public static implicit operator Vector2(PointF p1)
        { return new Vector2(p1.X, p1.Y); }

        #endregion

        #region Functions

        /// <summary>
        /// Determine the exterior product of two Vectors
        /// </summary>
        /// <param name="v1">The vector to multiply</param>
        /// <param name="v2">The vector to multiply by</param>
        /// <returns>Double value representing the exterior product</returns>
        public static double ExteriorProduct(Vector2 v1, Vector2 v2)
        { return v1.X*v2.Y - v1.Y*v2.X; }

        /// <summary>
        /// Determine the exterior product of two Vectors
        /// </summary>
        /// <param name="other">The vector to multiply by</param>
        /// <returns>Double value representing the exterior product</returns>
        public double ExteriorProduct(Vector2 other)
        { return ExteriorProduct(this, other); }

        /// <summary>
        /// Determine the dot product of two Vectors
        /// </summary>
        /// <param name="v1">The vector to multiply</param>
        /// <param name="v2">The vector to multiply by</param>
        /// <returns>Scalar representing the dot product of the two vectors</returns>
        public static double Dot(Vector2 v1, Vector2 v2)
        { return v1.X*v2.X + v1.Y*v2.Y; }

        /// <summary>
        /// Determine the dot product of this Vector2 and another
        /// </summary>
        /// <param name="v2">The vector to multiply by</param>
        /// <returns>Scalar representing the dot product of the two vectors</returns>
        public double Dot(Vector2 v2)
        { return Dot(this, v2); }

        public double Cross(Vector2 v2, Vector2 v3)
        {
            var ab = v2-this;
            var ac = v3-this;
            return ab.X*ac.Y - ab.Y*ac.X;
        }

        /// <summary>
        /// Get the normalized vector
        /// Scale the Vector2 so that the magnitude is 1
        /// </summary>
        /// <returns>The normalized Vector2</returns>
        /// <exception cref="System.DivideByZeroException">
        /// Thrown when the normalisation of a zero magnitude vector is attempted
        /// </exception>
        public Vector2 Normal
        {
            get
            {
                // Check for divide by zero errors
                var len = SqrLength;
                if (len==0)
                    throw new DivideByZeroException(MsgNormalize0);
                // find the inverse of the vectors magnitude
                var inverse = 1/SysMath.Sqrt(len);
                // multiply each component by the inverse of the magnitude
                return new Vector2(X*inverse, Y*inverse);
            }
        }

        /// <summary>
        /// Scale the Vector2 so that the magnitude is 1
        /// </summary>
        /// <exception cref="System.DivideByZeroException">
        /// Thrown when the normalisation of a zero magnitude vector is attempted
        /// </exception>
        public void Normalize()
        { this = Normal; }

        /// <summary>
        /// Take an interpolated value from between two Vectors or an extrapolated value if allowed
        /// </summary>
        /// <param name="v1">The Vector2 to interpolate from (where control ==0)</param>
        /// <param name="v2">The Vector2 to interpolate to (where control ==1)</param>
        /// <param name="control">The interpolated point between the two vectors to retrieve (fraction between 0 and 1), or an extrapolated point if allowed</param>
        /// <param name="allowExtrapolation">True if the control may represent a point not on the vertex between v1 and v2</param>
        /// <returns>The value at an arbitrary distance (interpolation) between two vectors or an extrapolated point on the extended virtex</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when the control is not between values of 0 and 1 and extrapolation is not allowed
        /// </exception>
        public static Vector2 Interpolate(Vector2 v1, Vector2 v2, double control, bool allowExtrapolation)
        {
            if (!allowExtrapolation && (control>1 || control<0))
            {
                // Error message includes information about the actual value of the argument
                throw new ArgumentOutOfRangeException("control", control,
                    MsgInterpolationRange + "\n" + MsgArgumentValue + control);
            }
            return new Vector2(v1.X*(1-control) + v2.X*control, v1.Y*(1-control) + v2.Y*control);
        }

        /// <summary>
        /// Take an interpolated value from between two Vectors
        /// </summary>
        /// <param name="v1">The Vector2 to interpolate from (where control ==0)</param>
        /// <param name="v2">The Vector2 to interpolate to (where control ==1)</param>
        /// <param name="control">The interpolated point between the two vectors to retrieve (fraction between 0 and 1)</param>
        /// <returns>The value at an arbitrary distance (interpolation) between two vectors</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when the control is not between values of 0 and 1
        /// </exception>
        public static Vector2 Interpolate(Vector2 v1, Vector2 v2, double control)
        { return Interpolate(v1, v2, control, false); }

        /// <summary>
        /// Take an interpolated value from between two Vectors
        /// </summary>
        /// <param name="other">The Vector2 to interpolate to (where control ==1)</param>
        /// <param name="control">The interpolated point between the two vectors to retrieve (fraction between 0 and 1)</param>
        /// <returns>The value at an arbitrary distance (interpolation) between two vectors</returns>
        public Vector2 Interpolate(Vector2 other, double control)
        { return Interpolate(this, other, control); }

        /// <summary>
        /// Take an interpolated value from between two Vectors or an extrapolated value if allowed
        /// </summary>
        /// <param name="other">The Vector2 to interpolate to (where control ==1)</param>
        /// <param name="control">The interpolated point between the two vectors to retrieve (fraction between 0 and 1), or an extrapolated point if allowed</param>
        /// <param name="allowExtrapolation">True if the control may represent a point not on the vertex between v1 and v2</param>
        /// <returns>The value at an arbitrary distance (interpolation) between two vectors or an extrapolated point on the extended virtex</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when the control is not between values of 0 and 1 and extrapolation is not allowed
        /// </exception>
        public Vector2 Interpolate(Vector2 other, double control, bool allowExtrapolation)
        { return Interpolate(this, other, control); }

        /// <summary>
        /// Find the distance between two Vectors
        /// Pythagoras theorem on two Vectors
        /// </summary>
        /// <param name="v1">The Vector2 to find the distance from </param>
        /// <param name="v2">The Vector2 to find the distance to </param>
        /// <returns>The distance between two Vectors</returns>
        public static double Distance(Vector2 v1, Vector2 v2)
        { return SysMath.Sqrt((v1.X-v2.X)*(v1.X-v2.X) + (v1.Y-v2.Y)*(v1.Y-v2.Y)); }

        /// <summary>
        /// Find the distance between two Vectors
        /// Pythagoras theorem on two Vectors
        /// </summary>
        /// <param name="other">The Vector2 to find the distance to </param>
        /// <returns>The distance between two Vectors</returns>
        public double Distance(Vector2 other)
        { return Distance(this, other); }

        /// <summary>
        /// Find the crossing angle between lines passing through two vectors.
        /// </summary>
        /// <param name="v1">The Vector2 to discern the angle from </param>
        /// <param name="v2">The Vector2 to discern the angle to</param>
        /// <returns>The crossing angle.</returns>
        public static double CrossingAngle(Vector2 v1, Vector2 v2)
        {
            var dot = SysMath.Abs(v1.Normal.Dot(v2.Normal));
            var error = dot-1;
            if (error>0)
                dot -= error;
            return SysMath.Acos(dot);
        }

        /// <summary>
        /// Find the crossing angle between lines passing through this Vector2 and another Vector2.
        /// </summary>
        /// <param name="other">The Vector2 to discern the angle to</param>
        /// <returns>The crossing angle.</returns>
        public double CrossingAngle(Vector2 other)
        { return CrossingAngle(this, other); }

        /// <summary>
        /// Find the angle between two Vectors
        /// </summary>
        /// <param name="v1">The Vector2 to discern the angle from </param>
        /// <param name="v2">The Vector2 to discern the angle to</param>
        /// <returns>The angle between two positional Vectors</returns>
        public static double Angle(Vector2 v1, Vector2 v2)
        { return SysMath.Acos(v1.Normal.Dot(v2.Normal)); }

        /// <summary>
        /// Find the angle between this Vector2 and another
        /// </summary>
        /// <param name="other">The Vector2 to discern the angle to</param>
        /// <returns>The angle between two positional Vectors</returns>
        public double Angle(Vector2 other)
        { return Angle(this, other); }

        /// <summary>
        /// Find the angle between two vectors. This will not only give the angle difference, but the direction.
        /// For example, it may give you -1 radian, or 1 radian, depending on the direction. Angle given will be the 
        /// angle from the FromVector to the DestVector, in radians.
        /// </summary>
        /// <param name="srcVector">Vector to start at.</param>
        /// <param name="dstVector">Destination vector.</param>
        /// <returns>Signed angle, in radians</returns>
        public static double RelativeAngle(Vector2 srcVector, Vector2 dstVector)
        {
            var srcAngle = OriginAngle(srcVector);
            var dstAngle = OriginAngle(dstVector);
            var angle = dstAngle-srcAngle;
            while (angle<-SysMath.PI)
                angle += 2.0*SysMath.PI;
            while (angle>SysMath.PI)
                angle -= 2.0*SysMath.PI;
            return angle;
        }

        /// <summary>
        /// Find the angle between two vectors. This will not only give the angle difference, but the direction.
        /// For example, it may give you -1 radian, or 1 radian, depending on the direction. Angle given will be the 
        /// angle from the FromVector to the DestVector, in radians.
        /// </summary>
        /// <param name="srcVector">Vector to start at.</param>
        /// <param name="dstVector">Destination vector.</param>
        /// <returns>Signed angle, in radians</returns>
        public double RelativeAngle(Vector2 dstVector)
        { return RelativeAngle(this, dstVector); }

        /// <summary>
        /// Calculates the signed angle corresponding to specified vector
        /// in range of -PI to PI both inclusive.
        /// </summary>
        /// <returns>Signed angle, in radians.</returns>
        public static double OriginAngle(Vector2 vector)
        {
            vector.Normalize();
            var angleSign = vector.Y>=0 ? 1 : -1;
            var angle = SysMath.Acos(vector.X);
            angle *= angleSign;
            return angle;
        }

        /// <summary>
        /// Calculates the signed angle corresponding to this vector
        /// in range of -PI to PI both inclusive.
        /// </summary>
        /// <returns>Signed angle, in radians.</returns>
        public double OriginAngle()
        { return OriginAngle(this); }

        /// <summary>
        /// Compares the magnitude of two Vectors and returns the greater Vector2
        /// </summary>
        /// <param name="v1">The vector to compare</param>
        /// <param name="v2">The vector to compare with</param>
        /// <returns>
        /// The greater of the two Vectors (based on magnitude)
        /// </returns>
        public static Vector2 Max(Vector2 v1, Vector2 v2)
        { return v1>=v2 ? v1 : v2; }

        /// <summary>
        /// Compares the magnitude of two Vectors and returns the greater Vector2
        /// </summary>
        /// <param name="other">The vector to compare with</param>
        /// <returns>
        /// The greater of the two Vectors (based on magnitude)
        /// </returns>
        public Vector2 Max(Vector2 other)
        { return Max(this, other); }

        /// <summary>
        /// compares the magnitude of two Vectors and returns the lesser Vector2
        /// </summary>
        /// <param name="v1">The vector to compare</param>
        /// <param name="v2">The vector to compare with</param>
        /// <returns>
        /// The lesser of the two Vectors (based on magnitude)
        /// </returns>
        public static Vector2 Min(Vector2 v1, Vector2 v2)
        { return v1<=v2 ? v1 : v2; }

        /// <summary>
        /// Compares the magnitude of two Vectors and returns the greater Vector2
        /// </summary>
        /// <param name="other">The vector to compare with</param>
        /// <returns>
        /// The lesser of the two Vectors (based on magnitude)
        /// </returns>
        public Vector2 Min(Vector2 other)
        { return Min(this, other); }

        /// <summary>
        /// Rotates a Vector2 around the Z axis
        /// </summary>
        /// <param name="v1">The Vector2 to be rotated</param>
        /// <param name="radians">The angle to rotate the Vector2 around in radians</param>
        /// <returns>Vector2 representing the rotation around the Z axis</returns>
        public static Vector2 Rotate(Vector2 v1, double radians)
        {
            var x = v1.X*SysMath.Cos(radians) - v1.Y*SysMath.Sin(radians);
            var y = v1.X*SysMath.Sin(radians) + v1.Y*SysMath.Cos(radians);
            return new Vector2(x, y);
        }

        /// <summary>
        /// Rotates a Vector2 around the origin.
        /// </summary>
        /// <param name="radians">The angle to rotate the Vector2 around in radians</param>
        /// <returns>Vector2 representing the rotation around the Z axis</returns>
        public void Rotate(double radians)
        { this = Rotate(this, radians); }
        
        /// <summary>
        /// Reflect a Vector2 about a given normal
        /// </summary>
        /// <param name="normal">The normal Vector2 to reflect about</param>
        /// <returns>
        /// The reflected Vector2
        /// </returns>
        public Vector2 Reflection(Vector2 normal)
        { return 2*(Dot(this, normal))*normal-this; }

        /// <summary>
        /// Returns Vector2 representing the absolute values of the vector
        /// </summary>
        public static Vector2 Abs(Vector2 v1)
        { return new Vector2(SysMath.Abs(v1.X), SysMath.Abs(v1.Y)); }

        /// <summary>
        /// Returns Vector2 representing the absolute values of the vector
        /// </summary>
        public Vector2 Abs()
        { return Abs(this); }

        public double DistanceToSegment(Vector2 v, Vector2 w)
        {
            // Return minimum distance between line segment vw and this vertex
            var l2 = (v-w).SqrLength; // i.e. |w-v|^2 -  avoid a sqrt
            if (l2==0.0)
                return Distance(v); // v == w case
            // Consider the line extending the segment, parameterized as v + t (w - v).
            // We find projection of point p onto the line. 
            // It falls where t = [(p-v) . (w-v)] / |w-v|^2
            // We clamp t from [0,1] to handle points outside the segment vw.
            var t = SysMath.Max(0, SysMath.Min(1, (this-v).Dot(w-v)/l2));
            Vector2 proj = v+t*(w-v); // Projection falls on the segment
            return Distance(proj);
        }

        private static double Sqr(double a) { return a*a; }

        public double DistanceToLine(Vector2 l1, Vector2 l2)
        {
            return SysMath.Abs((l2.X - l1.X)*(l1.Y - Y) - (l1.X - X)*(l2.Y - l1.Y)) / l1.Distance(l2);
        }

        #endregion

        #region Standard Functions

        /// <summary>
        /// Textual description of the Vector2
        /// </summary>
        /// <returns>Text (String) representing the vector</returns>
        public override string ToString()
        { return ToString(null, null); }

        /// <summary>
        /// Verbose textual description of the Vector2
        /// </summary>
        /// <returns>Text (string) representing the vector</returns>
        public string ToVerbString()
        {
            string output = null;
            if (IsUnitVector())
                output += MsgUnitVector;
            else
                output += MsgPositionalVector;
            output += string.Format("( x={0}, y={1})", X, Y);
            output += MsgMagnitude+Length;
            return output;
        }

        /// <summary>
        /// Textual description of the Vector2
        /// </summary>
        /// <param name="format">Formatting string: 'x','y','z' or '' followed by standard numeric format string characters valid for a double precision floating point</param>
        /// <param name="formatProvider">The culture specific fromatting provider</param>
        /// <returns>Text (String) representing the vector</returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            // If no format is passed
            if (string.IsNullOrEmpty(format))
                return String.Format("({0}, {1})", X, Y);
            char firstChar = format[0];
            string remainder = null;
            if (format.Length>1)
                remainder = format.Substring(1);
            switch (firstChar)
            {
                case 'x': return X.ToString(remainder, formatProvider);
                case 'y': return Y.ToString(remainder, formatProvider);
                default:
                    return String.Format("({0}, {1})",
                        X.ToString(format, formatProvider), Y.ToString(format, formatProvider));
            }
        }

        /// <summary>
        /// Get the hashcode
        /// </summary>
        /// <returns>Hashcode for the object instance</returns>
        public override int GetHashCode()
        { return (int)((X+Y)%Int32.MaxValue); }

        /// <summary>
        /// Comparator
        /// </summary>
        /// <param name="other">The other object (which should be a vector) to compare to</param>
        /// <returns>Truth if two vectors are equal within a tolerence</returns>
        public override bool Equals(object other)
        {
            // Check object other is a Vector2 object
            if (other is Vector2)
            {
                // Convert object to Vector2
                var otherVector = (Vector2)other;
                // Check for equality
                return otherVector==this;
            }
            return false;
        }

        /// <summary>
        /// Comparator
        /// </summary>
        /// <param name="other">The other Vector2 to compare to</param>
        /// <returns>Truth if two vectors are equal within a tolerence</returns>
        public bool Equals(Vector2 other)
        { return other==this; }

        /// <summary>
        /// compares the magnitude of this instance against the magnitude of the supplied vector
        /// </summary>
        /// <param name="other">The vector to compare this instance with</param>
        /// <returns>
        /// -1: The magnitude of this instance is less than the others magnitude
        /// 0: The magnitude of this instance equals the magnitude of the other
        /// 1: The magnitude of this instance is greater than the magnitude of the other
        /// </returns>
        public int CompareTo(Vector2 other)
        {
            if (this<other)
                return -1;
            if (this>other)
                return 1;
            return 0;
        }

        /// <summary>
        /// compares the magnitude of this instance against the magnitude of the supplied vector
        /// </summary>
        /// <param name="other">The vector to compare this instance with</param>
        /// <returns>
        /// -1: The magnitude of this instance is less than the others magnitude
        /// 0: The magnitude of this instance equals the magnitude of the other
        /// 1: The magnitude of this instance is greater than the magnitude of the other
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Throws an exception if the type of object to be compared is not known to this class
        /// </exception>
        public int CompareTo(object other)
        {
            if (other is Vector2)
                return CompareTo((Vector2)other);
            // Error condition: other is not a Vector2 object
            throw new ArgumentException(MsgNonVectorComparison+"\n"+MsgArgumentType+
                other.GetType().ToString(), "other");
        }

        #endregion

        #region Decisions

        /// <summary>
        /// Checks if a vector a unit vector
        /// Checks if the Vector2 has been normalized
        /// Checks if a vector has a magnitude of 1
        /// </summary>
        /// <param name="v1">
        /// The vector to be checked for Normalization
        /// </param>
        /// <returns>Truth if the vector is a unit vector</returns>
        public static bool IsUnitVector(Vector2 v1)
        { return SysMath.Abs(v1.Length-1)<=EqualityTolerence; }

        /// <summary>
        /// Checks if the vector a unit vector
        /// Checks if the Vector2 has been normalized 
        /// Checks if the vector has a magnitude of 1
        /// </summary>
        /// <returns>Truth if this vector is a unit vector</returns>
        public bool IsUnitVector()
        { return IsUnitVector(this); }

        /// <summary>
        /// Checks if a face normal vector represents back face
        /// Checks if a face is visible, given the line of sight
        /// </summary>
        /// <param name="normal">
        /// The vector representing the face normal Vector2
        /// </param>
        /// <param name="lineOfSight">
        /// The unit vector representing the direction of sight from a virtual camera
        /// </param>
        /// <returns>Truth if the vector (as a normal) represents a back face</returns>
        public static bool IsBackFace(Vector2 normal, Vector2 lineOfSight)
        { return normal.Dot(lineOfSight)<0; }

        /// <summary>
        /// Checks if a face normal vector represents back face
        /// Checks if a face is visible, given the line of sight
        /// </summary>
        /// <param name="lineOfSight">
        /// The unit vector representing the direction of sight from a virtual camera
        /// </param>
        /// <returns>Truth if the vector (as a normal) represents a back face</returns>
        public bool IsBackFace(Vector2 lineOfSight)
        { return IsBackFace(this, lineOfSight); }

        /// <summary>
        /// Checks if two Vectors are orthogonal
        /// </summary>
        /// <param name="v1">
        /// The vector to be checked for orthogonality
        /// </param>
        /// <param name="v2">
        /// The vector to be checked for orthogonality to
        /// </param>
        /// <returns>Truth if the two Vectors are orthogonal</returns>
        public static bool IsOrthogonal(Vector2 v1, Vector2 v2)
        { return v1.Dot(v2)==0; }

        /// <summary>
        /// Checks if two Vectors are orthogonal
        /// </summary>
        /// <param name="other">
        /// The vector to be checked for orthogonality
        /// </param>
        /// <returns>Truth if the two Vectors are orthogonal</returns>
        public bool IsOrthogonal(Vector2 other)
        { return IsOrthogonal(this, other); }

        #endregion

        #region Cartesian Vectors

        /// <summary>
        /// Vector2 representing the Cartesian origin
        /// </summary>
        public static readonly Vector2 Origin = new Vector2(0, 0);

        /// <summary>
        /// Vector2 representing the Cartesian XAxis
        /// </summary>
        public static readonly Vector2 UnitX = new Vector2(1, 0);

        /// <summary>
        /// Vector2 representing the Cartesian YAxis
        /// </summary>
        public static readonly Vector2 UnitY = new Vector2(0, 1);

        #endregion

        #region Messages

        /// <summary>
        /// Exception message descriptive text 
        /// Used for a failure for an array argument to have three components when three are needed 
        /// </summary>
        private const string MsgTwoComponents = "Array must contain exactly two components (x, y)";

        /// <summary>
        /// Exception message descriptive text 
        /// Used for a divide by zero event caused by the normalization of a vector with magnitude 0 
        /// </summary>
        private const string MsgNormalize0 = "Can not normalize a vector when it's magnitude is zero";

        /// <summary>
        /// Exception message descriptive text 
        /// Used when interpolation is attempted with a control parameter not between 0 and 1 
        /// </summary>
        private const string MsgInterpolationRange = "Control parameter must be a value in range 0..1";

        /// <summary>
        /// Exception message descriptive text 
        /// Used when attempting to compare a Vector2 to an object which is not a type of Vector2 
        /// </summary>
        private const string MsgNonVectorComparison = "Cannot compare a Vector2 to a non-Vector2";

        /// <summary>
        /// Exception message additional information text 
        /// Used when adding type information of the given argument into an error message 
        /// </summary>
        private const string MsgArgumentType = "The argument provided is a type of ";

        /// <summary>
        /// Exception message additional information text 
        /// Used when adding value information of the given argument into an error message 
        /// </summary>
        private const string MsgArgumentValue = "The argument provided has a value of ";

        /// <summary>
        /// Exception message additional information text 
        /// Used when adding length (number of components in an array) information of the given argument into an error message 
        /// </summary>
        private const string MsgArgumentLength = "The argument provided has a length of ";

        /// <summary>
        /// Exception message descriptive text 
        /// Used when attempting to set a Vectors magnitude to a negative value 
        /// </summary>
        private const string MsgNegativeMagnitude = "The magnitude of a Vector2 must be a positive value";

        /// <summary>
        /// Exception message descriptive text 
        /// Used when attempting to set a Vectors magnitude where the Vector2 represents the origin
        /// </summary>
        private const string MsgOraginVectorMagnitude = "Cannot change the magnitude of Vector2(0, 0)";

        ///////////////////////////////////////////////////////////////////////////////

        private const string MsgUnitVector = "Unit vector composing of ";

        private const string MsgPositionalVector = "Positional vector composing of  ";

        private const string MsgMagnitude = " of magnitude ";

        ///////////////////////////////////////////////////////////////////////////////

        #endregion

        #region Constants

        /// <summary>
        /// The tolerence used when determining the equality of two vectors 
        /// </summary>
        public const double EqualityTolerence = Double.Epsilon;

        /// <summary>
        /// The smallest vector possible (based on the double precision floating point structure)
        /// </summary>
        public static readonly Vector2 MinValue = new Vector2(Double.MinValue, Double.MinValue);

        /// <summary>
        /// The largest vector possible (based on the double precision floating point structure)
        /// </summary>
        public static readonly Vector2 MaxValue = new Vector2(Double.MaxValue, Double.MaxValue);

        /// <summary>
        /// The smallest positive (non-zero) vector possible (based on the double precision floating point structure)
        /// </summary>
        public static readonly Vector2 Epsilon = new Vector2(Double.Epsilon, Double.Epsilon);

        #endregion

    }
}
