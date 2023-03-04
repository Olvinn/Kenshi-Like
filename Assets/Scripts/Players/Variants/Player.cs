namespace Players.Variants
{
    public class Player
    {
        protected UnitManager _manager;

        public Player()
        {
            _manager = new UnitManager();
        }
        
        public virtual void Update() { }
    }
}
