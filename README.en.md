## CSTI Mod Manager

## Introduction

> mod I will upload gradually, no need to update Mod Manager.

This manager is based on the github open source project [MonkeModManager](https://github.com/DeadlyKitten/MonkeModManager).

Rely on gitee/github for cloud storage and management of mods.

Bugs can be feedback in the QQ group (641891277)

## Instructions for use

**Currently, only mods based on ModEditor support all features**.

### Player

#### First time user

The first time you open ModEditor, if you don't find the game under the default path, you need to specify the game installation directory, and in the interface that pops up, select the game executable file `Card Survival - Tropical Island.exe`.

![QQ screenshot 20240807162752.png](http://photos.szpt.top/i/2024/08/12/66b8e8d8805a2.png)

#### Install mods

Check the mods you want to install, click install, and wait patiently.

> You can check the current installation progress in the bottom left corner.

![QQ screenshot 20240807163245.png](http://photos.szpt.top/i/2024/08/12/66b8e8d8496e4.png)

![QQ screenshot 20240807163616.png](http://photos.szpt.top/i/2024/08/12/66b8e8d8cab94.png)

#### Delete mod

Check the mods you want to delete and click Delete!

![QQ screenshot 20240807163700.png](http://photos.szpt.top/i/2024/08/12/66b8e8d92424e.png)

![QQ screenshot 20240807163900.png](http://photos.szpt.top/i/2024/08/12/66b8e8de9ea33.png)

![QQ screenshot 20240807164228.png](http://photos.szpt.top/i/2024/08/12/66b8e8d8c7232.png)

#### Disable mod

> Disabling dlls is not supported.

Select an installed mod and click Disable Mod.

![QQ screenshot 20240807164408.png](http://photos.szpt.top/i/2024/08/12/66b8e8ddde165.png)

![QQ screenshot 20240807164506.png](http://photos.szpt.top/i/2024/08/12/66b8e8de4bf43.png)

#### Enable mod

Check a disabled mod and click Enable Mod.

![QQ screenshot 20240807164614.png](http://photos.szpt.top/i/2024/08/12/66b8e8dd76db2.png)

![QQ screenshot 20240807164710.png](http://photos.szpt.top/i/2024/08/12/66b8e8d8f1fc8.png)

#### More features

![QQ screenshot 20240807165005.png](http://photos.szpt.top/i/2024/08/12/66b8e8d9329ff.png)

### Mod author

#### don't know how to use git

##### Uploading a mod

Register an account with gitee, go to your personal center and click the + sign in the upper right corner to create a new repository.

![QQ screenshot 20240807165355.png](http://photos.szpt.top/i/2024/08/12/66b8e8db58bd1.png)

Enter the repository name (preferably the mod name)

![D22DE8590D32AAC49698608EA2C35834.png](http://photos.szpt.top/i/2024/08/12/66b8e8d758236.png)

Once the new repository is complete, click `initialize readme file`.

![999446e50efcd265f2d5fc8e63d3dc78.png](http://photos.szpt.top/i/2024/08/12/66b8e8dd0cddf.png)

Click on the right side of the distro to create it.

![4c15c073c7950e103a23d4087ccd1f0c.png](http://photos.szpt.top/i/2024/08/12/66b8e8d96173a.png)

Enter the mod version number and upload the mod zip file and click Create.

![8f82c50803c38eae9c7482d5b3a80ca1.png](http://photos.szpt.top/i/2024/08/12/66b8e8d848c63.png)

The following screen appears to indicate successful creation.

![ae876af190f137e3f390732a69a9485d.png](http://photos.szpt.top/i/2024/08/12/66b8e8d87526f.png)

Private message the URL of that screen to Coldwinds (825592085) and I'll add it to the mod manager for you.

![6fa18c99cc0f8eddfeb214d463845eeb.png](http://photos.szpt.top/i/2024/08/12/66b8e8d960663.png)

##### Updating a mod

As with uploading a mod, create a new distro and send the link to Coldwinds (825592085).

#### will use git

##### uploading a mod

Add the mod to the gitee repository and create the distribution as above.

Clone the mod information repository and import the information into the manager.

> git clone git@gitee.com:Cold_winds/cstimodinfo.git

**Private chat with Coldwinds (825592085)** for repository permissions and I'll send them to you.

Edit the mods.json file and add the mods in the following format, the category names can be viewed in groups.json.

```json
{
	“name": ‘BepInEx’,
    “modname": ‘BepInEx’,
    “author": ‘BepInEx Team’,
    
    
    
    “download_url": ”https://gitee.com/Cold_winds/BepInEx/releases/download/5.4.22.0/BepInEx_x64_5.4.22.0%EF%BC%88%E8%A7%A3%E5%8E%8B% E5%88%B0%E6%B8%B8%E6%88%8F%E6%A0%B9%E7%9B%AE%E5%BD%95%EF%BC%89.zip”
  }
```

> name: name of the mod.
>
> modname: must be the name in modinfo.json in the mod folder.
>
> author: author.
> >
> version: version number.
> >
> git_path: path to the gitee repository.
> >
> group: mod category.
>
> download_url: link to the mod file in the release (right click to copy the link address).

When the modification is done, push the changes to the repository.

> git add *

> git commit -m “Add day-after mod”
>
> Here, put “Add xxxmod”.

> git push

##### Updating mods

Create a new distribution and change the `version` and `download_url` in mods.json.

## About the mod update feature

If you want to support the mod update feature, you need to get the version number of the local mod, but currently most mod authors only mark the mod version in the zip file, so you can't get the version number once you unzip the zip file.

Therefore, the update function requires the cooperation of the mod author.

### Solution

Mods based on ModEditor will add a `Version` entry in `ModInfo.json` in the mod folder, change the value of the entry to the version number of your mod.

Example:

``json
{
    “Author": ‘Coldwinds’,
    “ModEditorVersion": ‘0.6.3’,
    “ModLoaderVerison": ‘2.0.1’,
    “Name": ‘TheDayAfterTomorrow’,
    “Version": ”0.3.2”
}
``
