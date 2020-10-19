clamp_integer_pattern = r'''[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static {type} Clamp(this {type} x, {type} min, {type} max)
{{
    Debug.Assert(min <= max);
    return x < min ? min : max < x ? max : x;
}}'''

clamp_with_min_max_pattern = r'''[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static {type} Clamp(this {type} x, {type} min, {type} max)
{{
    return Math.Max(min, Math.Min(x, max));
}}'''

print("#region Clamp generated")
for t in ('byte', 'sbyte', 'short', 'ushort', 'int', 'uint', 'long', 'ulong'):
    print(clamp_integer_pattern.format(type=t))
    print('\n')
for t in ('float', 'double', 'decimal'):
    print(clamp_with_min_max_pattern.format(type=t))
    print('\n')
print('#endregion')