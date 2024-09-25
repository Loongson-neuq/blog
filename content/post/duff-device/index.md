---
title: "从循环展开到达夫设备"
description: 介绍循环展开优化，达夫设备，以及达夫设备的应用
date: 2024-09-25T16:18:43+08:00
math: true
license: "MIT LICENSE"
hidden: false
comments: true
draft: false
---

假设你需要连续完成某个相同操作 2 次时，你可能这样写代码：

```CSharp
Foo();
Foo();
```

通常情况下你都会这样做，而不是使用一个 for loop。

当这个此时大于或等于 10 次时，你大概就不会一个一个写了，而是使用　for 循环。

然而，如果你*稍微*思考一下，你都知道：
```Csharp
Foo();
Foo();
Foo();
// 省略 99997 条 Foo　call。
```

与

```CSharp
for (ulong i = 0; i < 100_000; i++)
{
    Foo();
}
```

并不完全等价。

让我们以 CPU 的视角审视一下这个过程：

对于第一个版本，就完全是 100, 000 条 `call` 指令。

对于第二个版本，来说则是

```
设置寄存器　i = 0 // xor rcx, rcx, rcx

.loop_begin
判断 i 是否小于 100, 000， 如果不是就跳转　.exit_loop  // cmp rcx, 100000　&& jge label
调用 Foo 函数　// call Foo
自增索引寄存器 // inc rcx
回到循环开始 // jmp .loop_begin

.exit_loop
```

这两个版本各有各的优势以及缺陷：

对于第一个版本：
- 完成所有任务将执行 `100, 000` 条指令
- 所有指令也将占用 `100, 000 * 4` 字节的内存

对于第二个版本：
- 完成所有任务将执行大约 `100，000 * 6` 条指令
- 所有指令仅占用　`6 * 4` 字节的内存

