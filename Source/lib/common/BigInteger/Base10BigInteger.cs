using System;
using System.Text;


namespace BigIntegerLibrary
{

    /// <summary>
    /// Integer inefficiently represented internally using base-10 digits, in order to allow a
    /// visual representation as a base-10 string. Only for internal use.
    /// </summary>
    sealed class Base10BigInteger
    {

        #region Fields

        /// <summary>
        /// 10 numeration base for string representation, very inefficient for computations.
        /// </summary>
        private const long NumberBase = 10;

        /// <summary>
        /// Maximum size for numbers is up to 10240 binary digits or approximately (safe to use) 3000 decimal digits.
        /// The maximum size is, in fact, double the previously specified amount, in order to accommodate operations'
        /// overflow.
        /// </summary>
        private const int MaxSize = BigInteger.MaxSize * 5;


        /// Integer constants
        private static readonly Base10BigInteger Zero = new Base10BigInteger();
        private static readonly Base10BigInteger One = new Base10BigInteger(1);


        /// <summary>
        /// The array of digits of the number.
        /// </summary>
        private DigitContainer digits;

        /// <summary>
        /// The actual number of digits of the number.
        /// </summary>
        private int size;

        /// <summary>
        /// The number sign.
        /// </summary>
        private Sign sign;


        #endregion


        #region Internal Fields


        /// <summary>
        /// Sets the number sign.
        /// </summary>
        internal Sign NumberSign
        {
            set { sign = value; }
        }


        #endregion


        #region Constructors


        /// <summary>
        /// Default constructor, intializing the Base10BigInteger with zero.
        /// </summary>
        public Base10BigInteger()
        {
            digits = new DigitContainer();
            size = 1;
            digits[size] = 0;
            sign = Sign.Positive;
        }

        /// <summary>
        /// Constructor creating a new Base10BigInteger as a conversion of a regular base-10 long.
        /// </summary>
        /// <param name="n">The base-10 long to be converted</param>
        public Base10BigInteger(long n)
        {
            digits = new DigitContainer();
            sign = Sign.Positive;

            if (n == 0)
            {
                size = 1;
                digits[size] = 0;
            }

            else
            {
                if (n < 0)
                {
                    n = -n;
                    sign = Sign.Negative;
                }

                size = 0;
                while (n > 0)
                {
                    digits[size] = n % NumberBase;
                    n /= NumberBase;
                    size++;
                }
            }
        }

        /// <summary>
        /// Constructor creating a new Base10BigInteger as a copy of an existing Base10BigInteger.
        /// </summary>
        /// <param name="n">The Base10BigInteger to be copied</param>
        public Base10BigInteger(Base10BigInteger n)
        {
            digits = new DigitContainer();
            size = n.size;
            sign = n.sign;

            for (int i = 0; i < n.size; i++)
                digits[i] = n.digits[i];
        }


#endregion


        #region Public Methods


        /// <summary>
        /// Determines whether the specified Base10BigInteger is equal to the current Base10BigInteger.
        /// </summary>
        /// <param name="other">The Base10BigInteger to compare with the current Base10BigInteger</param>
        /// <returns>True if the specified Base10BigInteger is equal to the current Base10BigInteger,
        /// false otherwise</returns>
        public bool Equals(Base10BigInteger other)
        {
            if (sign != other.sign)
                return false;
            if (size != other.size)
                return false;

            for (int i = 0; i < size; i++)
                if (digits[i] != other.digits[i])
                    return false;

            return true;
        }

        /// <summary>
        /// Determines whether the specified System.Object is equal to the current Base10BigInteger.
        /// </summary>
        /// <param name="o">The System.Object to compare with the current Base10BigInteger</param>
        /// <returns>True if the specified System.Object is equal to the current Base10BigInteger,
        /// false otherwise</returns>
        public override bool Equals(object o)
        {
            if ((o is Base10BigInteger) == false)
                return false;

            return Equals((Base10BigInteger)o);
        }

        /// <summary>
        /// Serves as a hash function for the Base10BigInteger type.
        /// </summary>
        /// <returns>A hash code for the current Base10BigInteger</returns>
        public override int GetHashCode()
        {
            int result = 0;

            for (int i = 0; i < size; i++)
                result = result + (int)digits[i];

            return result;
        }

