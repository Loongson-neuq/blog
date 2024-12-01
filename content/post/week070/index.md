---
title: "[OS Week7] Coroutine, Iterator & Asynchronous Programming"
date: 2024-12-01
description: "Coroutine, Iterator & Asynchronous Programming"
tags:
  - Rust
categories: 
  - OS
---

# What is Coroutine?

协程，是相对与子过程（即我们熟悉的函数）的一种概念。子过程是一种被调用的过程，而协程是一种可以挂起和恢复的过程。简单地说，你调用一个子过程（函数），你就只能等它从头执行完毕，下一次你再调用它的时候，它还是从头开始执行。而协程，你可以在中途挂起它，然后再恢复它，它会从上次**挂起**的地方继续执行。

## Why Coroutine?

协程被称为 Green Thread，似乎与线程有些类似，但是他们完全不同。

对于用户程序来说，操作系统对 CPU 做了一层抽象，让用户程序感觉自己拥有了一个整个 CPU（核心）。而线程的意义在于用户程序，能够拥有多个 CPU 核心，从而提高程序的并发性。通过操作系统的抽象，你可以创建数千个线程，但是 CPU 实际上只有几个核心。通过多个 CPU 核心完成任务，来更好地利用一整块 CPU，这就是**Parallelism（并行）**。

协程，并不关注 CPU 核心的问题，相反，协程可以在任意一个或多个 CPU 核心上调度运行，取决于调度器的实现。通常，当我们谈论协程的时候，我们会只关心一个 CPU 核心的问题，而将调度的问题交给调度器来解决。因此，协程解决的问题是，如何更加**充分**地利用一个 CPU 核心。这就是**Concurrency（并发）**。

1. 定义角度：
- 并行(Parallelism)是指在同一时刻，有多个任务在**多个**处理器上同时执行
- 并发(Concurrency)是指在同一时间段内，有多个任务在交替执行，从**宏观**来看似乎是同时进行的，但是在**微观**上是交替执行（串行，单核心）的。

2. CPU利用角度：
- 并行要求程序能够充分利用多个CPU核心，将任务分配到不同的核心上同时执行
- 并发则是通过合理安排任务的执行顺序，让单个CPU核心能够高效地处理多个任务

3. 实现机制：
- 并行通常依赖于硬件的多核心支持，通过线程或进程实现多任务的真正同时执行
- 并发可以通过协程、时间片轮转等机制，在单个核心上实现任务的交替执行

4. 一个形象的比喻：
- 并行就像多个收银员同时为不同顾客结账
- 并发就像一个收银员通过快速切换为多个顾客轮流结账

5. 性能提升方式：
- 并行通过增加计算资源(CPU核心)来提高程序的整体吞吐量
- 并发通过优化任务调度和执行顺序来提高单个计算资源的利用效率

6. 适用场景：
- 并行适合计算密集型任务，如图像处理、科学计算等
- 并发适合I/O密集型任务，如网络请求、文件操作等

# 单线程串行运行那么好，为什么要一直切换正在运行的任务呢？

上节课中，展示了一个例子，即使用 Singlethreaded 允许 16 次 `Add_100000()` 函数，然后计算总时间。结果是 16 个任务串行执行的时间总和。然后，我们使用 Multithreaded 允许 16 次 `Add_100000()` 函数，然后计算总时间。结果是 16 个任务并行执行的时间总和。结果发现 Singlethreaded 的时间比 Multithreaded 的时间还要短。这是因为线程间争锁、切换线程等操作，会消耗额外的时间。

说明单线程对于性能来说其实是最好的，那么为什么要一直切换正在运行的任务呢？并且我们总是可以预料到，即使是单线程，一直切换任务也会消耗额外的时间。

这基于下面的两个主要原因：


## 1. 阻塞，更差的响应性

当我们串行运行任务时，如果一个任务阻塞了，那么整个程序都会被阻塞。这是因为我们的程序是单线程的，只有一个任务在运行。

考虑以下一个场景，你在电脑上编写了以下程序：

