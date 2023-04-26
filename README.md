# <div align="center">Mirrors All in One(MAIO)</div>

<div align="center">
  <img src="https://img.shields.io/badge/Build-passing-%2396C40F" alt="Build-passing"/>
  <img src="https://img.shields.io/badge/Version-1.0.0-%231081C1" alt="Version-1.0.0"/>
  <img src="https://img.shields.io/badge/License-Apache_2.0-%2396C40F" alt="License-Apache2.0"/>
  <img src="https://img.shields.io/badge/PoweredBy-Jiang_Liu-%2396C40F" alt="PoweredBy-Jiang_Liu"/>
</div>

## 背景介绍

每当我需要配置包管理器的镜像时，我都会去搜索一下（包括命令和镜像地址），然后复制粘贴，再去配置，
这样的过程很繁琐，而且有时会出现一些错误，比如镜像地址过期了（例如中科大的Anaconda镜像源已无法使用）。
所以我就想着能不能写一个程序，来帮我完成这些繁琐的工作，于是就有了这个项目。

## 项目介绍

MAIO是一个用于配置包管理器镜像的可视化软件，目前支持的包管理器有：Conda（后续会支持更多的包管理器）。

本项目的目的是为了让用户更加方便的配置包管理器的镜像，而不是去搜索一堆的命令和镜像地址。

项目基于WPF开发，使用C#语言编写。（由于本人是一个C#新手，所以代码可能会有一些不规范的地方，还请见谅）

## 项目所支持的包管理器

- Conda：于2023年4月27日02:39:55基本开发完毕

## 项目中引用的镜像列表

> 排序规则：按中文拼音字母排序
>
> 声明：所引用的大学简称仅为了精简镜像备注，不代表本人对该大学的任何态度，若有冒犯，请及时联系我，我会及时更改，谢谢！

### Anaconda

> 资料来源说明

