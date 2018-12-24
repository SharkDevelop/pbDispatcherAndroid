using Android.Support.V7.Widget;

namespace Dispatcher.Android
{
    public class CustomScrollListener : RecyclerView.OnScrollListener
    {
        public bool IsScrolling { get; private set; }
        
        public override void OnScrollStateChanged(RecyclerView recyclerView, int newState)
        {
            base.OnScrollStateChanged(recyclerView, newState);
            
            switch (newState)
            {
                case RecyclerView.ScrollStateDragging:
                    IsScrolling = true;
                    break;
                case RecyclerView.ScrollStateIdle:
                case RecyclerView.ScrollStateSettling:
                    IsScrolling = false;
                    break;
            }
        }
    }
}