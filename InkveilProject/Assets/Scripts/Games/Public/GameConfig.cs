using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏配置参数
/// </summary>
public sealed class GameConfig
{
    /// <summary>
    /// 游戏状态
    /// </summary>
    public static GameState.State gameState = GameState.State.None; 

    /// <summary>
    /// 游戏状态枚举类型
    /// </summary>
    public class GameState
    {
        public enum State { 
            None,               //无状态
            Ready,              //准备状态
            Start,              //开始状态
            Play,               //游戏中
            Pause,              //暂停
            Stop,               //停止
            Over,               //结束
            Victory,            //胜利
        };
    }
}