从性能上讲，第一个版本少执行大量指令，意味着更高的性能。但是从内存占用上讲，第一个版本将消耗大量内存，意味着更大的二进制可执行文件。详细可以看[最后一节](#循环展开的性能)

## 思考

那我们能否找到一个平衡点，即，损失一点内存占用，但是提高性能？

不难发现，第二个版本的性能损失来自于每次循环中的以下部分：

- 判断循环索引寄存器
- 自增循环索引寄存器
- 跳转 label

因此，我们的任务便是减少这三条指令的执行次数。那么如何减少呢？

答案非常简单，让我们按照第一个版本的代码，在循环体中多次调用 `Foo()`。这样，我们就可以减少循环次数，也就减少了额外的指令开销。

例如，我们可以按照以下方式改写：

```CSharp
// 循环次数变成了原来的 1/10
for (ulong i = 0; i < 10_000; i++)
{
    // 循环一次执行十条 call
    Foo();
    Foo();
    Foo();
    Foo();
    Foo();
    Foo();
    Foo();
    Foo();
    Foo();
    Foo();
}
```

这下，我们只需要执行大约 `10_000 * 5` + `100_000` 条指令，以及大幅减少了执行指令的条数。

这个过程就是循环优化。大部分编译器都支持这种优化。

## 问题

循环优化好是好，但是事情并不总是这么美好。当循环次数**不是**展开倍率的整数倍时，我们需要在循环完成后再手动执行。当循环次数不是常量时，就更麻烦了！

要实现一个对于任意给定循环次数 `n` 都能够正确执行的循环展开，我们可以使用以下代码来实现：

注意我切换成了 C 语言，这是为了给后面的达夫设备埋下伏笔

```C
void task(int n)
{
    const int UNROLL_COUNT = 10;

    size_t loop_count = (n + UNROLL_COUNT - 1) / UNROLL_COUNT;

    for (size_t i = 0; i < loop_count; i++)
    {
        foo();
        foo();
        foo();
        // 省略 7 行
    }

    // 执行剩余的次数
    switch (n % UNROLL_COUNT)
    {
        case 9: foo();
        case 8: foo();
        case 7: foo();
        case 6: foo();
        case 5: foo();
        case 4: foo();
        case 3: foo();
        case 2: foo();
        case 1: foo();
    }
}
```

## 达夫设备

我猜大部分人都会这样写，虽然可能会有一些小区别。然而 Tom Duff 给出了一个估计只有外星人才能一眼看明白的解法[^1]：

[^1]: https://swtch.com/duffs-device/td-1983.txt

```
void task(int count)
{
    register count;
    {
        register n = (count + 7) / 8;
        switch(count % 8) {
            case 0: do{ foo();
            case 7:     foo();
            case 6:     foo();
            case 5:     foo();
            case 4:     foo();
            case 3:     foo();
            case 2:     foo();
            case 1:     foo();
                    } while (--n > 0);
        }
    }
}
```

我特意关闭了语法高亮。一眼看上去，这 tm 是人能写出来的？你大概甚至很难相信这个代码能够通过编译。但它确实可以，并且据 Tom Duff 所说，它运行得很好[^1]。

我这里给出我直译的版本。尽管仍有一点区别，但是这个区别正是达夫设备的价值所在。

### 直译版

```C
void task(int count)
{
    register count;
    {
        register n = (count + 7) / 8;
        switch(count % 8) {
            case 0: goto .remainder_is_0;
            case 7: goto .remainder_is_7;
            case 6: goto .remainder_is_6;
            case 5: goto .remainder_is_5;
            case 4: goto .remainder_is_4;
            case 3: goto .remainder_is_3;
            case 2: goto .remainder_is_2;
            case 1: goto .remainder_is_1;
        }

        do
        {
            .remainder_is_0: foo();
            .remainder_is_1: foo();
            .remainder_is_2: foo();
            .remainder_is_3: foo();
            .remainder_is_4: foo();
            .remainder_is_5: foo();
            .remainder_is_6: foo();
            .remainder_is_7: foo();
        } while(--n > 0);
	}
}
```

当刚进入函数时，会先计算余数，并根据余数跳转到循环体内部，**先**执行相应次数的 `foo()`。接着，将会开始 `do-while` 循环，通过循环的方式执行剩下的次数。

当我们仅考虑 `foo()` 的执行情况，我们可以得出以下版本的代码。

### 意译版

```C
void task(int count)
{
    register count;
    {
        register n = (count + 7) / 8;
        switch(count % 8) {
            case 0: foo();
            case 7: foo();
            case 6: foo();
            case 5: foo();
            case 4: foo();
            case 3: foo();
            case 2: foo();
            case 1: foo();
        }

        do
        {
            foo();
            foo();
            foo();
            foo();
            foo();
            foo();
            foo();
            foo();
        } while(--n > 0);
	}
}
```

这个版本的代码仅用于你了解这个过程中发生了什么。让我们回到上面直译版的代码。

我前面提到，直译版的代码于 Tom Duff 给出的版本仍有一点区别。这个区别就是，我在 switch 语句的各个 case 中又使用　`goto`　跳转到了对应的位置，但是 Tom Duff 直接利用了 switch 语句跳转到了对应的部分。

在这个过程中，每一个 case 就像 label 一样仅用于标识指令的地址而不影响 switch 语句内部其他的语句的语义，也不影响内部的控制流。

## 达夫设备的性能

从性能上讲，达夫设备与我们编写的循环展开或者上面的意译版性能相同。不过，达夫设备并不总是在所有情况下提供最高性能（假设循环展开次数相同）。你可以查看 [达夫设备 - 维基百科](https://zh.wikipedia.org/wiki/%E8%BE%BE%E5%A4%AB%E8%AE%BE%E5%A4%87#%E6%80%A7%E8%83%BD%E8%A1%A8%E7%8E%B0) 了解更多。

## 达夫设备应用？

那么这个“特性”有什么作用呢？

PuTTY 的作者使用这种特性，在 C 语言中实现了**不改变代码控制流**的情况下的无栈协程。

通常来说，对于这样的协程方法

```CSharp
public static IEnumerable<int> Fib()
{
    int prev = 0, next = 1;
    yield return prev;
    yield return next;

    while (true)
    {
        int sum = prev + next;
        yield return sum;
        prev = next;
        next = sum;
    }
}
```

会被编译成一个完全状态机：
```CSharp
public static IEnumerable<int> Fib() => new FibStateMachine(-2);

[CompilerGenerated]
private sealed class FibStateMachine : IEnumerable<int>, IEnumerable, IEnumerator<int>, IEnumerator, IDisposable
{
    private int state;
    private int current;
    private int prev;
    private int next;
    private int sum;

    int IEnumerator<int>.Current => this.current;
    object IEnumerator.Current => this.current;

    public FibStateMachine(int state)
        => this.state = state;

    private bool MoveNext()
    {
        switch (this.state)
        {
            default:
                return false;
            case 0:
                this.state = -1;
                this.prev = 0;
                this.next = 1;
                this.current = this.prev;
                this.state = 1;
                return true;
            case 1:
                this.state = -1;
                this.current = this.next;
                this.state = 2;
                return true;
            case 2:
                this.state = -1;
                break;
            case 3:
                this.state = -1;
                this.prev = this.next;
                this.next = this.sum;
                break;
        }
        this.sum = this.prev + this.next;
        this.current = this.sum;
        this.state = 3;
        return true;
    }

    IEnumerator<int> IEnumerable<int>.GetEnumerator()
    {
        if (this.state == -2)
        {
            this.state = 0;
            return this;
        }
        return new FibStateMachine(0);
    }

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<int>)this).GetEnumerator();
    void IEnumerator.Reset() => throw new NotSupportedException();
    void IDisposable.Dispose() { }
}
```

可以看到，方法的控制流被编译器完全改变，因为每一个 yield return 语句都意味着下一次进入函数时要从一个新的状态开始继续运行。不仅如此，原有方法现在仅仅返回一个新的状态机对象，而不包含任何实现。这意味着，在原方法中，不改变控制流的情况下很难实现协程效果。

Simon Tatham 利用达夫设备和宏，在 C 语言中，仅需插入少许代码即可实现无栈协程！就像我在前面给出的 C# 版本的协程一样。你一眼就能理解修改后的协程方法的控制流。

你可以点击以下链接进行了解

- [原文](https://www.chiark.greenend.org.uk/~sgtatham/coroutines.html)（英语）
- [译文](https://mthli.xyz/coroutines-in-c/)（站外链接）

然而 Simon Tatham 的无栈协程也仅仅能存在于理论中，并且他给出的代码使用全局变量来保存协程上下文，因此不能同时调用同一个协程方法。而且，就像 Simon Tatham 在最后说的一样，这些“可怕破坏性的 crReturn 宏”，“非常糟糕的清晰度”以及“难如登天的重写复杂度”都阻止你在任何场合使用它。

不过在了解这些原理的过程中，你的能力又提升了不少，不是吗？

<a id="循环展开的性能"></a>
## 循环展开的性能

让我们回到文章一开始的三个代码片段。

并非第一个版本的性能就是第二个版本的 1/6，由于存在大量指令，CPU 取指令同样消耗时间，并且这是一个相对耗时的任务。CPU 也具有**分支预测**等优化技巧来减少每次条件判断的耗时。并且在这种情况下，分支预测通常有较高的正确率。

同时，Foo 内部的实现也影响执行效率的倍率。当 Foo 内部的实现越复杂，指令越多，循环所导致的性能缺陷就越不明显。

因此，当你实际 Benchmark 这两段代码时，可能并不会有那么可观的差距。

我使用以下代码进行 Benchmark：
```CSharp
public class BenchmarkLoopUnroll
{
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static void Foo()
    {
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
    public void ForLoopCompilerOptimized()
    {
        for (int i = 0; i < 1000; i++)
        {
            Foo();
        }
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public void ForLoop10TimesUnroll()
    {
        for (int i = 0; i < 100; i++)
        {
            Foo();
            Foo();
            Foo();
            Foo();
            Foo();
            Foo();
            Foo();
            Foo();
            Foo();
            Foo();
        }
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public void ForLoopNoOptimization()
    {
        for (int i = 0; i < 1000; i++)
        {
            Foo();
        }
    }

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public void HardCodedThousandCall()
    {
        Foo();
        Foo();
        Foo();
        // 省略 997 行
    }
}
```

使用 RyuJIT 得到以下测试结果：
```plaintext
// * Summary *

BenchmarkDotNet v0.14.0, Ubuntu 24.04 LTS (Noble Numbat) WSL
13th Gen Intel Core i5-13500H, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.108
  [Host]     : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2

| Method                   | Mean     | Error     | StdDev    | Ratio | RatioSD |
|------------------------- |---------:|----------:|----------:|------:|--------:|
| HardCodedThousandCall    | 1.095 us | 0.0129 us | 0.0120 us |  1.00 |    0.02 |
| ForLoopNoOptimization    | 1.110 us | 0.0211 us | 0.0187 us |  1.01 |    0.02 |
| ForLoop10TimesUnroll     | 1.050 us | 0.0193 us | 0.0180 us |  0.96 |    0.02 |
| ForLoopCompilerOptimized | 1.070 us | 0.0117 us | 0.0109 us |  0.98 |    0.01 |

// * Legends * 
  Mean    : Arithmetic mean of all measurements
  Error   : Half of 99.9% confidence interval
  StdDev  : Standard deviation of all measurements
  Ratio   : Mean of the ratio distribution ([Current]/[Baseline])
  RatioSD : Standard deviation of the ratio distribution ([Current]/[Baseline])
  1 us    : 1 Microsecond (0.000001 sec)

// ***** BenchmarkRunner: End *****
Run time: 00:00:59 (59.07 sec), executed benchmarks: 4
```

可以看到，`ForLoopNoOptimization` 相较于 `HardCodedThousandCall` 并没有慢很多，虽然确实慢了一些。不过我们手动展开的方法 `ForLoop10TimesUnroll` 确实是有效的，并且比编译器自带的优化效果还要好。

---------

- 作者: Caiyi Shyu 
- Email: caiyishyu@outlook.com