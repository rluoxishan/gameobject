namespace GlucoseWar.Data
{
    public enum Team { Player, Enemy }

    /// <summary>Enemy movement archetypes referenced by the wave/enemy tables.</summary>
    public enum MoveMode
    {
        StraightDown,   // 直线下冲
        SineHorizontal, // 正弦横移
        StaticBurst,    // 静止爆裂 (出血点)
        WrapLimit,      // 缠绕限位
        FloatSpread,    // 漂浮增殖 (高糖孢子)
        SwarmRush,      // 群体扑咬
        StaticSummon,   // 静止召唤 (精英)
        Drift,          // 缓动
        Tracking        // 跟踪
    }

    /// <summary>Bullet emission archetypes referenced by the tables.</summary>
    public enum PatternType
    {
        None,
        Single,         // 单发 / 直线1发
        DoubleStraight, // 直线2发
        Fan3,           // 扇形3发
        Ring8,          // 环形8发
        Ring12,         // 环形12发
        Ring16,         // 环形16发
        DeathSplash,    // 死亡溅射
        BossSpread,     // Boss 黏液扇形
        BossRadial,     // Boss 放射状
        BossDense       // Boss 高速密集
    }

    public enum ItemType
    {
        Insulin,    // 胰岛素结晶 -> 火力+1
        Fiber,      // 膳食纤维核心 -> 恢复护盾
        Energy,     // 运动能量 -> 加速 + 短时无敌
        SugarTrap   // 高糖陷阱 -> 负面：火力-1 + 屏幕泛红
    }

    public enum Difficulty { Easy, Normal, Hard }

    public enum GameState { Boot, Menu, Playing, Paused, Result }
}
