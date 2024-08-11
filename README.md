# CSTI Mod管理器

## 介绍

> 暂不支持mod更新，原因最后有写。

> mod我会逐步上传，无需更新Mod管理器。

本管理器基于github开源项目[MonkeModManager](https://github.com/DeadlyKitten/MonkeModManager)开发。

依托gitee对mod进行云储存及管理。

出现bug可以在qq群里反馈(641891277)

## 使用说明

**目前只有基于ModEditor制作的mod支持全部功能**

### 玩家

#### 第一次使用

第一次打开管理器时，如果在默认路径下没找到游戏，就需要指定游戏安装目录，在弹出来的界面中，选择游戏可执行文件`Card Survival - Tropical Island.exe`

![QQ截图20240807162752](C:\Users\Viper\Desktop\CSTIMod管理器\picture\QQ截图20240807162752.png)

#### 安装mod

勾选想要安装的mods，点击安装，耐心等待即可。

> 可以在左下角查看当前安装进度。

![QQ截图20240807163245](C:\Users\Viper\Desktop\CSTIMod管理器\picture\QQ截图20240807163245.png)

![QQ截图20240807163616](C:\Users\Viper\Desktop\CSTIMod管理器\picture\QQ截图20240807163616.png)

#### 删除mod

勾选想要删除的mod，然后点击删除

![QQ截图20240807163700](C:\Users\Viper\Desktop\CSTIMod管理器\picture\QQ截图20240807163700.png)

![QQ截图20240807163900](C:\Users\Viper\Desktop\CSTIMod管理器\picture\QQ截图20240807163900.png)

![QQ截图20240807164228](C:\Users\Viper\Desktop\CSTIMod管理器\picture\QQ截图20240807164228.png)

#### 禁用mod

> 不支持禁用dll。

选择一个已安装的mod，点击禁用mod。

![QQ截图20240807164408](C:\Users\Viper\Desktop\CSTIMod管理器\picture\QQ截图20240807164408.png)

![QQ截图20240807164506](C:\Users\Viper\Desktop\CSTIMod管理器\picture\QQ截图20240807164506.png)

#### 启用mod

勾选一个已禁用的mod，点击启用mod。

![QQ截图20240807164614](C:\Users\Viper\Desktop\CSTIMod管理器\picture\QQ截图20240807164614.png)

![QQ截图20240807164710](C:\Users\Viper\Desktop\CSTIMod管理器\picture\QQ截图20240807164710.png)

#### 更多功能

![QQ截图20240807165005](C:\Users\Viper\Desktop\CSTIMod管理器\picture\QQ截图20240807165005.png)

### Mod作者

#### 不会使用git

##### 上传mod

在gitee注册账号，进入个人中心后，点击右上角+号，新建仓库。

![QQ截图20240807165355](C:\Users\Viper\Desktop\CSTIMod管理器\picture\QQ截图20240807165355.png)

输入仓库名称（最好是mod名）

![D22DE8590D32AAC49698608EA2C35834](C:\Users\Viper\Desktop\CSTIMod管理器\picture\D22DE8590D32AAC49698608EA2C35834.png)

新建仓库完成后，点击`初始化readme文件`。

![999446e50efcd265f2d5fc8e63d3dc78](C:\Users\Viper\Desktop\CSTIMod管理器\picture\999446e50efcd265f2d5fc8e63d3dc78.png)

点击右侧发行版的创建。

![4c15c073c7950e103a23d4087ccd1f0c](C:\Users\Viper\Desktop\CSTIMod管理器\picture\4c15c073c7950e103a23d4087ccd1f0c.png)

输入mod版本号，并上传mod压缩包文件，点击创建。

![8f82c50803c38eae9c7482d5b3a80ca1](C:\Users\Viper\Desktop\CSTIMod管理器\picture\8f82c50803c38eae9c7482d5b3a80ca1.png)

出现如下界面说明创建成功。

![ae876af190f137e3f390732a69a9485d](C:\Users\Viper\Desktop\CSTIMod管理器\picture\ae876af190f137e3f390732a69a9485d.png)

将该界面的网址私发给Coldwinds(825592085)，我给你添加进mod管理器。

![6fa18c99cc0f8eddfeb214d463845eeb](C:\Users\Viper\Desktop\CSTIMod管理器\picture\6fa18c99cc0f8eddfeb214d463845eeb.png)

##### 更新mod

和上传mod一样，新建一个发行版，然后将链接发给Coldwinds(825592085)。

#### 会使用git

##### 上传mod

将mod添加至gitee仓库并创建发行版，流程如上。

克隆mod信息仓库，将信息导入至管理器。

> git clone git@gitee.com:Cold_winds/cstimodinfo.git

**仓库权限私聊Coldwinds(825592085)**，我给你发。

编辑mods.json文件，按照如下格式添加mod，类别名称可以在groups.json中查看。

```json
{
	"name": "BepInEx",
    "modname": "BepInEx",
    "author": "BepInEx Team",
    "version": "5.4.22.0",
    "git_path": "BepInEx/BepInEx",
    "group": "前置类",
    "download_url": "https://gitee.com/Cold_winds/BepInEx/releases/download/5.4.22.0/BepInEx_x64_5.4.22.0%EF%BC%88%E8%A7%A3%E5%8E%8B%E5%88%B0%E6%B8%B8%E6%88%8F%E6%A0%B9%E7%9B%AE%E5%BD%95%EF%BC%89.zip"
  }
```

> name：mod名称。
>
> modname：必须为mod文件夹中的modinfo.json中的name。
>
> author：作者。
>
> version：版本号。
>
> git_path：gitee仓库路径。
>
> group：mod类别。
>
> download_url：发行版(release)中的mod文件的链接(右键复制链接地址)。

修改完成后，将改动push到仓库中。

> git add *

> git commit -m "添加后天mod"
>
> 此处填写"添加xxxmod"

> git push

##### 更新mod

创建一个新的发行版，然后修改mods.json中的`version`和`download_url`即可。

## 关于mod更新功能

如果要支持mod更新功能，需要获取到本地mod的版本号，但是目前绝大部分mod作者仅在压缩包处标注了mod版本号，一旦解压后就无法获取到版本号。

因此，更新功能需要mod作者的配合。

### 解决方法

基于ModEditor的mod，会在mod文件夹中的`ModInfo.json`中添加一个`Version`词条，修改该词条的数值为你的mod版本号即可。

举例：

```json
{
    "Author": "Coldwinds",
    "ModEditorVersion": "0.6.3",
    "ModLoaderVerison": "2.0.1",
    "Name": "TheDayAfterTomorrow",
    "Version": "0.3.2"
}
```

