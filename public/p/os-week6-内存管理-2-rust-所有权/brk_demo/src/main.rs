// Run with:
// cargo +nightly run -Z build-std=core --target x86_64-unknown-linux-gnu

#![no_std]
#![no_main]

use core::arch::asm;
use core::panic::PanicInfo;

#[no_mangle]
pub extern "C" fn main() {
    let brk_start: usize;

    unsafe {
        // brk 是指向用户堆的指针。我们使用brk是扩大堆的大小，也就是分配内存。
        // 但是我们需要知道堆的起始位置，所以我们需要先获取当前的brk值。
        asm!(
            "mov rax, 12",  // sys_brk system call number on x86_64
            "mov rdi, 0",   // brk(0) to get the current brk value
            "syscall",
            out("rax") brk_start,
            options(nostack)
        );

        let mut new_brk: usize = brk_start + 1024; // 分配 1024 字节内存

        let brk_ret: isize; // 系统调用返回值

        // 调用 brk 系统调用进行内存分配
        asm!(
            "mov rax, 12",  // sys_brk system call number on x86_64
            "syscall",
            in("rdi") new_brk,
            out("rax") brk_ret,
            options(nostack)
        );

        if brk_ret == -1 {
            new_brk = brk_start; // 分配失败，恢复原来的 brk
            panic!("brk failed");
        }

        let mut ptr = brk_start as *mut u8;
        ptr.write_volatile(42); // 访问 brk 的第一个字节

        print("brk success\n");

        for i in 0..4099 {
            unsafe {
                ptr.add(i).write_volatile(42); // 访问超出分配的内存
                // 你应该一定会看到4096行的write success。一行不多，一行不少。
                print("write success\n");
            }
        }
    }
}

#[allow(unused)]
#[allow(unused_mut)]
#[allow(unreachable_code)]
#[no_mangle]
pub extern "C" fn __libc_start_main() -> ! {
    main();

    print("main returned\n");

    exit(0);
}

pub fn print(s: &str) {
    let ptr = s.as_ptr();
    let count = s.len();
    unsafe {
        asm!("syscall",
            in("rax") 1, // sys_write system call number on x86_64
            in("rdi") 1, // file descriptor 1 is stdout
            in("rsi") ptr, // pointer to the buffer
            in("rdx") count, // buffer size
        )
    }
}

fn exit(code: i32) -> ! {
    unsafe {
        // 退出程序
        asm!("syscall",
            in("rax") 60, // sys_exit system call number on x86_64
            in("rdi") code, // exit code;
            options(noreturn, nostack)
        )
    }
}

#[panic_handler]
fn panic(_info: &PanicInfo) -> ! {
    print("panic\n");
    exit(1);
}
