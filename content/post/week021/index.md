---
title: "[OS Week2] 作业常见问题"
date: 2024-10-22
description: 第二周 OS 作业常见问题
tags:
  - Rust
categories:
  - OS
  - 课程作业
---

## 编码规范

上次课上提醒了，Rust 有 `clippy` 和 `fmt --check` 工具来严格检查代码风格，这次作业中有同学的代码风格不够规范，希望大家能够注意一下。

总的来说，无非就是

- 该有空格的地方别省，不该有的地方别多。

- 不该 mutable 的变量别加 `mut`。
  - 定义变量时就要应该想明白这个变量需不需要是 mutable 的

### 空格

#### 1. 逗号后面要有空格

这也是英语语法的规范，逗号后面要有空格，前面没有。

Good:

```rust
foo(1, 2, 3);
```

Bad:

```rust
foo(1,2,3);
```

#### 2. Operator 前后要有空格

Good:

```rust
let x = 1 + 2;
if x > 1 {

}
```

Bad:

```rust
let x = 1+2;
if x>1 {

}
```

#### 3. 函数定义时，参数列表的括号后要有空格，前面不能有

Good:

```rust
fn foo(x: i32) {

}
```

Bad:

```rust
fn foo (x: i32) {

}

fn foo(x: i32){

}
```

#### 4. 括号内不要有空格

Good:

```rust
foo(1);
```

Bad:

```rust
foo( 1 );
```

#### 5. `:` 标明类型时，与类型间要有空格，前面没有

Good:

```rust
let x: i32 = 1;
```

Bad:

```rust
let x:i32 = 1;
let y : i32 = 1;
```

#### 6. `{` 与前面的元素间要有空格

Good:

```rust
if x > 1 {

}
```

Bad:

```rust
if x > 1{
}
```

#### 7. if 和逻辑表达式间要有空格

Good:

```rust
if x > 1 {

}
```

Bad:

没有 bad，你根本编译不过

#### 8. 不要连续搞多个空行

Good:

```rust
let x = false;

if x {
    println!("x is true");
}
```

Bad:

```rust
let x = false;


if x {
    println!("x is true");
}
```

#### 9. `->` 前后要有空格

Good:

```rust
fn foo() -> i32 {
    1
}
```

Bad:

```rust
fn foo()->i32 {
    1
}
```

#### 10. `{` 不要单独起一行

我不喜欢（因为我是 Microsoft 系的），但是写 Rust 就要符合 Rust 的规范

Good:

```rust
fn foo() {
    println!("foo");
}
```

Bad:

```rust
fn foo()
{
    println!("foo");
}
```

#### 11. 别瞎几把缩进

Good:

```rust
fn foo() {
    println!("foo");
}
```

Bad:

```rust
fn foo(){
println!("foo");
    }
```

#### 12. 没必要的地方别加空格

Good:

```rust
fn foo() {
    println!("foo");
}
```

Bad:

```rust
fn foo (·) {···
    println!("foo");····
}····
```

为了明显用 `·` 表示的空格

该加空格的地方一般加一个就够了，别加一大堆

例如，行尾注释的 `//` 前后一个空格就够了

## 编码规范总结

大概就这些规则最常用，希望大家能够注意一下，写代码的时候多注意一下，不要让自己的代码风格太差。

记不得让 `clippy` 和 `fmt --check` 来检查一下

## Rustlings

### if1

```rust
// if1.rs
//
// Execute `rustlings hint if1` or use the `hint` watch subcommand for a hint.

pub fn bigger(a: i32, b: i32) -> i32 {
    // Complete this function to return the bigger number!
    // Do not use:
    // - another function call
    // - additional variables
    if a > b {
        a
    } else {
        b
    }

    // HINT: in Rust, if you want to compare two values,
    // cmp is PREFERRED over >, <, >=, <=

    // eg.
    // match a.cmp(&b) {
    //     std::cmp::Ordering::Greater => a,
    //     _ => b,
    // }

    // std::cmp 可以处理 parital （偏序）关系，
    // 而运算符会无法处理偏序关系，因此可能出现a > b, a < b 和 a == b 同时为 false 的情况。
    // 搜索：自反性

    // 因此对于自定义类型，最好使用 cmp 方法。

    // 同时，使用 match 控制流还更容易处理多分支的情况。
}
```

### if2

