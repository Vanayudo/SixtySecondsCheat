# SixtySecondsCheat

一个基于 **BepInEx 5** 的《60秒！重制版》(60 Seconds! Reatomized) 游戏功能修改插件。

## 🌟 功能特性

- **内置 GUI 菜单**：游戏内按 `F6` 键即可快速开启/关闭修改界面。
- **资源修改**：
    - 自由增减、一键填满 **食物** (Food) 与 **水** (Water)。
- **道具管理**：
    - 支持 15 种关键道具的 **启用/禁用/卸载**。
    - 包含：医疗包、防毒面具、收音机、手电筒、斧头、地图、童子军手册、手提箱等。
    - 自动修复损坏状态并重置耐久度（针对可损耗道具）。
- **一键操作**：
    - **全部启用**：瞬间获得所有满耐久道具及 999 单位的食物和水。
    - **全部卸载**：清空所有物品，回归初始状态（或用于重置存档）。

## 🛠️ 安装方法

1. **安装 BepInEx**：
   确保你的游戏已经安装了 [BepInEx 5.x (x64)](https://github.com/BepInEx/BepInEx/releases)。
2. **放置插件**：
   将编译生成的 `SixtySecondsCheat.dll` 文件放入游戏根目录下的路径：
   `60 Seconds Reatomized/BepInEx/plugins/`
3. **启动游戏**：
   进入游戏后，在避难所界面按 **F6** 唤出菜单。

## 💻 开发说明

该插件使用 **C#** 编写，并利用反射技术直接操作游戏运行时的私有数据字段，无需修改原游戏程序集。

- **目标框架**: .NET Framework 4.7.2
- **依赖库**:
    - `BepInEx.dll`
    - `UnityEngine.dll`
    - `UnityEngine.IMGUIModule.dll`
    - `UnityEngine.InputLegacyModule.dll`
    - `Assembly-CSharp.dll` (来自游戏目录)

## ⚠️ 注意事项

- 本插件仅供单机娱乐及测试使用。
- 在修改数据前建议备份存档。
- 所有的修改操作均基于游戏运行时的内存数据，重启游戏或重新开始关卡后需重新设置。

## 📜 许可证

本项目采用 [MIT License](LICENSE) 开源。