# Unity《Block Breaker》：项目学习与实现记录

> 项目版本：Unity 2022.3.62f1c1，URP 2D  
> 项目目标：通过打砖块练习 2D 碰撞、反射、Prefab 生成和简单的回合循环。  
> 记录范围：截至 2026-10-20 的工程快照；这里只记录能从脚本和场景确认的实现。

## 一、项目目标与完成内容

打砖块的最小循环是“移动挡板 → 发球 → 球碰墙、挡板和砖块 → 消砖 → 球掉落后重新发球”。项目已有这条循环的主要对象：挡板、球、背景边界、砖块管理器，以及球和砖块的 Prefab。场景里的砖块管理器配置了 14 列、10 行，运行时生成砖块。当前还没有分数、剩余生命、清关或失败界面。

| 功能 | 当前状态 | 对应位置 |
| --- | --- | --- |
| 挡板左右移动与边界限制 | 已实现 | `PlayerControl.Update()` |
| 按空格发球 | 已实现 | `PlayerControl` → `BallControl.StartMove()` |
| 球碰撞与砖块销毁 | 已实现基础逻辑 | `BallControl.OnCollisionEnter2D()` |
| 球触底后补球 | 已实现基础逻辑 | `BallControl.OnTriggerEnter2D()`、`PlayerControl.Update()` |
| 动态砖块阵列 | 已实现 | `BrickManager.Start()`；场景配置 14×10 |
| 分数、胜负、关卡切换 | 未实现 | 暂无对应脚本或 UI |

## 二、实现过程与场景分工

| 阶段 | 工程中的做法 | 学习重点 |
| --- | --- | --- |
| 1. 搭建场景 | 放置 `player`、`bg`、`BrickManager`、相机与 2D 灯光，制作球和砖块 Prefab。 | Sprite、Collider2D、Rigidbody2D 与 URP 2D 灯光如何配合。 |
| 2. 让挡板移动 | `Input.GetAxisRaw("Horizontal")` 读取水平输入，位移乘 `Time.deltaTime`。 | 输入值、帧时间和活动边界。 |
| 3. 发射小球 | 挡板初始带着球；按空格调用 `StartMove(horizontal)`，再把球移出挡板层级。 | 通过 Prefab 复用球，以及父子层级对位置的影响。 |
| 4. 处理弹跳和消砖 | 球在碰撞回调中判断 `Brick` 标签并销毁砖块，使用碰撞法线求反射方向。 | `Collision2D.contacts[0].normal` 和 `Vector2.Reflect`。 |
| 5. 掉球后继续 | 底边触发器让球设置 `BallIsLost`，挡板随后实例化新球。 | `OnTriggerEnter2D` 与普通碰撞回调的区别。 |
| 6. 自动排砖 | 双层循环按列数、行数和间距实例化砖块。 | 场景参数如何控制 Prefab 阵列。 |

项目结构：

```text
Assets/
├─ Block Breaker/             四张 PNG 与对应 .meta
├─ Prefabs/
│  ├─ ball.prefab              小球、碰撞体、刚体和 BallControl
│  └─ brick.prefab             砖块 Sprite、碰撞体与 Brick 标签
├─ Scenes/SampleScene.unity    游戏场景
├─ Scripts/
│  ├─ PlayerControl.cs         挡板移动、发球、补球
│  ├─ BallControl.cs           球速度、碰撞反射、消砖、掉落
│  └─ BrickManager.cs          砖块阵列生成
└─ Settings/                   URP 2D 设置
```

## 三、关键代码与理解

### 1. 输入到移动

`Input.GetAxisRaw("Horizontal")` 在默认输入设置下可接受左右方向键和 `A` / `D`。脚本在 `Update()` 中将水平值乘移动速度和 `Time.deltaTime`，使移动距离按帧时长计算。越界时再反向移动同样的距离，保持挡板在左右边界之间。后续可以用 `Mathf.Clamp` 直接约束最终 x 坐标，让边界处理更直观。

### 2. 球的发射与反射

`StartMove(float dir)` 根据发球瞬间的水平输入建立方向：

```csharp
rb.velocity = new Vector3(dir, 1, 0).normalized * MoveSpeed;
```

`normalized` 先把方向变为单位长度，因此斜着发球和竖直发球使用同一初速度。碰撞时使用：

```csharp
Vector2 outVec = Vector2.Reflect(inVec, collision.contacts[0].normal);
rb.velocity = outVec.normalized * MoveSpeed;
```

碰撞法线代表接触面的朝向，`Vector2.Reflect` 计算镜面反射方向。当前 `inVec` 来自球当前位置与上次记录点的位移，尚未直接用碰撞前的 `rb.velocity`；连续或复杂碰撞时反弹方向需要重点实测。碰到挡板时另加一段向上的速度，避免球长期贴近挡板。

### 3. 砖块生成

`BrickManager` 从 `(0, 0)` 开始，沿 x 方向放置 `x` 列，沿负 y 方向放置行。当前场景将 `x` 设为 14、`y` 设为 -10，循环条件 `i > y` 因而得到 10 行。`y` 在这里实际表示负向终点，不是通常意义的正整数“行数”；后续重构时可改为正数行数与 `-row * height`，减少 Inspector 配置错误。

## 四、已知限制与下一步

1. **尚无关卡结束条件。** 砖块全部销毁后没有胜利提示；球掉落后无限补球，也没有剩余生命或失败状态。
2. **反弹需要实机检查。** 当前依据两次位置之差估计入射方向，碰撞后设置速度；应在侧墙、墙角、相邻砖块和挡板边缘逐一验证。
3. **触底判断较宽泛。** `OnTriggerEnter2D` 不检查碰到的是哪个触发器，未来加入其他触发区时可能误判掉球。可以给底部单独标签或引用。
4. **素材来源尚未记录。** 四张 PNG 的原作者和许可待核实，见[素材说明](../ArtSource/素材说明.md)。
5. **下一轮玩法扩展。** 先在 Play 模式确认基础反弹和补球，再补计分与剩余砖块统计；之后增加生命、胜负状态、重开按钮、音效和多关卡。

`SampleScene.unity` 已列入 Build Settings。本文是按现有文件做的学习整理，尚未将 Play 模式验收结果写成“已通过”。

## 五、编辑器 Demo 录制

项目加入 Unity Recorder 4.0.1。`Assets/Editor/DemoRecorderSettings.asset` 中的 **Auto Record On Play** 是序列化布尔选项，默认关闭。勾选后，编辑器的 `AutoDemoRecorder` 监听 Play 模式状态：进入 Play 时创建 Movie Recorder 并录制 Game View；退出 Play 前调用 `StopRecording()`，让 MP4 完成保存。视频写入项目根目录 `Recordings/`，该目录被 Git 忽略。此逻辑放在 `Editor` 目录，构建游戏时不会带入。

交互玩法使用 **Variable** 帧率录制，最高 60 FPS，让录制帧按实际经过的时间播放。若使用 **Constant 30 FPS** 却关闭帧率上限，Recorder 会让每帧的游戏时间固定前进 1/30 秒；编辑器实际渲染超过 30 FPS 时，游戏会看起来加速。MP4 还要求偶数宽高，因此保留 `MovieRecorderSettings` 自带的 Game View 输入设置，由 Recorder 自动处理奇数尺寸。
