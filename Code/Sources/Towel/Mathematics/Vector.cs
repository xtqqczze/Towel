﻿using System;
using Towel.Measurements;

namespace Towel.Mathematics
{
	/// <summary>Represents a vector with an arbitrary number of components of a generic type.</summary>
	/// <typeparam name="T">The numeric type of this Vector.</typeparam>
	[Serializable]
    public class Vector<T>
	{
		internal readonly T[] _vector;

		#region Basic Properties

		/// <summary>Index 0</summary>
		public T X
		{
			get
            {
                if (this.Dimensions < 1)
                {
                    throw new MathematicsException("This vector doesn't have an " + nameof(X) + " component.");
                }
                return _vector[0];
            }
			set
            {
                if (this.Dimensions < 1)
                {
                    throw new MathematicsException("This vector doesn't have an " + nameof(X) + " component.");
                }
                this._vector[0] = value;
            }
		}

		/// <summary>Index 1</summary>
		public T Y
		{
			get
            {
                if (this.Dimensions < 2)
                {
                    throw new MathematicsException("This vector doesn't have an " + nameof(Y) + " component.");
                }
                return _vector[1];
            }
            set
            {
                if (this.Dimensions < 2)
                {
                    throw new MathematicsException("This vector doesn't have an " + nameof(Y) + " component.");
                }
                this._vector[1] = value;
            }
        }

		/// <summary>Index 2</summary>
		public T Z
		{
            get
            {
                if (this.Dimensions < 3)
                {
                    throw new MathematicsException("This vector doesn't have an " + nameof(Z) + " component.");
                }
                return _vector[2];
            }
            set
            {
                if (this.Dimensions < 3)
                {
                    throw new MathematicsException("This vector doesn't have an " + nameof(Z) + " component.");
                }
                this._vector[2] = value;
            }
        }

		/// <summary>The number of components in this vector.</summary>
		public int Dimensions
        {
            get
            {
                return this._vector == null ? 0 : this._vector.Length;
            }
        }

