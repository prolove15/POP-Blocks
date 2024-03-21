using UnityEngine;
using UnityEngine.Playables;

namespace POPBlocks.Timeline.Playable.TransformMove
{
    public class TransformMoveMixerBehaviour : PlayableBehaviour
    {
        // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
        public override void ProcessFrame(UnityEngine.Playables.Playable playable, FrameData info, object playerData)
        {
            Transform trackBinding = playerData as Transform;

            if (!trackBinding)
                return;

            int inputCount = playable.GetInputCount ();

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                ScriptPlayable<TransformMoveBehaviour> inputPlayable = (ScriptPlayable<TransformMoveBehaviour>)playable.GetInput(i);
                TransformMoveBehaviour input = inputPlayable.GetBehaviour ();
            
                // Use the above variables to process each frame of this playable.
            
            }
        }
    }
}
