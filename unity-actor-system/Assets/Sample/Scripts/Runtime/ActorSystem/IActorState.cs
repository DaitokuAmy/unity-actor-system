using System.Collections.Generic;

namespace Sample {
    /// <summary>
    /// アクターステートマシン用のステートインターフェース
    /// </summary>
    public interface IActorState {
        /// <summary>
        /// セットアップ
        /// </summary>
        /// <param name="owner">所有主</param>
        void Setup(Actor owner);
        
        /// <summary>
        /// 開始処理
        /// </summary>
        void Enter();

        /// <summary>
        /// 更新処理
        /// </summary>
        /// <param name="commands">処理対象のコマンドリスト</param>
        /// <param name="signals">処理対象のシグナルリスト</param>
        /// <param name="deltaTime">変位時間</param>
        void Update(IReadOnlyList<ActorCommand> commands, IReadOnlyList<ActorSignal> signals, float deltaTime);

        /// <summary>
        /// 終了処理
        /// </summary>
        void Exit();
    }
}