1. Anaconda镜像列表总资料来源：[MirrorZ Help](https://mirror.nju.edu.cn/anaconda/)
2. 各大开源镜像站官方网站

> 时效性说明

本镜像源列表资料获取时间：2023年4月27日01:48:55

若出现镜像地址失效的情况，请及时联系我，我会及时更新，谢谢！

#### 01. 阿里巴巴开源镜像站

> 资料来源：[阿里巴巴开源镜像站-Anaconda镜像](https://developer.aliyun.com/mirror/anaconda)

> 阿里巴巴镜像源公告Anaconda镜像源公告：由于更新过快难以同步，我们不同步pytorch-nightly, pytorch-nightly-cpu, ignite-nightly这三个包。

- 阿里云main：[http://mirrors.aliyun.com/anaconda/pkgs/main](http://mirrors.aliyun.com/anaconda/pkgs/main)
- 阿里云r：[http://mirrors.aliyun.com/anaconda/pkgs/r](http://mirrors.aliyun.com/anaconda/pkgs/r)
- 阿里云msys2：[http://mirrors.aliyun.com/anaconda/pkgs/msys2](http://mirrors.aliyun.com/anaconda/pkgs/msys2)
- 阿里云附加库: [http://mirrors.aliyun.com/anaconda/cloud](http://mirrors.aliyun.com/anaconda/cloud)

#### 02. 北京外国语大学开源软件镜像站

> 资料来源：[北京外国语大学开源软件镜像站-Anaconda镜像使用帮助](https://mirrors.bfsu.edu.cn/help/anaconda/)

> 北京外国语大学开源软件镜像站Anaconda镜像源公告：由于更新过快难以同步，我们不同步pytorch-nightly, pytorch-nightly-cpu, ignite-nightly这三个包。

- 北外main：[https://mirrors.bfsu.edu.cn/anaconda/pkgs/main](https://mirrors.bfsu.edu.cn/anaconda/pkgs/main)
- 北外r：[https://mirrors.bfsu.edu.cn/anaconda/pkgs/r](https://mirrors.bfsu.edu.cn/anaconda/pkgs/r)
- 北外msys2：[https://mirrors.bfsu.edu.cn/anaconda/pkgs/msys2](https://mirrors.bfsu.edu.cn/anaconda/pkgs/msys2)
- 北外附加库：[https://mirrors.bfsu.edu.cn/anaconda/cloud](https://mirrors.bfsu.edu.cn/anaconda/cloud)

#### 03. 北京大学开源镜像站

> 资料来源：[北京大学开源镜像站-Anaconda镜像使用指南](https://mirrors.pku.edu.cn/Help/Anaconda)

- 北大main：[https://mirrors.pku.edu.cn/anaconda/pkgs/main](https://mirrors.pku.edu.cn/anaconda/pkgs/main)
- 北大r：[https://mirrors.pku.edu.cn/anaconda/pkgs/r](https://mirrors.pku.edu.cn/anaconda/pkgs/r)
- 北大附加库：[https://mirrors.pku.edu.cn/anaconda/cloud](https://mirrors.pku.edu.cn/anaconda/cloud)

#### 04. 豆瓣开源镜像站

> 未找到官方网站，根据一些博客和浏览判断，应该是豆瓣的开源镜像站，但不保证有效性，故本项目镜像仓库暂不添加

- 豆瓣simple：[http://pypi.douban.com/simple/](http://pypi.douban.com/simple/)

#### 05. 哈尔滨工业大学开源镜像站

> 资料来源：[哈尔滨工业大学开源镜像站](https://mirrors.hit.edu.cn/#/home)

- 哈工大main：[https://mirrors.hit.edu.cn/anaconda/pkgs/main/](https://mirrors.hit.edu.cn/anaconda/pkgs/main/)
- 哈工大msys2：[https://mirrors.hit.edu.cn/anaconda/pkgs/msys2/](https://mirrors.hit.edu.cn/anaconda/pkgs/msys2/)
- 哈工大r：[https://mirrors.hit.edu.cn/anaconda/pkgs/r/](https://mirrors.hit.edu.cn/anaconda/pkgs/r/)
- 哈工大附加库：[https://mirrors.hit.edu.cn/anaconda/cloud/](https://mirrors.hit.edu.cn/anaconda/cloud/)

#### 06. 兰州大学开源软件镜像站

> 资料来源：[兰州大学开源软件镜像站-Anaconda镜像使用帮助](https://mirrors.lzu.edu.cn/help/#/docs/anaconda)

- 兰大main：[https://mirrors.lzu.edu.cn/anaconda/pkgs/main](https://mirrors.lzu.edu.cn/anaconda/pkgs/main)
- 兰大r：[https://mirrors.lzu.edu.cn/anaconda/pkgs/r](https://mirrors.lzu.edu.cn/anaconda/pkgs/r)
- 兰大msys2：[https://mirrors.lzu.edu.cn/anaconda/pkgs/msys2](https://mirrors.lzu.edu.cn/anaconda/pkgs/msys2)
- 兰大附加库：[https://mirrors.lzu.edu.cn/anaconda/cloud/](https://mirrors.lzu.edu.cn/anaconda/cloud/)

#### 07. 南方科技大学开源软件镜像站

> 资料来源：[南方科技大学开源软件镜像站-Anaconda镜像使用帮助](https://mirrors.sustech.edu.cn/help/anaconda.html)

- 南科大main：[https://mirrors.sustech.edu.cn/anaconda/pkgs/main](https://mirrors.sustech.edu.cn/anaconda/pkgs/main)
- 南科大r：[https://mirrors.sustech.edu.cn/anaconda/pkgs/r](https://mirrors.sustech.edu.cn/anaconda/pkgs/r)
- 南科大msys2：[https://mirrors.sustech.edu.cn/anaconda/pkgs/msys2](https://mirrors.sustech.edu.cn/anaconda/pkgs/msys2)
- 南科大附加库：[https://mirrors.sustech.edu.cn/anaconda/cloud/](https://mirrors.sustech.edu.cn/anaconda/cloud/)

#### 08. 南京大学开源软件镜像站

> 资料来源：[南京大学开源软件镜像站-Anaconda软件仓库镜像使用帮助](https://mirror.nju.edu.cn/mirrorz-help/anaconda/?mirror=NJU)

- 南京大main：[https://mirror.nju.edu.cn/anaconda/pkgs/main](https://mirror.nju.edu.cn/anaconda/pkgs/main)
- 南京大r：[https://mirror.nju.edu.cn/anaconda/pkgs/r](https://mirror.nju.edu.cn/anaconda/pkgs/r)
- 南京大msys2：[https://mirror.nju.edu.cn/anaconda/pkgs/msys2](https://mirror.nju.edu.cn/anaconda/pkgs/msys2)
- 南京大附加库：[https://mirror.nju.edu.cn/anaconda/cloud](https://mirror.nju.edu.cn/anaconda/cloud)

#### 09. 南京工业大学开源软件镜像站

> 资料来源：[南京工业大学开源软件镜像站-Anaconda配置说明](https://mirrors.njtech.edu.cn/docs/anaconda)

- 南工main：[https://mirrors.njtech.edu.cn/anaconda/pkgs/main](https://mirrors.njtech.edu.cn/anaconda/pkgs/main)
- 南工r：[https://mirrors.njtech.edu.cn/anaconda/pkgs/r](https://mirrors.njtech.edu.cn/anaconda/pkgs/r)
- 南工msys2：[https://mirrors.njtech.edu.cn/anaconda/pkgs/msys2](https://mirrors.njtech.edu.cn/anaconda/pkgs/msys2)
- 南工附加库：[https://mirrors.njtech.edu.cn/anaconda/cloud/](https://mirrors.njtech.edu.cn/anaconda/cloud/)

#### 10. 南京邮电大学开源软件镜像站

> 资料来源：[南京邮电大学开源软件镜像站-Anaconda镜像使用帮助](https://mirrors.njupt.edu.cn/help/anaconda/)

- 南邮main：[https://mirrors.njupt.edu.cn/anaconda/pkgs/main](https://mirrors.njupt.edu.cn/anaconda/pkgs/main)
- 南邮r：[https://mirrors.njupt.edu.cn/anaconda/pkgs/r](https://mirrors.njupt.edu.cn/anaconda/pkgs/r)
- 南邮msys2：[https://mirrors.njupt.edu.cn/anaconda/pkgs/msys2](https://mirrors.njupt.edu.cn/anaconda/pkgs/msys2)
- 南邮附加库：[https://mirrors.njupt.edu.cn/anaconda/cloud/](https://mirrors.njupt.edu.cn/anaconda/cloud/)

#### 11. 清华大学开源软件镜像站

> 资料来源：[清华大学开源软件镜像站-Anaconda镜像使用帮助](https://mirrors.tuna.tsinghua.edu.cn/help/anaconda/)

> 清华大学开源软件镜像站Anaconda镜像源公告：由于更新过快难以同步，我们不同步pytorch-nightly, pytorch-nightly-cpu, ignite-nightly这三个包。

- 清华main：[https://mirrors.tuna.tsinghua.edu.cn/anaconda/pkgs/main](https://mirrors.tuna.tsinghua.edu.cn/anaconda/pkgs/main)
- 清华r：[https://mirrors.tuna.tsinghua.edu.cn/anaconda/pkgs/r](https://mirrors.tuna.tsinghua.edu.cn/anaconda/pkgs/r)
- 清华msys2：[https://mirrors.tuna.tsinghua.edu.cn/anaconda/pkgs/msys2](https://mirrors.tuna.tsinghua.edu.cn/anaconda/pkgs/msys2)
- 清华附加库：[https://mirrors.tuna.tsinghua.edu.cn/anaconda/cloud](https://mirrors.tuna.tsinghua.edu.cn/anaconda/cloud)

#### 12. 上海交通大学开源软件镜像站

> 资料来源：[上海交通大学思源 (Siyuan) 镜像服务器](https://mirror.sjtu.edu.cn/)
>
> 没搞懂是怎么配置的，故本项目暂时不使用。可以访问[https://mirror.sjtu.edu.cn/anaconda/](https://mirror.sjtu.edu.cn/anaconda/)查看

#### 13. 上海科技大学开源软件镜像站

> 资料来源：[上海科技大学开源软件镜像站-Anaconda镜像使用帮助](https://mirrors.shanghaitech.edu.cn/help/anaconda)

- 上科大main：[https://mirrors.shanghaitech.edu.cn/anaconda/pkgs/main](https://mirrors.shanghaitech.edu.cn/anaconda/pkgs/main)
- 上科大r：[https://mirrors.shanghaitech.edu.cn/anaconda/pkgs/r](https://mirrors.shanghaitech.edu.cn/anaconda/pkgs/r)
- 上科大msys2：[https://mirrors.shanghaitech.edu.cn/anaconda/pkgs/msys2](https://mirrors.shanghaitech.edu.cn/anaconda/pkgs/msys2)
- 上科大附加库：[https://mirrors.shanghaitech.edu.cn/anaconda/cloud/](https://mirrors.shanghaitech.edu.cn/anaconda/cloud/)

#### 14. 西安交通大学软件镜像站

> 资料来源：[西安交通大学软件镜像站-Anaconda镜像使用帮助](https://mirrors.xjtu.edu.cn/help/anaconda.html)

- 西交main：[https://mirrors.xjtu.edu.cn/anaconda/pkgs/main](https://mirrors.xjtu.edu.cn/anaconda/pkgs/main)
- 西交r：[https://mirrors.xjtu.edu.cn/anaconda/pkgs/r](https://mirrors.xjtu.edu.cn/anaconda/pkgs/r)
- 西交msys2：[https://mirrors.xjtu.edu.cn/anaconda/pkgs/msys2](https://mirrors.xjtu.edu.cn/anaconda/pkgs/msys2)
- 西交附加库：[https://mirrors.xjtu.edu.cn/anaconda/cloud/](https://mirrors.xjtu.edu.cn/anaconda/cloud/)

#### 15. 浙江大学开源软件镜像站

> 资料来源：[浙江大学开源软件镜像站-Anaconda镜像使用帮助](https://mirrors.zju.edu.cn/docs/anaconda/)

- 浙大main：[https://mirrors.zju.edu.cn/anaconda/pkgs/main](https://mirrors.zju.edu.cn/anaconda/pkgs/main)
- 浙大r：[https://mirrors.zju.edu.cn/anaconda/pkgs/r](https://mirrors.zju.edu.cn/anaconda/pkgs/r)
- 浙大msys2：[https://mirrors.zju.edu.cn/anaconda/pkgs/msys2](https://mirrors.zju.edu.cn/anaconda/pkgs/msys2)
- 浙大附加库：[https://mirrors.zju.edu.cn/anaconda/cloud/](https://mirrors.zju.edu.cn/anaconda/cloud/)

## 项目截图

## 版权声明

项目采用[Apache License 2.0开源协议](https://apache.org/licenses/LICENSE-2.0.txt)

Copyright (c) 2023 Jiang Liu