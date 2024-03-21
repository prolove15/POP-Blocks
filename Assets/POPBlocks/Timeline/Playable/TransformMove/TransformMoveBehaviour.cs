using System;
using UnityEngine;
using UnityEngine.Playables;

namespace POPBlocks.Timeline.Playable.TransformMove
{
    [Serializable]
    public class TransformMoveBehaviour : PlayableBehaviour
    {
        [SerializeField]
        public spaces space;
        public AnimationCurve curveX;
        public AnimationCurve curveY;
        public ITransformMove iTransformMove;
        private bool started;

        public override void OnPlayableCreate(UnityEngine.Playables.Playable playable)
        {

        }

        public override void PrepareFrame(UnityEngine.Playables.Playable playable, FrameData info)
        {
            started = false;
        }

        public override void ProcessFrame(UnityEngine.Playables.Playable playable, FrameData info, object playerData)
        {
            Transform trackBinding = playerData as Transform;
            if (!started)
            {
                iTransformMove = trackBinding.GetComponent<ITransformMove>();
                iTransformMove.OnStartClip();
                started = true;
            }
            if (iTransformMove == null)
                return;

            float normalisedTime = (float)(playable.GetTime() / playable.GetDuration());
            iTransformMove.OnPrecessFrame(normalisedTime);
        }
    }

    [Serializable]
    public enum spaces
    {
        World,
        Local,
        Screen,
        ViewPort
    }
}