---
title: Advanced Rust
date: 2024-10-25
description: What the fuck is Move Semantic, Ownership and Borrowing rule
tags: 
    - Rust
categories:
    - OS
    - 课程
    - Rust
---

## Prerequisite

### Memory Management

#### Stack

Remember:

All variables you _defined_ in a function/method is allocated on the _STACK_, even non-fixed size types and reference types.

But the thing you can _access_ directly or actually stored on the stack must be fixed size types.

```Cpp
void foo()
{
    int i = 0;
    int* p = &i;
    Object* obj = new Object();
    std::string str = "Hello, world";
    StructType value = StructType {1, 2, 3};
}
```

All variable you can use directly is allocated on the stack (frame).

##### This is not what I heard from others/the Internet!

- Reference types
- String
- Vector<T>
- Dynamic allocation (eg. malloc)

People all told me that these types are allocated on the HEAP!

I would say, that's the root of your confusion. Why don't we talk about the Heap first then?

And I used two different term: `define` and `access`

#### Heap

Heap is a large contigous memory managed by both the **Operating System** and **the language runtime**.

Runtime may be the **Standard library**, eg. the libC
or *GC*, eg. Go, .NET ...

We don't have to care about what's happening in the background, at least for now.

We only care about two functions:

1. One for alloacting memory
```C
void* alloc(size_t size_of_bytes);
```

which is `malloc`, `new`, `new[]` ...

2. Another for returning allocated memory
```C
void free(void* ptr);
```

which is `free`, `delete`, `delete[]` ...

One interesting thing you should have noticed is, when we returning a piece of memory, we only need to pass the pointer to the function, but not the size of the memory.

Why? Because *Someone* must have recorded the size of the memory when it was allocated.

The guy is the **Memory Allocator**, a part of the runtime.

When you call alloc method, the allocator simply find a piece of memory that is large enough to hold the data you want to store, and then record the size of the memory.

But where is the large piece of meory(the allocator uses) from?

The answer is the **Operating System**.

The OS provides a system call to allocate a piece of memory, and the allocator will use this system call to get the memory.

##### Unix-like system

The unix-like system provides a system call `brk` or `sbrk` to allocate memory.

The system call is like the function below, but is NOT a real function.

```C
int brk(void *addr);
void *sbrk(intptr_t increment);
```