		/// <summary>Allows indexed access to this vector.</summary>
		/// <param name="index">The index to access.</param>
		/// <returns>The value of the given index.</returns>
		public T this[int index]
		{
			get
            {
                if (0 > index || index > Dimensions)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, "!(0 <= " + nameof(index) + " <= " + nameof(Dimensions) + ")");
                }
                return this._vector[index];
            }
			set
            {
                if (0 > index || index > Dimensions)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, "!(0 <= " + nameof(index) + " <= " + nameof(Dimensions) + ")");
                }
                this._vector[index] = value;
            }
		}

        #endregion

        #region Constructors

        /// <summary>Creates a new vector with the given number of components.</summary>
        /// <param name="dimensions">The number of dimensions this vector will have.</param>
        public Vector(int dimensions)
		{
			if (dimensions < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(dimensions), dimensions, "!(" + nameof(dimensions) + " >= 0)");
            }
			this._vector = new T[dimensions];
		}

		/// <summary>Creates a vector out of the given values.</summary>
		/// <param name="vector">The values to initialize the vector to.</param>
		public Vector(params T[] vector)
		{
			this._vector = vector;
		}

        public Vector(int dimensions, Func<int, T> function) : this(dimensions)
        {
            for (int i = 0; i < dimensions; i++)
            {
                this._vector[i] = function(i);
            }
        }

		#endregion

		#region Factories

		/// <summary>Creates a vector with the given number of components with the values initialized to zeroes.</summary>
		/// <param name="dimensions">The number of components in the vector.</param>
		/// <returns>The newly constructed vector.</returns>
		public static Vector<T> FactoryZero(int dimensions)
        {
            return FactoryZeroImplementation(dimensions);
        }

        internal static Func<int, Vector<T>> FactoryZeroImplementation = dimensions =>
        {
            if (Compute.Equal(default(T), Constant<T>.Zero))
            {
                FactoryZeroImplementation = DIMENSIONS => new Vector<T>(DIMENSIONS);
            }
            else
            {
                FactoryZeroImplementation = DIMENSIONS =>
                {
                    T[] vector = new T[DIMENSIONS];
                    vector.Fill(Constant<T>.Zero);
                    return new Vector<T>(vector);
                };
            }
            return FactoryZeroImplementation(dimensions);
        };

        /// <summary>Creates a vector with the given number of components with the values initialized to ones.</summary>
        /// <param name="dimensions">The number of components in the vector.</param>
        /// <returns>The newly constructed vector.</returns>
        public static Vector<T> FactoryOne(int dimensions)
        {
            return FactoryOneImplementation(dimensions);
        }

        internal static Func<int, Vector<T>> FactoryOneImplementation = dimensions =>
        {
            if (Compute.Equal(default(T), Constant<T>.One))
            {
                FactoryZeroImplementation = DIMENSIONS => new Vector<T>(DIMENSIONS);
            }
            else
            {
                FactoryZeroImplementation = DIMENSIONS =>
                {
                    T[] vector = new T[DIMENSIONS];
                    vector.Fill(Constant<T>.One);
                    return new Vector<T>(vector);
                };
            }
            return FactoryZeroImplementation(dimensions);
        };

        #endregion

        #region Mathematics

        #region Magnitude

        public static T GetMagnitude(Vector<T> a)
        {
            if (a == null)
            {
                throw new ArgumentNullException(nameof(a));
            }
            return Compute.SquareRoot(GetMagnitudeSquared(a));
        }

        /// <summary>Computes the length of this vector.</summary>
        /// <returns>The length of this vector.</returns>
        public T Magnitude
        {
            get
            {
                return GetMagnitude(this);
            }
        }

        #endregion

        #region MagnitudeSquared

        public static T GetMagnitudeSquared(Vector<T> a)
        {
            if (a._vector == null)
            {
                throw new ArgumentNullException(nameof(a));
            }
            int Length = a.Dimensions;
            T result = Constant<T>.Zero;
            T[] A = a._vector;
            for (int i = 0; i < Length; i++)
            {
                result = Compute.Add(result, Compute.Multiply(A[i], A[i]));
            }
            return result;
        }

        /// <summary>Computes the length of this vector, but doesn't square root it for 
        /// possible optimization purposes.</summary>
        /// <returns>The squared length of the vector.</returns>
        public T MagnitudeSquared
        {
            get
            {
                return GetMagnitudeSquared(this);
            }
        }

        #endregion

        #region Negate

        public static void Negate(Vector<T> a, ref Vector<T> b)
        {
            if (a is null)
            {
                throw new ArgumentNullException(nameof(a));
            }
            T[] A = a._vector;
            int Length = A.Length;
            T[] B;
            if (b is null || b.Dimensions != Length)
            {
                b = new Vector<T>(Length);
                B = b._vector;
            }
            else
            {
                B = b._vector;
                if (B.Length != Length)
                {
                    b = new Vector<T>(Length);
                    B = b._vector;
                }
            }
            for (int i = 0; i < Length; i++)
            {
                B[i] = Compute.Negate(A[i]);
            }
        }

        /// <summary>Negates all the values in a vector.</summary>
        /// <param name="a">The vector to have its values negated.</param>
        /// <returns>The result of the negations.</returns>
        public static Vector<T> Negate(Vector<T> a)
        {
            Vector<T> b = null;
            Negate(a, ref b);
            return b;
        }

        /// <summary>Negates a vector.</summary>
		/// <param name="vector">The vector to negate.</param>
		/// <returns>The result of the negation.</returns>
		public static Vector<T> operator -(Vector<T> vector)
        {
            return Negate(vector);
        }

        /// <summary>Negates this vector.</summary>
		/// <returns>The result of the negation.</returns>
		public Vector<T> Negate()
        {
            return -this;
        }

        #endregion

        #region Add

        public static void Add(Vector<T> a, Vector<T> b, ref Vector<T> c)
        {
            if (a is null)
            {
                throw new ArgumentNullException(nameof(a));
            }
            if (b is null)
            {
                throw new ArgumentNullException(nameof(b));
            }
            T[] A = a._vector;
            T[] B = b._vector;
            int Length = A.Length;
            if (Length != B.Length)
            {
                throw new MathematicsException("Arguments invalid !(" + nameof(a) + "." + nameof(a.Dimensions) + " == " + nameof(b) + "." + nameof(b.Dimensions) + ")");
            }
            T[] C;
            if (c is null)
            {
                c = new Vector<T>(Length);
                C = c._vector;
            }
            else
            {
                C = c._vector;
                if (C.Length != Length)
                {
                    c = new Vector<T>(Length);
                    C = c._vector;
                }
            }
            for (int i = 0; i < Length; i++)
            {
                C[i] = Compute.Add(A[i], B[i]);
            }
        }

        /// <summary>Adds two vectors together.</summary>
        /// <param name="a">The first vector of the addition.</param>
        /// <param name="b">The second vector of the addiiton.</param>
        /// <returns>The result of the addiion.</returns>
        public static Vector<T> Add(Vector<T> a, Vector<T> b)
        {
            Vector<T> c = null;
            Add(a, b, ref c);
            return c;
        }

        /// <summary>Adds two vectors together.</summary>
        /// <param name="left">The first vector of the addition.</param>
        /// <param name="right">The second vector of the addition.</param>
        /// <returns>The result of the addition.</returns>
        public static Vector<T> operator +(Vector<T> left, Vector<T> right)
        {
            return Add(left, right);
        }

        /// <summary>Adds two vectors together.</summary>
		/// <param name="b">The vector to add to this one.</param>
		/// <returns>The result of the vector.</returns>
		public Vector<T> Add(Vector<T> b)
        {
            return this + b;
        }

        #endregion

        #region Subtract

        public static void Subtract(Vector<T> a, Vector<T> b, ref Vector<T> c)
        {
            if (a is null)
            {
                throw new ArgumentNullException(nameof(a));
            }
            if (b is null)
            {
                throw new ArgumentNullException(nameof(b));
            }
            T[] A = a._vector;
            T[] B = b._vector;
            int Length = A.Length;
            if (Length != B.Length)
            {
                throw new MathematicsException("Arguments invalid !(" + nameof(a) + "." + nameof(a.Dimensions) + " == " + nameof(b) + "." + nameof(b.Dimensions) + ")");
            }
            T[] C;
            if (c is null)
            {
                c = new Vector<T>(Length);
                C = c._vector;
            }
            else
            {
                C = c._vector;
                if (C.Length != Length)
                {
                    c = new Vector<T>(Length);
                    C = c._vector;
                }
            }
            for (int i = 0; i < Length; i++)
            {
                C[i] = Compute.Subtract(A[i], B[i]);
            }
        }

        /// <summary>Subtracts two vectors.</summary>
        /// <param name="a">The left vector of the subtraction.</param>
        /// <param name="b">The right vector of the subtraction.</param>
        /// <returns>The result of the vector subtracton.</returns>
        public static Vector<T> Subtract(Vector<T> a, Vector<T> b)
        {
            Vector<T> c = null;
            Subtract(a, b, ref c);
            return c;
        }

        /// <summary>Subtracts two vectors.</summary>
		/// <param name="a">The left operand of the subtraction.</param>
		/// <param name="b">The right operand of the subtraction.</param>
		/// <returns>The result of the subtraction.</returns>
		public static Vector<T> operator -(Vector<T> a, Vector<T> b)
        {
            return Subtract(a, b);
        }

        /// <summary>Subtracts another vector from this one.</summary>
		/// <param name="b">The vector to subtract from this one.</param>
		/// <returns>The result of the subtraction.</returns>
		public Vector<T> Subtract(Vector<T> b)
        {
            return this - b;
        }

        #endregion

        #region Multiply

        public static void Multiply(Vector<T> a, T b, ref Vector<T> c)
        {
            if (a is null)
            {
                throw new ArgumentNullException(nameof(a));
            }
            T[] A = a._vector;
            int Length = A.Length;
            T[] C;
            if (c is null)
            {
                c = new Vector<T>(Length);
                C = c._vector;
            }
            else
            {
                C = c._vector;
                if (C.Length != Length)
                {
                    c = new Vector<T>(Length);
                    C = c._vector;
                }
            }
            for (int i = 0; i < Length; i++)
            {
                C[i] = Compute.Multiply(A[i], b);
            }
        }

        /// <summary>Subtracts two vectors.</summary>
        /// <param name="a">The left vector of the subtraction.</param>
        /// <param name="b">The right vector of the subtraction.</param>
        /// <returns>The result of the vector subtracton.</returns>
        public static Vector<T> Multiply(Vector<T> a, T b)
        {
            Vector<T> c = null;
            Multiply(a, b, ref c);
            return c;
        }

        /// <summary>Multiplies all the values in a vector by a scalar.</summary>
		/// <param name="a">The vector to have all its values multiplied.</param>
		/// <param name="b">The scalar to multiply all the vector values by.</param>
		/// <returns>The result of the multiplication.</returns>
		public static Vector<T> operator *(Vector<T> a, T b)
        {
            return Multiply(a, b);
        }

        /// <summary>Multiplies all the values in a vector by a scalar.</summary>
		/// <param name="a">The scalar to multiply all the vector values by.</param>
		/// <param name="b">The vector to have all its values multiplied.</param>
		/// <returns>The result of the multiplication.</returns>
		public static Vector<T> operator *(T a, Vector<T> b)
        {
            return Multiply(b, a);
        }

        /// <summary>Multiplies the values in this vector by a scalar.</summary>
		/// <param name="b">The scalar to multiply these values by.</param>
		/// <returns>The result of the multiplications</returns>
		public Vector<T> Multiply(T b)
        {
            return this * b;
        }

        #endregion

        #region Divide

        public static void Divide(Vector<T> a, T b, ref Vector<T> c)
        {
            if (a is null)
            {
                throw new ArgumentNullException(nameof(a));
            }
            T[] A = a._vector;
            int Length = A.Length;
            T[] C;
            if (c is null)
            {
                c = new Vector<T>(Length);
                C = c._vector;
            }
            else
            {
                C = c._vector;
                if (C.Length != Length)
                {
                    c = new Vector<T>(Length);
                    C = c._vector;
                }
            }
            for (int i = 0; i < Length; i++)
            {
                C[i] = Compute.Divide(A[i], b);
            }
        }


        /// <summary>Divides all the components of a vector by a scalar.</summary>
        /// <param name="a">The vector to have the components divided by.</param>
        /// <param name="b">The scalar to divide the vector components by.</param>
        /// <returns>The resulting vector after teh divisions.</returns>
        public static Vector<T> Divide(Vector<T> a, T b)
        {
            Vector<T> c = null;
            Divide(a, b, ref c);
            return c;
        }

        /// <summary>Divides all the values in the vector by a scalar.</summary>
		/// <param name="a">The vector to have its values divided.</param>
		/// <param name="b">The scalar to divide all the vectors values by.</param>
		/// <returns>The vector after the divisions.</returns>
		public static Vector<T> operator /(Vector<T> a, T b)
        {
            return Divide(a, b);
        }

        /// <summary>Divides all the values in this vector by a scalar.</summary>
		/// <param name="b">The scalar to divide the values of the vector by.</param>
		/// <returns>The resulting vector after teh divisions.</returns>
		public Vector<T> Divide(T b)
        {
            return this / b;
        }

        #endregion

        #region DotProduct

        /// <summary>Computes the dot product between two vectors.</summary>
        /// <param name="a">The first vector of the dot product operation.</param>
        /// <param name="b">The second vector of the dot product operation.</param>
        /// <returns>The result of the dot product operation.</returns>
        public static T DotProduct(Vector<T> a, Vector<T> b)
        {
            if (a == null)
            {
                throw new ArgumentNullException(nameof(a));
            }
            if (b == null)
            {
                throw new ArgumentNullException(nameof(b));
            }
            int Length = a.Dimensions;
            if (Length != b.Dimensions)
            {
                throw new MathematicsException("Arguments invalid !(" + nameof(a) + "." + nameof(a.Dimensions) + " == " + nameof(b) + "." + nameof(b.Dimensions) + ")");
            }
            T result = Constant<T>.Zero;
            T[] A = a._vector;
            T[] B = b._vector;
            for (int i = 0; i < Length; i++)
            {
                result = Compute.Add(result, Compute.Multiply(A[i], B[i]));
            }
            return result;
        }

        /// <summary>Computes the dot product between this vector and another.</summary>
		/// <param name="right">The second vector of the dot product operation.</param>
		/// <returns>The result of the dot product.</returns>
		public T DotProduct(Vector<T> right)
        {
            return DotProduct(this, right);
        }

        #endregion

        #region CrossProduct

        public static void CrossProduct(Vector<T> a, Vector<T> b, ref Vector<T> c)
        {
            if (a == null)
            {
                throw new ArgumentNullException(nameof(a));
            }
            if (b == null)
            {
                throw new ArgumentNullException(nameof(b));
            }
            T[] A = a._vector;
            T[] B = b._vector;
            if (A.Length != 3)
            {
                throw new MathematicsException("Arguments invalid !(" + nameof(a) + "." + nameof(a.Dimensions) + " == 3)");
            }
            if (B.Length != 3)
            {
                throw new MathematicsException("Arguments invalid !(" + nameof(b) + "." + nameof(b.Dimensions) + " == 3)");
            }
            if (c == null || c.Dimensions != 3)
            {
                c = new Vector<T>(3);
            }
            T[] C = c._vector;
            C[0] = Compute.Subtract(Compute.Multiply(A[1], B[2]), Compute.Multiply(A[2], B[1]));
            C[1] = Compute.Subtract(Compute.Multiply(A[2], B[0]), Compute.Multiply(A[0], B[2]));
            C[2] = Compute.Subtract(Compute.Multiply(A[0], B[1]), Compute.Multiply(A[1], B[0]));
        }

        /// <summary>Computes teh cross product of two vectors.</summary>
        /// <param name="a">The first vector of the cross product operation.</param>
        /// <param name="b">The second vector of the cross product operation.</param>
        /// <returns>The result of the cross product operation.</returns>
        public static Vector<T> CrossProduct(Vector<T> a, Vector<T> b)
        {
            Vector<T> c = null;
            CrossProduct(a, b, ref c);
            return c;
        }

        /// <summary>Computes the cross product between this vector and another.</summary>
		/// <param name="right">The second vector of the dot product operation.</param>
		/// <returns>The result of the dot product operation.</returns>
		public Vector<T> CrossProduct(Vector<T> right)
        {
            return CrossProduct(this, right);
        }

        #endregion

        #region Normalize

        public static void Normalize(Vector<T> a, ref Vector<T> b)
        {
            if (a == null)
            {
                throw new ArgumentNullException(nameof(a));
            }
            int Dimensions = a.Dimensions;
            if (Dimensions < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(a), a, "!(" + nameof(a) + "." + nameof(a.Dimensions) + " > 0)");
            }
            T magnitude = a.Magnitude;
            if (Compute.Equal(magnitude, Constant<T>.Zero))
            {
                throw new ArgumentOutOfRangeException(nameof(a), a, "!(" + nameof(a) + "." + nameof(a.Magnitude) + " > 0)");
            }
            if (b == null ||
                b.Dimensions != Dimensions)
            {
                b = new Vector<T>(Dimensions);
            }
            T[] B = b._vector;
            for (int i = 0; i < Dimensions; i++)
            {
                b[i] = Compute.Divide(a[i], magnitude);
            }
        }

        /// <summary>Normalizes a vector.</summary>
        /// <param name="a">The vector to normalize.</param>
        /// <returns>The result of the normalization.</returns>
        public static Vector<T> Normalize(Vector<T> a)
        {
            Vector<T> b = null;
            Normalize(a, ref b);
            return b;
        }

        /// <summary>Normalizes this vector.</summary>
		/// <returns>The result of the normalization.</returns>
		public Vector<T> Normalize()
        {
            return Normalize(this);
        }

        #endregion

        #region Angle

        /// <summary>Computes the angle between two vectors.</summary>
        /// <param name="a">The first vector to determine the angle between.</param>
        /// <param name="b">The second vector to determine the angle between.</param>
        /// <returns>The angle between the two vectors in radians.</returns>
        public static Angle<T> Angle(Vector<T> a, Vector<T> b)
        {
            if (a == null)
            {
                throw new ArgumentNullException(nameof(a));
            }
            if (b == null)
            {
                throw new ArgumentNullException(nameof(b));
            }
            throw new NotImplementedException();
        }

        #endregion

        #region RotateBy

        /// <summary>Rotates a vector by the specified axis and rotation values.</summary>
        /// <param name="vector">The vector to rotate.</param>
        /// <param name="angle">The angle of the rotation.</param>
        /// <param name="x">The x component of the axis vector to rotate about.</param>
        /// <param name="y">The y component of the axis vector to rotate about.</param>
        /// <param name="z">The z component of the axis vector to rotate about.</param>
        /// <returns>The result of the rotation.</returns>
        public static Vector<T> RotateBy(Vector<T> vector, Angle<T> angle, T x, T y, T z)
        {
            throw new NotImplementedException();
        }

        /// <summary>Rotates this vector by quaternon values.</summary>
		/// <param name="angle">The amount of rotation about the axis.</param>
		/// <param name="x">The x component deterniming the axis of rotation.</param>
		/// <param name="y">The y component determining the axis of rotation.</param>
		/// <param name="z">The z component determining the axis of rotation.</param>
		/// <returns>The resulting vector after the rotation.</returns>
		public Vector<T> RotateBy(Angle<T> angle, T x, T y, T z)
        {
            return RotateBy(this, angle, x, y, z);
        }

        /// <summary>Rotates a vector by a quaternion.</summary>
        /// <param name="vector">The vector to rotate.</param>
        /// <param name="rotation">The quaternion to rotate the 3-component vector by.</param>
        public static void RotateBy(Vector<T> a, Quaternion<T> b, ref Vector<T> c)
        {
            Quaternion<T>.Quaternion_Rotate(b, a, ref c);
        }

        /// <summary>Rotates a vector by a quaternion.</summary>
        /// <param name="vector">The vector to rotate.</param>
        /// <param name="rotation">The quaternion to rotate the 3-component vector by.</param>
        /// <returns>The result of the rotation.</returns>
        public static Vector<T> RotateBy(Vector<T> a, Quaternion<T> b)
        {
            Vector<T> c = null;
            Quaternion<T>.Quaternion_Rotate(b, a, ref c);
            return c;
        }

        /// <summary>Rotates a vector by a quaternion.</summary>
		/// <param name="b">The quaternion to rotate the 3-component vector by.</param>
		/// <returns>The result of the rotation.</returns>
		public Vector<T> RotateBy(Quaternion<T> b)
        {
            return RotateBy(this, b);
        }

        #endregion

        #region LinearInterpolation

        /// <summary>Computes the linear interpolation between two vectors.</summary>
		/// <param name="a">The starting vector of the interpolation.</param>
		/// <param name="b">The ending vector of the interpolation.</param>
		/// <param name="blend">The ratio 0.0 to 1.0 of the interpolation between the start and end.</param>
		/// <returns>The result of the interpolation.</returns>
		public static void LinearInterpolation(Vector<T> a, Vector<T> b, T blend, ref Vector<T> c)
        {
            if (Compute.LessThan(blend, Constant<T>.Zero) || Compute.GreaterThan(blend, Constant<T>.One))
            {
                throw new ArgumentOutOfRangeException(nameof(blend), blend, "!(0 <= " + nameof(blend) + " <= 1)");
            }
            int Length = a.Dimensions;
            if (Length != b.Dimensions)
            {
                throw new MathematicsException("Arguments invalid !(" + nameof(a) + "." + nameof(a.Dimensions) + " ==" + nameof(b) + "." + nameof(b.Dimensions) + ")");
            }
            if (c == null || c.Dimensions != Length)
            {
                c = new Vector<T>(Length);
            }
            T[] A = a._vector;
            T[] B = b._vector;
            T[] C = c._vector;
            for (int i = 0; i < Length; i++)
            {
                C[i] = Compute.Add(A[i], Compute.Multiply(blend, Compute.Subtract(B[i], A[i])));
            }
        }

        /// <summary>Computes the linear interpolation between two vectors.</summary>
        /// <param name="a">The starting vector of the interpolation.</param>
        /// <param name="b">The ending vector of the interpolation.</param>
        /// <param name="blend">The ratio 0.0 to 1.0 of the interpolation between the start and end.</param>
        /// <returns>The result of the interpolation.</returns>
        public static Vector<T> LinearInterpolation(Vector<T> a, Vector<T> b, T blend)
        {
            Vector<T> c = null;
            LinearInterpolation(a, b, blend, ref c);
            return c;
        }

        /// <summary>Computes the linear interpolation between two vectors.</summary>
		/// <param name="b">The ending vector of the interpolation.</param>
		/// <param name="blend">The ratio 0.0 to 1.0 of the interpolation between the start and end.</param>
		/// <returns>The result of the interpolation.</returns>
		public Vector<T> LinearInterpolation(Vector<T> b, T blend)
        {
            return LinearInterpolation(this, b, blend);
        }

        #endregion

        #region SphericalInterpolation

        /// <summary>Spherically interpolates between two vectors.</summary>
        /// <param name="a">The starting vector of the interpolation.</param>
        /// <param name="b">The ending vector of the interpolation.</param>
        /// <param name="blend">The ratio 0.0 to 1.0 defining the interpolation distance between the two vectors.</param>
        public static Vector<T> SphericalInterpolation(Vector<T> a, Vector<T> b, T blend, ref Vector<T> c)
        {
            throw new NotImplementedException();
        }

        /// <summary>Spherically interpolates between two vectors.</summary>
        /// <param name="a">The starting vector of the interpolation.</param>
        /// <param name="b">The ending vector of the interpolation.</param>
        /// <param name="blend">The ratio 0.0 to 1.0 defining the interpolation distance between the two vectors.</param>
        /// <returns>The result of the slerp operation.</returns>
        public static Vector<T> SphericalInterpolation(Vector<T> a, Vector<T> b, T blend)
        {
            Vector<T> c = null;
            SphericalInterpolation(a, b, blend, ref c);
            return c;
        }

        /// <summary>Sphereically interpolates between two vectors.</summary>
		/// <param name="b">The ending vector of the interpolation.</param>
		/// <param name="blend">The ratio 0.0 to 1.0 defining the interpolation distance between the two vectors.</param>
		/// <returns>The result of the slerp operation.</returns>
		public Vector<T> SphericalInterpolation(Vector<T> b, T blend)
        {
            return SphericalInterpolation(this, b, blend);
        }

        #endregion

        #region BarycentricInterpolation

        /// <summary>Interpolates between three vectors using barycentric coordinates.</summary>
        /// <param name="a">The first vector of the interpolation.</param>
        /// <param name="b">The second vector of the interpolation.</param>
        /// <param name="c">The thrid vector of the interpolation.</param>
        /// <param name="u">The "U" value of the barycentric interpolation equation.</param>
        /// <param name="v">The "V" value of the barycentric interpolation equation.</param>
        public static void BarycentricInterpolation(Vector<T> a, Vector<T> b, Vector<T> c, T u, T v, ref Vector<T> d)
        {
            if (a is null)
            {
                throw new ArgumentNullException(nameof(a));
            }
            if (b is null)
            {
                throw new ArgumentNullException(nameof(b));
            }
            if (c is null)
            {
                throw new ArgumentNullException(nameof(c));
            }
            if (Compute.Equal(a.Dimensions, b.Dimensions, c.Dimensions))
            {
                throw new MathematicsException("Arguments invalid !(" + 
                    nameof(a) + "." + nameof(a.Dimensions) + " == " +
                    nameof(b) + "." + nameof(b.Dimensions) + " == " +
                    nameof(c) + "." + nameof(c.Dimensions) + ")");
            }

            // Note: needs optimization (call the "ref" methods)
            d = a + (u * (b - a)) + (v * (c - a));
        }

        /// <summary>Interpolates between three vectors using barycentric coordinates.</summary>
        /// <param name="a">The first vector of the interpolation.</param>
        /// <param name="b">The second vector of the interpolation.</param>
        /// <param name="c">The thrid vector of the interpolation.</param>
        /// <param name="u">The "U" value of the barycentric interpolation equation.</param>
        /// <param name="v">The "V" value of the barycentric interpolation equation.</param>
        /// <returns>The resulting vector of the barycentric interpolation.</returns>
        public static Vector<T> BarycentricInterpolation(Vector<T> a, Vector<T> b, Vector<T> c, T u, T v)
        {
            Vector<T> d = null;
            BarycentricInterpolation(a._vector, b._vector, c._vector, u, v, ref d);
            return d;
        }

        /// <summary>Interpolates between three vectors using barycentric coordinates.</summary>
        /// <param name="a">The first vector of the interpolation.</param>
        /// <param name="b">The second vector of the interpolation.</param>
        /// <param name="c">The thrid vector of the interpolation.</param>
        /// <param name="u">The "U" value of the barycentric interpolation equation.</param>
        /// <param name="v">The "V" value of the barycentric interpolation equation.</param>
        /// <returns>The resulting vector of the barycentric interpolation.</returns>
        public Vector<T> BarycentricInterpolation(Vector<T> b, Vector<T> c, T u, T v)
        {
            return BarycentricInterpolation(this, b._vector, c._vector, u, v);
        }

        #endregion

        #region Equal

        /// <summary>Does a value equality check.</summary>
        /// <param name="a">The first vector to check for equality.</param>
        /// <param name="b">The second vector	to check for equality.</param>
        /// <returns>True if values are equal, false if not.</returns>
        public static bool Equal(Vector<T> a, Vector<T> b)
        {
            if (object.ReferenceEquals(a, b))
            {
                return true;
            }
            else if (object.ReferenceEquals(a, null))
            {
                return false;
            }
            else if (object.ReferenceEquals(b, null))
            {
                return false;
            }
            else
            {
                int Length = a.Dimensions;
                if (Length != b.Dimensions)
                    return false;
                T[] A = a._vector;
                T[] B = b._vector;
                for (int i = 0; i < Length; i++)
                    if (Compute.Equal(A[i], B[i]))
                        return false;
                return true;
            }
        }

        /// <summary>Does an equality check by value. (warning for float errors)</summary>
		/// <param name="a">The first vector of the equality check.</param>
		/// <param name="b">The second vector of the equality check.</param>
		/// <returns>true if the values are equal, false if not.</returns>
		public static bool operator ==(Vector<T> a, Vector<T> b)
        {
            return Equal(a, b);
        }

        /// <summary>Does an anti-equality check by value. (warning for float errors)</summary>
		/// <param name="a">The first vector of the anit-equality check.</param>
		/// <param name="b">The second vector of the anti-equality check.</param>
		/// <returns>true if the values are not equal, false if they are.</returns>
		public static bool operator !=(Vector<T> a, Vector<T> b)
        {
            return !Equal(a, b);
        }

        /// <summary>Check for equality by value.</summary>
		/// <param name="b">The other vector of the equality check.</param>
		/// <returns>true if the values were equal, false if not.</returns>
		public bool Equal(Vector<T> b)
        {
            return this == b;
        }

        /// <summary>Does a value equality check with leniency.</summary>
        /// <param name="a">The first vector to check for equality.</param>
        /// <param name="b">The second vector to check for equality.</param>
        /// <param name="leniency">How much the values can vary but still be considered equal.</param>
        /// <returns>True if values are equal, false if not.</returns>
        public static bool Equal(Vector<T> a, Vector<T> b, T leniency)
        {
            if (a == null && b == null)
                return true;
            if (a == null)
                return false;
            if (b == null)
                return false;
            int Length = a.Dimensions;
            if (Length != b.Dimensions)
                return false;
            T[] A = a._vector;
            T[] B = b._vector;
            for (int i = 0; i < Length; i++)
                if (Compute.EqualLeniency(A[i], B[i], leniency))
                    return false;
            return true;
        }

        /// <summary>Checks for equality by value with some leniency.</summary>
		/// <param name="right">The other vector of the equality check.</param>
		/// <param name="leniency">The ammount the values can differ but still be considered equal.</param>
		/// <returns>true if the values were cinsidered equal, false if not.</returns>
		public bool EqualsValue(Vector<T> right, T leniency)
        {
            return Equal(this, right, leniency);
        }

        #endregion

        #endregion

        #region Casting Operators

        /// <summary>Implicit conversions from Vector to T[].</summary>
        /// <param name="vector">The Vector to be converted to a T[].</param>
        /// <returns>The T[] of the vector.</returns>
        public static implicit operator T[](Vector<T> vector)
		{
            return vector._vector;
        }

		/// <summary>Implicit conversions from Vector to T[].</summary>
		/// <param name="array">The Vector to be converted to a T[].</param>
		/// <returns>The T[] of the vector.</returns>
		public static implicit operator Vector<T>(T[] array)
		{
            return new Vector<T>(array);
        }

		/// <summary>Converts a vector into a matrix.</summary>
		/// <param name="vector">The vector to convert.</param>
		/// <returns>The resulting matrix.</returns>
		public static explicit operator Matrix<T>(Vector<T> vector)
		{
            return new Matrix<T>(vector);
        }

		/// <summary>Implicitly converts a scalar into a one dimensional vector.</summary>
		/// <param name="scalar">The scalar value.</param>
		/// <returns>The one dimensional vector </returns>
		public static explicit operator Vector<T>(T scalar)
		{
            return new Vector<T>(scalar);
        }

		#endregion

		#region Steppers

		/// <summary>Invokes a delegate for each entry in the data structure.</summary>
		/// <param name="step_function">The delegate to invoke on each item in the structure.</param>
		public void Stepper(Step<T> step_function)
		{
			for (int i = 0; i < this._vector.Length; i++)
				step_function(this._vector[i]);
		}

		/// <summary>Invokes a delegate for each entry in the data structure.</summary>
		/// <param name="step_function">The delegate to invoke on each item in the structure.</param>
		public void Stepper(StepRef<T> step_function)
		{
			for (int i = 0; i < this._vector.Length; i++)
				step_function(ref this._vector[i]);
		}

		/// <summary>Invokes a delegate for each entry in the data structure.</summary>
		/// <param name="step_function">The delegate to invoke on each item in the structure.</param>
		/// <returns>The resulting status of the iteration.</returns>
		public StepStatus Stepper(StepBreak<T> step_function)
		{
			for (int i = 0; i < this._vector.Length; i++)
				if (step_function(this._vector[i]) == StepStatus.Break)
					return StepStatus.Break;
			return StepStatus.Continue;
		}

		/// <summary>Invokes a delegate for each entry in the data structure.</summary>
		/// <param name="step_function">The delegate to invoke on each item in the structure.</param>
		/// <returns>The resulting status of the iteration.</returns>
		public StepStatus Stepper(StepRefBreak<T> step_function)
		{
			for (int i = 0; i < this._vector.Length; i++)
				if (step_function(ref this._vector[i]) == StepStatus.Break)
					return StepStatus.Break;
			return StepStatus.Continue;
		}

		#endregion

		#region Overrides

		/// <summary>Computes a hash code from the values of this matrix.</summary>
		/// <returns>A hash code for the matrix.</returns>
		public override int GetHashCode()
		{
			int hash = this._vector[0].GetHashCode();
            for (int i = 1; i < this._vector.Length; i++)
            {
                hash ^= this._vector[i].GetHashCode();
            }
			return hash;
		}

		/// <summary>Does an equality check by reference.</summary>
		/// <param name="right">The object to compare to.</param>
		/// <returns>True if the references are equal, false if not.</returns>
		public override bool Equals(object right)
		{
            if (!(right is Vector<T>))
            {
                return false;
            }
            else
            {
                return Equal(this, (Vector<T>)right);
            }
		}

		#endregion
	}
}
