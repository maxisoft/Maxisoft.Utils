using System;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Maxisoft.Utils.Logic
{
    public enum TriBoolValue : byte
    {
        False = 0,
        True = 1,
        Indeterminate = 2
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct ConvertTriBoolHelpStruct
    {
        [FieldOffset(0)] internal bool b;
        [FieldOffset(0)] internal TriBoolValue tri;
        [FieldOffset(0)] internal byte unsigned;
        [FieldOffset(0)] internal sbyte signed;
    }

    /// <summary>
    ///     3-state boolean.
    ///     The 3 states are <c>true</c>, <c>false</c>, and <c>null</c>(aka <c>indeterminate</c>)
    /// </summary>
    /// <seealso cref="TriBoolValue" />
    /// <seealso cref="Nullable" />
    /// <seealso cref="bool?" />
    /// <remarks>
    ///     There's already a similar <c>bool?</c> type why reinvent the wheel ?
    ///     <list type="bullet">
    ///         <item><c>bool?</c> sizeof is 4 bytes while <c>TriBool</c> is "only" 1 byte</item>
    ///         <item>
    ///             with <c>bool?</c> one cannot write directly
    ///             <code>
    /// if (x &amp;&amp; y) {} 
    /// </code>
    ///             but had to write the operation
    ///             <code>
    /// if (x &amp; y) {} 
    /// </code>
    ///             (notice the different number of <c>&amp;</c>)
    ///         </item>
    ///         <item>There's a lack of specialized bool? method like <see cref="TryParse" /></item>
    ///     </list>
    /// </remarks>
    /// <remarks>
    ///     Inspired by <see cref="!:https://www.boost.org/doc/libs/1_59_0/doc/html/tribool.html">Boost.Tribool</see>
    /// </remarks>
    [Serializable]
    [JsonConverter(typeof(TriBoolJsonConverter))]
    public readonly struct TriBool : IConvertible, IEquatable<TriBool>, IEquatable<bool>, IEquatable<bool?>,
        IComparable<TriBool>, IComparable
    {
        public readonly TriBoolValue TriValue;
        public bool IsTrue => TriValue == TriBoolValue.True;
        public bool IsFalse => TriValue == TriBoolValue.False;
        public bool IsIndeterminate => TriValue == TriBoolValue.Indeterminate;

        public bool HasValue => !IsIndeterminate;

        public bool Value
        {
            get
            {
                if (!HasValue)
                {
                    throw new InvalidOperationException();
                }

                return IsTrue;
            }
        }

        public static readonly TriBool True = true;
        public static readonly TriBool False = false;
        public static readonly TriBool Indeterminate = null;

        public TriBool(TriBoolValue value)
        {
            TriValue = value;
        }

        public TriBool(bool value)
        {
            TriValue = BoolToTriBool(value);
        }

        public TriBool(bool? value)
        {
            TriValue = BoolToTriBool(value);
        }

        public bool Coalesce(bool value)
        {
            return IsIndeterminate ? value : IsTrue;
        }

        public static explicit operator bool(TriBool tri)
        {
            return tri.IsTrue;
        }

        public static implicit operator bool?(TriBool tri)
        {
            return tri.TriValue switch
            {
                TriBoolValue.False => false,
                TriBoolValue.True => true,
                _ => null
            };
        }

        public static implicit operator TriBool(bool value)
        {
            return new TriBool(value);
        }

        public static implicit operator TriBool(bool? value)
        {
            return new TriBool(value);
        }

        public static bool operator true(TriBool tri)
        {
            return tri.IsTrue;
        }

        public static bool operator false(TriBool tri)
        {
            return tri.IsFalse;
        }

        public static TriBool operator &(TriBool left, TriBool right)
        {
            return (bool) !left || (bool) !right
                ? false
                : (bool) left && (bool) right
                    ? true
                    : Indeterminate;
        }

        public static TriBool operator |(TriBool left, TriBool right)
        {
            return (bool) !left && (bool) !right
                ? false
                : (bool) left || (bool) right
                    ? true
                    : Indeterminate;
        }

        public static TriBool operator ^(TriBool left, TriBool right)
        {
            if (left.IsIndeterminate)
            {
                return left;
            }

            if (right.IsIndeterminate)
            {
                return right;
            }

            return (bool) left ^ (bool) right;
        }

        public static TriBool operator !(TriBool tri)
        {
            return tri.TriValue switch
            {
                TriBoolValue.False => new TriBool(TriBoolValue.True),
                TriBoolValue.True => new TriBool(TriBoolValue.False),
                _ => Indeterminate
            };
        }

        public bool Equals(TriBool other)
        {
            return TriValue == other.TriValue;
        }

        public bool Equals(bool other)
        {
            return !IsIndeterminate && other == IsTrue;
        }

        public bool Equals(bool? other)
        {
            if (other.HasValue)
            {
                return IsTrue == other.Value;
            }

            return IsIndeterminate;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
            {
                return IsIndeterminate;
            }

            if (obj is bool b)
            {
                return Equals(b);
            }

            return obj is TriBool other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (int) TriValue;
        }

        public static TriBool operator ==(TriBool left, TriBool right)
        {
            if (left.IsIndeterminate)
            {
                return left;
            }

            if (right.IsIndeterminate)
            {
                return right;
            }

            return left.Equals(right);
        }

        public static TriBool operator !=(TriBool left, TriBool right)
        {
            if (left.IsIndeterminate)
            {
                return left;
            }

            if (right.IsIndeterminate)
            {
                return right;
            }

            return !left.Equals(right);
        }


        public int CompareTo(TriBool other)
        {
            return TriValue.CompareTo(other.TriValue);
        }

        public int CompareTo(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return 1;
            }

            if (obj is bool b)
            {
                obj = new TriBool(b);
            }

            return obj is TriBool other
                ? CompareTo(other)
                : throw new ArgumentException($"Object must be of type {nameof(TriBool)}");
        }

        public static bool operator <(TriBool left, TriBool right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(TriBool left, TriBool right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(TriBool left, TriBool right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(TriBool left, TriBool right)
        {
            return left.CompareTo(right) >= 0;
        }

        private static TriBoolValue BoolToTriBool(bool value)
        {
            var v = new ConvertTriBoolHelpStruct {b = value};
            return v.tri;
        }

        private static TriBoolValue BoolToTriBool(bool? value)
        {
            return value.HasValue ? BoolToTriBool(value.Value) : TriBoolValue.Indeterminate;
        }

        public const string TrueString = "true";
        public const string FalseString = "false";
        public const string IndeterminateString = "null";

        public static bool TryParse(string value, out TriBool result)
        {
            value = value?.Trim()!;

            if (StringComparer.OrdinalIgnoreCase.Equals(IndeterminateString, value))
            {
                result = new TriBool(TriBoolValue.Indeterminate);
                return true;
            }

            if (StringComparer.OrdinalIgnoreCase.Equals(TrueString, value))
            {
                result = new TriBool(TriBoolValue.True);
                return true;
            }

            if (StringComparer.OrdinalIgnoreCase.Equals(FalseString, value))
            {
                result = new TriBool(TriBoolValue.False);
                return true;
            }

            result = new TriBool(TriBoolValue.Indeterminate);
            return false;
        }

        public override string ToString()
        {
            return IsIndeterminate ? IndeterminateString : IsTrue ? TrueString : FalseString;
        }

        public TypeCode GetTypeCode()
        {
            return TypeCode.Boolean;
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            if (IsIndeterminate)
            {
                throw new InvalidOperationException();
            }

            return IsTrue;
        }

        public byte ToByte(IFormatProvider provider)
        {
            if (IsIndeterminate)
            {
                throw new InvalidOperationException();
            }

            return (byte) TriValue;
        }

        public char ToChar(IFormatProvider provider)
        {
            if (IsIndeterminate)
            {
                throw new InvalidOperationException();
            }

            return (char) TriValue;
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            throw new InvalidOperationException();
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            if (IsIndeterminate)
            {
                throw new InvalidOperationException();
            }

            return (int) TriValue;
        }

        public double ToDouble(IFormatProvider provider)
        {
            return IsIndeterminate ? double.NaN : IsTrue ? 1.0 : 0.0;
        }

        public short ToInt16(IFormatProvider provider)
        {
            if (IsIndeterminate)
            {
                throw new InvalidOperationException();
            }

            return (short) TriValue;
        }

        public int ToInt32(IFormatProvider provider)
        {
            if (IsIndeterminate)
            {
                throw new InvalidOperationException();
            }

            return (int) TriValue;
        }

        public long ToInt64(IFormatProvider provider)
        {
            if (IsIndeterminate)
            {
                throw new InvalidOperationException();
            }

            return (long) TriValue;
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            if (IsIndeterminate)
            {
                throw new InvalidOperationException();
            }

            return (sbyte) TriValue;
        }

        public float ToSingle(IFormatProvider provider)
        {
            return IsIndeterminate ? float.NaN : IsTrue ? 1.0f : 0.0f;
        }

        public string ToString(IFormatProvider provider)
        {
            return ToString();
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return Convert.ChangeType(this, conversionType, provider);
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            if (IsIndeterminate)
            {
                throw new InvalidOperationException();
            }

            return (ushort) TriValue;
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            if (IsIndeterminate)
            {
                throw new InvalidOperationException();
            }

            return (uint) TriValue;
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            if (IsIndeterminate)
            {
                throw new InvalidOperationException();
            }

            return (ulong) TriValue;
        }
    }


    public class TriBoolJsonConverter : JsonConverter<TriBool>
    {
        public override TriBool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                return reader.GetBoolean();
            }
            catch (InvalidOperationException)
            {
                var s = reader.GetString();
                if (s is null)
                {
                    return TriBool.Indeterminate;
                }

                if (TriBool.TryParse(s, out var res))
                {
                    return res;
                }

                throw;
            }
        }

        public override void Write(Utf8JsonWriter writer, TriBool value, JsonSerializerOptions options)
        {
            if (value.IsIndeterminate)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteBooleanValue(value.IsTrue);
            }
        }
    }
}