```rust
fn main() {
    while true {
        println!("Hello, World!");
    }
}
```

然后运行它，好了，由于程序串行运行，你的 OS 无法处理你的键盘鼠标输入，也无法绘制屏幕输出，因为这个程序一直在运行，不会停止，导致排队在后面的任务无法执行。

但是如果我们不停切换任务，尽管一个任务阻塞，一段时间后，我们总是会切换到其他任务，这样就可以保证所有任务都能得到执行（至少有一小段时间），而不会因为一个任务阻塞而导致整个程序无法运行。

在这个过程中，这个死循环任务被称为**CPU-bound（计算密集型）**任务，因为它一直在占用 CPU，而不会释放。

## 2. Comsumer-Producer 间的协作

那还有什么其他类型的任务呢？大多数情况下，我们的程序**不是**在一直不停地做 CPU 计算，而是在等待 I/O 操作完成，比如等待网络请求返回、等待文件读写，甚至等待用户鼠标键盘输入。这些任务被称为**I/O-bound（I/O 密集型）**任务，因为它们需要等待 I/O 操作完成，才能继续执行。

这些任务等待的对象通常归根结底是**外部资源**，比如网络、文件、用户输入等。这些资源的读写速度远远慢于 CPU 的计算速度，因此我们的程序在等待这些资源的时候，实际上是在**浪费** CPU 的时间。如果我们不停切换任务，那么当一个任务等待 I/O 时，我们可以切换到其他任务，让它们继续执行，这样就可以充分利用 CPU 的时间，提高程序的**响应性**。

在这个过程中，我们的任务被称为 Comsumer 任务，因为它们需要外设产生的数据；而外设产生数据的任务被称为 Producer 任务，因为它们给 Comsumer 任务提供数据。

对于 Comsumer-Producer 的协作关系，解决 IO 阻塞是相对简单的。更重要的是，我们需要一种编程模型，简化 Comsumer-Producer 之间的协作，让我们能够更加方便地编写这种类型的程序。而协程就是这样一种编程模型。

让我们考虑以下一种情况，其中一个 Produecer 一直不停地产生数据 1, 2, 3, 4, 5, ...，而一个 Comsumer 一直不停地消费这些数，然后打印出来。你大概率 100% 会写出以下代码：

```rust
fn main() {
    let mut i = 0;
    loop {
        println!("{}", i);
        i += 1;
    }
}
```

这是相当糟糕的代码，因为生产者与消费者高度耦合在一起，无法分离。我们希望的是，生产者与消费者能够分离，互不干扰，这样我们可以更加方便地编写这种类型的程序。就像下面一样：

```rust

static mut queue: VecDeque<i32> = VecDeque::new();

fn producer() {
    let mut i = 0;
    loop {
        queue.push_back(i); // 把产生的数据放到队列中
        i += 1;
    }
}

fn consumer() {
    loop {
        if let Some(i) = queue.pop_front() { // 从队列中取出数据
            println!("{}", i);
        } else {
            // 如果队列为空，等待一段时间
        }
    }
}
```

这样，生产者与消费者之间就可以分离，互不干扰，我们可以更加方便地编写这种类型的程序。但是这样的代码还是有问题，因为我们的程序是单线程的，如果生产者一直在产生数据，那么消费者就无法执行，反之亦然。我们需要一种编程模型，让我们能够更加方便地编写这种类型的程序，这就是协程。

让我们考虑这个过程中的问题，即*生产者一直在产生数据，那么消费者就无法执行*。因此解决问题的方法就是，让其中一方，在执行到一定程度后，**暂停执行**，然后切换到另一方执行（调度方处理），这样就可以保证两者都能得到执行。这就是协程的作用。

# Implement a Coroutine

我们知道，要让一个任务能够在中途挂起，然后再恢复，我们需要保存这个任务的**上下文**，然后再恢复这个任务的上下文。这就是协程的基本原理。

## Stackful Coroutine

