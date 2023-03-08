namespace Players.Variants
{
    public class RTSPlayer
    {
        protected UnitManager _manager;

        public RTSPlayer()
        {
            _manager = new UnitManager();
        }
        
        public virtual void Update() { }
    }
}
