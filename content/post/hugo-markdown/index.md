---
title: Hugo's markdown extension usages
description: Hugo's markdown extension usages
date: 2023-08-24 00:00:00+0000
math: true
# Uncomment the following line to enable image
# image: cover.jpg
---

## Math Typesetting

Stack has built-in support for math typesetting using [KaTeX](https://katex.org/).

**It's not enabled by default side-wide,** but you can enable it for individual posts by adding `math: true` to the front matter. Or you can enable it side-wide by adding `math = true` to the `params.article` section in `config.toml`.

### Inline math

This is an inline mathematical expression: $\varphi = \dfrac{1+\sqrt5}{2}= 1.6180339887…$

```markdown
$\varphi = \dfrac{1+\sqrt5}{2}= 1.6180339887…$
```

### Block math

$$
    \varphi = 1+\frac{1} {1+\frac{1} {1+\frac{1} {1+\cdots} } } 
$$

```markdown
$$
    \varphi = 1+\frac{1} {1+\frac{1} {1+\frac{1} {1+\cdots} } } 
$$
```

$$
    f(x) = \int_{-\infty}^\infty\hat f(\xi)\,e^{2 \pi i \xi x}\,d\xi
$$

```markdown
$$
    f(x) = \int_{-\infty}^\infty\hat f(\xi)\,e^{2 \pi i \xi x}\,d\xi
$$
```

## Inline Shortcodes

For more details, check out the [documentation](https://stack.jimmycai.com/writing/shortcodes).

### Bilibili video

{{< bilibili "BV1d4411N7zD" >}}

### Tencent video

{{< tencent "g0014r3khdw" >}}

### YouTube video

{{< youtube "0qwALOOvUik" >}}

### Generic video file

{{< video "https://www.w3schools.com/tags/movie.mp4" >}}

### Gist

{{< gist CaiJimmy e2751a943de10b2a5b3a8a6c2120cb86 >}}

##### GitLab

{{< gitlab 2589724 >}}

### Quote

{{< quote author="A famous person" source="The book they wrote" url="https://en.wikipedia.org/wiki/Book">}}
Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.
{{< /quote >}}

-----

> Photo by [Codioful](https://unsplash.com/@codioful) on [Unsplash](https://unsplash.com/photos/WDSN62Qdxuk)

## Image gallery


Hugo theme Stack supports the creation of interactive image galleries using Markdown. It's powered by [PhotoSwipe](https://photoswipe.com/) and its syntax was inspired by [Typlog](https://typlog.com/).

To use this feature, the image must be in the same directory as the Markdown file, as it uses Hugo's page bundle feature to read the dimensions of the image. **External images are not supported.**

### Syntax

```markdown
![Image 1](1.jpg) ![Image 2](2.jpg)
```

### Result

![Image 1](1.jpg) ![Image 2](2.jpg)

> Photo by [mymind](https://unsplash.com/@mymind) and [Luke Chesser](https://unsplash.com/@lukechesser) on [Unsplash](https://unsplash.com/)