Additional info: [brk syscall](https://www.man7.org/linux/man-pages/man2/brk.2.html)

A note from the page:
```ascii
    Avoid using brk() and sbrk(): the malloc(3) memory allocation
    package is the portable and comfortable way of allocating memory.
```

When you call the brk, the OS will allocate memories Right After the end of the previous memory block. So when you constently call the brk, the memory block will grow larger and larger, but the memory block is always contiguous.

The `malloc` function will use the `brk` system call to allocate memory, and uses the memory block for the allocation and recording the size of the memory.

How do the OS gurantee the memory block is contiguous or the end of the memory block always have enough space for the next allocation?

The answer is, the OS atually doesn't gurantee that. Actually, memory pieces that a user program feels contiguous may *NOT* be contiguous in the physical memory. The OS uses a technique called **Virtual Memory** and **Page Table** to make the memory block contiguous in the user program's view. 

The OS simply maps the virtual memory to the physical memory. This technique is done with the help of hardwares like MMU, a part of the CPU.

##### Windows

Windows does basically the same thing, but with a different system call, and the system call is wrapped by the `VirtualAlloc` function form the `Kernel32.dll`.

```Cpp
LPVOID VirtualAlloc(
  [in, optional] LPVOID lpAddress,
  [in]           SIZE_T dwSize,
  [in]           DWORD  flAllocationType,
  [in]           DWORD  flProtect
);
```

Additional info: [VirtualAlloc](https://learn.microsoft.com/zh-cn/windows/win32/api/memoryapi/nf-memoryapi-virtualalloc)

Basically same as the `brk` system call from the Unix-like system, but with more options.

#### Continue to talk about the Heap

Think about all code you write about the heap, all allocated object you want to access, you must have a pointer to the object.

That's exactly the problem.

Take a look at the words again:

All variables you _defined_ in a function/method is allocated on the _STACK_, even non-fixed size types and reference types.

What you can access directly is Only the pointer, not the object itself. You MUST uses the pointer to access the object. And the pointer is allocated on the stack.

Note that reference type is basically safe pointer, the reference is allocated on the stack, and the object is allocated on the heap.

There's still one thing we have to talk about:

```Cpp
vector<int> vec1;
vector<int>* vec2 = new vector<int>();
```

What's the difference between the two?

I want to talk about the implementation and underlying of the `vector` first.

When we talk about the dynamic array, we always have a pointer to the actually array, and the size of the array. So the vector is like this:

```C
// Note that I declared it as a struct, I'll explain it later
struct Vector<T>
{
    T* data;
    size_t size;
    size_t capacity;
}
```

The first line of the code above actually allocated the three fields on the stack, Pointer to the data, size of the data, and the capacity of the data.

The second line of the code above actually allocated the three fields on the heap, Pointer to the data, size of the data, and the capacity of the data. And we uses a pointer to access the object.

Values Allocated in Rust without Smart Pointer were all like the first line of the code above. Which is, All fields allocated on the stack. 

Smart pointer is like the Vector above(actually vector is a smart pointer), it allocates the fields on the stack, including a pointer to the actual object.

#### Fixed size?

Actually, a class defination determined that class is also fixed size, just like the struct.

But class has a special feature: **Polymorphism**.

That's is, when you reference an class object, it may not be the exactly the type, but a derived type. And derived type may have more fields than the base type. Which means the size of the object is not fixed.

#### Struct and Class

Struct instance were allocated on the stack by default, and class instance were allocated on the heap by default.

Since the struct is allocated on the stack, we Call it **Value Type**. Value means we are not accessing it by a pointer, reference or something like that. But directly, we can touch it.

##### Why I declared the `Vector` as a struct?

Simply because C++ allows you allocate a class instance on the stack. And I don't want to make things confusing for people uses other languages.

although this loses the most important feature of the class: polymorphism, which makes a class like a struct.

#### Why All instances allocated on the stack MUST be fixed size?

Allocation calls for Value Types were generated at the the compile time, and the size of the object must be known at the compile time.

Also, the allocations calls hardcoded the size of the object, which means the size of the object must be fixed.

Since the class has a special feature: Polymorphism, the size of the object is not fixed, so we can't allocate it on the stack. But without Polymorphism, we can allocate a class instance on the stack, just like C++ does.

##### Micro views of the stack

I've talked about where is the heap, but not the stack. Stack is also a contiguous memory block. For simplicity, I'll say that stack were managed by the OS, although it's not true in some cases.

Stack of a program is Program Stack or Call Stack or Execution Stack. It's used to store the local variables, function parameters, and the return address of the function.

When a process starts, the OS will allocate a memory block for the process. And make a certain register point to the end of the memory block(High address). This register is called the **Stack Pointer**, which is `rsp` in x86_64 and `sp` in RISC-V.

When you try allocate an instance on the stack, like, a int, we simply minus the stack pointer.

You might know stack is FILO or LIFO, but that doesn't mean we have to pop the stack if we want to access the inner object. The FILO or LIFO is only for the stack frame, which keeps everything essential to allow function calling/returning.

Since all instance on the stack is fixed size, all of their position is fixed, we know where the object is at the compile time. We know that all local variables can be accessed by frame pointer plus a fixed offset.

Actually, we have to uses **address** to access all memory blocks, including those allocated on the stack. But we seems never uses a pointer. That's because when we have to uses a pointer, the address can NOT be known at the compile time, we have to fetch the address at runtime. But for those allocated on the stack, the known local variables, we know where they are, we can access them directly with `sp + offset`, where offset is a constant. So the address(`sp + offset`) is **embedded** in the instruction.

The stack is array-like, but not a real array. It's a memory block, a memory block means that you can access whatever you want with the memory block. Stack is just a convention which constraints the way we access the memory block - FILO or LIFO.

An example is that, in Rust, we don't have a specific data structure for Stack. In C++, in python, in .NET you would have a Type like `Stack<T>`, but in Rust, we don't have that. We just use `Vec<T>`(`vector<T>` in Cpp). As long as you only call push and pop method.

###### Why the stack is faster than the heap?

When we access the object on the stack, we know where the object is, we access it with a single instruction which contains the frame pointer and the offset. 

But when we access the object on the heap, we have to read the pointer to a register, and then access the object with the pointer. That's two instructions.

Also, accessing instance on the heap may cause cache miss, which is the REAL reason that heap is slower than the stack.

##### Micro views of the stack allocation

Having talked about the stack so much, you might wonder when do we push and pop.

The name of call stack implied that the stack has strong connection with *Fuction Call*.

You must have seen stack trace when the runtime throw an exception. The stack trace is actually the call stack.

Stack trace when a exception is thrown in .NET:
```ascii
Unhandled exception. System.Exception: Exception from Buz
   at Program.Buz() in /home/caiyi/loongson-blog/content/post/move-semantic/StackTraceDemo/Program.cs:line 24
   at Program.Bar() in /home/caiyi/loongson-blog/content/post/move-semantic/StackTraceDemo/Program.cs:line 17
   at Program.Foo() in /home/caiyi/loongson-blog/content/post/move-semantic/StackTraceDemo/Program.cs:line 11
   at Program.Main(String[] args) in /home/caiyi/loongson-blog/content/post/move-semantic/StackTraceDemo/Program.cs:line 5
```

Remove some of the information, we get:
```ascii
   at Program.Buz() in Program.cs:line 24
   at Program.Bar() in Program.cs:line 17
   at Program.Foo() in Program.cs:line 11
   at Program.Main(String[] args) in Program.cs:line 5
```

Code can be obtained from [https://github.com/Loongson-neuq/blog/tree/main/content/post/move-semantic/StackTraceDemo](https://github.com/Loongson-neuq/blog/tree/main/content/post/move-semantic/StackTraceDemo)

Why do we have so many functions from the Stack trace? Because the functions are called nestedly.

The top function is where the exception was actually thrown, and the lower function is where the top function was called. Since the main thread of our program begins with `Main()` function, the bottom function is always `Main()`.

When a function is called, the runtime will push a new frame to the stack, and when the function returns, the runtime will pop the frame from the stack.

**A frame stores everyting essential to restore the envrionment before the function was the frame call another function.**

We know that CPU must read datas to its own *register* to do the calculation, and the register is limited. So we have to store the local variables and other things in somewhere else. That's the stack frame. A frame only stores datas of the function, and the frame is popped when the function returns.

###### Let's look at some assembly code

Having talked about how the stack so much, you might think it's rather complicated to push and pop the stack. But it's not. As I said before, we only have to minus the stack pointer and the minused size of memory is yours! To return the memory, we only have to add the size to the stack pointer. 

**NO NEED TO CLEAR THE MEMORY when we push/pop the stack. CAN YOU THINK ABOUT WHY?**

The same code as the one at the beginning of the article:

```Cpp
#include <string>

class Object
{
    private:
        int _value;
};

struct StructType
{
    int value1;
    int value2;
    int value3;
};

void foo()
{
    int i = 0;
    int* p = &i;
    Object* obj = new Object();
    std::string str = "Hello, world";
    StructType value = StructType {1, 2, 3};
}
```

The assembly code of the function `foo` is like this:

```assembly
section .data
str db "Hello, world", 0

section .text
global foo

foo:
    ; 函数开始，保存栈帧
    push rbp                     ; 保存原始栈帧
    mov rbp, rsp                 ; 设置新的栈帧

    ; int i = 0;
    mov dword ptr [rbp-4], 0     ; 将变量 i 初始化为 0，并保存在栈中偏移 -4

    ; int* p = &i;
    lea rax, [rbp-4]             ; 取得变量 i 的地址
    mov qword ptr [rbp-8], rax   ; 将 p 指向 i 的地址并保存偏移 -8

    ; Object* obj = new Object();
    mov edi, 4                   ; Object 的大小为 4 字节
    call _Znwm                   ; 调用 operator new
    mov qword ptr [rbp-16], rax  ; 保存返回的对象地址到 obj 中（偏移 -16）

    ; std::string str = "Hello, world";
    lea rdi, [rel str]           ; 将字符串地址加载到 rdi
    lea rsi, [rbp-32]            ; 准备 str 变量的栈位置（偏移 -32）
    call _ZNSsC1EPKc             ; 调用 std::string 构造函数

    ; StructType value = StructType {1, 2, 3};
    mov dword ptr [rbp-48], 1    ; 将 1 存储到 value.value1（偏移 -48）
    mov dword ptr [rbp-44], 2    ; 将 2 存储到 value.value2（偏移 -44）
    mov dword ptr [rbp-40], 3    ; 将 3 存储到 value.value3（偏移 -40）

    ; 函数结束，恢复栈帧并返回
    mov rsp, rbp                 ; 恢复原始栈指针
    pop rbp                      ; 弹出原始栈帧
    ret                          ; 返回
```

We even do have to actually DO a allocation operation. We just know that where every variable should be and read/wirte the place directly. See instructions like `mov dword ptr [rbp...], ...`

Additinally, `push` and `pop` instructions are also used to store/access the stack, but they are just pseduo instructions. The real instructions are `mov` and `add`.

From the assembly code, we can see that how much we push/pop the stack is determined at the compile time, hard coded in the assembly instructions. You should know why all value types must be fixed size now.

Since we only care about current frame, which is at the top of the stack, we don't need to have store the size of every frame, the base of the whole stack, or the end of the stack. We only need to store the stack pointer, which is the top of the stack, or the base of the current frame.

Then I'll talk about why do we never clean the stack.

When we pop a frame, the depth of stack just got smaller, and there will be no chance of reading uninitialized data or overwriting the data.

When we push a frame, we always write the data before we can access it. Remember that the Compiler always say `Uninitialized variable` when you try to access a variable before you write it. The compiler gurantee that you will never read uninitialized data at the compile time, so we don't have to clear the memory, which makes function call faster.

### Data inconsistency issue in Multi-threaded scenarios

Watch a demo

or Download and run yourself

[https://github.com/Loongson-neuq/blog/tree/main/content/post/move-semantic/MuitlThreadDemo](https://github.com/Loongson-neuq/blog/tree/main/content/post/move-semantic/MuitlThreadDemo)

Note: You need to have a .NET 8 runtime installed to run the demo. Install it from
[dot.net](https://dot.net/download)

#### Lock

#### Read-write Lock

We don't really have to lock the whole object always. In fact, the lock is to resolve the data inconsistency issue, and the data inconsistency issue is caused by the write operation.

If all threads is just *reading* the data, the data never changes, it's just there. So we can allow multiple threads to read the data at the same time as long as there's no thread is writing the data.

So the key is:

- Only allow one thread to write the data
- Allow multiple threads to read the data
- Can't have read lock and write lock at the same time

The lock that can resolve the issue is called **Read-Write Lock**.

Think about Rust's borrow rule? Did you find the similarity?

## Rust

**WARN: the follow content were generated by ChatGPT**
<!-- TODO: Rewrite it -->

### Understanding Rust's Ownership and Borrowing Rules through the Lens of Read-Write Locks and Stack Memory Management

Rust’s **Single Owner Rule** and **Single Mutable Reference Rule** can be better understood when examined through the principles behind **read-write locks** and **stack-based memory management**.

#### Rust’s Ownership and Borrowing Rules as a Read-Write Lock Analogy

In multi-threaded environments, a **read-write lock** is a synchronization mechanism allowing multiple readers to access a resource simultaneously or granting exclusive access to a single writer. Rust’s ownership and borrowing rules mirror this access control strategy, enforcing exclusive or shared access to data at compile time:

1. **Single Owner Rule**: Rust’s concept of a single owner aligns with the idea of an **exclusive lock** on a resource. Only one variable or function can own a piece of data at any point, ensuring exclusive access and avoiding any conflicts in memory access.

2. **Single Mutable Reference Rule**: This rule is conceptually similar to read-write locks and provides two access states:
   - **Shared, Immutable Access**: Multiple immutable references (using `&T`) to a resource are allowed, resembling the behavior of a read lock.
   - **Exclusive, Mutable Access**: Only one mutable reference (using `&mut T`) can exist at any time, akin to a write lock, preventing simultaneous modifications by others and ensuring safe mutation.

By enforcing these rules, Rust’s compiler performs a static analysis to eliminate race conditions and memory conflicts at compile time, achieving thread safety without the runtime overhead of locks.

#### Safe Memory Management with Stack Allocation

Rust’s ownership rules apply to both **stack** and **heap memory** management, maintaining safety and efficiency across both types of allocations:

1. **Stack Memory Management**: In Rust, variables allocated on the stack are assigned a clear, finite lifecycle determined at compile time, corresponding to the stack’s Last-In-First-Out (LIFO) principle. The ownership system prevents issues like double frees, as the ownership rules guarantee that only the active owner has control over memory deallocation.

2. **Heap Memory Management**: When data is allocated on the heap, Rust’s ownership rules still apply, managing the memory lifecycle through single ownership. Heap memory is controlled by the owning variable, and once the variable goes out of scope, the data is automatically deallocated, freeing developers from the need for manual memory management.

Borrowing rules further ensure that data on the heap avoids race conditions. By allowing multiple immutable references (akin to a read lock) but only one mutable reference (like a write lock), Rust’s system dynamically enforces safety similar to a runtime read-write lock.

#### Case Study: Ownership and Borrowing in Action with Concurrency

To illustrate Rust’s ownership and borrowing in action, let’s consider a simple multithreaded example:

```rust
use std::thread;

fn main() {
    let mut data = vec![1, 2, 3];
    
    // Transfer ownership of `data` to the spawned thread.
    let handle = thread::spawn(move || {
        data.push(4); // Mutating `data`, ownership is now exclusively with the new thread.
    });
    
    handle.join().unwrap();
    
    // Any attempt to access `data` in the main thread would result in a compilation error,
    // as ownership has been transferred.
}
```

In this example, ownership of `data` is transferred to the new thread using `move`, meaning the main thread no longer has access to it. This model functions similarly to an exclusive lock but relies on ownership transfer rather than explicit locks. Rust’s ownership system enforces access control here without locks, allowing memory allocation to remain efficient while eliminating data races at compile time.

#### Rust’s Advantage: Compile-Time Lock-Like Guarantees

Compared to traditional lock-based synchronization, Rust’s ownership and borrowing rules rely on compile-time checks to ensure memory safety, removing the need for runtime locks. The Rust compiler statically analyzes a variable’s lifecycle and reference status, ensuring memory access safety without runtime overhead, which both boosts memory efficiency and minimizes errors.

### Conclusion

Rust’s ownership and borrowing model integrates the benefits of read-write locks with stack-based memory management principles. By statically enforcing exclusive or shared access, Rust guarantees thread safety and efficient memory management without the performance costs of locks. This unique approach ensures that concurrent programming in Rust is both efficient and safe by design.
