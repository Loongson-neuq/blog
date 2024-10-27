---
title: "[OS Week2] Get started with Rust"
date: 2024-10-27
description: 第二周 OS 课程 Slides
tags: 
    - Rust
    - System Programming
categories:
    - OS
    - 课程
    - tutorial
---

本次课程的部分内容摘抄自 [Microsoft Learn](https://learn.microsoft.com/learn) 上的 [Get started with Rust](https://learn.microsoft.com/zh-cn/windows/dev-environment/rust/) 课程。

## Overview

Rust 是一种系统编程语言，因此可用于编写系统（如操作系统）。 但它也可用于编写性能和可信度很重要的应用程序。 Rust 语言语法可以与 C++ 语法相媲美，提供了与新式 C++ 相当的性能；

> ❕INFO
> 
> &nbsp; C++是什么垃圾也配

对于许多有经验的开发人员来说，Rust 在编译和运行时模型、类型系统和确定性终止化方面都是正确的。

> ❕INFO
> 
> &nbsp; 在系统编程中，控制流可能被扰乱，某些优化并不完全正确，需要开发者具有丰富的经验对生成的汇编代码进行审查。不能完全依赖编译器。

此外，Rust 的设计保证了内存安全，而不需要进行垃圾回收。

那么，我们为什么要选择 Rust 作为 Windows 的最新语言投影呢？ 其中一个因素是，Stack Overflow 的年度开发人员调查显示，Rust 是目前为止年复一年最受欢迎的编程语言。 虽然你可能会发现此语言有陡峭的学习曲线，但一旦你越过了这个峰，就很难不爱上它了。

## Rust development toolset/ecosystem

- `crate` 是 Rust **编译**和**链接**单元。 crate 可以源代码形式存在，然后能够被处理成以二进制可执行文件（简称二进制文件）或二进制库（简称库）形式存在的 crate 。通常一个 crate 就是一个 **project**。

- Rust 项目称为`包`。 一个包可以包含一个或多个 crate，以及描述如何生成这些 crate 的 `Cargo.toml` 文件。更准确的说法是 **solution**。

- `rustup` 是 Rust 工具链的安装程序和更新程序。

- `Cargo` 是 Rust 包管理工具的名称。也用于构建、测试和发布 Rust 项目。

- `rustc` 是 Rust 编译器。 大多数情况下，你不会直接调用 rustc，而是通过 Cargo 间接调用它。

- `crates.io` (https://crates.io/) 是 Rust 社区的 crate 注册表。crates.io 托管大量的 crate，可以通过 Cargo 下载，并自动解决依赖关系。

## Progarmming language Concepts

### Type syetem

#### Strong typing? Weak typing?

在编程语言的类型系统中，强类型（strong typing）和弱类型（weak typing）是两个核心概念，用于描述编程语言对数据类型的约束程度。

### 强类型（Strong Typing）
强类型语言要求变量的数据类型在使用时要严格遵守，通常不允许不同类型之间的隐式转换。例如，Rust、.NET都属于强类型语言。以下是强类型的特征：

1. **严格的类型检查**：强类型语言在**编译期**或运行期都会进行严格的类型检查，如果类型不匹配，代码就会报错。例如，在Rust中将整数赋值给一个字符串类型的变量会直接报错，而不会自动转换类型。

2. **安全性**：强类型语言通常可以防止许多潜在的错误，因为它们在操作不兼容类型时会立即报错，帮助程序员更早地发现错误。例如在Rust中，试图将整型变量作为浮点型来处理，编译器会立即提醒，这避免了许多运行时错误。

3. **类型转换需要显式**：在强类型语言中，类型转换一般需要显式声明，编译器不会进行隐式转换。例如在Rust中，`let x: i32 = 10; let y: f64 = x as f64;`。`as`关键字显式地将`i32`类型转换成了`f64`。

> ⚠️ NOTE
> 
> &nbsp; Rust 仅在安全的情况下允许隐式转换，例如`let x: i32 = 10; let y: f64 = x;`是合法的，因为`i32`可以隐式转换成`f64`。并且 Rust 还有**自动解引用**机制，实现了一定程度的隐式转换。

4. **内存安全**：强类型语言更容易实现内存安全，因为严格的类型系统有助于防止无效的内存访问。例如，Rust的所有权系统和借用检查在类型系统中嵌入了内存管理的概念，确保了线程安全和内存安全。

### 弱类型（Weak Typing）
弱类型语言对类型的限制较少，通常允许不同类型之间的隐式转换，例如C和JavaScript都具有弱类型的特性。以下是弱类型的特征：

1. **更宽松的类型转换**：弱类型语言在不同类型之间可以自由转换。例如在JavaScript中，`"5" + 10` 会自动将数字`10`转换成字符串，然后得到字符串`"510"`。这种隐式转换提供了便利，但也可能导致难以发现的错误。

2. **更高的灵活性**：弱类型允许开发者快速编写代码，减少了类型检查的约束，代码在运行时的适应性更高。例如，JavaScript中的函数可以接受任何类型的参数，不必进行严格的类型定义。

3. **容易出错**：由于类型不严格，弱类型语言更容易引发错误，尤其是在无意中发生隐式类型转换时。比如在C语言中，整数和指针之间可以自由转换，这会导致很多内存和安全问题。

### Rust 的强类型优势
Rust 是一种强类型系统的语言，其设计注重内存安全和性能，通过严格的类型检查和所有权模型来保证代码的可靠性。Rust 的强类型特性让开发者在编译时可以捕捉到许多潜在的错误，减少了运行时的崩溃风险，同时通过显式转换机制避免了隐式转换带来的隐患。

最后，类型是仅对于高级语言抽象层的概念，在底层的硬件层，一切都是二进制的。所有的类型实例不过是一段 memory block，在汇编中我们使用同样的指令来操作所有的类型。因此，在 C 这种仅对汇编进行薄封装的语言中，类型的概念并不是很重要。

### Systems programming language

“系统编程语言”通常指的是适合底层开发、硬件交互和性能优化的语言，与更高层抽象的应用编程语言相比，它们有一些独特的特点：

1. **直接硬件访问和内存控制**(Most important to us)：
   系统编程语言通常支持对硬件和内存进行低层次的访问，例如手动管理内存（Rust、C/C++的`malloc/free`或`new/delete`）。这让开发者能精确控制程序的内存分配和释放，提高性能和资源利用率。

2. **高效的执行性能**：
   系统编程语言（如Rust、C、C++）通常会编译成原生机器码（针对特定架构及操作系统的汇编指令），以确保代码在执行时的效率和速度。这在操作系统、嵌入式系统等需要实时响应和高效性能的场景中尤为重要。

> ⚠️ NOTE
> 
> &nbsp; 不完全正确，事实上，JIT 和 GC 的组合更能够在保证极端性能的完全释放和最大延迟。只是 JIT 依赖运行时，并且 GC 不能保证确定性时延。这些缺陷在系统编程中是不可接受的。因为运行时依赖操作系统。而 GC 导致的不确定性时延会导致系统的不可预测性。

3. **细粒度的并发控制**：
   系统编程语言支持低级并发控制（如Rust中的无锁数据结构、C++的线程库和原子操作）。Rust特别强调安全的并发，通过借用检查器和所有权系统来避免数据竞争，帮助在保持并发性能的同时防止线程安全问题。

4. **内存安全**：
   像Rust这样的现代系统编程语言注重内存安全，避免空指针和悬空指针等问题。Rust的所有权系统在编译期防止了数据竞争、悬挂引用和双重释放等内存问题，大幅降低了由于内存管理引发的漏洞风险。

5. **零成本抽象**：
   系统编程语言（特别是Rust和C++）支持高效的抽象机制，允许编写高性能、模块化的代码。

系统编程语言的这些特点使它们适合于操作系统、驱动程序、嵌入式系统、数据库引擎和游戏引擎等对性能和硬件直接交互有严格要求的场景。相比之下，高层次的编程语言（如Python、JavaScript）更适合于快速开发和构建应用程序接口（API）、数据处理或前端交互，因为它们提供了更丰富的标准库、内置内存管理和更高的抽象能力，但牺牲了一部分性能和对系统的直接控制。

## Setup your own Rust development environment

### Prerequisites

#### Windows

由于 Rust 依赖 C 编译套件用于编译的最终阶段，因此在 Windows 上安装 Rust 时，需要安装 C 编译套件。 Windows 上的 C 运行时主要是 MSVS，因此你需要安装 Microsoft Visual C++。你可以下载 [Microsoft C++ Build Tools](https://visualstudio.microsoft.com/visual-cpp-build-tools/)，也可以（推荐）首选直接安装 [Microsoft Visual Studio](https://visualstudio.microsoft.com/downloads/)。安装 Community 版本的 Visual Studio 即可。安装时仅勾选 **Desktop development with C++** 选项即可。

[Detailed instructions](https://rust-lang.github.io/rustup/installation/windows-msvc.html)

#### Linux

安装对应平台和与宿主主机相同的架构的 GCC 即可。

### Development environment

#### RustRover?

RustRover 是 JetBrains 开发的 Rust 语言的 IDE，它是一个基于 IntelliJ 平台的 IDE，提供了 Rust 语言的代码编辑、调试、自动补全、代码重构等功能。RustRover 也支持 Cargo 包管理工具，可以帮助你更方便地管理 Rust 项目。类似于 Idea, PyCharm, Clion 等。并且 RustRover 社区版是免费的。

但是！

不要用！由于我们的最终目的是系统编程，RustRover 是面向用户级应用的 IDE，它的调试器和代码提示等功能对于系统编程并不友好。

因此我要求大家使用 Cargo 命令行　+　你自己喜欢的文本编辑器（VSCode, Vim, Emacs, Sublime Text, Notepad++）进行开发。

#### Install Rust

不管你用的什么操作系统，打开 [`https://rustup.rs`](https://rustup.rs)。

如果你是 Windows 用户，点击最上面的`rustup-init.exe`下载并运行。
如果你是 Linux 用户，复制网址下面的命令到终端运行。

然后根据提示，一路回车即可。

> 某些发行版可能会将 `rustup` 添加至软件源，当然上述方式也可以

#### Verify installation

打开终端，输入`cargo --version`，如果输出了版本号，说明安装成功。

```powershell
PS C:\Users\Caiyi Hsu> cargo --version
cargo 1.80.0 (376290515 2024-07-16)
```

### Rust with Visual Studio Code

确保你已经安装了 Visual Studio Code 和 Cargo。打开 Visual Studio Code，安装 `rust-analyzer` 插件。

More usages:

[Rust with Visual Studio Code](https://code.visualstudio.com/docs/languages/rust)

### Rust basic syntax

<!-- TODO 该干啥了？ -->

[The Rust Programming Language](https://doc.rust-lang.org/stable/book/)

[Unofficial Chinese Translation](https://kaisery.github.io/trpl-zh-cn/)

### 下集预告

有重量级内容，敬请期待！

[Understanding Rust via Memory management](https://loongson-neuq.pages.dev/p/advanced-rust/)

做了 Rustlings 的同学可以提前看一下。
