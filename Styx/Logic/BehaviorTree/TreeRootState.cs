namespace Styx.Logic.BehaviorTree
{
	/// <summary>
	/// State machine for TreeRoot lifecycle — matches HB 6.2.3.
	/// Prevents race conditions between UI thread (Stop) and worker thread (Tick).
	/// </summary>
	public enum TreeRootState
	{
		Stopped,
		Starting,
		Running,
		Stopping
	}
}
