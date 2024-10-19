---
title: "[OS Week1] Git and Linux Basics"
date: 2024-10-19
description: 第一周 OS 课程 Slides
image: background.png
tags: 
    - Git
    - Linux
categories:
    - OS
    - 课程
    - tutorial
---

## Git

### Installations

#### Windows

Download installer from [Git For Windows](https://git-scm.com/downloads/win)

or use winget
    
```bash
winget install Git.Git
```

#### Linux

Install with your package manager

#### MacOS

Install with Homebrew

### Configuration

#### Global Configuration

Detailed instructions: [manual.caiyi1.me](https://manual.caiyi1.me/use-git/configure/)

```bash
git config --global user.name "Your Name"
git config --global user.email "Your Email"
```

#### Authentication

For code hosting services like GitHub, GitLab, Gitee, etc, you have to proof the user pushing(or pull) the code HAS the permission to do so.

Traditional way is to use SSH key, which is a pair of keys, one is public, one is private. Public key is stored on the server, and private key is stored on your local machine.

国内平台如 Gitee 只能使用 SSH key 进行认证, 不能使用 credential helper。

Still widely used, but is outdated. As Git allows custom credential helper, you can use a more secure way(and easier) to authenticate.

##### Windows

Uses `git credential-manager`, which is a part of Git for Windows.

###### Auth Github

```bash
git credential-manager github login
```

Follow the GUI instructions to login.

#### Linux

##### Auth Github

uses GitHub CLI, which is provided by the distro's package manager.

```bash
# Debian/Ubuntu
sudo apt install gh
```

Login with
```bash
gh auth login
```

Follow the instructions to login.

### Basic Usage

Detailed instructions: [manual.caiyi1.me](https://manual.caiyi1.me/use-git/try-git/)

#### Init
#### Clone

#### Commit

#### Push
#### Pull
#### Fetch

## Linux Basics

Linux 不只是一个工具，也可以是像 Windows 一样用于日常工作的操作系统。

KDE 桌面环境提供与 Windows 类似的体验，几乎没有任何学习成本。

### Installations

#### Choose a way to install Linux

- Physical Machine
    - Full performance
    - Graphics interface
    - Take up a lot of space
    - May not be easy to install if you are not experienced
    - Recommend for those who want immersive experience

- WSL2
    - Easy to install
    - Extremely low performance cost
    - Disk-friendly
    - Battery-friendly
    - Only Command Line Interface, but you still uses Windows' GUI
    - Good integration with Host OS(Windows)
    - Can run Linux GUI applications with X server (although not recommended for performance)

- Virtual Machine -  Really not recommended
    - Really low performance
    - Memory unfriendly
    - Battery unfriendly
    - Graphics interface

[Installation Guide](#additional-linux-installation-guide)

### Why Linux

- Excellent Command Line Interface
  - Shell
  - 各种命令行工具
  - 丰富的管道命令
  - 丰富的脚本语言，易于自动化

- Software Package Management
  - 无需手动下载安装
  - 依赖自动解决
  - 减少重复软件下载
  - 便于卸载，没有毒瘤软件

- Developer-friendly
    - Editor
        - Vim/Nvim
        - VSCode
        - ...
    - Compiler
        - GCC
        - Clang
        - ...

- Cross-platform development
    - CMake/Make
    - LLVM，GCC
    - OS 内核需要 cross-compile

- Highly customizable
    - Shell
    - Window Manager
    - Desktop Environment
    - ...

- Open Source
    - 无需担心软件的安全性
    - 无需担心软件的可用性
    - 无需担心软件的可维护性
    - 无需担心软件的隐私问题 (大多数情况，取决于你的使用方式)

#### Shortcomings

- Sucks when you need to use Windows-only software
- Suck graphics driver support (Not for all hardware)
- Suck graphics backend support
    - X.org
        - Old, and has not been updated for a long time
            - May have some security issues
            - May not support some new features
        - But still widely used
        - Good support for software
    - Wayland
        - Lacks many features
        - Bad support for some software
        - Really HIGH rendering latency
            - You can feel it!
            - Not suitable for latency-sensitive games, like rhythm games
        - Nvidia support sucks
            - Games may run with lower performance
        - Higher power consumption than X.org
        - Modern, updated frequently
            - Trending

But I still recommend you to use Wayland as long as you don't have any problems with it.

### Unboxing

#### Package Manager

<!-- TODO -->

##### Software Repository

<!-- TODO -->

##### Mirror

<!-- TODO -->

#### Install common software

<!-- TODO -->

### Command Line

#### Foreword

Shell 是一个解释器，它接受用户输入的命令，然后**调用相应的应用程序或内建命令函数**。

- Windows
    - PowerShell - fairly good, but lack customization. Slow, for still using .NET Framework(capability reasons, but can be replaced by .NET)
        - Update to PowerShell 7, which is cross-platform
        - https://aka.ms/PSWindows
    - CMD

- Unix-like (inclue Linux, MacOS, FreeBSD...)
    - Bash - default, but MUCH better than Windows'
    - Zsh  - most popular, maybe hard to configure
    - Fish - easy to use, but not recommended for scripting
    - ... like sh, dash

对于 Unix, 各种 Shell 的语法大致相同，内建命令大多数与 Bash 相同。甚至 Windows 的 PowerShell 也开始为某些 Bash 的内建命令通过 alias 提供支持。

Fish 是一个很好的 Shell，但是不适合用于编写脚本，因为它的语法和其他 Shell 不同。因此建议大家使用 Bash 或者 Zsh。如果你想配置一个好看并且功能强大的 Shell，可以尝试使用 Zsh。

Detailed instructions: [manual.caiyi1.me](https://manual.caiyi1.me/use-git/hug-cli/)

#### Shell script

Shell 脚本是一种文本文件，其中包含了一系列的命令（和我们在 shell 前端中输入的一样）。Shell 会按照脚本中的命令顺序执行。

下面尝试把你输入过的命令写入一个脚本文件，然后执行这个脚本文件。

```bash
pwd
echo "----------------"
ls
```

执行时，如果使用`./script.sh`执行，需要给予执行权限，使用`chmod +x script.sh`。如果调用 shell 执行，例如`bash script.sh`，则不需要给予执行权限。

执行 shell 脚本时，会新开一个 shell 进程执行脚本，因此脚本中的变量不会影响到当前 shell。

脚本的工作目录与执行脚本的 shell 的工作目录相同。不是脚本文件的目录。

### Additional: Linux Installation Guide

#### WSL2

Boot up your Windows, enter Microsoft Store, search for "WSL", select an distro and install it.

Ubuntu is recommended for beginners as it has official support.

After installation, you can open it from Start Menu or Windows Terminal.

The first time you uses it, you have to set up a username and password, not asking for your windows's password.

#### Physical Machine

Partition your disk in Windows. You have to create at least two partitions, one for Boot volume, one for Root(Where the system files are stored).

The Boot volume should be at least 1GB, and the Root volume should be at least 50GB.

Download a distro's ISO file from its official website, and flash it to a USB drive to make a bootable drive.

Reboot your computer to BIOS/UEFI, and boot from the USB drive.

Choose manual partitioning if you don't want to lose your data and Windows.

Assign the Boot volume to `/boot`, and the Root volume to `/`.

Choose the boot volume to be formatted as FAT32, and the root volume to be formatted as ext4 or Btrfs.

After installation, you can shutdown your computer and unplug the USB drive.

##### ##MUST READ##

You may lose the ability to boot into Windows, as the bootloader is replaced by the Linux bootloader.

You can either select the system to boot in the BIOS/UEFI, or use a bootloader like GRUB/rEFInd.

Note that GRUB can NOT detect bootable devices at runtime while rEFInd can.

#### Virtual Machine

Download a distro's ISO file from its official website, and create a new VM in your VM software. Assign at least 50GB of disk space and 4GB of RAM.

Mount the ISO file to the VM, and boot from it.

Install the system following the instructions along the way.

You can choose "Clean Install" since we don't have any data to lose.

After installation, you can shutdown the VM and unmount the ISO file. Then you can boot into the system.
