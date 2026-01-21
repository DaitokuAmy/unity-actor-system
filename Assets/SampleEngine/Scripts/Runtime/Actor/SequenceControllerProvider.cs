using ActionSequencer;
using UnityEngine;

namespace Sample.Presentation {
    /// <summary>
    /// ActionSequencerのSequenceClipプレビュー用
    /// </summary>
    public sealed class SequenceControllerProvider : MonoBehaviour,ISequenceControllerProvider {
        private SequenceController _sequenceController;
        
        /// <inheritdoc/>
        SequenceController ISequenceControllerProvider.SequenceController => _sequenceController;
        
        /// <summary>
        /// SequenceControllerの設定
        /// </summary>
        public void SetSequenceController(SequenceController controller) {
            _sequenceController = controller;
        }
    }
}