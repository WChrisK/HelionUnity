using System;
using UnityEngine;

namespace Helion.Core.Util
{
    /// <summary>
    /// A simple class to invoke the Optional constructor with better
    /// readability.
    /// </summary>
    public class OptionalHelper
    {
        /// <summary>
        /// The instance to use for creating optionals.
        /// </summary>
        /// <remarks>
        /// You can use: using static Helion.Core.Util.OptionalHelper;
        /// </remarks>
        public static readonly OptionalHelper Empty = new OptionalHelper();

        private OptionalHelper()
        {
        }
    }

    /// <summary>
    /// Indicates an element may or may not be present.
    /// </summary>
    /// <remarks>
    /// Intended to be a safe way to work around references which can be null.
    /// </remarks>
    public class Optional<T> where T : class
    {
        /// <summary>
        /// The value contained in the optional. This may be null, and usage
        /// should be compared against <see cref="HasValue"/> before using.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// True if the optional has a value and it is safe to extract the
        /// <see cref="Value"/> object, or false if null and no object is
        /// contained.
        /// </summary>
        public bool HasValue => Value != null;

        /// <summary>
        /// Creates an empty optional.
        /// </summary>
        public Optional()
        {
            Value = null;
        }

        /// <summary>
        /// Creates an optional with an element. The element may be null, and
        /// that will generate an optional that is empty.
        /// </summary>
        /// <param name="element">The element to contain.</param>
        public Optional(T element)
        {
            Value = element;
        }

        /// <summary>
        /// Allows us to use this in boolean expressions.
        /// </summary>
        /// <param name="self">The optional value.</param>
        /// <returns>True if it has a value, false if empty.</returns>
        public static implicit operator bool(Optional<T> self) => self.HasValue;

        /// <summary>
        /// A simple way to allow for more readable empty optionals. This way
        /// you can assign the OptionalHelper.Empty to an optional field and
        /// import the class statically so it is less cluttered instead of
        /// calling Optional[T].Empty() to make an empty optional.
        /// </summary>
        /// <param name="helper">A reference that will create an empty
        /// optional.</param>
        /// <returns>The empty optional.</returns>
        public static implicit operator Optional<T>(OptionalHelper helper) => new Optional<T>(null);

        /// <summary>
        /// A conversion value from an object to an optional.
        /// </summary>
        /// <param name="value">The value to make an optional from.</param>
        /// <returns>The optional for the value.</returns>
        public static implicit operator Optional<T>(T value) => new Optional<T>(value);

        /// <summary>
        /// Creates an empty optional.
        /// </summary>
        /// <returns>An empty optional.</returns>
        public static Optional<T> Empty() => new Optional<T>(null);

        /// <summary>
        /// Returns a new optional that is a shallow copy of the old one. This
        /// means if the value is present in this, the reference is copied. If
        /// not, then an empty optional is returned.
        /// </summary>
        /// <returns>An optional with a shallow copy of the value.</returns>
        public Optional<T> Copy() => new Optional<T>(Value);

        /// <summary>
        /// Calls the function with the value if present, otherwise the
        /// function is not called.
        /// </summary>
        /// <param name="func">The function to call on the object if the
        /// optional contains a value.</param>
        public void With(Action<T> func)
        {
            if (HasValue)
                func(Value);
        }

        /// <summary>
        /// Returns the inner value, or a default value if it is not present.
        /// If the creation of the object is expensive, you can use
        /// <see cref="Or(T)"/> or try null coalescing.
        /// </summary>
        /// <param name="other">The value to return if it does not exist.
        /// </param>
        /// <returns>The value inside the optional if it exists, or the value
        /// passed in as the argument if this optional is empty.</returns>
        public T Or(T other) => HasValue ? Value : other;

        /// <summary>
        /// Returns the inner value, or calls the generator function if the
        /// value does not exist. This is designed such that lazy evaluation
        /// on a generator can be done to avoid Or() having a new object being
        /// instantiated.
        /// </summary>
        /// <param name="itemGenerator">The function to be called to generate
        /// the item if the value is not present.
        /// </param>
        /// <returns>The value inside the optional if it exists, or the value
        /// passed in as the argument if this optional is empty.</returns>
        public T Or(Func<T> itemGenerator) => HasValue ? Value : itemGenerator();

