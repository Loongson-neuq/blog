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
    String str = "Hello, world";
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

The stack is array-like, but not a real array. It's a memory block, a memory block means that you can access whatever you want with the memory block. Stack is just a convention which constraints the way we access the memory block - FILO or LIFO.

An example is that, in Rust, we don't have a specific data structure for Stack. In C++, in python, in .NET you would have a Type like `Stack<T>`, but in Rust, we don't have that. We just use `Vec<T>`(`vector<T>` in Cpp). As long as you only call push and pop method.

###### Why the stack is faster than the heap?

When we access the object on the stack, we know where the object is, we access it with a single instruction which contains the frame pointer and the offset. 

But when we access the object on the heap, we have to read the pointer to a register, and then access the object with the pointer. That's two instructions.

Also, accessing instance on the heap may cause cache miss, which is the REAL reason that heap is slower than the stack.

##### Micro views of the stack allocation

<!-- TODO: with assembly code -->

### Data inconsistency issue in Multi-threaded scenarios

Watch a demo

or Download and run yourself

[https://github.com/Loongson-neuq/blog/tree/main/content/post/move-semantic/MuitlThreadDemo](https://github.com/Loongson-neuq/blog/tree/main/content/post/move-semantic/MuitlThreadDemo)

Note: You need to have a .NET 8 runtime installed to run the demo. Install it from
[dot.net](https://dot.net/download)

#### Lock

#### Read-write Lock

<!-- TODO -->

## Rust

### Single owner rule

<!-- TODO -->

### Single mutable referece rule

<!-- TODO -->
