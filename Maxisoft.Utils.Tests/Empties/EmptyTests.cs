using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using Maxisoft.Utils.Empties;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Maxisoft.Utils.Tests.Empties
{
    public class EmptyTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly ImmutableArray<Type> _testedTypes;

        public EmptyTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _testedTypes = typeof(IEmpty).Assembly.ExportedTypes.Where(type => typeof(IEmpty).IsAssignableFrom(type))
                .ToImmutableArray();
        }

        [Fact]
        public void Test_HasAnyType()
        {
            Assert.NotEmpty(_testedTypes);
            Assert.NotEqual(new[] {typeof(IEmpty)}, _testedTypes);
        }

        [Fact]
        public void Test_HasDefaultConstructor()
        {
            foreach (var type in _testedTypes)
            {
                if (type.IsInterface) continue;
                if (type.IsAbstract) continue;
                if (type.IsValueType) continue;
                try
                {
                    Assert.NotNull(type.GetConstructor(Type.EmptyTypes));
                }
                catch (XunitException)
                {
                    _testOutputHelper.WriteLine($"{type.FullName} doesn't have a default constructor");
                    throw;
                }
            }
        }

        private HashSet<Type> _noSize = new HashSet<Type> {typeof(EmptyClass), typeof(IEmpty)};

        [Fact]
        public void Test_HasMinimalSize()
        {
            foreach (var type in _testedTypes)
            {
                var size = 0;
                var effectiveType = type;
                if (type.ContainsGenericParameters)
                {
                    effectiveType = type.MakeGenericType(Enumerable
                        .Repeat(typeof(EmptyStruct), type.GetGenericArguments().Length)
                        .ToArray());
                }

                dynamic instance = null;
                if (!type.IsInterface && !type.IsAbstract)
                {
                    instance = Activator.CreateInstance(effectiveType);
                }

                try
                {
                    size = Marshal.SizeOf(instance ?? effectiveType);
                }
                catch (Exception)
                {
                    try
                    {
                        size = Marshal.SizeOf(effectiveType);
                    }
                    catch (Exception e)
                    {
                        if (_noSize.Contains(type))
                        {
                            continue;
                        }
                        _testOutputHelper.WriteLine($"unable to get sizeof({type}): {e}");
                        throw;
                    }
                }

                try
                {
                    Assert.True(size <= 1);
                }
                catch (XunitException)
                {
                    _testOutputHelper.WriteLine($"{type.FullName} doesn't have an empty size");
                    throw;
                }
            }
        }
    }
}