        /// <summary>
        /// Maps an optional to another optional type. If this optional is
        /// empty, then an empty optional of the mapped type will be returned.
        /// </summary>
        /// <param name="func">The transformation function. This does not need
        /// to return an optional, and can return null to indicate that the
        /// optional should be empty. This will not be called if the optional
        /// being operated on is empty.</param>
        /// <typeparam name="U">The conversion type.</typeparam>
        /// <returns>An optional from the function being called if the current
        /// optional has a value, or an empty optional if the current optional
        /// is empty.</returns>
        public Optional<U> Map<U>(Func<T, U> func) where U : class
        {
            return new Optional<U>(HasValue ? func(Value) : null);
        }

        /// <summary>
        /// Same as <see cref="Map{U}"/> but returns the value provided if
        /// either this optional is empty, or the returned optional is empty.
        /// </summary>
        /// <remarks>
        /// If the creation of the object is expensive, it is probably better
        /// to use the null coalescing operator by getting Value. This is
        /// primarily intended for when you have the value created and want to
        /// get it by reference.
        /// </remarks>
        /// <param name="func">The function to transform the optional value if
        /// it exists.</param>
        /// <param name="elseValue">The value to return if either this optional
        /// is empty, or the transformation function returns an empty optional.
        /// </param>
        /// <typeparam name="U">The transformed type.</typeparam>
        /// <returns>The transformed value, or the other value if either this
        /// optional is empty or the transformation yields an empty optional.
        /// </returns>
        public U MapOr<U>(Func<T, U> func, U elseValue) where U : class
        {
            return HasValue ? (func(Value) ?? elseValue) : elseValue;
        }

        public override int GetHashCode() => HasValue ? Value.GetHashCode() : 0;

        public override string ToString() => $"{Value}";
    }

    /// <summary>
    /// An optional that also contains an exception.
    /// </summary>
    /// <typeparam name="T">The maybe type.</typeparam>
    /// <typeparam name="E">The exception if the value is null.</typeparam>
    public class Optional<T, E> where T : class where E : Exception
    {
        /// <summary>
        /// The value contained in the optional. This may be null, and usage
        /// should be compared against <see cref="HasValue"/> before using.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// The exception present if the value is not.
        /// </summary>
        public E Exception { get; }

        /// <summary>
        /// True if the optional has a value and it is safe to extract the
        /// <see cref="Value"/> object, or false if null and no object is
        /// contained (and the Exception is available).
        /// </summary>
        public bool HasValue => Value != null;

        private Optional(T value, E exception)
        {
            Value = Value;
            Exception = Exception;
        }

        /// <summary>
        /// Creates an optional from a value.
        /// </summary>
        /// <param name="value">The value to use. Should not be null.</param>
        /// <returns>A new optional.</returns>
        public static Optional<T, E> FromValue(T value)
        {
            Debug.Assert(value != null, "Cannot make an optional error with both being null");
            return new Optional<T, E>(value, null);
        }

        /// <summary>
        /// Creates an empty optional with an exception as a reason.
        /// </summary>
        /// <param name="exception">The exception to use for the empty
        /// optional.</param>
        /// <returns>A new optional from an exception.</returns>
        public static Optional<T, E> FromException(E exception)
        {
            Debug.Assert(exception != null, "Cannot make an optional error with both being null");
            return new Optional<T, E>(null, exception);
        }

        /// <summary>
        /// Allows us to use this in boolean expressions.
        /// </summary>
        /// <param name="self">The optional value.</param>
        /// <returns>True if it has a value, false if empty.</returns>
        public static implicit operator bool(Optional<T, E> self) => self.HasValue;

        /// <summary>
        /// A conversion value from an object to an optional.
        /// </summary>
        /// <param name="value">The value to make an optional from.</param>
        /// <returns>The optional for the value.</returns>
        public static implicit operator Optional<T, E>(T value) => FromValue(value);

        /// <summary>
        /// A conversion an exception to an error optional.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>The error optional for the exception.</returns>
        public static implicit operator Optional<T, E>(E exception) => FromException(exception);

        public override string ToString() => $"{Value}";
    }
}
