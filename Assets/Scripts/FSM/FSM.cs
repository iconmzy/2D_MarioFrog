using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.FSM
{
    /// <summary>
    /// 有限状态机（FSM）泛型实现类
    /// 用于管理状态的切换、更新和状态间的流转
    /// </summary>
    /// <typeparam name="T">状态类型，必须实现IFSMState接口</typeparam>
    public class FSM<T> where T : IFSMState
    {
        /// <summary>
        /// 状态注册表，存储所有已注册的状态实例
        /// Key：状态类型（Type），Value：状态实例（T）
        /// </summary>
        public Dictionary<System.Type, T> StateTable { get; protected set; }


        public T PrevState { get; protected set; }

        protected T curState;


        public FSM()
        {
            // 初始化状态字典
            StateTable = new Dictionary<System.Type, T>();
            // 初始化当前状态和上一状态为默认值（null）
            curState = PrevState = default;
        }

        /// <summary>
        /// 向状态机注册状态
        /// </summary>
        /// <param name="state">要注册的状态实例</param>
        public void AddState(T state)
        {
            // 以状态类型为键，将状态实例存入注册表
            StateTable.Add(state.GetType(), state);
        }

        /// <summary>
        /// 启动状态机并进入指定初始状态（直接传入状态实例）
        /// </summary>
        /// <param name="startState">初始状态实例</param>
        public void SwitchOn(T startState)
        {
            // 设置当前状态为初始状态
            curState = startState;
            // 执行状态进入逻辑
            curState.Enter();
        }

        /// <summary>
        /// 启动状态机并进入指定初始状态（通过状态类型）
        /// 需确保该状态已通过AddState注册
        /// </summary>
        /// <param name="startState">初始状态的Type类型</param>
        public void SwitchOn(System.Type startState)
        {
            // 从状态表中获取对应类型的状态实例
            curState = StateTable[startState];
            // 执行状态进入逻辑
            curState.Enter();
        }

        /// <summary>
        /// 切换到指定的下一个状态（直接传入状态实例）
        /// 会先执行当前状态的退出逻辑，再执行新状态的进入逻辑
        /// </summary>
        /// <param name="nextState">要切换到的目标状态实例</param>
        public void ChangeState(T nextState)
        {
            // 记录当前状态为上一状态
            PrevState = curState;
            // 执行当前状态的退出逻辑
            curState.Exit();
            // 更新当前状态为目标状态
            curState = nextState;
            // 执行目标状态的进入逻辑
            curState.Enter();
        }

        /// <summary>
        /// 切换到指定的下一个状态（通过状态类型）
        /// 需确保该状态已通过AddState注册
        /// </summary>
        /// <param name="nextState">要切换到的目标状态Type类型</param>
        public void ChangeState(System.Type nextState)
        {
            // 记录当前状态为上一状态
            PrevState = curState;
            // 执行当前状态的退出逻辑
            curState.Exit();
            // 从状态表中获取目标状态实例并更新当前状态
            curState = StateTable[nextState];
            // 执行目标状态的进入逻辑
            curState.Enter();
        }

        /// <summary>
        /// 回退到上一个状态
        /// 若上一状态为null（无历史状态），则不执行任何操作
        /// </summary>
        public void RevertToPrevState()
        {
            if (PrevState != null)
            {

                ChangeState(PrevState);
            }
        }

        /// <summary>
        /// 状态机逻辑更新方法
        /// 需在每一帧调用，执行当前状态的逻辑更新
        /// </summary>
        public void OnUpdate()
        {

            curState.Tick();
        }
    }
}