最基本的实现是 Stackful Coroutine，即我们需要在挂起一个任务时，保存这个任务的**栈**和完整的**寄存器**状态。

为什么要保存栈呢？因为栈保存了函数的局部变量、参数、返回地址等信息，是函数能够**嵌套**调用的基础。如果我们不保存栈，那么函数退出后，无法返回到调用方，甚至，我们无法保存函数的局部变量等信息，因为这些信息都保存在栈帧中（上两节课的内容）。那为什么要保存寄存器呢？因为寄存器保存了函数的**全局**状态，比如函数的指令指针、栈指针等信息，是函数能够**恢复**执行的基础。如果我们不保存寄存器，那么函数退出后，无法恢复到函数的执行状态，因为这些信息都保存在寄存器中。

但是我们是否有必要保存整个栈呢？答案是当然的，因为我们需要保存函数的**完整**状态，包括局部变量、参数、返回地址等信息。因为所有嵌套调用的函数都保存在栈中，所以我们需要保存整个栈的状态。

但是我们是否有必要保存整个寄存器呢？答案是不一定的。

其实按道理说，我们需要保证在恢复任务后，任务的执行状态和挂起时一样，这就需要保存整个寄存器。但是实际上，我们可以从挂起任务的过程中做一些简化。Stackful Coroutine 挂起一个任务看起来就像调用一个函数一样，我们需要在这个函数中，将任务的状态保存到一个**上下文**中，然后再恢复任务的时候，将任务的状态从上下文中恢复。这个上下文保存了任务的**完整**状态，包括栈和寄存器等信息。写成代码就是这样：

```rust

pub fn producer() {
    let mut i = 0;
    loop {
        queue.push_back(i); // 把产生的数据放到队列中
        i += 1;
        yield(); // 挂起任务，任务从此处暂停，下次也将从此处恢复
    }
}

```

