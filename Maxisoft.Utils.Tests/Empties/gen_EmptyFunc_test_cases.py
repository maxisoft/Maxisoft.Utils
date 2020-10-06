import random
template = """[Fact]
public void Test_Func{n}() 
{{
    Func<{types}, object> f = new EmptyFunc<{types}, object>();
    Assert.Equal(default(object), f({args}));
}}
"""

def gen(n: int):
    types = ", ".join("int" for _ in range(n))
    args = ", ".join(hex(random.randint(0, 1 << 16)) for _ in range(n))

    return template.format(n=n, types=types, args=args)

for i in range(1, 16):
    print(gen(i))