        /// <summary>
        /// String representation of the current Base10BigInteger, converted to its base-10 representation.
        /// </summary>
        /// <returns>The string representation of the current Base10BigInteger</returns>
        public override string ToString()
        {
            StringBuilder output;

            if (sign == Sign.Negative)
            {
                output = new StringBuilder(size + 1);
                output.Append('-');
            }

            else
                output = new StringBuilder(size);

            for (int i = size - 1; i >= 0; i--)
                output.Append(digits[i]);

            return output.ToString();
        }

        /// <summary>
        /// Base10BigInteger inverse with respect to addition.
        /// </summary>
        /// <param name="n">The Base10BigInteger whose opposite is to be computed</param>
        /// <returns>The Base10BigInteger inverse with respect to addition</returns>
        public static Base10BigInteger Opposite(Base10BigInteger n)
        {
            Base10BigInteger res = new Base10BigInteger(n);

            if (res != Zero)
            {
                if (res.sign == Sign.Positive)
                    res.sign = Sign.Negative;
                else
                    res.sign = Sign.Positive;
            }

            return res;
        }

        /// <summary>
        /// Greater test between two Base10BigIntegers.
        /// </summary>
        /// <param name="a">The 1st Base10BigInteger</param>
        /// <param name="b">The 2nd Base10BigInteger</param>
        /// <returns>True if a &gt; b, false otherwise</returns>
        public static bool Greater(Base10BigInteger a, Base10BigInteger b)
        {
            if (a.sign != b.sign)
            {
                if ((a.sign == Sign.Negative) && (b.sign == Sign.Positive))
                    return false;

                if ((a.sign == Sign.Positive) && (b.sign == Sign.Negative))
                    return true;
            }

            else
            {
                if (a.sign == Sign.Positive)
                {
                    if (a.size > b.size)
                        return true;
                    if (a.size < b.size)
                        return false;
                    for (int i = (a.size) - 1; i >= 0; i--)
                        if (a.digits[i] > b.digits[i])
                            return true;
                        else if (a.digits[i] < b.digits[i])
                            return false;
                }

                else
                {
                    if (a.size < b.size)
                        return true;
                    if (a.size > b.size)
                        return false;
                    for (int i = (a.size) - 1; i >= 0; i--)
                        if (a.digits[i] < b.digits[i])
                            return true;
                        else if (a.digits[i] > b.digits[i])
                            return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Greater or equal test between two Base10BigIntegers.
        /// </summary>
        /// <param name="a">The 1st Base10BigInteger</param>
        /// <param name="b">The 2nd Base10BigInteger</param>
        /// <returns>True if a &gt;= b, false otherwise</returns>
        public static bool GreaterOrEqual(Base10BigInteger a, Base10BigInteger b)
        {
            return Greater(a, b) || Equals(a, b);
        }

        /// <summary>
        /// Smaller test between two Base10BigIntegers.
        /// </summary>
        /// <param name="a">The 1st Base10BigInteger</param>
        /// <param name="b">The 2nd Base10BigInteger</param>
        /// <returns>True if a &lt; b, false otherwise</returns>
        public static bool Smaller(Base10BigInteger a, Base10BigInteger b)
        {
            return !GreaterOrEqual(a, b);
        }

        /// <summary>
        /// Smaller or equal test between two Base10BigIntegers.
        /// </summary>
        /// <param name="a">The 1st Base10BigInteger</param>
        /// <param name="b">The 2nd Base10BigInteger</param>
        /// <returns>True if a &lt;= b, false otherwise</returns>
        public static bool SmallerOrEqual(Base10BigInteger a, Base10BigInteger b)
        {
            return !Greater(a, b);
        }

        /// <summary>
        /// Computes the absolute value of a Base10BigInteger.
        /// </summary>
        /// <param name="n">The Base10BigInteger whose absolute value is to be computed</param>
        /// <returns>The absolute value of the given BigInteger</returns>
        public static Base10BigInteger Abs(Base10BigInteger n)
        {
            Base10BigInteger res = new Base10BigInteger(n);
            res.sign = Sign.Positive;
            return res;
        }

        /// <summary>
        /// Addition operation of two Base10BigIntegers.
        /// </summary>
        /// <param name="a">The 1st Base10BigInteger</param>
        /// <param name="b">The 2nd Base10BigInteger</param>
        /// <returns>The Base10BigInteger result of the addition</returns>
        public static Base10BigInteger Addition(Base10BigInteger a, Base10BigInteger b)
        {
            Base10BigInteger res = null;

            if ((a.sign == Sign.Positive) && (b.sign == Sign.Positive))
            {
                if (a >= b)
                    res = Add(a, b);
                else
                    res = Add(b, a);

                res.sign = Sign.Positive;
            }

            if ((a.sign == Sign.Negative) && (b.sign == Sign.Negative))
            {
                if (a <= b)
                    res = Add(-a, -b);
                else
                    res = Add(-b, -a);

                res.sign = Sign.Negative;
            }

            if ((a.sign == Sign.Positive) && (b.sign == Sign.Negative))
            {
                if (a >= (-b))
                {
                    res = Subtract(a, -b);
                    res.sign = Sign.Positive;
                }
                else
                {
                    res = Subtract(-b, a);
                    res.sign = Sign.Negative;
                }
            }

            if ((a.sign == Sign.Negative) && (b.sign == Sign.Positive))
            {
                if ((-a) <= b)
                {
                    res = Subtract(b, -a);
                    res.sign = Sign.Positive;
                }
                else
                {
                    res = Subtract(-a, b);
                    res.sign = Sign.Negative;
                }
            }

            return res;
        }

        /// <summary>
        /// Subtraction operation of two Base10BigIntegers.
        /// </summary>
        /// <param name="a">The 1st Base10BigInteger</param>
        /// <param name="b">The 2nd Base10BigInteger</param>
        /// <returns>The Base10BigInteger result of the subtraction</returns>
        public static Base10BigInteger Subtraction(Base10BigInteger a, Base10BigInteger b)
        {
            Base10BigInteger res = null;

            if ((a.sign == Sign.Positive) && (b.sign == Sign.Positive))
            {
                if (a >= b)
                {
                    res = Subtract(a, b);
                    res.sign = Sign.Positive;
                }
                else
                {
                    res = Subtract(b, a);
                    res.sign = Sign.Negative;
                }
            }

            if ((a.sign == Sign.Negative) && (b.sign == Sign.Negative))
            {
                if (a <= b)
                {
                    res = Subtract(-a, -b);
                    res.sign = Sign.Negative;
                }
                else
                {
                    res = Subtract(-b, -a);
                    res.sign = Sign.Positive;
                }
            }

            if ((a.sign == Sign.Positive) && (b.sign == Sign.Negative))
            {
                if (a >= (-b))
                    res = Add(a, -b);
                else
                    res = Add(-b, a);

                res.sign = Sign.Positive;
            }

            if ((a.sign == Sign.Negative) && (b.sign == Sign.Positive))
            {
                if ((-a) >= b)
                    res = Add(-a, b);
                else
                    res = Add(b, -a);

                res.sign = Sign.Negative;
            }

            return res;
        }

        /// <summary>
        /// Multiplication operation of two Base10BigIntegers.
        /// </summary>
        /// <param name="a">The 1st Base10BigInteger</param>
        /// <param name="b">The 2nd Base10BigInteger</param>
        /// <returns>The Base10BigInteger result of the multiplication</returns>
        public static Base10BigInteger Multiplication(Base10BigInteger a, Base10BigInteger b)
        {
            if ((a == Zero) || (b == Zero))
                return Zero;

            Base10BigInteger res = Multiply(Abs(a), Abs(b));
            if (a.sign == b.sign)
                res.sign = Sign.Positive;
            else
                res.sign = Sign.Negative;

            return res;
        }


        #endregion


        #region Overloaded Operators


        /// <summary>
        /// Implicit conversion operator from long to Base10BigInteger.
        /// </summary>
        /// <param name="n">The long to be converted to a Base10BigInteger</param>
        /// <returns>The Base10BigInteger converted from the given long</returns>
        public static implicit operator Base10BigInteger(long n)
        {
            return new Base10BigInteger(n);
        }

        /// <summary>
        /// Equality test between two Base10BigIntegers.
        /// </summary>
        /// <param name="a">The 1st Base10BigInteger</param>
        /// <param name="b">The 2nd Base10BigInteger</param>
        /// <returns>True if a == b, false otherwise</returns>
        public static bool operator==(Base10BigInteger a, Base10BigInteger b)
        {
            return Equals(a, b);
        }

        /// <summary>
        /// Inequality test between two Base10BigIntegers.
        /// </summary>
        /// <param name="a">The 1st Base10BigInteger</param>
        /// <param name="b">The 2nd Base10BigInteger</param>
        /// <returns>True if a != b, false otherwise</returns>
        public static bool operator!=(Base10BigInteger a, Base10BigInteger b)
        {
            return !Equals(a, b);
        }

        /// <summary>
        /// Greater test between two Base10BigIntegers.
        /// </summary>
        /// <param name="a">The 1st Base10BigInteger</param>
        /// <param name="b">The 2nd Base10BigInteger</param>
        /// <returns>True if a &gt; b, false otherwise</returns>
        public static bool operator>(Base10BigInteger a, Base10BigInteger b)
        {
            return Greater(a, b);
        }

        /// <summary>
        /// Smaller test between two Base10BigIntegers.
        /// </summary>
        /// <param name="a">The 1st Base10BigInteger</param>
        /// <param name="b">The 2nd Base10BigInteger</param>
        /// <returns>True if a &lt; b, false otherwise</returns>
        public static bool operator<(Base10BigInteger a, Base10BigInteger b)
        {
            return Smaller(a, b);
        }

        /// <summary>
        /// Greater or equal test between two Base10BigIntegers.
        /// </summary>
        /// <param name="a">The 1st Base10BigInteger</param>
        /// <param name="b">The 2nd Base10BigInteger</param>
        /// <returns>True if a &gt;= b, false otherwise</returns>
        public static bool operator>=(Base10BigInteger a, Base10BigInteger b)
        {
            return GreaterOrEqual(a, b);
        }

        /// <summary>
        /// Smaller or equal test between two Base10BigIntegers.
        /// </summary>
        /// <param name="a">The 1st Base10BigInteger</param>
        /// <param name="b">The 2nd Base10BigInteger</param>
        /// <returns>True if a &lt;= b, false otherwise</returns>
        public static bool operator<=(Base10BigInteger a, Base10BigInteger b)
        {
            return SmallerOrEqual(a, b);
        }

        /// <summary>
        /// Base10BigInteger inverse with respect to addition.
        /// </summary>
        /// <param name="n">The Base10BigInteger whose opposite is to be computed</param>
        /// <returns>The Base10BigInteger inverse with respect to addition</returns>
        public static Base10BigInteger operator-(Base10BigInteger n)
        {
            return Opposite(n);
        }

        /// <summary>
        /// Addition operation of two Base10BigIntegers.
        /// </summary>
        /// <param name="a">The 1st Base10BigInteger</param>
        /// <param name="b">The 2nd Base10BigInteger</param>
        /// <returns>The Base10BigInteger result of the addition</returns>
        public static Base10BigInteger operator+(Base10BigInteger a, Base10BigInteger b)
        {
            return Addition(a, b);
        }

        /// <summary>
        /// Subtraction operation of two Base10BigIntegers.
        /// </summary>
        /// <param name="a">The 1st Base10BigInteger</param>
        /// <param name="b">The 2nd Base10BigInteger</param>
        /// <returns>The Base10BigInteger result of the subtraction</returns>
        public static Base10BigInteger operator-(Base10BigInteger a, Base10BigInteger b)
        {
            return Subtraction(a, b);
        }

        /// <summary>
        /// Multiplication operation of two Base10BigIntegers.
        /// </summary>
        /// <param name="a">The 1st Base10BigInteger</param>
        /// <param name="b">The 2nd Base10BigInteger</param>
        /// <returns>The Base10BigInteger result of the multiplication</returns>
        public static Base10BigInteger operator*(Base10BigInteger a, Base10BigInteger b)
        {
            return Multiplication(a, b);
        }

        /// <summary>
        /// Incremetation by one operation of a Base10BigInteger.
        /// </summary>
        /// <param name="n">The Base10BigInteger to be incremented by one</param>
        /// <returns>The Base10BigInteger result of incrementing by one</returns>
        public static Base10BigInteger operator++(Base10BigInteger n)
        {
            Base10BigInteger res = n + One;
            return res;
        }

        /// <summary>
        /// Decremetation by one operation of a Base10BigInteger.
        /// </summary>
        /// <param name="n">The Base10BigInteger to be decremented by one</param>
        /// <returns>The Base10BigInteger result of decrementing by one</returns>
        public static Base10BigInteger operator--(Base10BigInteger n)
        {
            Base10BigInteger res = n - One;
            return res;
        }


#endregion


        #region Private Methods


        /// <summary>
        /// Adds two BigNumbers a and b, where a >= b, a, b non-negative.
        /// </summary>
        private static Base10BigInteger Add(Base10BigInteger a, Base10BigInteger b)
        {
            Base10BigInteger res = new Base10BigInteger(a);
            long trans = 0, temp;
            int i;

            for (i = 0; i < b.size; i++)
            {
                temp = res.digits[i] + b.digits[i] + trans;
                res.digits[i] = temp % NumberBase;
                trans = temp / NumberBase;
            }

            for (i = b.size; ((i < a.size) && (trans > 0)); i++)
            {
                temp = res.digits[i] + trans;
                res.digits[i] = temp % NumberBase;
                trans = temp / NumberBase;
            }

            if (trans > 0)
            {
                res.digits[res.size] = trans % NumberBase;
                res.size++;
                trans /= NumberBase;
            }

            return res;
        }

        /// <summary>
        /// Subtracts the Base10BigInteger b from the Base10BigInteger a, where a >= b, a, b non-negative.
        /// </summary>
        private static Base10BigInteger Subtract(Base10BigInteger a, Base10BigInteger b)
        {
            Base10BigInteger res = new Base10BigInteger(a);
            int i;
            long temp, trans = 0;
            bool reducible = true;

            for (i = 0; i < b.size; i++)
            {
                temp = res.digits[i] - b.digits[i] - trans;
                if (temp < 0)
                {
                    trans = 1;
                    temp += NumberBase;
                }
                else trans = 0;
                res.digits[i] = temp;
            }

            for (i = b.size; ((i < a.size) && (trans > 0)); i++)
            {
                temp = res.digits[i] - trans;
                if (temp < 0)
                {
                    trans = 1;
                    temp += NumberBase;
                }
                else trans = 0;
                res.digits[i] = temp;
            }

            while ((res.size - 1 > 0) && (reducible == true))
            {
                if (res.digits[res.size - 1] == 0)
                    res.size--;
                else reducible = false;
            }

            return res;
        }

        /// <summary>
        /// Multiplies two Base10BigIntegers.
        /// </summary>
        private static Base10BigInteger Multiply(Base10BigInteger a, Base10BigInteger b)
        {
            int i, j;
            long temp, trans = 0;

            Base10BigInteger res = new Base10BigInteger();
            res.size = a.size + b.size - 1;
            for (i = 0; i < res.size + 1; i++)
                res.digits[i] = 0;

            for (i = 0; i < a.size; i++)
                if (a.digits[i] != 0)
                    for (j = 0; j < b.size; j++)
                        if (b.digits[j] != 0)
                            res.digits[i + j] += a.digits[i] * b.digits[j];

            for (i = 0; i < res.size; i++)
            {
                temp = res.digits[i] + trans;
                res.digits[i] = temp % NumberBase;
                trans = temp / NumberBase;
            }

            if (trans > 0)
            {
                res.digits[res.size] = trans % NumberBase;
                res.size++;
                trans /= NumberBase;
            }

            return res;
        }


#endregion



      private class DigitContainer
      {
         private readonly long[][] digits;
         private const int ChunkSize = 32;
         private const int ChunkSizeDivisionShift = 5;
         private const int ChunkCount = Base10BigInteger.MaxSize >> ChunkSizeDivisionShift;

         public DigitContainer()
         {
            digits = new long[ChunkCount][];
         }

         public long this[int index]
         {
            get
            {
               var chunkIndex = index >> ChunkSizeDivisionShift;
               var chunk = digits[chunkIndex];
               return chunk == null ? 0 : chunk[index%ChunkSize];
            }
            set
            {
               var chunkIndex = index >> ChunkSizeDivisionShift;
               var chunk = digits[chunkIndex] ?? (digits[chunkIndex] = new long[ChunkSize]);
               chunk[index%ChunkSize] = value;
            }
         }
      }
   }
}
