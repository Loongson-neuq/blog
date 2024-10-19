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

以下内容以相对稳定且简单 Ubuntu 22 作为演示。

#### Package Manager

##### Whats Package Manager

包管理器用于管理 GNU/Linux 发行版的包（应用），不同于 Windows 的手动下载并安装，在 Linux 下安装 Git 只用输入一条命令。

可以通过类比的方式理解包管理器和包：

> 左右两项不等价

| Windows | Linux |
|:-:|:-:|
| 应用商店 | 包管理器 |
| App | 包 |

大多数 Linux 发行版都有自己的包管理器：

| Debian | RPM | Pacman |
|:-:|:-:|:-:|
| apt, dpkg | yum, rpm | pacman |

Ubuntu 的包管理器是 `apt` 和 `dpkg`，其中 apt 用于安装云端软件源的包，dpkg 则用于安装本地包。

```bash
apt --version
# output: apt 2.4.12 (amd64)
```

##### Usage

如果遇到了网络问题，请跳转下方 Mirror。

1. 更新软件包列表

在安装包之前，一般会同步云端软件包信息，保证依赖关系的正确。

```bash
sudo apt update
```

2. 更新所有软件包

```bash
sudo apt upgrade
```

3. 安装软件源的包

将 `<name>` 换成要安装的包名，多个则以空格分隔。

```bash
sudo apt install <name>
```

4. 安装本地 deb 包

安装中可能会提示依赖缺失，应使用 apt 安装缺失的依赖。

```bash
sudo dpkg -i /path/to/xxx.deb
```

5. 卸载包

将 `<name>` 换成要卸载的包名，多个则以空格分隔。

```bash
sudo apt remove <name>
```

6. 查找包

```bash
apt search xxx
```

##### Mirror

在使用 apt 时提示网络错误时，可以通过换源解决。

修改系统重要文件前记得备份：

```bash
sudo cp /etc/apt/sources.list /etc/apt/sources.list.back
sudo vim /etc/apt/sources.list
```

> 在 Ubuntu 24.04 之前，Ubuntu 的软件源配置文件使用传统的 One-Line-Style，路径为 /etc/apt/sources.list；从 Ubuntu 24.04 开始，Ubuntu 的软件源配置文件变更为 DEB822 格式，路径为 /etc/apt/sources.list.d/ubuntu.sources。
> 参考 https://mirrors.tuna.tsinghua.edu.cn/help/ubuntu/

在文件的顶部加入以下行：

```
# 默认注释了源码镜像以提高 apt update 速度，如有需要可自行取消注释
deb https://mirrors.tuna.tsinghua.edu.cn/ubuntu/ noble main restricted universe multiverse
# deb-src https://mirrors.tuna.tsinghua.edu.cn/ubuntu/ noble main restricted universe multiverse
deb https://mirrors.tuna.tsinghua.edu.cn/ubuntu/ noble-updates main restricted universe multiverse
# deb-src https://mirrors.tuna.tsinghua.edu.cn/ubuntu/ noble-updates main restricted universe multiverse
deb https://mirrors.tuna.tsinghua.edu.cn/ubuntu/ noble-backports main restricted universe multiverse
# deb-src https://mirrors.tuna.tsinghua.edu.cn/ubuntu/ noble-backports main restricted universe multiverse
```

最后更新软件包列表

```bash
sudo apt update
```

#### Install Common Software

##### Pakage for OS Development

- Git

```bash
sudo apt update
sudo apt install git
```

- VSCode

以下内容来自 [manual](https://manual.caiyi1.me/get-started/vscode)

实体机用户请在 Linux 下安装 VSCode：

手动安装：

1. 从 [VSCode 官网](https://code.visualstudio.com) 下载 deb 包。

2. 使用 dpkg 安装。

```bash
sudo dpkg -i code_xxx.deb
```

包管理器安装：

1. 添加源。

```bash
sudo apt-get install wget gpg
wget -qO- https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > packages.microsoft.gpg
sudo install -D -o root -g root -m 644 packages.microsoft.gpg /etc/apt/keyrings/packages.microsoft.gpg
echo "deb [arch=amd64,arm64,armhf signed-by=/etc/apt/keyrings/packages.microsoft.gpg] https://packages.microsoft.com/repos/code stable main" |sudo tee /etc/apt/sources.list.d/vscode.list > /dev/null
rm -f packages.microsoft.gpg
```

2. 安装。

```bash
sudo apt install apt-transport-https
sudo apt update
sudo apt install code # or code-insiders
```

- Rust

以下内容源自 [rCore-Tutorial-Guide-2024S 文档](https://learningos.cn/rCore-Tutorial-Guide-2024S/0setup-devel-env.html)

0. 如果遇到网络问题

配置环境变量：

可以在当前终端执行（当前终端有效），或者写入 `~/.bashrc`（永久，打开新的终端后）。

```bash
export RUSTUP_DIST_SERVER=https://mirrors.ustc.edu.cn/rust-static
export RUSTUP_UPDATE_ROOT=https://mirrors.ustc.edu.cn/rust-static/rustup
```

编辑 `~/.cargo/config`：

添加以下行：

```toml
[source.crates-io]
replace-with = 'ustc'

[source.ustc]
registry = "sparse+https://mirrors.ustc.edu.cn/crates.io-index/"
```

1. 安装 rustup

```bash
curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | sh
```

- QEMU

```bash
# 安装依赖
sudo apt install autoconf automake autotools-dev curl libmpc-dev libmpfr-dev libgmp-dev \
              gawk build-essential bison flex texinfo gperf libtool patchutils bc \
              zlib1g-dev libexpat-dev pkg-config  libglib2.0-dev libpixman-1-dev git tmux python3

# 下载 QEMU 源码
wget https://download.qemu.org/qemu-7.0.0.tar.xz

# 解压
tar xvJf qemu-7.0.0.tar.xz

# 进入子目录
cd qemu-7.0.0

# 编译安装并配置 RISC-V 支持
./configure --target-list=riscv64-softmmu,riscv64-linux-user
make -j$(nproc)
```

##### Awesome Tools

为了提高开发效率，推荐部分小工具。

1. CLI tools

- ranger: 文件管理器

- bat: 文件查看器，更好的 `less`

- tmux: 终端复用器

- lazygit: `git` TUI 管理工具

- eza: 有色彩和图标的 `ls`

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
