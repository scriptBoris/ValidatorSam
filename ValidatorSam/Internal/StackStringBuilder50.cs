using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ValidatorSam.Internal
{
    /// <summary>
    /// StringBuilder который аллоцируется на стэке
    /// </summary>
    internal unsafe struct StackStringBuilder50
    {
        private fixed char _buffer[50];
        private byte _length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(char c)
        {
            if (_length >= 50)
                throw new InvalidOperationException("Buffer overflow");

            _buffer[_length++] = c;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(ReadOnlySpan<char> chars)
        {
            if (_length + chars.Length > 50)
                throw new InvalidOperationException("Buffer overflow");

            fixed (char* ptr = _buffer)
            {
                var span = new Span<char>(ptr + _length, chars.Length);
                chars.CopyTo(span);
            }
            _length += (byte)chars.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            _length = 0;
        }

        public int Length => _length;
        public string DebugText => $"_____length:{_length} <<{ToString()}>>_____";

        public ReadOnlySpan<char> AsSpan()
        {
            fixed (char* ptr = _buffer)
            {
                return new ReadOnlySpan<char>(ptr, _length);
            }
        }

        public override string ToString()
        {
            fixed (char* ptr = _buffer)
            {
                return new string(ptr, 0, _length);
            }
        }
    }
}