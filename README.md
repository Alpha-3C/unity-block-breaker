# Block Breaker（Unity 打砖块复刻练习）

使用 Unity **2022.3.62f1c1** 和 URP 2D 制作的打砖块玩法原型。当前可移动挡板、发射小球、碰撞消砖，并在球掉出底部后补一颗球；计分、胜负判定和关卡流程尚未实现。

## 打开与操作

1. 在 Unity Hub 中添加本目录，使用项目记录的 **2022.3.62f1c1** 版本打开。首次打开会重新生成 `Library`。
2. 打开 `Assets/Scenes/SampleScene.unity` 并按 Play。该场景已加入 Build Settings。
3. 用 `A` / `D` 或左右方向键移动挡板，按 `Space` 发球。小球触底后会回到挡板，按 `Space` 再次发球。

## 在编辑器录制 Demo

选中 `Assets/Editor/DemoRecorderSettings.asset`，在 Inspector 勾选 **Auto Record On Play**。之后每次进入 Play 模式会用 Unity Recorder 录制 Game View，退出 Play 时结束并保存 MP4 到项目根目录的 `Recordings/`。取消勾选即可停止自动录制；开关默认关闭。录制文件不会提交到 Git。此功能只在 Unity 编辑器中运行，不进入游戏构建。

## 当前实现

- `PlayerControl.cs`：读取水平输入、限制挡板活动范围、发球与掉球后的补球。
- `BallControl.cs`：设置发球速度、按碰撞法线计算反射方向、消除碰到的砖块、触底通知挡板。
- `BrickManager.cs`：按场景中配置的列数、行数和间距实例化砖块。当前场景配置为 14 列、10 行。
- `Assets/Prefabs/`：球和砖块 Prefab；`Assets/Block Breaker/`：背景、挡板、球与砖块图片。
- `Assets/Editor/`：录制开关资源和 Play 模式录制控制脚本。

项目的实际结构、关键实现及待解决问题见[学习与实现记录](Docs/BlockBreaker项目学习与实现记录.md)。

## 素材说明

工程现有四张 PNG：`ball.png`、`bg.png`、`brick.png`、`player.png`。当前工程中没有找到它们的原始下载地址或许可文件；请在公开分发游戏或复用图片前核实来源与授权。记录见[素材说明](ArtSource/素材说明.md)。