这个 `yield()` 函数就是将任务的状态保存到一个上下文中，然后再恢复任务的时候，将任务的状态从上下文中恢复。这个上下文保存了任务的**完整**状态，包括栈和寄存器等信息。如果你比较好奇这样的函数长什么样子，可以看看[我的操作系统里的实现](https://github.com/caiyih/bakaos/blob/b28d8421f6c030476ca0ae9050098983ac31e58c/kernel/src/trap/user.rs#L126-L142)，我选中的这几行是保存操作系统内核协程上下文的代码，其他的则是保存/恢复用户程序上下文的的代码，基本上就是把一些寄存器和调用栈栈顶保存到一个结构体中，然后切换调用栈，然后再恢复的时候，将这些寄存器从结构体中恢复。注意这段代码必须使用汇编代码实现，一是我们需要手动操作指定寄存器，二是编译器生成的函数包含 Prologue 和 Epilogue，会修改我们的调用栈（同样是上两节课的内容）。

而我们*偷懒*的做法就以来于，这是一个函数调用。既然是函数调用，就必须要遵循**Calling Conventions**。

**Calling Conventions**要求将寄存器的保存职责分为两部分：Caller-Save 和 Callee-Save。Caller-Save 负责保存调用者需要保存的寄存器，Callee-Save 负责保存被调用者需要保存的寄存器。

Caller-saved 要求，Caller在调用一个函数前保存这些寄存器，子过程可能使用这些寄存器的任意或全部，结束后，恢复也是 Caller 的责任。

而 Callee-saved 要求，Callee 在调用一个函数前保存这些寄存器，子过程可能使用这些寄存器的任意或全部，在回到 Caller 时，Callee 必须保证这些寄存器的值与 Caller 调用自己前一样。

因此，我们在这个过程中，只需要保存 Caller-saved 寄存器，而不需要保存 Callee-saved 寄存器。这样就可以简化我们的实现，只需要保存 Caller-saved 寄存器，而不需要保存 Callee-saved 寄存器。

## Stackless Coroutine

你一定觉得，什么和调用栈切换，保存寄存器，还要用汇编指令进行操作太麻烦了，而且开销还一定特别大。因为我们需要保存整个栈，这个栈可能非常大，而且我们还需要保存整个寄存器，这个寄存器可能非常多。这样的开销是非常大的，而且实现起来也非常复杂。

你的感觉没错！通常来说，一个栈的大小在，0.5 MB 到 4 MB 之间，而一个寄存器的数量在 16 到 32 个之间。这样的开销是非常大的，而且实现起来也非常复杂。

考虑一下一个简单的生产者消费者模型，我们的生产者只产生 3 个数据，然后消费者消费这 3 个数据。

```rust
fn producer() {
    // 产生数据 1
    yield();
    // 产生数据 2
    yield();
    // 产生数据 3
    yield();
}
```

我们可以联想到，由于我们的代码是确定的，因此协程被挂起的*时机*或者说*地点*也是确定的！因此我们其实可以把这个过程视为一个 State Machine，即我们的协程有一个状态，然后根据这个状态，我们执行不同的代码。这样我们就可以实现 Stackless Coroutine。下面的函数是对上面代码所抽象的协程的实现：

```rust

static mut PRODUCER_STATE: i32 = -1;
static mut PRODUCER_DATA: i32 = 0;

fn producer() -> {
    match PRODUCER_STATE {
        -1 => {
            PRODUCER_STATE = 0;
            // 产生数据 1，然后返回调用者
            PRODUCER_DATA = <数据1>;
        }
        0 => {
            PRODUCER_STATE = 1;
            // 产生数据 2，然后返回调用者
            PRODUCER_DATA = <数据2>;
        }
        1 => {
            PRODUCER_STATE = 2;
            // 产生数据 3，然后返回调用者
            PRODUCER_DATA = <数据3>;
        }
        _ => {
            PRODUCER_STATE = i32::MAX; // 标识协程过程的结束
            // 任务结束，没有什么要做的
        }
    }
}
```

接下来，我们做一层包装，使得我们的协程看起来更加像一个函数：

```rust
fn get_data() -> Option<i32> {
    if PRODUCER_STATE == i32::MAX {
        None
    } else {
        producer();
        Some(PRODUCER_DATA)
    }
}
```

当我们调用一次 `get_data()` 函数，我们的协程就会执行一次，然后返回一个数据。这样我们就实现了 Stackless Coroutine。

不过我们这个 Stackless Coroutine 仍然非常 basic。倒不是说状态比较少。因为我们的状态是确定的，所以我们可以很容易地将状态转换为代码。事实上，像 .NET 这样的语言就会在编译期就将 Coroutine 方法转换为状态机函数，我们在编写代码时可以直接编写下面的代码：

```C#
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

我想说的问题在于，我们的状态仍然是全局的，不与调用上下文关联，导致我们在不同地方想多次调用这个协程时，会出现问题。因此我们需要将状态与调用上下文关联，这就是下面要讲的 Generator。最显而易见的问题是，表示状态机状态的字段以及返回值的字段都是全局的，这样就无法在多个地方调用这个协程，因为这个协程的状态是全局的，不与调用上下文关联。因此我们定义一个 Struct。现在让我们尝试实现上面的 Fibonacci 协程：

```rust
enum State {
    Uninitialized,
    State1
    State2,
    State3,
    Finished,
    Corrupted,
}

struct FibonacciCoroutine {
    state: i32,
}

impl Fibonacci {
    fn new() -> Self {
        Self {
            state: State::Uninitialized,
        }
    }
}
```

当我们调用协程时，协程函数事实上需要与协程上下文关联，因此看起来像这样：

```rust
impl Fibonacci {
    fn next(&mut self) -> Option<i32> {
        match self.state {
            State::Uninitialized => {
                self.state = State::State1;
                Some(0)
            }
            State::State1 => {
                self.state = State::State2;
                Some(1)
            }
            State::State2 => {
                self.state = State::State3;
                // ???
            }
            _ => {
                self.state = State::Finished;
                None
            }
        }
    }
}
```

这样我们就实现了一个与调用上下文关联的协程。现在我们的无栈协程能够处理前两种状态了，也就是

```C#
public static IEnumerable<int> Fib()
{
    int prev = 0, next = 1;
    yield return prev;
    yield return next;
}
```

但是后面的状态就麻烦了。当我们处于 State2 时，我们需要计算下一个数，然后返回。我们需要知道 prev 和 next 的值，但是这两个值是在上一个状态中计算的，我们无法在这个状态中访问这两个值。因此我们需要将这两个值保存到协程上下文中。让我们改造一下我们的状态机，将函数的所有**局部变量**都保存到协程上下文中：

```rust
struct FibonacciCoroutine {
    state: i32,
    prev: i32,
    next: i32,
}
```

然后我们的协程函数就变成了这样：

```rust
impl FibonacciCoroutine {
    fn next(&mut self) -> Option<i32> {
        match self.state {
            State::Uninitialized => {
                self.state = State::State1;
                self.prev = 0;
                self.next = 1;
                Some(0)
            }
            State::State1 => {
                self.state = State::State2;
                Some(1)
            }
            State::State2 => {
                self.state = State::State3;
                // Watch this!
                let sum = self.prev + self.next;
                self.prev = self.next;
                self.next = sum;
                Some(sum)
            }
            _ => {
                self.state = State::Finished;
                None
            }
        }
    }
}
```

或者用更加 Type-safe 的方式：

```rust
enum FibonacciCoroutine {
    Uninitialized,
    State1,
    State2,
    State3 {
        prev: i32,
        next: i32,
    },
    Finished,
}

impl FibonacciCoroutine {
    fn new() -> Self {
        Self::Uninitialized
    }

    fn next(&mut self) -> Option<i32> {
        match self {
            Self::Uninitialized => {
                *self = Self::State1;
                Some(0)
            }
            Self::State1 => {
                *self = Self::State2;
                Some(1)
            }
            Self::State2 => {
                *self = Self::State3 { prev: 0, next: 1 };
                Some(1)
            }
            Self::State3 { prev, next } => {
                let sum = *prev + *next;
                *prev = *next;
                *next = sum;
                Some(sum)
            }
            _ => {
                *self = Self::Finished;
                None
            }
        }
    }
}
```

可以看到，我们通过将局部变量保存到协程上下文中，解决了这个问题。这就是无栈协程的基本原理。你可以思考一下，这个过程和有栈协程保存栈和寄存器的过程有什么不同，它们为什么明明看起来这么不同，但是实际上却是一样的。

# Iterator

Iterator 事实上就是一种无栈协程，只是是手动编写的。也并不总是一个显式状态机，但是它的实现原理和无栈协程是一样的。

这里我给出一个遍历数组和前序遍历二叉树的例子：

## Enumerating an array

```rust
struct ArrayIterator<'a, T> {
    array: &'a [T],
    index: usize,
}

