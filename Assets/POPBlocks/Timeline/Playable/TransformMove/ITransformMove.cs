namespace POPBlocks.Timeline.Playable.TransformMove
{
    public interface ITransformMove
    {
        void OnStartClip();

        void OnPrecessFrame(float normalisedTime);
    }
}