```rust

// if2.rs
//
// Step 1: Make me compile!
// Step 2: Get the bar_for_fuzz and default_to_baz tests passing!
//
// Execute `rustlings hint if2` or use the `hint` watch subcommand for a hint.

pub fn foo_if_fizz(fizzish: &str) -> &str {
    if fizzish == "fizz" {
        "foo"
    } else if fizzish == "fuzz" {
        "bar"
    } else {
        "baz"
    }

    // HINT: in such circumstances, pattern matching is more elegant
    // match fizzish {
    //     "fizz" => "foo",
    //     "fuzz" => "bar",
    //     _ => "baz"
    // }
}
```

### if3

```rust
// if3.rs
//
// Execute `rustlings hint if3` or use the `hint` watch subcommand for a hint.

pub fn animal_habitat(animal: &str) -> &'static str {
    // HINT
    // In real development situations,
    // if you really need to convert an animal to an identifier and then to habitat,
    // You should split this function into two functions.
    // Also, pattern matching is more elegant in this case.
    let identifier = if animal == "crab" {
        1
    } else if animal == "gopher" {
        2
    } else if animal == "snake" {
        3
    } else {
        -1
    };

    // DO NOT CHANGE THIS STATEMENT BELOW
    let habitat = if identifier == 1 {
        "Beach"
    } else if identifier == 2 {
        "Burrow"
    } else if identifier == 3 {
        "Desert"
    } else {
        "Unknown"
    };

    habitat
}
```

### quiz1

```rust
fn calculate_price_of_apples(qty: i32) -> i32 {
    if qty > 40 {
        qty
    } else {
        qty * 2
    }

    // HINT: You will like pattern matching
    // match qty {
    //     0..=40 => qty * 2,
    //     _ => qty
    // }
}
```

### alphabetic?

你可能不信，除了你认为的 ascii 英文字母，以下字符都是 alphabetic 的

- `字` a Chinese character
- `あ` and `ア` Hiragana and Katakana
- `ㅎ` Korean Hangul
- `ａ` Fullwidth Letter(this is NOT `a`)
- and characters from other languages!

in fact, all CJK characters are alphabetic. And letters from other languages like russian, greek, etc. are also alphabetic. But I can't show you because I don't know how to type them.

这些是由 Unicode 规范定义的，所以 Rust 的 `char::is_alphabetic` 方法也是按照 Unicode 规范来判断的。

```rust
fn main() {
    assert!('字'.is_alphabetic());
    assert!('あ'.is_alphabetic());
    assert!('ア'.is_alphabetic());
    assert!('ㅎ'.is_alphabetic());
    assert!('ａ'.is_alphabetic());

    // uncomment this line will cause panic
    // assert!('1'.is_alphabetic());

    println!("All tests passed!");
}
```

[run this code!](https://play.rust-lang.org/?version=stable&mode=debug&edition=2021&code=fn+main%28%29+%7B%0D%0A++++assert%21%28%27%E5%AD%97%27.is_alphabetic%28%29%29%3B%0D%0A++++assert%21%28%27%E3%81%82%27.is_alphabetic%28%29%29%3B%0D%0A++++assert%21%28%27%E3%82%A2%27.is_alphabetic%28%29%29%3B%0D%0A++++assert%21%28%27%E3%85%8E%27.is_alphabetic%28%29%29%3B%0D%0A++++assert%21%28%27%EF%BD%81%27.is_alphabetic%28%29%29%3B%0D%0A++++%0D%0A++++%2F%2F+uncomment+this+line+to+see+panic%0D%0A++++%2F%2F+assert%21%28%271%27.is_alphabetic%28%29%29%3B%0D%0A++++%0D%0A++++println%21%28%22All+tests+passed%21%22%29%3B%0D%0A%7D)

Links:

- [Unicode standard](https://www.unicode.org/versions/latest/)

- [Rust char::is_alphabetic](https://github.com/rust-lang/rust/blob/9ccfedf186d1ee3ef7c17737167f2f90276f9ed0/library/core/src/char/methods.rs#L741-L749)

```rust
#[must_use]
#[stable(feature = "rust1", since = "1.0.0")]
#[inline]
pub fn is_alphabetic(self) -> bool {
    match self {
        'a'..='z' | 'A'..='Z' => true,
        c => c > '\x7f' && unicode::Alphabetic(c),
    }
}
```

### Extract the second element of a tuple

Common solution:

```rust
let second = tuple.1;
```

Also, you can use pattern matching to deconstruct the tuple:

```rust
let (_, second, ..) = tuple;
```

if you need to the first 3 elements of a 10-element tuple, this is the way to go.

### Inclusive range

Rust just merged inclusive ranges in 1.80.0(quite recent)

see https://github.com/rust-lang/rust/issues/37854

so you can write:

```rust
let slice = &array[1..=3];
```

This is basically equivalent to:

```rust
let nice_slice = &a[1..4];
```