impl<'a, T> ArrayIterator<'a, T> {
    fn new(array: &'a [T]) -> Self {
        Self {
            array,
            index: 0,
        }
    }
}

impl<'a, T> ArrayIterator<'a, T> {
    pub fn next(&mut self) -> Option<&'a T> {
        if self.index < self.array.len() {
            let result = &self.array[self.index];
            self.index += 1;
            Some(result)
        } else {
            None
        }
    }
}
```

## Post-order traversal of a binary tree
```rust
enum BinaryTree<'a, T> {
    Leaf(&'a T),
    Node(&'a T, Box<BinaryTree<'a, T>>, Box<BinaryTree<'a, T>>),
}

struct BinaryTreeIterator<'a, T> {
    stack: Vec<&'a BinaryTree<'a, T>>,
}

impl<'a, T> BinaryTreeIterator<'a, T> {
    fn new(tree: &'a BinaryTree<'a, T>) -> Self {
        let mut stack = Vec::new();
        stack.push(tree);
        Self { stack }
    }
}

impl<'a, T> BinaryTreeIterator<'a, T> {
    pub fn next(&mut self) -> Option<&'a T> {
        while let Some(node) = self.stack.pop() {
            match node {
                BinaryTree::Leaf(leaf) => return Some(leaf),
                BinaryTree::Node(node, left, right) => {
                    self.stack.push(left);
                    self.stack.push(right);
                    return Some(node);
                }
            }
        }
        None
    }
}
```

# Asynchronous Programming

C# 最早引入了异步编程模型和`async/await`编程模式，也是将异步应用得最成功的语言。，通过 `async` 和 `await` 关键字，使得异步编程变得非常简单。Rust 也引入了异步编程模型，通过 `async` 和 `await` 关键字，使得异步编程变得非常简单。

这里引用 Microsoft .NET team 员工 Stephen Toub 的文章，你可以在这里找到原文：[Stephen Toub - How Async/Await Really Works in C#](https://devblogs.microsoft.com/dotnet/how-async-await-really-works/#async/await-under-the-covers)。直接从我给的地方开始阅读即可。

Async/Await 编程模型最大的好处在于，你只需要做很小的修改，就可以将同步代码转换为异步代码。这样就可以非常方便地编写异步代码，而不需要关心异步编程的细节。很小的修改在于：

1. 为方法添加 `async` 关键字，表示这个函数是一个异步函数
2. 将方法中调用的方法全部替换为异步方法，并添加 `await` 关键字来等待异步方法的返回值

然后就结束了！考虑以下两段代码，一眼看上去几乎没有什么区别：

Synchronous Version:
```C#
public void CopyStreamToStream(Stream source, Stream destination)
{
    var buffer = new byte[0x1000];
    int numRead;
    while ((numRead = source.Read(buffer, 0, buffer.Length)) != 0)
    {
        destination.Write(buffer, 0, numRead);
    }
}
```

Asynchronous Version:
```C#
public async Task CopyStreamToStreamAsync(Stream source, Stream destination)
{
    var buffer = new byte[0x1000];
    int numRead;
    while ((numRead = await source.ReadAsync(buffer, 0, buffer.Length)) != 0)
    {
        await destination.WriteAsync(buffer, 0, numRead);
    }
}
```

这个过程与无栈协程非常相似，只是我们的状态是确定的，因此我们可以将状态转换为代码。首先我们可以构建以下状态机对象：

```C#
struct CopyStreamToStreamAsyncStateMachine : IAsyncStateMachine
{
    public int state;
    public Stream source;
    public Stream destination;
    private byte[] buffer
    ...
}
```

然后我们编写的方法被替换成一个状态机初始化器，并返回一个状态机对象

```C#
public Task CopyStreamToStreamAsync(Stream source, Stream destination)
{
    var stateMachine = new CopyStreamToStreamAsyncStateMachine
    {
        state = 0,
        source = source,
        destination = destination,
        buffer = new byte[0x1000],
    };
    return stateMachine.Task;
}
```

当我们调用这个方法时，实际上并不会执行这个方法，而是返回一个状态机对象（C# 有些不同，具有 Fire and forget 的特性，但是这里我们不讨论这个问题）。然后们要调用这个方法时，实际上需要使用 await 关键字。

这个关键字的作用是，将当前的方法挂起，然后将控制权交给状态机对象，然后状态机对象根据当前的状态，执行相应的代码。这个过程就是异步编程的基本原理。

也就是说，对于下面的语句`await CopyStreamToStreamAsync(src, dst);`，实际上会被转换为：

```C#
while (stateMachine.MoveNext()) // 轮询一次状态机，如果有工作，就去工作，如果没有，返回 false
{
    yield(); // 这里的 yield，就是跟上面的 yield 一样，将当前任务挂起，然后返回调用者
    // 在实际的代码中，就是 return.
}
result = stateMachine.Result; // 获取状态机的返回值
```

这就是异步编程的基本原理。你可以看到，异步编程的本质就是无栈协程，只是我们的状态是确定的，因此我们可以将状态转换为代码。这就是异步编程的基本原理。
